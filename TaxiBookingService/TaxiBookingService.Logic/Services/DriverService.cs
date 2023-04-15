using Microsoft.Extensions.Logging;
using TaxiBookingService.Common;
using TaxiBookingService.DAL.Repositories;
using TaxiBookingServices.API.Service_Contract;

namespace TaxiBookingService.Logic.Services
{
    public interface IDriverService
    {
        Task<ResponseBase> UpdateLocation(UpdateLocationRequestDTO updateLocation);
        Task<ResponseBase> UpdateAvailability(bool availability);
        Task<BookRideDriverResponseDTO> GetRideRequest();
    }

    public class DriverService : IDriverService
    {
        private readonly IDriverRepository driverRepository;
        private readonly IUserRepository userRepository;
        private readonly IUserService userService;
        private readonly ILogger<DriverService> logger;
        private readonly ResponseBase responseBase;

        #region Constructor
        public DriverService(IDriverRepository driverRepository, IUserRepository userRepository, IUserService userService, ILogger<DriverService> logger)
        {
            this.driverRepository = driverRepository;
            this.userRepository = userRepository;
            this.userService = userService;
            this.logger = logger;
            responseBase = new ResponseBase();

        }
        #endregion

        #region UpdateLocation
        public async Task<ResponseBase> UpdateLocation(UpdateLocationRequestDTO updateLocation)
        {
            try
            {
                int locationId = updateLocation.LocationId;
                if ((await userRepository.LocationExists(locationId)) == null)
                {
                    responseBase.ResponseMsg = "Location does not exist";
                    responseBase.ResponseResult = ResponseResult.Warning;
                    logger.LogWarning("Error in UpdateLocation: Location does not exist");
                    return responseBase;
                }
                var claim = userService.GetCurrentUser();
                var UserEmail = claim.Email;
                var user = await userRepository.UserEmailExists(UserEmail);
                if (user == null)
                {
                    responseBase.ResponseMsg = "User is null";
                    responseBase.ResponseResult = ResponseResult.Warning;
                    logger.LogWarning("Error in UpdateLocation: User is null");
                    return responseBase;
                }
                var driver = await driverRepository.DriverExist(user);
                if (driver == null)
                {
                    responseBase.ResponseMsg = "Driver is null";
                    responseBase.ResponseResult = ResponseResult.Warning;
                    logger.LogWarning("Error in UpdateLocation: Driver is null");
                    return responseBase;
                }
                await driverRepository.UpdateLocation(locationId, driver);
                responseBase.ResponseMsg = "Location updated successfully";
                responseBase.ResponseResult = ResponseResult.Success;
                return responseBase;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in UpdateLocation: {ex.Message}");
                responseBase.ResponseMsg = $"Error in UpdateLocation: {ex.Message}";
                responseBase.ResponseResult = ResponseResult.Exception;
                return responseBase;
            }
        }
        #endregion

        #region UpdateAvailability
        public async Task<ResponseBase> UpdateAvailability(bool availability)
        {
            try
            {
                var claim = userService.GetCurrentUser();
                var UserEmail = claim.Email;
                var user = await userRepository.UserEmailExists(UserEmail);
                if (user == null)
                {
                    responseBase.ResponseMsg = "User is null";
                    responseBase.ResponseResult = ResponseResult.Warning;
                    logger.LogError("Error in UpdateLocation: User is null");
                    return responseBase;
                }
                var driver = await driverRepository.DriverExist(user);
                if (driver == null)
                {
                    responseBase.ResponseMsg = "Driver is null";
                    responseBase.ResponseResult = ResponseResult.Warning;
                    logger.LogError("Error in UpdateLocation: Driver is null");
                    return responseBase;
                }
                await driverRepository.UpdateAvailability(availability, driver);
                responseBase.ResponseMsg = "Availability updated successfully";
                responseBase.ResponseResult = ResponseResult.Success;
                return responseBase;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in UpdateAvailability: {ex.Message}");
                responseBase.ResponseMsg = $"Error in UpdateAvailability: {ex.Message}";
                responseBase.ResponseResult = ResponseResult.Exception;
                return responseBase;
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
                var driver = driverRepository.DriverExist(user);
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
