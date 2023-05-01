using Microsoft.EntityFrameworkCore;
using TaxiBookingService.Common;
using TaxiBookingService.DAL.Models;
using TaxiBookingService.DAL.RepositoriesContract;

namespace TaxiBookingService.DAL.Repositories
{
    public class RideRepository : IRideRepository
    {
        private readonly TbsDbContext db;

        #region Constructor
        public RideRepository(TbsDbContext db)
        {
            this.db = db;
        }
        #endregion

        #region GetLocation
        public async Task<Location> GetLocation(int LocationId)
        {
            return await db.Locations.FirstOrDefaultAsync(o => o.Id == LocationId);
        }
        #endregion

        #region GetBestDriver
        public async Task<Driver> GetBestDriver(int vehicleTypeId, int rideId)
        {
            var ride = await db.Rides.FirstOrDefaultAsync(r => r.Id == rideId);
            var pickUpLocation = await db.Locations.FirstOrDefaultAsync(u => u.Id == ride.PickUpId);
            var driverRange = await db.Settings.FirstOrDefaultAsync(d => d.Id == (int)SettingsEnum.DriverRange);
            var availableDrivers = db.Drivers.Where(d => d.Available == true &&
                                                    d.Vehicle.VehicleTypeId == vehicleTypeId &&
                                                    !d.RidesDeclined.Any(U => U.RideId == rideId))
                                             .ToList();
            var bestDriver = availableDrivers.Select(d => new
            {
                Driver = d,
                Distance = CalculateCoordinatesDistance.CalculateDistance(pickUpLocation.Latitude, pickUpLocation.Longitude, GetLocation(d.LocationId).Result.Latitude, GetLocation(d.LocationId).Result.Longitude)
            }).OrderBy(d => d.Distance).ThenByDescending(d=>d.Driver.Rating).FirstOrDefault(d => d.Distance <= (double)driverRange.Value)?.Driver;
            return bestDriver;
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

        #region InsertRidesDeclined
        public async Task<RidesDeclined> InsertRidesDecline(RidesDeclined ridesDeclined)
        {
            var result = await db.RidesDeclined.AddAsync(ridesDeclined);
            await db.SaveChangesAsync();
            return result.Entity;
        }
        #endregion        

        #region GetCustomerById
        public async Task<Customer> GetCustomerByID(int customerId)
        {
            var customer = await db.Customers.FirstOrDefaultAsync(c => c.Id == customerId);
            return customer;
        }
        #endregion

        #region GetDriverRide
        public async Task<Ride> GetDriverRide(int driverId)
        {
            var ride = await db.Rides.FirstOrDefaultAsync(r => r.DriverId == driverId && r.StatusId == (int)RideStatus.Searching);
            return ride;
        }
        #endregion

        #region GetDriverRide
        public async Task<Ride> GetOngoingDriverRide(int driverId)
        {
            var ride = await db.Rides.FirstOrDefaultAsync(r => r.DriverId == driverId && (r.StatusId == (int)RideStatus.Searching || r.StatusId == (int)RideStatus.Booked || r.StatusId == (int)RideStatus.RideStarted));
            return ride;
        }
        #endregion

        #region GetDriverLatestRideForStart
        public async Task<Ride> GetDriverLatestRide(int driverID, int rideId)
        {
            var ride = await db.Rides.Where(r => r.Id == rideId && r.DriverId == driverID && r.StatusId == (int)RideStatus.Booked).FirstOrDefaultAsync();
            return ride;
        }
        #endregion

        #region GetDriverLatestRideForComplete
        public async Task<Ride> GetDriverRideForComplete(int driverID, int rideId)
        {
            var ride = await db.Rides.FirstOrDefaultAsync(r => r.Id == rideId && r.DriverId == driverID && r.StatusId == (int)RideStatus.RideStarted);
            return ride;
        }
        #endregion

        #region GetCustomerRide
        public async Task<Ride> GetCustomerRide(Customer customer)
        {
            var ride = await db.Rides.Where(r => r.StatusId == (int)RideStatus.Searching || r.StatusId == (int)RideStatus.Booked || r.StatusId == (int)RideStatus.RideStarted).FirstOrDefaultAsync(r => r.CustomerId == customer.Id);
            return ride;
        }
        #endregion

        #region RideCompleted
        public async Task<bool> RideCompleted(User user)
        {
            var customer = await db.Customers.FirstOrDefaultAsync(x => x.UserId == user.Id);
            var eligible = await db.Rides.AnyAsync(u => u.CustomerId == customer.Id && (u.StatusId == (int)RideStatus.Searching || u.StatusId == (int)RideStatus.Booked || u.StatusId == (int)RideStatus.RideStarted));
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

        #region CustomerRatedRideCount
        public async Task<int> CustomerRatedRideCount(int CustomerId)
        {
            var count = await db.Rides.Where(r=>r.CustomerId == CustomerId && r.CustomerRating != null).CountAsync();
            return count;
        }
        #endregion

        #region DriverRatedRideCount
        public async Task<int> DriverRatedRideCount(int DriverId)
        {
            var count = await db.Rides.Where(r => r.DriverId == DriverId && r.DriverRating != null).CountAsync();
            return count;
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
    }
}
