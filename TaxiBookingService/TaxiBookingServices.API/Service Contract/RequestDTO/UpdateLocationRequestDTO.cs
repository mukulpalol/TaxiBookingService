using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiBookingServices.API.Service_Contract
{
    public class UpdateLocationRequestDTO
    {
        [Required]
        [RegularExpression("^[1-9][0-9]*$", ErrorMessage ="Enter valid location id")]
        public int LocationId { get; set; }
    }
}
