using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TaxiBookingService.Common;
using TaxiBookingService.Logic.Services;
using TaxiBookingServices.API.Service_Contract;

namespace TaxiBookingService.Controller.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ResponseBase responseBase;
        private readonly IRideService rideService;
        private readonly ILogger<CustomerController> logger;

        #region Constructor
        public CustomerController(IRideService rideService, ILogger<CustomerController> logger)
        {
            responseBase = new ResponseBase();
            this.rideService = rideService;
            this.logger = logger;
        }
        #endregion

        #region BookRide
        [HttpPost]
        public async Task<ResponseBase> BookRide(BookRideRequestDTO riderequest)
        {
            try
            {
                logger.LogInformation("BookRide called");
                var result = await rideService.BookRide(riderequest);
                return result;
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

        #region ViewRideStatus
        [HttpGet]
        public async Task<CustomerViewRideResponseDTO> ViewRideStatus()
        {
            try
            {
                logger.LogInformation("ViewRideStatus called");
                var response = await rideService.CustomerViewRideStatus();
                return response;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in ViewRideStatus: {ex.Message}");
                CustomerViewRideResponseDTO response = new CustomerViewRideResponseDTO()
                {
                    ResponseMsg = $"Error in ViewRideStatus: {ex.Message}",
                    ResponseResult = ResponseResult.Exception
                };
                return response;
            }
        }
        #endregion

        #region CancelRide
        [HttpPost]
        public async Task<ResponseBase> CancelRide(CancelRideRequestDTO cancelRideRequest)
        {
            try
            {
                logger.LogInformation("CancelRide called");
                var response = await rideService.CancelRide(cancelRideRequest);
                return response;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in CancelRide: {ex.Message}");
                responseBase.ResponseMsg = $"Error in CancelRide: {ex.Message}";
                responseBase.ResponseResult = ResponseResult.Exception;
                return responseBase;
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
