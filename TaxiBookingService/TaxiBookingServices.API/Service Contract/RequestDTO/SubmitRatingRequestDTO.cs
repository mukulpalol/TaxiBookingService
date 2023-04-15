using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiBookingServices.API.Service_Contract
{
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
}
