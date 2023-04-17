using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TaxiBookingService.Common;
using TaxiBookingService.Logic.Services;
using TaxiBookingServices.API.CustomerContract;
using TaxiBookingServices.API.DriverContract;

namespace TaxiBookingService.Controller.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Roles = "Driver")]
    public class DriverController : ControllerBase
    {
        private readonly ResponseBase responseBase;
        private readonly IDriverService driverService;
        private readonly IRideService rideService;
        private readonly ILogger<DriverController> logger;

        #region Constructor
        public DriverController(IDriverService driverService, IRideService rideService, ILogger<DriverController> logger)
        {
            responseBase = new ResponseBase();
            this.driverService = driverService;
            this.rideService = rideService;
            this.logger = logger;
        }
        #endregion

        #region UpdateLocation
        [HttpPost]
        public async Task<UpdateLocationResponseDTO> UpdateLocation(UpdateLocationRequestDTO updateLocation)
        {
            try
            {
                logger.LogInformation("UpdateLocation called");
                var response = await driverService.UpdateLocation(updateLocation);
                return response;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in UpdateLocation: {ex.Message}");
                UpdateLocationResponseDTO response = new UpdateLocationResponseDTO()
                {
                    ResponseMsg = $"Error in UpdateLocation: {ex.Message}",
                    ResponseResult = ResponseResult.Exception
                };
                return response;
            }
        }
        #endregion

        #region UpdateAvailability
        [HttpPost]
        public async Task<UpdateAvailabilityResponseDTO> UpdateAvailability(UpdateAvailabilityRequestDTO availability)
        {
            try
            {
                logger.LogInformation("UpdateAvailability called");
                var response = await driverService.UpdateAvailability(availability);
                return response;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in UpdateAvailability: {ex.Message}");
                var response = new UpdateAvailabilityResponseDTO()
                {
                    ResponseMsg = $"Error in UpdateAvailability: {ex.Message}",
                    ResponseResult = ResponseResult.Exception
                };
                return response;
            }
        }
        #endregion

        #region GetRideRequest
        [HttpGet]
        public async Task<DriverViewRideResponseDTO> GetRideRequest()
        {
            try
            {
                logger.LogInformation("GetRideRequest called");
                var response = await rideService.DriverViewRideRequest();
                return response;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in GetRideRequest: {ex.Message}");
                DriverViewRideResponseDTO response = new DriverViewRideResponseDTO()
                {
                    ResponseMsg = $"Error in GetRideRequest: {ex.Message}",
                    ResponseResult = ResponseResult.Exception
                };
                return response;
            }
        }
        #endregion

        #region RideAccept
        [HttpPost]
        public async Task<RideAcceptResponseDTO> RideAccept(RideAcceptRequestDTO rideAccept)
        {
            try
            {
                logger.LogInformation("RideAccept called");
                var response = await rideService.DriverRideAccept(rideAccept);
                return response;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in RideAccept: {ex.Message}");
                RideAcceptResponseDTO response = new RideAcceptResponseDTO()
                {
                    ResponseMsg = $"Error in RideAccept: {ex.Message}",
                    ResponseResult = ResponseResult.Exception
                };
                return response;
            }
        }
        #endregion

        #region RideStarted
        [HttpPost]
        public async Task<RideStartedResponseDTO> RideStarted(RideIdRequestDTO rideStarted)
        {
            try
            {
                logger.LogInformation("RideStarted called");
                var response = await rideService.RideStarted(rideStarted);
                return response;
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

        #region RideCompleted
        [HttpPost]
        public async Task<RideCompleteResponseDTO> RideCompleted(RideIdRequestDTO rideCompleted)
        {
            try
            {
                logger.LogInformation("RideCompleted called");
                var response = await rideService.RideCompleted(rideCompleted);
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

        #region SubmitRating
        [HttpPost]
        public async Task<RatingResponseDTO> SubmitRating(SubmitRatingRequestDTO submitRatingRequest)
        {
            try
            {
                logger.LogInformation("SubmitRating called");
                var response = await rideService.SubmitRating(submitRatingRequest);
                return response;
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
