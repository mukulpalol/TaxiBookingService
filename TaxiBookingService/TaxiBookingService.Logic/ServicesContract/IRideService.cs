using TaxiBookingService.DAL.Models;
using TaxiBookingServices.API.Customer.CustomerServiceContract;
using TaxiBookingServices.API.Driver.DriverServiceContract;

namespace TaxiBookingService.Logic.ServicesContract
{
    public interface IRideService
    {        
        Task<BookRideResponseDTO> BookRide(BookRideRequestDTO rideRequest);
        Task<CustomerViewRideResponseDTO> CustomerViewRideStatus();
        Task<RideAcceptResponseDTO> DriverRideAccept(int rideID, bool accept);
        Task<DriverViewRideResponseDTO> DriverViewRideRequest();
        Task<RideStartedResponseDTO> RideStarted(RideIdRequestDTO rideStarted);
        Task<RideCompleteResponseDTO> RideCompleted(RideIdRequestDTO rideCompleted);
        Task<CancelRideResponseDTO> CancelRide(CancelRideRequestDTO cancelRideRequest);
        Task<RatingResponseDTO> DriverSubmitRating(SubmitRatingRequestDTO ratingDTO);
        Task<RatingResponseDTO> CustomerSubmitRating(SubmitRatingRequestDTO ratingDTO);
    }
}
