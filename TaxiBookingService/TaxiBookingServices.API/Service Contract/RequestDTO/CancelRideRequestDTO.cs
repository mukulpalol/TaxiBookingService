using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiBookingService.Common;

namespace TaxiBookingServices.API.Service_Contract
{
    public class CancelRideRequestDTO
    {
        [Required]
        [RegularExpression("^[0-9]{1,}$", ErrorMessage = "Enter valid ride id")]
        public int RideId { get; set; }

        [Required]
        [RegularExpression("^[0-9]{1,}$", ErrorMessage = "Enter valid cancellation reason id")]
        public int CancelReasonId { get; set; }
    }
}
