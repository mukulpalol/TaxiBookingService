using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiBookingServices.API.Auth.AuthDataContract
{
    #region UserAddDTO
    public class UserAddDTO
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
        [StringLength(50, MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@$&_])[A-Za-z\d@$!%*?&]{8,}$", ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, one special character(@,$,&,_), and be at least 8 characters long.")]
        public string Password { get; set; }

        [Required]
        [RegularExpression(@"^\d{4}\-(0[1-9]|1[012])\-(0[1-9]|[12][0-9]|3[01])$", ErrorMessage = "Enter valid date")]
        [DataType(DataType.Date, ErrorMessage = "Enter valid date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public string Dob { get; set; }

        [Required]
        [RegularExpression("^([mM][aA][lL][eE]|[fF][eE][mM][aA][lL][eE])", ErrorMessage = "Enter valid gender")]
        public string Gender { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"\d{10}", ErrorMessage = "Enter valid phone number")]
        public string PhoneNumber { get; set; }
    }
    #endregion
}
