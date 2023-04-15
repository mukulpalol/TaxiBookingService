using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TaxiBookingService.Common;
using TaxiBookingService.Logic.Services;
using TaxiBookingServices.API.Service_Contract;

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
        public async Task<ResponseBase> UpdateLocation(UpdateLocationRequestDTO updateLocation)
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
                responseBase.ResponseMsg = $"Error in UpdateLocation: {ex.Message}";
                responseBase.ResponseResult = ResponseResult.Exception;
                return responseBase;
            }
        }
        #endregion

        #region UpdateAvailability
        [HttpPost]
        public async Task<ResponseBase> UpdateAvailability(bool availability)
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
                responseBase.ResponseMsg = $"Error in UpdateAvailability: {ex.Message}";
                responseBase.ResponseResult = ResponseResult.Exception;
                return responseBase;
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
        public async Task<ResponseBase> RideAccept(int rideId, bool Accept)
        {
            try
            {
                logger.LogInformation("RideAccept called");
                var response = await rideService.DriverRideAccept(rideId, Accept);
                return response;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in RideAccept: {ex.Message}");
                responseBase.ResponseMsg = $"Error in RideAccept: {ex.Message}";
                responseBase.ResponseResult = ResponseResult.Exception;
                return responseBase;
            }
        }
        #endregion

        #region RideStarted
        [HttpPost]
        public async Task<ResponseBase> RideStarted(int rideId)
        {
            try
            {
                logger.LogInformation("RideStarted called");
                var response = await rideService.RideStarted(rideId);
                return response;
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

        #region RideCompleted
        [HttpPost]
        public async Task<RideCompleteResponseDTO> RideCompleted(int rideId)
        {
            try
            {
                logger.LogInformation("RideCompleted called");
                var response = await rideService.RideCompleted(rideId);
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
        public async Task<ResponseBase> SubmitRating(SubmitRatingRequestDTO submitRatingRequest)
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
                responseBase.ResponseMsg = $"Error in SubmitRating: {ex.Message}";
                responseBase.ResponseResult = ResponseResult.Exception;
                return responseBase;
            }
        }
        #endregion
    }
}
