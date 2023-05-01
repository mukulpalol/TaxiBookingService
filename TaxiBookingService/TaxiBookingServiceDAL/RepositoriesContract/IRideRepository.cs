using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiBookingService.DAL.Models;

namespace TaxiBookingService.DAL.RepositoriesContract
{
    public interface IRideRepository
    {
        Task<Location> GetLocation(int LocationId);
        Task<Driver> GetBestDriver(int vehicleTypeID, int rideId);
        Task<Ride> InsertRide(Ride ride);
        Task<RidesDeclined> InsertRidesDecline(RidesDeclined ridesDeclined);
        Task<bool> RideCompleted(User user);
        Task<Ride> GetDriverRide(int driverId);
        Task<Ride> GetOngoingDriverRide(int driverId);
        Task<Ride> GetDriverLatestRide(int driverID, int rideID);
        Task<Ride> GetDriverRideForComplete(int driverID, int rideId);
        Task<Ride> GetCustomerRide(Customer customer);
        Task<Customer> GetCustomerByID(int customerId);
        Task<int> CustomerRatedRideCount(int CustomerId);
        Task<int> DriverRatedRideCount(int DriverId);
        Task<Payment> InsertPayment(Payment payment);
        Task<Ride> GetRideById(int rideId);        
    }
}
