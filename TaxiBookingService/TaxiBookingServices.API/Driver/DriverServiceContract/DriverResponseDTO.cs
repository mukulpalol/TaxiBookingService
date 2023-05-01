using TaxiBookingService.Common;

namespace TaxiBookingServices.API.Driver.DriverServiceContract
{
    #region BookRideDriverResponse
    public class BookRideDriverResponseDTO : ResponseBase
    {
        public int PickupLocationId { get; set; }
        public int DropoffLocationId { get; set; }
    }
    #endregion

    #region DriverViewRideResponse
    public class DriverViewRideResponseDTO : ResponseBase
    {
        public int RideId { get; set; }
        public int PickUpId { get; set; }
        public int DropId { get; set; }
    }
    #endregion

    #region RideCompletedResponse
    public class RideCompleteResponseDTO : ResponseBase
    {
        public decimal RideFare { get; set; }
    }
    #endregion

    #region UpdateLocationResponse
    public class UpdateLocationResponseDTO : ResponseBase { }
    #endregion

    #region UpdateAvailabilityResponse
    public class UpdateAvailabilityResponseDTO : ResponseBase { }
    #endregion

    #region RideAcceptResponse
    public class RideAcceptResponseDTO : ResponseBase { }
    #endregion

    #region RideStartedResponse
    public class RideStartedResponseDTO : ResponseBase { }
    #endregion

    #region RatingResponse
    public class RatingResponseDTO : ResponseBase { }
    #endregion

}
