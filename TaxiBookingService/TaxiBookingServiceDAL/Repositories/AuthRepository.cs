using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaxiBookingService.DAL.Models;
using TaxiBookingService.DAL.RepositoriesContract;
using TaxiBookingServices.API.Auth.AuthServiceContract;

namespace TaxiBookingService.DAL.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly TbsDbContext db;

        #region Constructor
        public AuthRepository(TbsDbContext db)
        {
            this.db = db;    
        }
        #endregion

        #region UserExist
        public async Task<User> UserExist(LoginRequestDTO userLogin)
        {
            var user =await db.Users.FirstOrDefaultAsync(x => x.Email == userLogin.Email && x.Password == userLogin.Password);
            return user;
        }
        #endregion

        #region GetUser
        public async Task<User> GetUser(string Email)
        {
            var user = await db.Users.FirstOrDefaultAsync(x => x.Email == Email);
            return user;
        }
        #endregion

        #region EmailExist
        public async Task<User> EmailExist(LoginRequestDTO userLogin)
        {
            var user = db.Users.FirstOrDefault(x => x.Email == userLogin.Email);
            return user;
        }
        #endregion

        #region GetRole
        public async Task<Role> GetRole(int roleId)
        {
            Role role = await Task.FromResult((Role)(db.Roles.FirstOrDefault(x => x.Id == roleId)));
            return role;
        }
        #endregion
    }
}
