using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiBookingServices.API.Service_Contract
{
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
}
