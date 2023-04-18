using Microsoft.Extensions.Logging;
using TaxiBookingService.Common;
using TaxiBookingService.DAL.Repositories;
using TaxiBookingServices.API.DriverContract;

namespace TaxiBookingService.Logic.Services
{
    public interface IDriverService
    {
        Task<UpdateLocationResponseDTO> UpdateLocation(UpdateLocationRequestDTO updateLocation);
        Task<UpdateAvailabilityResponseDTO> UpdateAvailability(bool availability);
        Task<BookRideDriverResponseDTO> GetRideRequest();
    }

    public class DriverService : IDriverService
    {
        private readonly IDriverRepository driverRepository;
        private readonly IUserRepository userRepository;
        private readonly IRideRepository rideRepository;
        private readonly IUserService userService;
        private readonly ILogger<DriverService> logger;
        private readonly ResponseBase responseBase;

        #region Constructor
        public DriverService(IDriverRepository driverRepository, IUserRepository userRepository, IRideRepository rideRepository, IUserService userService, ILogger<DriverService> logger)
        {
            this.driverRepository = driverRepository;
            this.userRepository = userRepository;
            this.rideRepository = rideRepository;
            this.userService = userService;
            this.logger = logger;
            responseBase = new ResponseBase();

        }
        #endregion

        #region UpdateLocation
        public async Task<UpdateLocationResponseDTO> UpdateLocation(UpdateLocationRequestDTO updateLocation)
        {
            try
            {
                int locationId = updateLocation.LocationId;
                if ((await userRepository.LocationExists(locationId)) == null)
                {
                    UpdateLocationResponseDTO response = new UpdateLocationResponseDTO()
                    {
                        ResponseMsg = "Location does not exist",
                        ResponseResult = ResponseResult.Warning
                    };
                    logger.LogWarning("Error in UpdateLocation: Location does not exist");
                    return response;
                }
                var claim = userService.GetCurrentUser();
                var UserEmail = claim.Email;
                var user = await userRepository.UserEmailExists(UserEmail);
                if (user == null)
                {
                    UpdateLocationResponseDTO response = new UpdateLocationResponseDTO()
                    {
                        ResponseMsg = "User is null",
                        ResponseResult = ResponseResult.Warning
                    };
                    logger.LogWarning("Error in UpdateLocation: User is null");
                    return response;
                }
                var driver = await driverRepository.DriverExist(user);
                var ride = await rideRepository.GetDriverRide(driver.Id);
                if (ride != null)
                {
                    var response = new UpdateLocationResponseDTO()
                    {
                        ResponseMsg = "Cannot update location when in a ride",
                        ResponseResult = ResponseResult.Warning
                    };
                    return response;
                }
                if (driver == null)
                {
                    UpdateLocationResponseDTO response = new UpdateLocationResponseDTO()
                    {
                        ResponseMsg = "Driver is null",
                        ResponseResult = ResponseResult.Warning
                    };
                    logger.LogWarning("Error in UpdateLocation: Driver is null");
                    return response;
                }
                await driverRepository.UpdateLocation(locationId, driver);
                UpdateLocationResponseDTO responseDto = new UpdateLocationResponseDTO()
                {
                    ResponseMsg = "Location updated successfully",
                    ResponseResult = ResponseResult.Success
                };
                return responseDto;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in UpdateLocation: {ex.Message}");
                UpdateLocationResponseDTO responseDto = new UpdateLocationResponseDTO()
                {
                    ResponseMsg = $"Error in UpdateLocation: {ex.Message}",
                    ResponseResult = ResponseResult.Exception
                };
                return responseDto;
            }
        }
        #endregion

        #region UpdateAvailability
        public async Task<UpdateAvailabilityResponseDTO> UpdateAvailability(bool availabilityRequest)
        {
            try
            {
                var claim = userService.GetCurrentUser();
                var UserEmail = claim.Email;
                var user = await userRepository.UserEmailExists(UserEmail);
                if (user == null)
                {
                    var response = new UpdateAvailabilityResponseDTO()
                    {
                        ResponseMsg = "User is null",
                        ResponseResult = ResponseResult.Warning
                    };
                    logger.LogError("Error in UpdateLocation: User is null");
                    return response;
                }
                var driver = await driverRepository.DriverExist(user);
                var ride = await rideRepository.GetDriverRide(driver.Id);
                if (ride != null)
                {
                    var response = new UpdateAvailabilityResponseDTO()
                    {
                        ResponseMsg = "Cannot update availability when in a ride",
                        ResponseResult = ResponseResult.Warning
                    };
                    return response;
                }
                if (driver == null)
                {
                    var response = new UpdateAvailabilityResponseDTO()
                    {
                        ResponseMsg = "Driver is null",
                        ResponseResult = ResponseResult.Warning
                    };
                    logger.LogError("Error in UpdateLocation: Driver is null");
                    return response;
                }
                await driverRepository.UpdateAvailability(availabilityRequest, driver);
                var responseDto = new UpdateAvailabilityResponseDTO()
                {
                    ResponseMsg = "Availability updates successfully",
                    ResponseResult = ResponseResult.Success
                };
                return responseDto;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in UpdateAvailability: {ex.Message}");
                var response = new UpdateAvailabilityResponseDTO()
                {
                    ResponseMsg = $"Error in UpdateAvailability: {ex.Message}",
                    ResponseResult = ResponseResult.Warning
                };
                responseBase.ResponseMsg = $"Error in UpdateAvailability: {ex.Message}";
                responseBase.ResponseResult = ResponseResult.Exception;
                return response;
            }
        }
        #endregion

        #region GetRideRequest
        public async Task<BookRideDriverResponseDTO> GetRideRequest()
        {
            try
            {
                var claim = userService.GetCurrentUser();
                if(claim == null)
                {
                    BookRideDriverResponseDTO rideResponse = new BookRideDriverResponseDTO()
                    {
                        ResponseMsg = ("No user claim available"),
                        ResponseResult = ResponseResult.Warning
                    };
                    logger.LogWarning("No user claim available");
                    return rideResponse;
                }
                var user = await userRepository.UserEmailExists(claim.Email);
                if(user == null)
                {
                    BookRideDriverResponseDTO rideResponse = new BookRideDriverResponseDTO()
                    {
                        ResponseMsg = ("No user available"),
                        ResponseResult = ResponseResult.Warning
                    };
                    logger.LogWarning("No user available");
                    return rideResponse;
                }
                var driver = await driverRepository.DriverExist(user);
                if(driver == null)
                {
                    BookRideDriverResponseDTO rideResponse = new BookRideDriverResponseDTO()
                    {
                        ResponseMsg = ("No driver of this id"),
                        ResponseResult = ResponseResult.Warning
                    };
                    logger.LogWarning("No driver of this id");
                    return rideResponse;
                }
                var ride = await driverRepository.GetRideByDriver(driver.Id);
                if(ride == null)
                {
                    BookRideDriverResponseDTO rideResponse = new BookRideDriverResponseDTO()
                    {
                        ResponseMsg = ("No ride available"),
                        ResponseResult = ResponseResult.Warning
                    };
                    logger.LogWarning("No ride available");
                    return rideResponse;
                }
                BookRideDriverResponseDTO driverResponse = new BookRideDriverResponseDTO()
                {
                    PickupLocationId = ride.PickUpId,
                    DropoffLocationId = ride.DropId,
                    ResponseResult = ResponseResult.Success
                };
                return driverResponse;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in GetRideRequest: {ex.Message}");
                BookRideDriverResponseDTO rideResponse = new BookRideDriverResponseDTO()
                {
                    ResponseMsg = $"Error in GetRideRequest: {ex.Message}",
                    ResponseResult = ResponseResult.Exception
                };
                return rideResponse;
            }
        }
        #endregion
    }
}
