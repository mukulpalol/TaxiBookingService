using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaxiBookingService.Common;
using TaxiBookingService.DAL.Models;
using TaxiBookingService.DAL.RepositoriesContract;
using TaxiBookingService.Logic.ServicesContract;
using TaxiBookingServices.API.Auth.AuthServiceContract;

namespace TaxiBookingService.Logic.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository authRepository;
        private readonly IConfiguration configuration;
        private readonly IUserService userService;
        private readonly ILogger<AuthService> logger;

        #region Constructor
        public AuthService(IAuthRepository authRepository, IConfiguration configuration, IUserService userService, ILogger<AuthService> logger)
        {
            this.authRepository = authRepository;
            this.configuration = configuration;
            this.userService = userService;
            this.logger = logger;
        }
        #endregion

        #region AuthenticateUser
        public async Task<LoginResponseDTO> AuthenticateUser(LoginRequestDTO userLogin)
        {
            try
            {
                var currentUser = await authRepository.UserExist(userLogin);
                currentUser = CheckValidation.NullCheck(currentUser, "User Does not exist");
                var authorizeResponse = await AuthorizeUser(currentUser);
                LoginResponseDTO response = new LoginResponseDTO()
                {
                    ResponseMsg = authorizeResponse.ResponseMsg,
                    ResponseResult = authorizeResponse.ResponseResult,
                    AccessToken = authorizeResponse.AccessToken
                };
                return response;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in AuthenticateUser: {ex.Message}");
                LoginResponseDTO response = new LoginResponseDTO()
                {
                    ResponseMsg = $"Error in AuthenticateUser: {ex.Message}",
                    ResponseResult = ResponseResult.Exception
                };
                return response;
            }
        }
        #endregion
        
        #region AuthorizeUser
        public async Task<LoginResponseDTO> AuthorizeUser(User userLogin)
        {
            LoginResponseDTO response = new LoginResponseDTO();
            try
            {
                int roleId = userLogin.RoleId;
                var role = await authRepository.GetRole(roleId);
                role = CheckValidation.NullCheck(role, "No role found");
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
                response.ResponseMsg = "User authorized successfully";
                response.ResponseResult = ResponseResult.Success;
                response.AccessToken = accessToken;                
                return response;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in AuthorizeUser: {ex.Message}");
                response.ResponseMsg = $"Error in AuthorizeUser: {ex.Message}";
                response.ResponseResult = ResponseResult.Exception;                
                return response;
            }
        }
        #endregion
        
        #region RefreshToken
        public async Task<LoginResponseDTO> RefreshToken()
        {
            LoginResponseDTO response = new LoginResponseDTO();
            try
            {
                var claim = userService.GetCurrentUser();
                if (claim.ResponseResult != ResponseResult.Success)
                {
                    logger.LogWarning(claim.ResponseMsg);
                    response.ResponseMsg = claim.ResponseMsg;
                    response.ResponseResult = claim.ResponseResult;                    
                    return response;
                }
                var user = await authRepository.GetUser(claim.Email);
                user = CheckValidation.NullCheck(user, "User does not exist");
                var responseDTO = await AuthorizeUser(user);
                return responseDTO;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in RefreshToken: {ex.Message}");
                response.ResponseMsg = $"Error in RefreshToken: {ex.Message}";
                response.ResponseResult = ResponseResult.Exception;                
                return response;
            }
        }
        #endregion        
    }
}
