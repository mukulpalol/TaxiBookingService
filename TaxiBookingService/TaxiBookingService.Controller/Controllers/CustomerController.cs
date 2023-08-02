using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TaxiBookingService.Common;
using TaxiBookingService.Logic.Services;
using TaxiBookingService.Logic.ServicesContract;
using TaxiBookingServices.API.Customer.CustomerServiceContract;
using TaxiBookingServices.API.Driver.DriverServiceContract;

namespace TaxiBookingService.Controller.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize(Roles = "Customer")]
    public class CustomerController : ControllerBase
    {
        private readonly IRideService rideService;
        private readonly ILogger<CustomerController> logger;

        #region Constructor
        public CustomerController(IRideService rideService, ILogger<CustomerController> logger)
        {
            this.rideService = rideService;
            this.logger = logger;
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

        #region BookRide
        [HttpPost]
        public async Task<BookRideResponseDTO> BookRide(BookRideRequestDTO riderequest)
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
                BookRideResponseDTO response = new BookRideResponseDTO()
                {
                    ResponseMsg = $"Error in BookRide: {ex.Message}",
                    ResponseResult = ResponseResult.Exception
                };
                return response;
            }
        }
        #endregion

        #region CancelRide
        [HttpPost]
        public async Task<CancelRideResponseDTO> CancelRide(CancelRideRequestDTO cancelRideRequest)
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
                CancelRideResponseDTO response = new CancelRideResponseDTO()
                {
                    ResponseMsg = $"Error in CancelRide: {ex.Message}",
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
                var response = await rideService.CustomerSubmitRating(submitRatingRequest);
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
