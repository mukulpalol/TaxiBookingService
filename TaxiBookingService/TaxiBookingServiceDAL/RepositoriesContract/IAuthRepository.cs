using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiBookingService.DAL.Models;
using TaxiBookingServices.API.Auth.AuthServiceContract;

namespace TaxiBookingService.DAL.RepositoriesContract
{
    public interface IAuthRepository
    {
        Task<User> UserExist(LoginRequestDTO userLogin);
        Task<User> GetUser(string Email);
        Task<User> EmailExist(LoginRequestDTO userLogin);
        Task<Role> GetRole(int roleId);
    }
}
