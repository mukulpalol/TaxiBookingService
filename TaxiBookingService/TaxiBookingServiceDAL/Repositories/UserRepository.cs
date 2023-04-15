using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaxiBookingService.DAL.Models;
using TaxiBookingServices.API.Service_Contract;

namespace TaxiBookingService.DAL.Repositories
{
    public interface IUserRepository
    {
        Task<User> UserEmailExists(string email);
        Task<User> UserPhoneExists(string phone);
        Task<Vehicle> VehicleExists(string vehicleNumber);
        Task<Location> LocationExists(int locationId);
        Task<Customer> CustomerExist(User user);
        Task AddCustomer(User user, Customer customer);
        Task AddDriver(Vehicle vehicle, User user, Driver driver);
    }
    public class UserRepository : IUserRepository
    {
        private readonly TbsDbContext db;
        private readonly ILogger<UserRepository> logger;

        #region Constructor
        public UserRepository(TbsDbContext db, ILogger<UserRepository> logger)
        {
            this.db = db;
            this.logger = logger;
        }
        #endregion

        #region UserEmailExists
        public async Task<User> UserEmailExists(string email)
        {
            try
            {
                var user = await db.Users.FirstOrDefaultAsync(x => x.Email == email);
                return user;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in UserExists: {ex.Message}");
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region UserPhoneExists
        public async Task<User> UserPhoneExists(string phone)
        {
            try
            {
                var user = await db.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phone);
                return user;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in UserExists: {ex.Message}");
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region VehicleExists
        public async Task<Vehicle> VehicleExists(string vehicleNumber)
        {
            try
            {
                var vehicle = await db.Vehicles.FirstOrDefaultAsync(x => x.VehicleNumber == vehicleNumber);
                return vehicle;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in VehicleExists: {ex.Message}");
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region LocationExists
        public async Task<Location> LocationExists(int locationId)
        {
            try
            {
                var location = await db.Locations.FirstOrDefaultAsync(x => x.Id == locationId);
                return location;
            }
            catch(Exception ex)
            {
                logger.LogError($"Error in LocationExists: {ex.Message}");
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region CustomerExist
        public async Task<Customer> CustomerExist(User user)
        {
            var id = user.Id;
            var customer = await db.Customers.FirstOrDefaultAsync(x => x.UserId == id);
            return customer;
        }
        #endregion

        #region AddCustomer
        public async Task AddCustomer(User user, Customer customer)
        {
            try
            {              
                await db.Users.AddAsync(user);
                await db.Customers.AddAsync(customer);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in AddCustomer: {ex.Message}");
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region AddDriver
        public async Task AddDriver(Vehicle vehicle, User user,Driver driver)
        {
            try
            {
                await db.Users.AddAsync(user);
                await db.Vehicles.AddAsync(vehicle);
                await db.Drivers.AddAsync(driver);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in AddDriver: {ex.Message}");
                throw new Exception(ex.Message);
            }
        }
        #endregion
    }
}
