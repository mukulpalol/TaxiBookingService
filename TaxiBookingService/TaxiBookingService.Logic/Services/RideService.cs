using Microsoft.Extensions.Logging;
using TaxiBookingService.Common;
using TaxiBookingService.DAL.Models;
using TaxiBookingService.DAL.RepositoriesContract;
using TaxiBookingService.Logic.ServicesContract;
using TaxiBookingServices.API.Customer.CustomerServiceContract;
using TaxiBookingServices.API.Driver.DriverServiceContract;

namespace TaxiBookingService.Logic.Services
{
    public class RideService : IRideService
    {
        private readonly ILogger<RideService> logger;
        private readonly IRideRepository rideRepository;
        private readonly IUserRepository userRepository;
        private readonly IDriverRepository driverRepository;
        private readonly IVehicleRepository vehicleRepository;
        private readonly ICancelRepository cancelRepository;
        private readonly IUserService userService;

        #region Constructor
        public RideService(ILogger<RideService> logger, IRideRepository rideRepository, IUserRepository userRepository, IDriverRepository driverRepository, IVehicleRepository vehicleRepository, ICancelRepository cancelRepository, IUserService userService)
        {
            this.logger = logger;
            this.rideRepository = rideRepository;
            this.userRepository = userRepository;
            this.driverRepository = driverRepository;
            this.vehicleRepository = vehicleRepository;
            this.cancelRepository = cancelRepository;
            this.userService = userService;
        }
        #endregion

        #region GetTime        
        private static TimeSpan GetTime(double distance, int speed)
        {
            double hours = distance / speed;
            TimeSpan time = TimeSpan.FromHours(hours);
            return time;
        }
        #endregion
        
        #region CustomerViewRideStatus
        public async Task<CustomerViewRideResponseDTO> CustomerViewRideStatus()
        {
            CustomerViewRideResponseDTO response = new CustomerViewRideResponseDTO();
            try
            {
                var email = userService.GetCurrentUser();
                var user = await userRepository.UserEmailExists(email.Email);
                var customer = await userRepository.CustomerExist(user);
                var ride = await rideRepository.GetCustomerRide(customer);               
                ride = CheckValidation.NullCheck(ride, "No ongoing ride");
                if (ride.StatusId == (int)RideStatus.Searching)
                {
                    response.RideId = ride.Id;
                    response.ResponseMsg = "Searching for driver";
                    response.ResponseResult = ResponseResult.Success;
                    return response;
                }
                response.RideId = ride.Id;
                response.DriverId = (int)ride.DriverId;
                response.ETA = (DateTime)ride.EstimatedPickUpTime;
                response.ResponseResult = ResponseResult.Success;
                return response;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in CustomerViewRideStatus: {ex.Message}");
                response.ResponseMsg = $"Error in CustomerViewRideStatus: {ex.Message}";
                response.ResponseResult = ResponseResult.Exception;
                return response;
            }
        }
        #endregion
        
        #region BookRide
        public virtual async Task<BookRideResponseDTO> BookRide(BookRideRequestDTO rideRequest)
        {
            BookRideResponseDTO response = new BookRideResponseDTO();
            try
            {
                rideRequest = CheckValidation.NullCheck(rideRequest, "Null input");
                var email = userService.GetCurrentUser();
                var user = await userRepository.UserEmailExists(email.Email);
                if ((await rideRepository.RideCompleted(user)))
                {
                    response.ResponseMsg = "One customer can book only one ride at a time";
                    response.ResponseResult = ResponseResult.Warning;
                    logger.LogWarning("One customer can book only one ride at a time");
                    return response;
                }
                var pickup = await userRepository.LocationExists(rideRequest.PickupLocationId);
                var drop = await userRepository.LocationExists(rideRequest.DropLocationId);
                pickup = CheckValidation.NullCheck(pickup, "Invalid pickup location");
                drop = CheckValidation.NullCheck(drop, "Incalid drop off location");
                drop = CheckValidation.NullCheck(drop, "Invalid drop location");
                var pickupArea = await userRepository.AreaExists(pickup.AreaId);
                var dropArea = await userRepository.AreaExists(drop.AreaId);
                if (pickupArea.CityId != dropArea.CityId)
                {
                    response.ResponseMsg = "Pickup location and drop location should be in the same city";
                    response.ResponseResult = ResponseResult.Warning;
                    logger.LogWarning("Pickup location and drop location should be in the same city");
                    return response;
                }
                if (rideRequest.PickupLocationId == rideRequest.DropLocationId)
                {
                    response.ResponseMsg = "Both pickup location and drop location cannot be the same";
                    response.ResponseResult = ResponseResult.Warning;
                    logger.LogWarning("Both pickup location and drop location cannot be the same");
                    return response;
                }
                var vehicleType = await vehicleRepository.GetVehicleType((int)rideRequest.VehicleTypeId);
                vehicleType = CheckValidation.NullCheck(vehicleType, "Invalid vehicle type");
                var distance = (decimal)CalculateCoordinatesDistance.CalculateDistance(pickup.Latitude, pickup.Longitude, drop.Latitude, drop.Longitude);
                var customer = await userRepository.CustomerExist(user);
                Ride ride = new Ride()
                {
                    CustomerId = customer.Id,
                    PickUpId = rideRequest.PickupLocationId,
                    DropId = rideRequest.DropLocationId,
                    Distance = distance,
                    Amount = Math.Round((distance * vehicleType.FareFactor), 0),
                    StatusId = (int)RideStatus.Searching
                };
                var newRide = await rideRepository.InsertRide(ride);
                var driver = await rideRepository.GetBestDriver((int)rideRequest.VehicleTypeId, newRide.Id);
                if (driver == null)
                {
                    newRide.StatusId = (int)RideStatus.NoDriversAvailable;
                    await userRepository.UpdateDatabase();
                    response.ResponseMsg = "No drivers available";
                    response.ResponseResult = ResponseResult.Success;
                    return response;
                }
                newRide.DriverId = driver.Id;
                await userRepository.UpdateDatabase();
                await driverRepository.UpdateAvailability(false, driver);
                response.ResponseMsg = "Ride requested successfully";
                response.ResponseResult = ResponseResult.Success;
                return response;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in BookRide: {ex.Message}");
                response.ResponseMsg = $"Error in BookRide: {ex.Message}";
                response.ResponseResult = ResponseResult.Exception;
                return response;
            }
        }
        #endregion

        #region DriverRideAccept
        public async Task<RideAcceptResponseDTO> DriverRideAccept(int rideID, bool accept)
        {
            RideAcceptResponseDTO response = new RideAcceptResponseDTO();
            try
            {
                var email = userService.GetCurrentUser();
                var user = await userRepository.UserEmailExists(email.Email);
                var driver = await driverRepository.DriverExist(user);
                var ride = await rideRepository.GetDriverRide(driver.Id);
                ride = CheckValidation.NullCheck(ride, "Invalid ride id");
                if (ride.DriverId != driver.Id)
                {
                    response.ResponseMsg = "Driver id does not match with driver in ride";
                    response.ResponseResult = ResponseResult.Warning;
                    logger.LogWarning("Driver id does not match with driver in ride");
                    return response;
                }
                if (ride.StatusId != (int)RideStatus.Searching)
                {
                    response.ResponseMsg = "This ride is not in searching phase";
                    response.ResponseResult = ResponseResult.Exception;
                    logger.LogWarning("This ride is not in searching phase");
                    return response;
                }
                var vehicle = await vehicleRepository.VehicleById(driver.VehicleId);
                var vehicleType = await vehicleRepository.GetVehicleType(vehicle.VehicleTypeId);
                var pickup = await userRepository.LocationExists(ride.PickUpId);
                var driverLocation = await userRepository.LocationExists(driver.LocationId);
                var distanceOfDriverToPickup = CalculateCoordinatesDistance.CalculateDistance(pickup.Latitude, pickup.Longitude, driverLocation.Latitude, driverLocation.Longitude);
                var time = GetTime(distanceOfDriverToPickup, vehicleType.AverageSpeed);
                if (accept)
                {
                    ride.StatusId = (int)RideStatus.Booked;
                    ride.EstimatedPickUpTime = DateTime.Now + time;
                    await userRepository.UpdateDatabase();
                    response.ResponseMsg = "Ride accepted";
                    response.ResponseResult = ResponseResult.Success;
                    return response;
                }
                else
                {
                    response.ResponseMsg = "Ride declined";
                    response.ResponseResult = ResponseResult.Success;
                    ride.DriverId = null;
                    await userRepository.UpdateDatabase();
                    RidesDeclined ridesDeclined = new RidesDeclined()
                    {
                        DriverId = driver.Id,
                        RideId = ride.Id
                    };
                    var rDeclined = await rideRepository.InsertRidesDecline(ridesDeclined);
                    await driverRepository.UpdateAvailability(true, driver);
                    var newdriver = await rideRepository.GetBestDriver(vehicle.VehicleTypeId, ride.Id);
                    if (newdriver == null)
                    {
                        ride.StatusId = (int)RideStatus.NoDriversAvailable;
                        await userRepository.UpdateDatabase();
                        return response;
                    }
                    ride.DriverId = newdriver.Id;
                    await userRepository.UpdateDatabase();
                    return response;
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in DriverRideAccept: {ex.Message}");
                response.ResponseMsg = $"Error in DriverRideAccept: {ex.Message}";
                response.ResponseResult = ResponseResult.Exception;
                return response;
            }
        }
        #endregion
        
        #region DriverViewRideRequest
        public async Task<DriverViewRideResponseDTO> DriverViewRideRequest()
        {
            DriverViewRideResponseDTO response = new DriverViewRideResponseDTO();
            try
            {
                var email = userService.GetCurrentUser();
                var user = await userRepository.UserEmailExists(email.Email);
                var driver = await driverRepository.DriverExist(user);
                var ride = await rideRepository.GetDriverRide(driver.Id);
                ride = CheckValidation.NullCheck(ride, "No ride request available");
                response.RideId = ride.Id;
                response.PickUpId = ride.PickUpId;
                response.DropId = ride.DropId;
                response.ResponseResult = ResponseResult.Success;
                return response;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in DriverViewRideRequest : {ex.Message}");
                response.ResponseMsg = $"Error in DriverViewRideRequest : {ex.Message}";
                response.ResponseResult = ResponseResult.Exception;
                return response;
            }
        }
        #endregion
        
        #region DriverRideStarted
        public async Task<RideStartedResponseDTO> RideStarted(RideIdRequestDTO rideStarted)
        {
            RideStartedResponseDTO response = new RideStartedResponseDTO();
            try
            {
                int rideId = rideStarted.RideId;
                var email = userService.GetCurrentUser();
                var user = await userRepository.UserEmailExists(email.Email);
                var driver = await driverRepository.DriverExist(user);
                var ride = await rideRepository.GetDriverLatestRide(driver.Id, rideId);
                ride = CheckValidation.NullCheck(ride, "No booked ride");
                var vehicle = await vehicleRepository.VehicleById(driver.VehicleId);
                var vehicleType = await vehicleRepository.GetVehicleType(vehicle.VehicleTypeId);
                var time = GetTime((double)ride.Distance, vehicleType.AverageSpeed);
                ride.StatusId = (int)RideStatus.RideStarted;
                ride.PickUpTime = DateTime.Now;
                ride.EstimatedDropTime = DateTime.Now + time;
                await userRepository.UpdateDatabase();
                response.ResponseMsg = "Ride started";
                response.ResponseResult = ResponseResult.Warning;
                return response;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in RideStarted: {ex.Message}");
                response.ResponseMsg = $"Error in RideStarted: {ex.Message}";
                response.ResponseResult = ResponseResult.Exception;
                return response;
            }
        }
        #endregion
        
        #region DriverRideCompleted
        public async Task<RideCompleteResponseDTO> RideCompleted(RideIdRequestDTO rideCompleted)
        {
            try
            {
                int rideId = rideCompleted.RideId;
                var email = userService.GetCurrentUser();
                var user = await userRepository.UserEmailExists(email.Email);
                var driver = await driverRepository.DriverExist(user);
                var ride = await rideRepository.GetDriverRideForComplete(driver.Id, rideId);
                if (ride == null)
                {
                    RideCompleteResponseDTO responseDto = new RideCompleteResponseDTO()
                    {
                        ResponseMsg = "No booked ride",
                        ResponseResult = ResponseResult.Warning
                    };
                    return responseDto;
                }
                var customer = await rideRepository.GetCustomerByID(ride.CustomerId);
                ride.StatusId = (int)RideStatus.RideCompleted;
                ride.DropTime = DateTime.Now;
                ride.Duration = DateTime.Now.Subtract((DateTime)ride.PickUpTime);
                Payment payment = new Payment()
                {
                    Amount = (decimal)(ride.Amount + customer.AmountDue),
                    PaymentTimeStamp = DateTime.Now
                };
                payment = await rideRepository.InsertPayment(payment);
                ride.Payment = payment;
                driver.LocationId = ride.DropId;
                driver.Available = true;
                driver.TotalRides++;
                customer.TotalRides++;
                customer.AmountDue = 0;
                await userRepository.UpdateDatabase();
                RideCompleteResponseDTO response = new RideCompleteResponseDTO()
                {
                    RideFare = payment.Amount,
                    ResponseMsg = "Ride completed",
                    ResponseResult = ResponseResult.Success
                };
                return response;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in RideCompleted: {ex.Message}");
                RideCompleteResponseDTO response = new RideCompleteResponseDTO()
                {
                    ResponseMsg = $"Error in RideCompleted: {ex.Message}",
                    ResponseResult = ResponseResult.Exception
                };
                return response;
            }
        }
        #endregion
        
        #region CheckRideStatusForCancel
        private async Task<CancelRideResponseDTO> CheckRideStatusForCancel(Ride ride, CancelReason cancelReason, Driver driver, Customer customer)
        {
            CancelRideResponseDTO response = new CancelRideResponseDTO();
            try
            {
                if (ride.StatusId == (int)RideStatus.Searching)
                {
                    ride.DriverId = null;
                    ride.StatusId = (int)RideStatus.RideCancelled;
                    ride.CancelReasonId = cancelReason.Id;
                    await driverRepository.UpdateAvailability(true, driver);
                    await userRepository.UpdateDatabase();
                    response.ResponseMsg = "Ride cancelled successfully";
                    response.ResponseResult = ResponseResult.Success;
                    return response;
                }
                else if (ride.StatusId == (int)RideStatus.Booked)
                {
                    ride.DriverId = null;
                    ride.StatusId = (int)RideStatus.RideCancelled;
                    ride.CancelReasonId = cancelReason.Id;
                    if (!cancelReason.ValidReason)
                    {
                        customer.AmountDue += Math.Round(((await cancelRepository.GetCancellationFactor()) * (decimal)ride.Amount), 0);
                    }
                    await driverRepository.UpdateAvailability(true, driver);
                    await userRepository.UpdateDatabase();
                    response.ResponseMsg = "Ride cancelled successfully";
                    response.ResponseResult = ResponseResult.Success;
                    return response;
                }
                else if (ride.StatusId == (int)RideStatus.RideStarted)
                {
                    response.ResponseMsg = "Ride cannot be cancelled after it has started";
                    response.ResponseResult = ResponseResult.Warning;
                    logger.LogWarning("Ride cannot be cancelled after it has started");
                    return response;
                }
                else
                {
                    response.ResponseMsg = "Ride cannot be cancelled after it is completed";
                    response.ResponseResult = ResponseResult.Warning;
                    logger.LogWarning("Ride cannot becancelled after it is completed");
                    return response;
                }

            }
            catch (Exception ex)
            {
                logger.LogError($"Error in CheckRideStatusForCancel: {ex.Message}");
                response.ResponseResult = ResponseResult.Exception;
                response.ResponseMsg = $"Error in CheckRideStatusForCancel: {ex.Message}";
                return response;
            }
        }
        #endregion
        
        #region CancelRide
        public async Task<CancelRideResponseDTO> CancelRide(CancelRideRequestDTO cancelRideRequest)
        {
            CancelRideResponseDTO response = new CancelRideResponseDTO();
            try
            {
                var email = userService.GetCurrentUser();
                var user = await userRepository.UserEmailExists(email.Email);
                var ride = await rideRepository.GetRideById(cancelRideRequest.RideId);
                var cancelReason = await cancelRepository.GetCancelReason(cancelRideRequest.CancelReasonId);
                ride = CheckValidation.NullCheck(ride, "Invalid Ride Id");
                cancelReason = CheckValidation.NullCheck(cancelReason, "Invalid CancelRide Id");
                var driver = await driverRepository.GetDriverById((int)ride.DriverId);
                var customer = await userRepository.CustomerExist(user);
                if (customer != null)
                {
                    if (customer.Id != ride.CustomerId)
                    {
                        response.ResponseMsg = "Customer mismatch";
                        response.ResponseResult = ResponseResult.Exception;
                        logger.LogWarning("Customer mismatch");
                        return response;
                    }
                    response = await CheckRideStatusForCancel(ride, cancelReason, driver, customer);
                    return response;
                }
                else
                {
                    response.ResponseMsg = "Error in CancelRide";
                    response.ResponseResult = ResponseResult.Warning;
                    logger.LogWarning("Error in CancelRide");
                    return response;
                }
            }
            catch (Exception ex)
            {
                response.ResponseMsg = $"Error in CancelRide: {ex.Message}";
                response.ResponseResult = ResponseResult.Exception;
                logger.LogWarning($"Error in CancelRide: {ex.Message}");
                return response;
            }
        }
        #endregion
        
        #region DriverSubmitRating
        public async Task<RatingResponseDTO> DriverSubmitRating(SubmitRatingRequestDTO ratingDTO)
        {
            RatingResponseDTO response = new RatingResponseDTO();
            try
            {
                var email = userService.GetCurrentUser();
                var user = await userRepository.UserEmailExists(email.Email);
                var ride = await rideRepository.GetRideById(ratingDTO.RideId);
                var driver = await driverRepository.DriverExist(user);
                ride = CheckValidation.NullCheck(ride, "Invalid Ride Id");
                driver = CheckValidation.NullCheck(driver, "Driver does not exist");
                if (ride.StatusId != (int)RideStatus.RideCompleted)
                {
                    response.ResponseMsg = "Cannot rate rides which are not complete";
                    response.ResponseResult = ResponseResult.Warning;
                    return response;
                }
                if (driver.Id != ride.DriverId)
                {
                    response.ResponseMsg = "Cannot rate a ride of other driver";
                    response.ResponseResult = ResponseResult.Warning;
                    return response;
                }
                if (ride.CustomerRating != null)
                {
                    response.ResponseMsg = "Ride already rated";
                    response.ResponseResult = ResponseResult.Warning;
                    return response;
                }
                var customer = await rideRepository.GetCustomerByID(ride.CustomerId);
                ride.CustomerRating = (decimal)ratingDTO.Rating;
                var ratedRidesCount = await rideRepository.CustomerRatedRideCount(ride.CustomerId);
                customer.Rating = ((customer.Rating * (ratedRidesCount)) + (decimal)ratingDTO.Rating) / (ratedRidesCount + 1);
                await userRepository.UpdateDatabase();
                response.ResponseMsg = "Rating added successfully";
                response.ResponseResult = ResponseResult.Success;
                return response;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in DriverSubmitRating: {ex.Message}");
                response.ResponseMsg = $"Error in DriverSubmitRating: {ex.Message}";
                response.ResponseResult = ResponseResult.Exception;
                return response;
            }
        }
        #endregion
        
        #region CustomerSubmitRating
        public async Task<RatingResponseDTO> CustomerSubmitRating(SubmitRatingRequestDTO ratingDTO)
        {
            RatingResponseDTO response = new RatingResponseDTO();
            try
            {
                var email = userService.GetCurrentUser();
                var user = await userRepository.UserEmailExists(email.Email);
                var ride = await rideRepository.GetRideById(ratingDTO.RideId);
                var customer = await userRepository.CustomerExist(user);
                customer = CheckValidation.NullCheck(customer, "Customer does not exist");
                ride = CheckValidation.NullCheck(ride, "Invalid Ride Id");
                if (ride.StatusId != (int)RideStatus.RideCompleted)
                {
                    response.ResponseMsg = "Cannot rate rides which are not complete";
                    response.ResponseResult = ResponseResult.Warning;
                    return response;
                }
                if (customer.Id != ride.CustomerId)
                {
                    response.ResponseMsg = "Cannot rate a ride of another customer";
                    response.ResponseResult = ResponseResult.Warning;
                    return response;
                }
                if (ride.DriverRating != null)
                {
                    response.ResponseMsg = "Ride already rated";
                    response.ResponseResult = ResponseResult.Warning;
                    return response;
                }
                var driver = await driverRepository.GetDriverById((int)ride.DriverId);
                ride.DriverRating = (decimal)ratingDTO.Rating;
                var ratedRidesCount = await rideRepository.DriverRatedRideCount(driver.Id);
                driver.Rating = ((driver.Rating * ratedRidesCount) + (decimal)ratingDTO.Rating) / (ratedRidesCount+1);
                await userRepository.UpdateDatabase();
                response.ResponseMsg = "Rating added successfully";
                response.ResponseResult = ResponseResult.Success;
                return response;                
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in SubmitRating: {ex.Message}");
                response.ResponseMsg = $"Error in SubmitRating: {ex.Message}";
                response.ResponseResult = ResponseResult.Exception;
                return response;
            }
        }
        #endregion        
    }
}
