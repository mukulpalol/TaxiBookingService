using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TaxiBookingService.Common;
using TaxiBookingService.Logic.ServicesContract;
using TaxiBookingServices.API.Customer.CustomerServiceContract;

namespace TaxiBookingService.Controller.Controllers
{
    [Route("api/Customer/[action]")]
    [ApiController]
    [ApiVersion("2.0")]
    [Authorize(Roles = "Customer")]
    public class CustomerV2Controller : ControllerBase
    {
        private readonly IRideService rideService;
        private readonly ILogger<CustomerController> logger;

        public CustomerV2Controller(IRideService rideService, ILogger<CustomerController> logger)
        {
            this.rideService = rideService;
            this.logger = logger;
        }

        [HttpPost]
        public async Task<BookRideResponseDTO> BookRide(BookRideRequestDTO riderequest)
        {
            try
            {
                logger.LogInformation("BookRide form api version 2 called");
                return new BookRideResponseDTO()
                {
                    ResponseMsg = "BookRide method called from version 2",
                    ResponseResult = Common.ResponseResult.Success
                };
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in BookRide: {ex.Message}");
                return new BookRideResponseDTO()
                {
                    ResponseMsg = $"Error in BookRide: {ex.Message}",
                    ResponseResult = ResponseResult.Exception
                };
            }
        }
    }
}
