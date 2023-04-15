using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiBookingServices.API.Service_Contract
{
    public class DriverAddDTO
    {
        [Required]
        [RegularExpression("^[a-zA-Z]{1,}$", ErrorMessage = "Enter a valid name")]
        [StringLength(25, MinimumLength =2, ErrorMessage ="First name should be between 4-25")]
        public string FirstName { get; set; }

        [RegularExpression("^[a-zA-Z]{1,}$", ErrorMessage = "Enter a valid name")]
        [StringLength(25, MinimumLength = 1, ErrorMessage = "Last name should be between 4-25")]
        public string? LastName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress, ErrorMessage = "Enter valid email address")]
        [RegularExpression("^[a-zA-Z0-9._]+@[a-zA-Z0-9]+\\.[a-zA-Z]{2,}$", ErrorMessage = "Enter valid email address")]
        [StringLength(50)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(50, MinimumLength =8)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Dob { get; set; }

        [Required]
        [RegularExpression("^([mM][aA][lL][eE]|[fF][eE][mM][aA][lL][eE])", ErrorMessage = "Enter valid gender")]
        public string Gender { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"\d{10}", ErrorMessage = "Enter valid phone number")]
        public string PhoneNumber { get; set; }

        [Required]
        public int LocationId { get; set; }

        [Required]
        [RegularExpression("^[A-Z]{2}[0-9]{2}(19|20)\\d{2}\\d{7}$", ErrorMessage ="Enter valid license number")]
        public string DrivingLicenseNumber { get; set; }

        [Required]
        public int VehicleTypeId { get; set; }

        [Required]
        [RegularExpression("^[A-Z]{2}[0-9]{2}[A-Z]{2}[0-9]{4}$", ErrorMessage = "Enter valid vehicle number")]
        public string VehicleNumber { get; set; }

        [Required]
        [StringLength(20, MinimumLength =4)]
        public string ModelName { get; set; }
    }
}
