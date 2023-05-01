using Microsoft.EntityFrameworkCore;
using TaxiBookingService.DAL.Models;
using TaxiBookingService.DAL.RepositoriesContract;

namespace TaxiBookingService.DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly TbsDbContext db;

        #region Constructor
        public UserRepository(TbsDbContext db)
        {
            this.db = db;
        }
        #endregion

        #region UserEmailExists
        public async Task<User> UserEmailExists(string email)
        {
            var user = await db.Users.FirstOrDefaultAsync(x => x.Email == email);
            return user;
        }
        #endregion

        #region UserPhoneExists
        public async Task<User> UserPhoneExists(string phone)
        {
            var user = await db.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phone);
            return user;
        }
        #endregion

        #region VehicleExists
        public async Task<Vehicle> VehicleExists(string vehicleNumber)
        {
            var vehicle = await db.Vehicles.FirstOrDefaultAsync(x => x.VehicleNumber == vehicleNumber);
            return vehicle;
        }
        #endregion

        #region LocationExists
        public async Task<Location> LocationExists(int locationId)
        {
            var location = await db.Locations.FirstOrDefaultAsync(x => x.Id == locationId);
            return location;
        }
        #endregion

        #region AreaExists
        public async Task<Area> AreaExists(int areaId)
        {
            var area = await db.Areas.FirstOrDefaultAsync(x => x.Id == areaId);
            return area;
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
            await db.Users.AddAsync(user);
            await db.Customers.AddAsync(customer);
            await db.SaveChangesAsync();
        }
        #endregion

        #region AddDriver
        public async Task AddDriver(Vehicle vehicle, User user, Driver driver)
        {
            await db.Users.AddAsync(user);
            await db.Vehicles.AddAsync(vehicle);
            await db.Drivers.AddAsync(driver);
            await db.SaveChangesAsync();
        }
        #endregion

        #region UpdateDatabase
        public async Task UpdateDatabase()
        {
            await db.SaveChangesAsync();
        }
        #endregion
    }
}
