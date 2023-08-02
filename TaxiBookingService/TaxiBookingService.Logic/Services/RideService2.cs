using Microsoft.Extensions.Logging;
using TaxiBookingService.DAL.RepositoriesContract;
using TaxiBookingService.Logic.ServicesContract;
using TaxiBookingServices.API.Customer.CustomerServiceContract;

namespace TaxiBookingService.Logic.Services
{
    public interface IRideService2
    {
        Task<BookRideResponseDTO> BookRide(BookRideRequestDTO rideRequest);
    }
    public interface IRideServiceBike : IRideService2
    {

    }
    public interface IRideServiceCar : IRideService2
    {

    }
    public class RideServiceBike : IRideServiceBike
    {

        public async Task<BookRideResponseDTO> BookRide(BookRideRequestDTO rideRequest)
        {
            BookRideResponseDTO response = new BookRideResponseDTO();
            response.ResponseResult = Common.ResponseResult.Success;
            response.ResponseMsg = "Called from 2nd implementation of BookRide";
            return response;
        }
    }
    public class RideServiceCar : IRideServiceCar
    {
        public async Task<BookRideResponseDTO> BookRide(BookRideRequestDTO rideRequest)
        {
            BookRideResponseDTO response = new BookRideResponseDTO();
            response.ResponseResult = Common.ResponseResult.Success;
            response.ResponseMsg = "Called from 1st implementation of BookRide";
            return response;
        }
    }
}
