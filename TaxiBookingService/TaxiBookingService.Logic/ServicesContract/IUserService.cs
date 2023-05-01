using TaxiBookingServices.API.Auth.AuthServiceContract;

namespace TaxiBookingService.Logic.ServicesContract
{
    public interface IUserService
    {
        Task<SignUpResponseDTO> AddCustomer(CustomerAddDTO customerAdd);
        Task<SignUpResponseDTO> AddDriver(DriverAddDTO driverAdd);
        ClaimResponseDTO GetCurrentUser();
    }
}
