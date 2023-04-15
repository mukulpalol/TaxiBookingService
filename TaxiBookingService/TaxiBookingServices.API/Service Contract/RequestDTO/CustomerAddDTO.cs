using System.ComponentModel.DataAnnotations;

namespace TaxiBookingServices.API.Service_Contract
{
    public class CustomerAddDTO
    {
        [Required]
        [RegularExpression("^[a-zA-Z]{1,}$", ErrorMessage = "Enter a valid name")]
        [StringLength(25, MinimumLength = 2, ErrorMessage = "First name should be between 4-25")]
        public string FirstName { get; set; }

        [RegularExpression("^[a-zA-Z]{1,}$", ErrorMessage = "Enter a valid name")]
        [StringLength(25, MinimumLength = 2, ErrorMessage = "First name should be between 4-25")]
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
    }
}
