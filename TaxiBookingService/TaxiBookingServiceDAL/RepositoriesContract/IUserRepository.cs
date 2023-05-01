using TaxiBookingService.DAL.Models;

namespace TaxiBookingService.DAL.RepositoriesContract
{
    public interface IUserRepository
    {
        Task<User> UserEmailExists(string email);
        Task<User> UserPhoneExists(string phone);
        Task<Vehicle> VehicleExists(string vehicleNumber);
        Task<Location> LocationExists(int locationId);
        Task<Area> AreaExists(int areaId);
        Task<Customer> CustomerExist(User user);
        Task AddCustomer(User user, Customer customer);
        Task AddDriver(Vehicle vehicle, User user, Driver driver);
        Task UpdateDatabase();
    }
}
