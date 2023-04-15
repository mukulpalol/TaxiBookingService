using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaxiBookingService.Common;
using TaxiBookingService.DAL.Models;
using TaxiBookingService.DAL.Repositories;
using TaxiBookingServices.API.Service_Contract;

namespace TaxiBookingService.Logic.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDTO> AuthenticateUser(LoginRequestDTO userLogin);
        Task<LoginResponseDTO> AuthorizeUser(User userLogin);
    }

    public class AuthService : IAuthService
    {
        private readonly IAuthRepository authRepository;
        private readonly IConfiguration configuration;
        private readonly ILogger<AuthService> logger;

        #region Constructor
        public AuthService(IAuthRepository authRepository, IConfiguration configuration, ILogger<AuthService> logger)
        {
            this.authRepository = authRepository;
            this.configuration = configuration;
            this.logger = logger;
        }
        #endregion

        #region AuthenticateUser
        public async Task<LoginResponseDTO> AuthenticateUser(LoginRequestDTO userLogin)
        {
            try
            {
                var currentUser = await authRepository.UserExist(userLogin);
                var userName = await authRepository.EmailExist(userLogin);
                if (currentUser != null)
                {
                    var authorizeResponse = await AuthorizeUser(currentUser);
                    LoginResponseDTO response = new LoginResponseDTO()
                    {
                        ResponseMsg = authorizeResponse.ResponseMsg,
                        ResponseResult = authorizeResponse.ResponseResult,
                        AccessToken = authorizeResponse.AccessToken
                    };
                    return response;
                }
                else if (userName != null)
                {
                    logger.LogError("Error in AuthenticateUser: Password is incorrect");
                    LoginResponseDTO response = new LoginResponseDTO()
                    {
                        ResponseMsg = "Error in AuthenticateUser: Password is incorrect",
                        ResponseResult = ResponseResult.Warning
                    };
                    return response;
                }
                else
                {
                    logger.LogError("Error in AuthenticateUser: User does not exist");
                    LoginResponseDTO response = new LoginResponseDTO()
                    {
                        ResponseMsg = "Error in AuthenticateUser: User does not exist",
                        ResponseResult = ResponseResult.Warning
                    };
                    return response;
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in AuthenticateUser: {ex.Message}");
                LoginResponseDTO response = new LoginResponseDTO()
                {
                    ResponseMsg = "Error in AuthenticateUser: {ex.Message}",
                    ResponseResult = ResponseResult.Exception
                };
                return response;
            }
        }
        #endregion

        #region AuthorizeUser
        public async Task<LoginResponseDTO> AuthorizeUser(User userLogin)
        {
            try
            {
                int roleId = userLogin.RoleId;
                var role = await authRepository.GetRole(roleId);
                if (role == null)
                {
                    logger.LogError("Error in AuthorizeUser: No role found");
                    LoginResponseDTO response = new LoginResponseDTO()
                    {
                        ResponseMsg = "No role found",
                        ResponseResult = ResponseResult.Warning
                    };
                    return response;
                }
                var claims = new[]
                {
                new Claim(ClaimTypes.Email,userLogin.Email),
                new Claim(ClaimTypes.Role, role.RoleType)
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: credentials);

                var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

                LoginResponseDTO responseBase = new LoginResponseDTO()
                {
                    ResponseMsg = "User authorized successfully",
                    ResponseResult = ResponseResult.Success,
                    AccessToken = accessToken
                };
                return responseBase;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in AuthorizeUser: {ex.Message}");
                LoginResponseDTO response = new LoginResponseDTO()
                {
                    ResponseMsg = $"Error in AuthorizeUser: {ex.Message}",
                    ResponseResult = ResponseResult.Exception
                };
                return response;
            }
        }
        #endregion
    }
}
