using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TaxiBookingService.Common;
using TaxiBookingService.Logic.Services;
using TaxiBookingServices.API.LoginContract;

namespace TaxiBookingService.Controller.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IAuthService authService;
        private readonly ILogger<AuthController> logger;

        #region Constructor
        public AuthController(IUserService userService, IAuthService authService, ILogger<AuthController> logger)
        {
            this.userService = userService;
            this.authService = authService;
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
        public async Task<SignUpResponseDTO> SignUpCustomer(CustomerAddDTO customerAdd)
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
                SignUpResponseDTO response = new SignUpResponseDTO()
                {
                    ResponseMsg = $"Error in SignUpCustomer: {ex.Message}",
                    ResponseResult = ResponseResult.Exception
                };
                return response;
            }
        }
        #endregion

        #region SignUpDriver
        [HttpPost]
        public async Task<SignUpResponseDTO> SignUpDriver(DriverAddDTO driverAdd)
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
                SignUpResponseDTO response = new SignUpResponseDTO()
                {
                    ResponseMsg = $"Error in SignUpDriver: {ex.Message}",
                    ResponseResult = ResponseResult.Exception
                };
                return response;
            }
        }
        #endregion

        #region RefreshToken
        [Authorize]
        [HttpGet]
        public async Task<LoginResponseDTO> RefreshToken()
        {
            try
            {
                logger.LogInformation("RefreshToken called");
                var response = await authService.RefreshToken();
                HttpContext.Session.SetString("Token", response.AccessToken);
                return response;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in RefreshToken: {ex.Message}");
                LoginResponseDTO response = new LoginResponseDTO()
                {
                    ResponseMsg = $"Error in RefreshToken: {ex.Message}",
                    ResponseResult = ResponseResult.Exception
                };
                return response;
            }
        }
        #endregion
    }
}
