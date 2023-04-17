using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaxiBookingService.DAL.Models;

namespace TaxiBookingService.DAL.Repositories
{
    public interface IRideRepository
    {
        Task<VehicleType> GetVehicleType(int VehicleTypeId);
        Task<Location> GetLocation(int LocationId);
        Task<Driver> GetBestDriver(int areaID, int vehicleTypeID, int rideId);
        Task<Ride> InsertRide(Ride ride);
        Task RideReload(Ride ride);
        Task<RidesDeclined> InsertRidesDecline(RidesDeclined ridesDeclined);
        Task<bool> RideCompleted(User user);
        Task<Ride> GetDriverRide(int driverId);
        Task<Ride> GetDriverLatestRide(int driverID, int rideID);
        Task<Ride> GetDriverRideForComplete(int driverID, int rideId);
        Task<Ride> GetCustomerRide(Customer customer);
        Task UpdateRide(Ride ride);
        Task UpdateRidesDeclined(int driverId, int rideId);
        Task<Customer> GetCustomerByID(int customerId);
        Task<Ride> GetRide(Driver driver);
        Task<Payment> InsertPayment(Payment payment);
        Task<Ride> GetRideById(int rideId);
        Task<Driver> GetDriverById (int driverId);
        Task<CancelReason> GetCancelReason(int cancelReasonId);
        Task<decimal> GetCancellationFactor();
    }

    public class RideRepository : IRideRepository
    {
        private readonly TbsDbContext db;

        #region Constructor
        public RideRepository(TbsDbContext db)
        {
            this.db = db;
        }
        #endregion

        #region GetVehicleType
        public async Task<VehicleType> GetVehicleType(int VehicleTypeId)
        {
            return await db.VehicleTypes.FirstOrDefaultAsync(o => o.Id == VehicleTypeId);
        }
        #endregion

        #region GetLocation
        public async Task<Location> GetLocation(int LocationId)
        {
            return await db.Locations.FirstOrDefaultAsync(o => o.Id == LocationId);
        }
        #endregion

        #region GetBestDriver
        public async Task<Driver> GetBestDriver(int areaID, int vehicleTypeID, int rideId)
        {
            var driver = await db.Drivers.Where(
                d => d.Location.AreaId == areaID &&
                d.Vehicle.VehicleTypeId == vehicleTypeID &&
                !d.RidesDeclined.Any(o => o.RideId == rideId) &&
                d.Available == true)
                .OrderByDescending(o => o.Rating)
                .FirstOrDefaultAsync();
            return driver;
        }
        #endregion

        #region InsertRide
        public async Task<Ride> InsertRide(Ride ride)
        {
            var result = await db.Rides.AddAsync(ride);
            await db.SaveChangesAsync();
            return result.Entity;
        }
        #endregion

        #region UpdateRide
        public async Task UpdateRide(Ride ride)
        {
            //ride.DriverId = driverId;
            await db.SaveChangesAsync();
        }
        #endregion

        #region InsertRidesDeclined
        public async Task<RidesDeclined> InsertRidesDecline(RidesDeclined ridesDeclined)
        {
            var result = await db.RidesDeclined.AddAsync(ridesDeclined);
            await db.SaveChangesAsync();
            return result.Entity;
        }
        #endregion

        #region UpdateRidesDeclined
        public async Task UpdateRidesDeclined(int driverId, int rideId)
        {
            RidesDeclined ridesDeclined = new RidesDeclined()
            {
                DriverId = driverId,
                RideId = rideId
            };
            await db.SaveChangesAsync();
        }
        #endregion

        #region GetCustomerById
        public async Task<Customer> GetCustomerByID(int customerId)
        {
            var customer = await db.Customers.FirstOrDefaultAsync(c => c.Id == customerId);
            return customer;
        }
        #endregion

        #region GetDriverByID
        public async Task<Driver> GetDriverById(int driverId)
        {
            var driver = await db.Drivers.FirstOrDefaultAsync(d=>d.Id == driverId);
            return driver;
        }
        #endregion

        #region GetDriverRide
        public async Task<Ride> GetDriverRide(int driverId)
        {
            var ride = await db.Rides.FirstOrDefaultAsync(r => r.DriverId == driverId && r.StatusId == 1);
            return ride;
        }
        #endregion

        #region GetDriverLatestRideForStart
        public async Task<Ride> GetDriverLatestRide(int driverID, int rideId)
        {
            var ride = await db.Rides.Where(r =>r.Id == rideId && r.DriverId == driverID && r.StatusId == 2).FirstOrDefaultAsync();
            return ride;
        }
        #endregion

        #region GetDriverLatestRideForComplete
        public async Task<Ride> GetDriverRideForComplete(int driverID, int rideId)
        {
            var ride = await db.Rides.Where(r => r.Id == rideId && r.DriverId == driverID && r.StatusId == 3).FirstOrDefaultAsync();
            return ride;
        }
        #endregion

        #region GetCustomerRide
        public async Task<Ride> GetCustomerRide(Customer customer)
        {
            var ride = await db.Rides.Where(r => r.StatusId == 1 || r.StatusId == 2 || r.StatusId == 3).FirstOrDefaultAsync(r => r.CustomerId == customer.Id);
            return ride;
        }
        #endregion

        #region RideReload
        public async Task RideReload(Ride ride)
        {
            await db.Entry(ride).ReloadAsync();
            //return ride;                        
        }
        #endregion

        #region RideCompleted
        public async Task<bool> RideCompleted(User user)
        {
            var customer = await db.Customers.FirstOrDefaultAsync(x => x.UserId == user.Id);
            var eligible = await (db.Rides.AnyAsync(u => u.CustomerId == customer.Id && (u.StatusId == 1 || u.StatusId == 2 || u.StatusId == 3)));
            return eligible;
        }
        #endregion

        #region GetRideById
        public async Task<Ride> GetRideById(int rideId)
        {
            var ride = await db.Rides.FirstOrDefaultAsync(r => r.Id == rideId);
            return ride;
        }
        #endregion

        #region GetRide
        public async Task<Ride> GetRide(Driver driver)
        {
            var ride = await db.Rides.Where(r => r.DriverId == driver.Id).OrderByDescending(r => r.Id).FirstOrDefaultAsync();
            return ride;
        }
        #endregion

        #region GetCancelReason
        public async Task<CancelReason> GetCancelReason(int cancelReasonId)
        {
            var cancelReason = await db.CancelReasons.FirstOrDefaultAsync(c => c.Id == cancelReasonId);
            return cancelReason;
        }
        #endregion

        #region InsertPayment
        public async Task<Payment> InsertPayment(Payment payment)
        {
            var result = await db.Payments.AddAsync(payment);
            await db.SaveChangesAsync();
            return result.Entity;
        }
        #endregion

        #region GetCancellationFactor
        public async Task<decimal> GetCancellationFactor()
        {
            var setting = await db.Settings.FirstOrDefaultAsync(s => s.Id == 1);
            return setting.Value;
        }
        #endregion
    }
}
