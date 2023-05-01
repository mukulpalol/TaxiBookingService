using Microsoft.Extensions.Logging;
using TaxiBookingService.Common;
using TaxiBookingService.DAL.RepositoriesContract;
using TaxiBookingService.Logic.ServicesContract;
using TaxiBookingServices.API.Driver.DriverServiceContract;

namespace TaxiBookingService.Logic.Services
{
    public class DriverService : IDriverService
    {
        private readonly IDriverRepository driverRepository;
        private readonly IUserRepository userRepository;
        private readonly IRideRepository rideRepository;
        private readonly IUserService userService;
        private readonly ILogger<DriverService> logger;

        #region Constructor
        public DriverService(IDriverRepository driverRepository, IUserRepository userRepository, IRideRepository rideRepository, IUserService userService, ILogger<DriverService> logger)
        {
            this.driverRepository = driverRepository;
            this.userRepository = userRepository;
            this.rideRepository = rideRepository;
            this.userService = userService;
            this.logger = logger;

        }
        #endregion

        #region UpdateLocation
        public async Task<UpdateLocationResponseDTO> UpdateLocation(UpdateLocationRequestDTO updateLocation)
        {
            UpdateLocationResponseDTO response = new UpdateLocationResponseDTO();
            try
            {
                int locationId = updateLocation.LocationId;
                var location = await userRepository.LocationExists(locationId);
                location = CheckValidation.NullCheck(location, "Location does not exist");
                var claim = userService.GetCurrentUser();
                var UserEmail = claim.Email;
                var user = await userRepository.UserEmailExists(UserEmail);
                user = CheckValidation.NullCheck(user, "User does not exist");
                var driver = await driverRepository.DriverExist(user);
                var ride = await rideRepository.GetOngoingDriverRide(driver.Id);
                ride = CheckValidation.NotNullCheck(ride, "Cannot update location when in a ride");
                driver = CheckValidation.NullCheck(driver, "Driver does not exist");
                await driverRepository.UpdateLocation(locationId, driver);
                response.ResponseMsg = "Location updated successfully";
                response.ResponseResult = ResponseResult.Success;
                return response;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in UpdateLocation: {ex.Message}");
                response.ResponseMsg = $"Error in UpdateLocation: {ex.Message}";
                response.ResponseResult = ResponseResult.Exception;
                return response;
            }
        }
        #endregion
        
        #region UpdateAvailability
        public async Task<UpdateAvailabilityResponseDTO> UpdateAvailability(bool availabilityRequest)
        {
            UpdateAvailabilityResponseDTO response = new UpdateAvailabilityResponseDTO();
            try
            {
                var claim = userService.GetCurrentUser();
                var UserEmail = claim.Email;
                var user = await userRepository.UserEmailExists(UserEmail);
                user = CheckValidation.NullCheck(user, "User does not exist");
                var driver = await driverRepository.DriverExist(user);
                var ride = await rideRepository.GetOngoingDriverRide(driver.Id);
                ride = CheckValidation.NotNullCheck(ride, "Cannot update availability when in a ride");
                driver = CheckValidation.NullCheck(driver, "Driver does not exist");
                await driverRepository.UpdateAvailability(availabilityRequest, driver);
                response.ResponseMsg = "Availability updates successfully";
                response.ResponseResult = ResponseResult.Success;
                return response;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in UpdateAvailability: {ex.Message}");
                response.ResponseMsg = $"Error in UpdateAvailability: {ex.Message}";
                response.ResponseResult = ResponseResult.Warning;
                return response;
            }
        }
        #endregion        
        
        #region GetRideRequest
        public async Task<BookRideDriverResponseDTO> GetRideRequest()
        {
            BookRideDriverResponseDTO response = new BookRideDriverResponseDTO();
            try
            {                
                var claim = userService.GetCurrentUser();
                claim = CheckValidation.NullCheck(claim, "No user logged in");
                var user = await userRepository.UserEmailExists(claim.Email);
                user = CheckValidation.NullCheck(user, "No user available");
                var driver = await driverRepository.DriverExist(user);
                driver = CheckValidation.NullCheck(driver, "No driver found");
                var ride = await driverRepository.GetRideByDriver(driver.Id);
                ride = CheckValidation.NullCheck(ride, "No ride available");
                response.PickupLocationId = ride.PickUpId;
                response.DropoffLocationId = ride.DropId;
                response.ResponseResult = ResponseResult.Success;
                return response;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in GetRideRequest: {ex.Message}");
                response.ResponseMsg = $"Error in GetRideRequest: {ex.Message}";
                response.ResponseResult = ResponseResult.Exception;
                return response;
            }
        }
        #endregion        
    }
}
