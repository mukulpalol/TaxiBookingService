using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiBookingServices.API.CustomerContract
{
    #region BookRideRequest
    public class BookRideRequestDTO
    {
        [Required]
        [RegularExpression("^[0-9]{1,}$", ErrorMessage = "Enter valid pickup id")]
        public int PickupLocationId { get; set; }

        [Required]
        [RegularExpression("^[0-9]{1,}$", ErrorMessage = "Enter valid drop id")]
        public int DropLocationId { get; set; }

        [Required]
        [RegularExpression("^[0-9]{1,}$", ErrorMessage = "Enter valid vehicle type id")]
        public int VehicleTypeId { get; set; }
    }
    #endregion

    #region CancelRideRequest
    public class CancelRideRequestDTO
    {
        [Required]
        [RegularExpression("^[0-9]{1,}$", ErrorMessage = "Enter valid ride id")]
        public int RideId { get; set; }

        [Required]
        [RegularExpression("^[0-9]{1,}$", ErrorMessage = "Enter valid cancellation reason id")]
        public int CancelReasonId { get; set; }
    }
    #endregion

    #region SubmitRatingRequest
    public class SubmitRatingRequestDTO
    {
        [Required]
        [RegularExpression("^[0-9]{1,}$", ErrorMessage = "Enter valid ride id")]
        public int RideId { get; set; }

        [Required]
        [RegularExpression("^[1-5]{1}$", ErrorMessage = "Enter valid rating")]
        [Range(1, 5)]
        public int Rating { get; set; }
    }
    #endregion
}
