using Microsoft.Extensions.Logging;
using TaxiBookingService.Common;
using TaxiBookingService.DAL.Models;
using TaxiBookingService.DAL.Repositories;
using TaxiBookingServices.API.Service_Contract;

namespace TaxiBookingService.Logic.Services
{
    public interface IRideService
    {
        double CalculateDistance(decimal latitude1, decimal longitude1, decimal latitude2, decimal longitude2);
        Task<ResponseBase> BookRide(BookRideRequestDTO rideRequest);
        Task<CustomerViewRideResponseDTO> CustomerViewRideStatus();
        Task<ResponseBase> DriverRideAccept(int rideId, bool accept);
        Task<DriverViewRideResponseDTO> DriverViewRideRequest();
        Task<ResponseBase> RideStarted(int rideId);
        Task<RideCompleteResponseDTO> RideCompleted(int rideId);
        Task<ResponseBase> CancelRide(CancelRideRequestDTO cancelRideRequest);
        Task<ResponseBase> SubmitRating(SubmitRatingRequestDTO ratingDTO);
    }
    public class RideService : IRideService
    {
        private readonly ILogger<RideService> logger;
        private readonly IRideRepository rideRepository;
        private readonly IUserRepository userRepository;
        private readonly IDriverRepository driverRepository;
        private readonly IUserService userService;
        private readonly ResponseBase responseBase;

        #region Constructor
        public RideService(ILogger<RideService> logger, IRideRepository rideRepository, IUserRepository userRepository, IDriverRepository driverRepository, IUserService userService)
        {
            this.logger = logger;
            this.rideRepository = rideRepository;
            this.userRepository = userRepository;
            this.driverRepository = driverRepository;
            responseBase = new ResponseBase();
            this.userService = userService;

        }
        #endregion

        #region CalculateDistance From Latitude & Longitutde
        private const double EarthRadius = 6371.0;
        
        public double CalculateDistance(decimal latitude1, decimal longitude1, decimal latitude2, decimal longitude2)
        {
            var Latitude = ToRadians((double)(latitude2 - latitude1));
            var Longitude = ToRadians((double)(longitude2 - longitude1));
            var haversine = Math.Sin(Latitude / 2) * Math.Sin(Latitude / 2) +
                    Math.Cos(ToRadians((double)latitude1)) * Math.Cos(ToRadians((double)latitude2)) *
                    Math.Sin(Longitude / 2) * Math.Sin(Longitude / 2);
            var intermediate = 2 * Math.Atan2(Math.Sqrt(haversine), Math.Sqrt(1 - haversine));

            var distance = EarthRadius * intermediate;

            return distance;
        }

        private static double ToRadians(double angle)
        {
            return Math.PI * angle / 180.0;
        }
        #endregion

        #region BookRide
        public async Task<ResponseBase> BookRide(BookRideRequestDTO rideRequest)
        {
            try
            {
                if (rideRequest == null)
                {
                    responseBase.ResponseMsg = "Error in BookRide: Null input";
                    responseBase.ResponseResult = ResponseResult.Warning;
                    logger.LogWarning("Error in BookRide: Null input");
                    return responseBase;
                }
                var email = userService.GetCurrentUser();
                var user = await userRepository.UserEmailExists(email.Email);
                if ((await rideRepository.RideCompleted(user)))
                {
                    responseBase.ResponseMsg = "One customer can book only one ride at a time";
                    responseBase.ResponseResult = ResponseResult.Warning;
                    logger.LogWarning("One customer can book only one ride at a time");
                    return responseBase;
                }
                if (rideRequest.PickupLocationId == rideRequest.DropLocationId)
                {
                    responseBase.ResponseMsg = "Both pickup location and drop location cannot be the same";
                    responseBase.ResponseResult = ResponseResult.Warning;
                    logger.LogWarning("Both pickup location and drop location cannot be the same");
                    return responseBase;
                }
                var pickup = await rideRepository.GetLocation(rideRequest.PickupLocationId);
                var drop = await rideRepository.GetLocation(rideRequest.DropLocationId);
                var vehicleType = await rideRepository.GetVehicleType(rideRequest.VehicleTypeId);
                if (pickup == null)
                {
                    responseBase.ResponseMsg = "Invalid pickup location";
                    responseBase.ResponseResult = ResponseResult.Warning;
                    logger.LogWarning("Invalid pickup location");
                    return responseBase;
                }
                if (drop == null)
                {
                    responseBase.ResponseMsg = "Invalid drop location";
                    responseBase.ResponseResult = ResponseResult.Warning;
                    logger.LogWarning("Invalid drop location");
                    return responseBase;
                }
                if (vehicleType == null)
                {
                    responseBase.ResponseMsg = "Invalid vehicl type";
                    responseBase.ResponseResult = ResponseResult.Warning;
                    logger.LogWarning("Invalid vehicle type");
                    return responseBase;
                }
                var distance = (decimal)CalculateDistance(pickup.Latitude, pickup.Longitude, drop.Latitude, drop.Longitude);
                var customer = await userRepository.CustomerExist(user);
                Ride ride = new Ride()
                {
                    CustomerId = customer.Id,
                    PickUpId = rideRequest.PickupLocationId,
                    DropId = rideRequest.DropLocationId,
                    Distance = distance,
                    Amount = distance * vehicleType.FareFactor,
                    StatusId = 1
                };
                var newRide = await rideRepository.InsertRide(ride);
                var driver = await rideRepository.GetBestDriver(pickup.AreaId, rideRequest.VehicleTypeId, newRide.Id);
                if (driver == null)
                {
                    newRide.StatusId = 6;
                    var bn = await rideRepository.InsertRide(newRide);
                    responseBase.ResponseMsg = "No drivers available";
                    responseBase.ResponseResult = ResponseResult.Success;
                    return responseBase;
                }
                newRide.DriverId = driver.Id;
                await rideRepository.UpdateRide(newRide);
                await driverRepository.UpdateAvailability(false, driver);
                responseBase.ResponseMsg = "Ride requested successfully";
                responseBase.ResponseResult = ResponseResult.Success;
                return responseBase;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in BookRide: {ex.Message}");
                responseBase.ResponseMsg = $"Error in BookRide: {ex.Message}";
                responseBase.ResponseResult = ResponseResult.Exception;
                return responseBase;
            }
        }
        #endregion

        #region CustomerViewRideStatus
        public async Task<CustomerViewRideResponseDTO> CustomerViewRideStatus()
        {
            try
            {
                var email = userService.GetCurrentUser();
                var user = await userRepository.UserEmailExists(email.Email);
                var customer = await userRepository.CustomerExist(user);
                var ride = await rideRepository.GetCustomerRide(customer);
                if (ride == null)
                {
                    CustomerViewRideResponseDTO response = new CustomerViewRideResponseDTO()
                    {
                        ResponseMsg = "No ongoing ride",
                        ResponseResult = ResponseResult.Success
                    };
                    return response;
                }
                if (ride.StatusId == 1)
                {
                    CustomerViewRideResponseDTO response = new CustomerViewRideResponseDTO()
                    {
                        RideId = ride.Id,
                        ResponseMsg = "Searching for driver",
                        ResponseResult = ResponseResult.Success
                    };
                    return response;
                }
                CustomerViewRideResponseDTO rideResponseDTO = new CustomerViewRideResponseDTO()
                {
                    RideId = ride.Id,
                    DriverId = (int)ride.DriverId,
                    ResponseResult = ResponseResult.Success
                };
                return rideResponseDTO;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in CustomerViewRideStatus: {ex.Message}");
                CustomerViewRideResponseDTO response = new CustomerViewRideResponseDTO()
                {
                    ResponseMsg = $"Error in CustomerViewRideStatus: {ex.Message}",
                    ResponseResult = ResponseResult.Exception
                };
                return response;
            }
        }
        #endregion

        #region DriverRideAccept
        public async Task<ResponseBase> DriverRideAccept(int rideId, bool accept)
        {
            try
            {
                var email = userService.GetCurrentUser();
                var user = await userRepository.UserEmailExists(email.Email);
                var driver = await driverRepository.DriverExist(user);
                var ride = await rideRepository.GetRide(driver);
                if (ride == null)
                {
                    responseBase.ResponseMsg = "Invalid ride id";
                    responseBase.ResponseResult = ResponseResult.Warning;
                    logger.LogWarning("Invalid ride id");
                    return responseBase;
                }
                if (ride.DriverId != driver.Id)
                {
                    responseBase.ResponseMsg = "Driver id does not match with driver in ride";
                    responseBase.ResponseResult = ResponseResult.Warning;
                    logger.LogWarning("Driver id does not match with driver in ride");
                    return responseBase;
                }
                if (ride.StatusId != 1)
                {
                    responseBase.ResponseMsg = "This is old ride";
                    responseBase.ResponseResult = ResponseResult.Exception;
                    logger.LogError("This is an old ride");
                    return responseBase;
                }
                if (accept)
                {
                    ride.StatusId = 2;
                    await rideRepository.UpdateRide(ride);
                    responseBase.ResponseMsg = "Ride accepted";
                    responseBase.ResponseResult = ResponseResult.Success;
                    return responseBase;
                }
                else
                {
                    responseBase.ResponseMsg = "Ride declined";
                    responseBase.ResponseResult = ResponseResult.Success;
                    ride.DriverId = null;
                    await rideRepository.UpdateRide(ride);
                    RidesDeclined ridesDeclined = new RidesDeclined()
                    {
                        DriverId = driver.Id,
                        RideId = ride.Id
                    };
                    var rDeclined = await rideRepository.InsertRidesDecline(ridesDeclined);
                    await driverRepository.UpdateAvailability(true, driver);
                    var pickup = await userRepository.LocationExists(ride.PickUpId);
                    var vehicle = await driverRepository.VehicleByDriverId(driver);
                    var newdriver = await rideRepository.GetBestDriver(pickup.AreaId, vehicle.VehicleTypeId, ride.Id);
                    if (newdriver == null)
                    {
                        ride.StatusId = 6;
                        await rideRepository.UpdateRide(ride);
                        return responseBase;
                    }
                    ride.DriverId = newdriver.Id;
                    await rideRepository.UpdateRide(ride);
                    return responseBase;
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in DriverRideAccept: {ex.Message}");
                responseBase.ResponseMsg = $"Error in DriverRideAccept: {ex.Message}";
                responseBase.ResponseResult = ResponseResult.Exception;
                return responseBase;
            }
        }
        #endregion

        #region DriverViewRideRequest
        public async Task<DriverViewRideResponseDTO> DriverViewRideRequest()
        {
            try
            {
                var email = userService.GetCurrentUser();
                var user = await userRepository.UserEmailExists(email.Email);
                var driver = driverRepository.DriverExist(user);
                var ride = await rideRepository.GetDriverRide(driver.Id);
                if (ride == null)
                {
                    DriverViewRideResponseDTO respone = new DriverViewRideResponseDTO()
                    {
                        ResponseMsg = "No ride requests available",
                        ResponseResult = ResponseResult.Success
                    };
                    return respone;
                }
                DriverViewRideResponseDTO rideDetails = new DriverViewRideResponseDTO()
                {
                    RideId = ride.Id,
                    PickUpId = ride.PickUpId,
                    DropId = ride.DropId,
                    ResponseResult = ResponseResult.Success
                };
                return rideDetails;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in DriverViewRideRequest : {ex.Message}");
                DriverViewRideResponseDTO response = new DriverViewRideResponseDTO()
                {
                    ResponseMsg = $"Error in DriverViewRideRequest : {ex.Message}",
                    ResponseResult = ResponseResult.Exception
                };
                return response;
            }
        }
        #endregion

        #region DriverRideStarted
        public async Task<ResponseBase> RideStarted(int rideId)
        {
            try
            {
                var email = userService.GetCurrentUser();
                var user = await userRepository.UserEmailExists(email.Email);
                var driver = driverRepository.DriverExist(user);
                var ride = await rideRepository.GetDriverLatestRide(driver.Id, rideId);
                if (ride == null)
                {
                    responseBase.ResponseMsg = $"No booked ride";
                    responseBase.ResponseResult = ResponseResult.Warning;
                    return responseBase;
                }
                ride.StatusId = 3;
                ride.PickUpTime = DateTime.Now;
                await rideRepository.UpdateRide(ride);
                responseBase.ResponseMsg = $"Ride started";
                responseBase.ResponseResult = ResponseResult.Success;
                return responseBase;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in RideStarted: {ex.Message}");
                responseBase.ResponseMsg = $"Error in RideStarted: {ex.Message}";
                responseBase.ResponseResult = ResponseResult.Exception;
                return responseBase;
            }
        }
        #endregion

        #region DriverRideCompleted
        public async Task<RideCompleteResponseDTO> RideCompleted(int rideId)
        {
            try
            {
                var email = userService.GetCurrentUser();
                var user = await userRepository.UserEmailExists(email.Email);
                var driver = await driverRepository.DriverExist(user);
                var ride = await rideRepository.GetDriverRideForComplete(driver.Id, rideId);
                var customer = await rideRepository.GetCustomerByID(ride.CustomerId);
                if (ride == null)
                {
                    RideCompleteResponseDTO responseDto = new RideCompleteResponseDTO()
                    {
                        ResponseMsg = "No booked ride",
                        ResponseResult = ResponseResult.Warning
                    };
                    return responseDto;
                }
                ride.StatusId = 4;
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
                await rideRepository.UpdateRide(ride);
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
                logger.LogError($"Error in RideStarted: {ex.Message}");
                RideCompleteResponseDTO response = new RideCompleteResponseDTO()
                {
                    ResponseMsg = $"Error in RideStarted: {ex.Message}",
                    ResponseResult = ResponseResult.Exception
                };
                return response;
            }
        }
        #endregion

        #region CancelRide
        public async Task<ResponseBase> CancelRide(CancelRideRequestDTO cancelRideRequest)
        {
            try
            {
                var email = userService.GetCurrentUser();
                var user = await userRepository.UserEmailExists(email.Email);
                var ride = await rideRepository.GetRideById(cancelRideRequest.RideId);
                var cancelReason = await rideRepository.GetCancelReason(cancelRideRequest.CancelReasonId);
                if (ride == null)
                {
                    responseBase.ResponseMsg = "Invalid Ride id";
                    responseBase.ResponseResult = ResponseResult.Warning;
                    logger.LogWarning("Invalid ride id");
                    return responseBase;
                }
                if (cancelReason == null)
                {
                    responseBase.ResponseMsg = "Invalid CancelRide id";
                    responseBase.ResponseResult = ResponseResult.Warning;
                    logger.LogWarning("Invalid CacnelRide id");
                    return responseBase;
                }
                var driver = await rideRepository.GetDriverById((int)ride.DriverId);
                var customer = await userRepository.CustomerExist(user);
                if (customer != null)
                {
                    if (customer.Id != ride.CustomerId)
                    {
                        responseBase.ResponseMsg = "Customer mismatch";
                        responseBase.ResponseResult = ResponseResult.Exception;
                        logger.LogWarning("Customer mismatch");
                        return responseBase;
                    }
                    if (ride.StatusId == 1)
                    {
                        ride.DriverId = null;
                        ride.StatusId = 5;
                        ride.CancelReasonId = cancelReason.Id;
                        await driverRepository.UpdateAvailability(true, driver);
                        await rideRepository.UpdateRide(ride);
                        responseBase.ResponseMsg = "Ride cancelled successfully";
                        responseBase.ResponseResult = ResponseResult.Success;
                        return responseBase;
                    }
                    else if (ride.StatusId == 2)
                    {
                        ride.DriverId = null;
                        ride.StatusId = 5;
                        ride.CancelReasonId = cancelReason.Id;
                        if (!cancelReason.ValidReason)
                        {
                            customer.AmountDue += (decimal)0.05 * (decimal)ride.Amount;
                        }
                        await driverRepository.UpdateAvailability(true, driver);
                        await rideRepository.UpdateRide(ride);
                        responseBase.ResponseMsg = "Ride cancelled successfully";
                        responseBase.ResponseResult = ResponseResult.Success;
                        return responseBase;
                    }
                    else if (ride.StatusId == 3)
                    {
                        responseBase.ResponseMsg = "Ride cannot be cancelled after it has started";
                        responseBase.ResponseResult = ResponseResult.Warning;
                        return responseBase;
                    }
                    else
                    {
                        responseBase.ResponseMsg = "Ride cannot be cancelled after it is completed";
                        responseBase.ResponseResult = ResponseResult.Warning;
                        return responseBase;
                    }
                }
                else
                {
                    responseBase.ResponseResult = ResponseResult.Exception;
                    responseBase.ResponseMsg = "Error in CancelRide";
                    logger.LogError("Error in CancelRide");
                    return responseBase;
                }
            }
            catch (Exception ex)
            {
                responseBase.ResponseResult = ResponseResult.Exception;
                responseBase.ResponseMsg = $"Error in CancelRide: {ex.Message}";
                return responseBase;
            }
        }
        #endregion

        #region SubmitRating
        public async Task<ResponseBase> SubmitRating(SubmitRatingRequestDTO ratingDTO)
        {
            try
            {
                var email = userService.GetCurrentUser();
                var user = await userRepository.UserEmailExists(email.Email);
                var ride = await rideRepository.GetRideById(ratingDTO.RideId);
                var customer = await userRepository.CustomerExist(user);
                var driver = await driverRepository.DriverExist(user);
                if (ride == null)
                {
                    responseBase.ResponseMsg = "Invalid ride id";
                    responseBase.ResponseResult = ResponseResult.Warning;
                    logger.LogWarning("Invalid ride id");
                    return responseBase;
                }
                if(ride.StatusId != 4)
                {
                    responseBase.ResponseMsg = "Cannot rate rides which are not complete";
                    responseBase.ResponseResult = ResponseResult.Warning;
                    return responseBase;
                }
                if (driver == null)
                {
                    driver = await rideRepository.GetDriverById((int)ride.DriverId);
                    ride.DriverRating = (decimal)ratingDTO.Rating;
                    driver.Rating = ((driver.Rating * (driver.TotalRides - 1)) + (decimal)ratingDTO.Rating) / driver.TotalRides;
                    await rideRepository.UpdateRide(ride);
                    responseBase.ResponseMsg = "Rating addedd successfully";
                    responseBase.ResponseResult = ResponseResult.Success;
                    return responseBase;
                }
                if(customer == null)
                {
                    customer = await rideRepository.GetCustomerByID(ride.CustomerId);
                    ride.CustomerRating = (decimal)ratingDTO.Rating;
                    customer.Rating = ((customer.Rating * (customer.TotalRides - 1)) + (decimal)ratingDTO.Rating) / customer.TotalRides;
                    await rideRepository.UpdateRide(ride);
                    responseBase.ResponseMsg = "Rating added successfully";
                    responseBase.ResponseResult = ResponseResult.Success;
                    return responseBase;
                }
                responseBase.ResponseMsg = "Some error occured";
                responseBase.ResponseResult = ResponseResult.Exception;
                return responseBase;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in SubmitRating: {ex.Message}");
                responseBase.ResponseMsg = $"Error in SubmitRating: {ex.Message}";
                responseBase.ResponseResult = ResponseResult.Exception;
                return responseBase;
            }
        }
        #endregion
    }
}
