using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TaxiBookingService.Common;
using TaxiBookingService.Logic.Services;
using TaxiBookingServices.API.Service_Contract;

namespace TaxiBookingService.Controller.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IAuthService authService;
        private readonly ResponseBase responseBase;
        private readonly ILogger<LoginController> logger;

        #region Constructor
        public LoginController(IUserService userService, IAuthService authService, ILogger<LoginController> logger)
        {
            this.userService = userService;
            this.authService = authService;
            responseBase = new ResponseBase();
            this.logger = logger;
        }
        #endregion

        #region LoginUser
        [HttpPost]
        public async Task<LoginResponseDTO> LoginUser([FromBody] LoginRequestDTO userLogin)
        {
            try
            {
                logger.LogInformation("LoginUser called");
                var response = await authService.AuthenticateUser(userLogin);
                HttpContext.Session.SetString("Token", response.AccessToken);
                return response;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in LoginUser: {ex.Message}");
                LoginResponseDTO response = new LoginResponseDTO()
                {
                    ResponseMsg = $"Error in LoginUser: {ex.Message}",
                    ResponseResult = ResponseResult.Exception
                };
                return response;
            }
        }
        #endregion

        #region SignUpCustomer
        [HttpPost]
        public async Task<ResponseBase> SignUpCustomer(CustomerAddDTO customerAdd)
        {
            try
            {
                logger.LogInformation("SignUpCustomer called");
                var result = await userService.AddCustomer(customerAdd);
                return result;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in SignUpCustomer: {ex.Message}");
                responseBase.ResponseMsg = $"Error in SignUpCustomer: {ex.Message}";
                responseBase.ResponseResult = ResponseResult.Exception;
                return responseBase;
            }
        }
        #endregion

        #region SignUpDriver
        [HttpPost]
        public async Task<ResponseBase> SignUpDriver(DriverAddDTO driverAdd)
        {
            try
            {
                logger.LogInformation("SignUpDriver called");
                var result = await userService.AddDriver(driverAdd);
                return result;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in SignUpDriver: {ex.Message}");
                responseBase.ResponseMsg = $"Error in SignUpDriver: {ex.Message}";
                responseBase.ResponseResult = ResponseResult.Exception;
                return responseBase;
            }
        }
        #endregion
    }
}
