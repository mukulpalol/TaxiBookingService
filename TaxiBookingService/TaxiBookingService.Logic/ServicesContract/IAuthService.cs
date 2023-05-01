using TaxiBookingService.DAL.Models;
using TaxiBookingServices.API.Auth.AuthServiceContract;

namespace TaxiBookingService.Logic.ServicesContract
{
    public interface IAuthService
    {
        Task<LoginResponseDTO> AuthenticateUser(LoginRequestDTO userLogin);
        Task<LoginResponseDTO> AuthorizeUser(User userLogin);
        Task<LoginResponseDTO> RefreshToken();
    }
}
