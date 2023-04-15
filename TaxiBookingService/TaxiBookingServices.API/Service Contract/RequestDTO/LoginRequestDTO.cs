using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiBookingServices.API.Service_Contract
{
    public class LoginRequestDTO
    {
        [Required]
        [DataType(DataType.EmailAddress, ErrorMessage ="Enter valid email address")]
        [RegularExpression("^[a-zA-Z0-9._]+@[a-zA-Z0-9]+\\.[a-zA-Z]{2,}$", ErrorMessage = "Enter valid email address")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
