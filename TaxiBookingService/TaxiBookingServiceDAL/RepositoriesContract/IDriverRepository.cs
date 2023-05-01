using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiBookingService.DAL.Models;

namespace TaxiBookingService.DAL.RepositoriesContract
{
    public interface IDriverRepository
    {
        Task<Driver> DriverExist(User user);
        Task<Vehicle> VehicleByDriverId(Driver driver);
        Task UpdateLocation(int locationId, Driver driver);
        Task UpdateAvailability(bool availability, Driver driver);
        Task<Ride> GetRideByDriver(int driverId);
        Task<Driver> GetDriverById(int driverId);
    }
}
