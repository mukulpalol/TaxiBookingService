using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaxiBookingService.DAL.Models;

namespace TaxiBookingService.DAL.Repositories
{
    public interface IDriverRepository
    {
        Task<Driver> DriverExist(User user);
        Task<Vehicle> VehicleByDriverId(Driver driver);
        Task UpdateLocation(int locationId, Driver driver);
        Task UpdateAvailability(bool availability, Driver driver);
        Task<Ride> GetRideByDriver(int driverId);
    }

    public class DriverRepository : IDriverRepository
    {
        private readonly TbsDbContext db;

        #region Constructor
        public DriverRepository(TbsDbContext db)
        {
            this.db = db;
        }
        #endregion

        #region DriverExist
        public async Task<Driver> DriverExist(User user)
        {
            var id = user.Id;
            var driver = await db.Drivers.FirstOrDefaultAsync(x => x.UserId == id);
            return driver;
        }
        #endregion

        #region VehicleByDriverId
        public async Task<Vehicle> VehicleByDriverId (Driver driver)
        {
            var vehicle = await db.Vehicles.FirstOrDefaultAsync(x => x.Id == driver.VehicleId);
            return vehicle;
        }
        #endregion

        #region UpdateLocation
        public async Task UpdateLocation(int locationId, Driver driver)
        {
            driver.LocationId = locationId;
            await db.SaveChangesAsync();
        }
        #endregion

        #region UpdateAvailability
        public async Task UpdateAvailability(bool availability, Driver driver)
        {
            driver.Available = availability;
            await db.SaveChangesAsync();
        }
        #endregion

        #region GetRideByDriver
        public async Task<Ride> GetRideByDriver(int driverId)
        {
            return await db.Rides.FirstOrDefaultAsync(r => r.DriverId == driverId);
        }
        #endregion
    }
}
