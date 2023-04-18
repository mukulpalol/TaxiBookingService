using Microsoft.Extensions.Logging;
using TaxiBookingService.Common;
using TaxiBookingService.DAL.Models;
using TaxiBookingService.DAL.Repositories;
using TaxiBookingServices.API.CustomerContract;
using TaxiBookingServices.API.DriverContract;

namespace TaxiBookingService.Logic.Services
{
    public interface IRideService
    {
        double CalculateDistance(decimal latitude1, decimal longitude1, decimal latitude2, decimal longitude2);
        Task<BookRideResponseDTO> BookRide(BookRideRequestDTO rideRequest);
        Task<CustomerViewRideResponseDTO> CustomerViewRideStatus();
        Task<RideAcceptResponseDTO> DriverRideAccept(int rideID, bool accept);
        Task<DriverViewRideResponseDTO> DriverViewRideRequest();
        Task<RideStartedResponseDTO> RideStarted(RideIdRequestDTO rideStarted);
        Task<RideCompleteResponseDTO> RideCompleted(RideIdRequestDTO rideCompleted);
        Task<CancelRideResponseDTO> CancelRide(CancelRideRequestDTO cancelRideRequest);
        Task<RatingResponseDTO> SubmitRating(SubmitRatingRequestDTO ratingDTO);        
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
        public async Task<BookRideResponseDTO> BookRide(BookRideRequestDTO rideRequest)
        {
            try
            {
                if (rideRequest == null)
                {
                    BookRideResponseDTO response = new BookRideResponseDTO()
                    {
                        ResponseMsg = "Error in BookRide: Null input",
                        ResponseResult = ResponseResult.Warning
                    };
                    logger.LogWarning("Error in BookRide: Null input");
                    return response;
                }
                var email = userService.GetCurrentUser();
                var user = await userRepository.UserEmailExists(email.Email);
                if ((await rideRepository.RideCompleted(user)))
                {
                    BookRideResponseDTO response = new BookRideResponseDTO()
                    {
                        ResponseMsg = "One customer can book only one ride at a time",
                        ResponseResult = ResponseResult.Warning
                    };
                    logger.LogWarning("One customer can book only one ride at a time");
                    return response;
                }
                var pickup = await userRepository.LocationExists(rideRequest.PickupLocationId);
                var drop = await userRepository.LocationExists (rideRequest.DropLocationId);
                if(pickup ==null)
                {
                    BookRideResponseDTO response = new BookRideResponseDTO()
                    {
                        ResponseMsg = "Invalid pickup location",
                        ResponseResult = ResponseResult.Warning
                    };
                    logger.LogWarning("Invalid pickup location");
                    return response;
                }
                if(drop == null)
                {
                    BookRideResponseDTO response = new BookRideResponseDTO()
                    {
                        ResponseMsg = "Invalid drop location",
                        ResponseResult = ResponseResult.Warning
                    };
                    logger.LogWarning("Invalid drop location");
                    return response;
                }
                var pickupArea = await userRepository.AreaExists(pickup.AreaId);
                var dropArea = await userRepository.AreaExists(drop.AreaId);
                if(pickupArea.CityId != dropArea.CityId)
                {
                    BookRideResponseDTO response = new BookRideResponseDTO()
                    {
                        ResponseMsg = "Pickup location and drop location should be in the same city",
                        ResponseResult = ResponseResult.Warning
                    };
                    logger.LogWarning("Pickup location and drop location should be in the same city");
                    return response;
                }
                if (rideRequest.PickupLocationId == rideRequest.DropLocationId)
                {
                    BookRideResponseDTO response = new BookRideResponseDTO()
                    {
                        ResponseMsg = "Both pickup location and drop location cannot be the same",
                        ResponseResult = ResponseResult.Warning
                    };
                    logger.LogWarning("Both pickup location and drop location cannot be the same");
                    return response;
                }                
                var vehicleType = await rideRepository.GetVehicleType(rideRequest.VehicleTypeId);
                if (pickup == null)
                {
                    BookRideResponseDTO response = new BookRideResponseDTO()
                    {
                        ResponseMsg = "Invalid pickup location",
                        ResponseResult = ResponseResult.Warning
                    };
                    logger.LogWarning("Invalid pickup location");
                    return response;
                }
                if (drop == null)
                {
                    BookRideResponseDTO response = new BookRideResponseDTO()
                    {
                        ResponseMsg = "Invalid drop location",
                        ResponseResult = ResponseResult.Warning
                    };
                    logger.LogWarning("Invalid drop location");
                    return response;
                }
                if (vehicleType == null)
                {
                    BookRideResponseDTO response = new BookRideResponseDTO()
                    {
                        ResponseMsg = "Invalid vehicle type",
                        ResponseResult = ResponseResult.Warning
                    };
                    logger.LogWarning("Invalid vehicle type");
                    return response;
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
                    BookRideResponseDTO response = new BookRideResponseDTO()
                    {
                        ResponseMsg = "No drivers available",
                        ResponseResult = ResponseResult.Success
                    };
                    return response;
                }
                newRide.DriverId = driver.Id;
                await rideRepository.UpdateRide(newRide);
                await driverRepository.UpdateAvailability(false, driver);
                BookRideResponseDTO responseDto = new BookRideResponseDTO()
                {
                    ResponseMsg = "Ride requested successfully",
                    ResponseResult = ResponseResult.Success
                };
                return responseDto;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in BookRide: {ex.Message}");
                BookRideResponseDTO responseDto = new BookRideResponseDTO()
                {
                    ResponseMsg = "$\"Error in BookRide: {ex.Message}",
                    ResponseResult = ResponseResult.Exception
                };
                return responseDto;
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
        public async Task<RideAcceptResponseDTO> DriverRideAccept(int rideID, bool accept)
        {
            try
            {
                var email = userService.GetCurrentUser();
                var user = await userRepository.UserEmailExists(email.Email);
                var driver = await driverRepository.DriverExist(user);
                var ride = await rideRepository.GetRide(driver);
                if (ride == null)
                {
                    RideAcceptResponseDTO response = new RideAcceptResponseDTO()
                    {
                        ResponseMsg = "Invalid ride id",
                        ResponseResult = ResponseResult.Warning
                    };
                    logger.LogWarning("Invalid ride id");
                    return response;
                }
                if (ride.DriverId != driver.Id)
                {
                    RideAcceptResponseDTO response = new RideAcceptResponseDTO()
                    {
                        ResponseMsg = "Driver id does not match with driver in ride",
                        ResponseResult = ResponseResult.Warning
                    };
                    logger.LogWarning("Driver id does not match with driver in ride");
                    return response;
                }
                if (ride.StatusId != 1)
                {
                    RideAcceptResponseDTO response = new RideAcceptResponseDTO()
                    {
                        ResponseMsg = "This ride is not in searching phase",
                        ResponseResult = ResponseResult.Exception
                    };
                    logger.LogWarning("This ride is not in searching phase");
                    return response;
                }
                if (accept)
                {
                    ride.StatusId = 2;
                    await rideRepository.UpdateRide(ride);
                    RideAcceptResponseDTO response = new RideAcceptResponseDTO()
                    {
                        ResponseMsg = "Ride accepted",
                        ResponseResult = ResponseResult.Success
                    };
                    return response;
                }
                else
                {
                    RideAcceptResponseDTO response = new RideAcceptResponseDTO()
                    {
                        ResponseMsg = "Ride declined",
                        ResponseResult = ResponseResult.Success
                    };
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
                        return response;
                    }
                    ride.DriverId = newdriver.Id;
                    await rideRepository.UpdateRide(ride);
                    return response;
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in DriverRideAccept: {ex.Message}");
                RideAcceptResponseDTO response = new RideAcceptResponseDTO()
                {
                    ResponseMsg = $"Error in DriverRideAccept: {ex.Message}",
                    ResponseResult = ResponseResult.Exception
                };
                return response;
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
                var driver = await driverRepository.DriverExist(user);
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
        public async Task<RideStartedResponseDTO> RideStarted(RideIdRequestDTO rideStarted)
        {
            try
            {
                int rideId = rideStarted.RideId;
                var email = userService.GetCurrentUser();
                var user = await userRepository.UserEmailExists(email.Email);
                var driver = await driverRepository.DriverExist(user);
                var ride = await rideRepository.GetDriverLatestRide(driver.Id, rideId);
                if (ride == null)
                {
                    RideStartedResponseDTO response = new RideStartedResponseDTO()
                    {
                        ResponseMsg = "No booked ride",
                        ResponseResult = ResponseResult.Warning
                    };
                    return response;
                }
                ride.StatusId = 3;
                ride.PickUpTime = DateTime.Now;
                await rideRepository.UpdateRide(ride);
                RideStartedResponseDTO responseDto = new RideStartedResponseDTO()
                {
                    ResponseMsg = "Ride started",
                    ResponseResult = ResponseResult.Warning
                };
                return responseDto;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in RideStarted: {ex.Message}");
                RideStartedResponseDTO response = new RideStartedResponseDTO()
                {
                    ResponseMsg = $"Error in RideStarted: {ex.Message}",
                    ResponseResult = ResponseResult.Exception
                };
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

        #region CancelRide
        public async Task<CancelRideResponseDTO> CancelRide(CancelRideRequestDTO cancelRideRequest)
        {
            try
            {
                var email = userService.GetCurrentUser();
                var user = await userRepository.UserEmailExists(email.Email);
                var ride = await rideRepository.GetRideById(cancelRideRequest.RideId);
                var cancelReason = await rideRepository.GetCancelReason(cancelRideRequest.CancelReasonId);
                if (ride == null)
                {
                    CancelRideResponseDTO response = new CancelRideResponseDTO()
                    {
                        ResponseMsg = "Invalid Ride id",
                        ResponseResult = ResponseResult.Warning
                    };
                    logger.LogWarning("Invalid Ride id");
                    return response;
                }
                if (cancelReason == null)
                {
                    CancelRideResponseDTO response = new CancelRideResponseDTO()
                    {
                        ResponseMsg = "Invalid CancelRide id",
                        ResponseResult = ResponseResult.Warning
                    };
                    logger.LogWarning("Invalid CancelRide id");
                    return response;
                }
                var driver = await rideRepository.GetDriverById((int)ride.DriverId);
                var customer = await userRepository.CustomerExist(user);
                if (customer != null)
                {
                    if (customer.Id != ride.CustomerId)
                    {
                        CancelRideResponseDTO response = new CancelRideResponseDTO()
                        {
                            ResponseMsg = "Customer mismatch",
                            ResponseResult = ResponseResult.Exception
                        };
                        logger.LogWarning("Customer mismatch");
                        return response;
                    }
                    if (ride.StatusId == 1)
                    {
                        ride.DriverId = null;
                        ride.StatusId = 5;
                        ride.CancelReasonId = cancelReason.Id;
                        await driverRepository.UpdateAvailability(true, driver);
                        await rideRepository.UpdateRide(ride);
                        CancelRideResponseDTO response = new CancelRideResponseDTO()
                        {
                            ResponseMsg = "Ride cancelled successfully",
                            ResponseResult = ResponseResult.Success
                        };
                        return response;
                    }
                    else if (ride.StatusId == 2)
                    {
                        ride.DriverId = null;
                        ride.StatusId = 5;
                        ride.CancelReasonId = cancelReason.Id;
                        if (!cancelReason.ValidReason)
                        {
                            customer.AmountDue += (await rideRepository.GetCancellationFactor()) * (decimal)ride.Amount;
                        }
                        await driverRepository.UpdateAvailability(true, driver);
                        await rideRepository.UpdateRide(ride);
                        CancelRideResponseDTO response = new CancelRideResponseDTO()
                        {
                            ResponseMsg = "Ride cancelled successfully",
                            ResponseResult = ResponseResult.Success
                        };
                        logger.LogWarning("Ride cancelled successfully");
                        return response;
                    }
                    else if (ride.StatusId == 3)
                    {
                        CancelRideResponseDTO response = new CancelRideResponseDTO()
                        {
                            ResponseMsg = "Ride cannot be cancelled after it has started",
                            ResponseResult = ResponseResult.Warning
                        };
                        logger.LogWarning("Ride cannot be cancelled after it has started");
                        return response;
                    }
                    else
                    {
                        CancelRideResponseDTO response = new CancelRideResponseDTO()
                        {
                            ResponseMsg = "Ride cannot be cancelled after it is completed",
                            ResponseResult = ResponseResult.Warning
                        };
                        logger.LogWarning("Ride cannot becancelled after it is completed");
                        return response;
                    }
                }
                else
                {
                    CancelRideResponseDTO response = new CancelRideResponseDTO()
                    {
                        ResponseMsg = "Error in CancellRide",
                        ResponseResult = ResponseResult.Warning
                    };
                    logger.LogWarning("Error in CancelRide");
                    return response;
                }
            }
            catch (Exception ex)
            {
                CancelRideResponseDTO response = new CancelRideResponseDTO()
                {
                    ResponseMsg = $"Error in CancelRide: {ex.Message}",
                    ResponseResult = ResponseResult.Exception
                };
                logger.LogWarning($"Error in CancelRide: {ex.Message}");
                return response;                
            }
        }
        #endregion

        #region SubmitRating
        public async Task<RatingResponseDTO> SubmitRating(SubmitRatingRequestDTO ratingDTO)
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
                    RatingResponseDTO response = new RatingResponseDTO()
                    {
                        ResponseMsg = "Invalid ride id",
                        ResponseResult = ResponseResult.Warning
                    };
                    logger.LogWarning("Invalid ride id");
                    return response;
                }
                if(ride.StatusId != 4)
                {
                    RatingResponseDTO response = new RatingResponseDTO()
                    {
                        ResponseMsg = "Cannot rate rides which are not complete",
                        ResponseResult = ResponseResult.Warning
                    };
                    return response;
                }
                if (driver == null)
                {
                    if(customer.Id != ride.CustomerId)
                    {
                        RatingResponseDTO response = new RatingResponseDTO()
                        {
                            ResponseMsg = "Cannot rate a ride of another customer",
                            ResponseResult = ResponseResult.Warning
                        };
                        return response;
                    }
                    if(ride.DriverRating != null)
                    {
                        RatingResponseDTO response = new RatingResponseDTO()
                        {
                            ResponseMsg = "Ride already rated",
                            ResponseResult = ResponseResult.Warning
                        };
                        return response;
                    }
                    driver = await rideRepository.GetDriverById((int)ride.DriverId);
                    ride.DriverRating = (decimal)ratingDTO.Rating;
                    driver.Rating = ((driver.Rating * (driver.TotalRides - 1)) + (decimal)ratingDTO.Rating) / driver.TotalRides;
                    await rideRepository.UpdateRide(ride);
                    RatingResponseDTO responseDto = new RatingResponseDTO()
                    {
                        ResponseMsg = "Rating added successfully",
                        ResponseResult = ResponseResult.Success
                    };
                    return responseDto;
                }
                if(customer == null)
                {
                    if(driver.Id != ride.DriverId)
                    {
                        RatingResponseDTO responseDTO = new RatingResponseDTO()
                        {
                            ResponseMsg = "Cannot rate a ride of other driver",
                            ResponseResult = ResponseResult.Warning
                        };
                        return responseDTO;
                    }
                    if (ride.CustomerRating != null)
                    {
                        RatingResponseDTO responseDTO = new RatingResponseDTO()
                        {
                            ResponseMsg = "Ride already rated",
                            ResponseResult = ResponseResult.Warning
                        };
                        return responseDTO;
                    }
                    customer = await rideRepository.GetCustomerByID(ride.CustomerId);
                    ride.CustomerRating = (decimal)ratingDTO.Rating;
                    customer.Rating = ((customer.Rating * (customer.TotalRides - 1)) + (decimal)ratingDTO.Rating) / customer.TotalRides;
                    await rideRepository.UpdateRide(ride);
                    RatingResponseDTO response = new RatingResponseDTO()
                    {
                        ResponseMsg = "Rating added successfully",
                        ResponseResult = ResponseResult.Success
                    };
                    return response;
                }
                RatingResponseDTO responseRating = new RatingResponseDTO()
                {
                    ResponseMsg = "Some error occurred",
                    ResponseResult = ResponseResult.Exception
                };
                logger.LogWarning("Some error occurred");
                return responseRating;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in SubmitRating: {ex.Message}");
                RatingResponseDTO response = new RatingResponseDTO()
                {
                    ResponseMsg = $"Error in SubmitRating: {ex.Message}",
                    ResponseResult = ResponseResult.Exception
                };
                return response;
            }
        }
        #endregion
    }
}
