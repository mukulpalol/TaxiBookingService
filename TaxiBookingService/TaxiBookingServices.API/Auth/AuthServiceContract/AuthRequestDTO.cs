using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiBookingServices.API.Auth.AuthDataContract;

namespace TaxiBookingServices.API.Auth.AuthServiceContract
{
    #region LoginRequestDTO
    public class LoginRequestDTO
    {
        [Required]
        [DataType(DataType.EmailAddress, ErrorMessage = "Enter valid email address")]
        [RegularExpression("^[a-zA-Z0-9._]+@[a-zA-Z0-9]+\\.[a-zA-Z]{2,}$", ErrorMessage = "Enter valid email address")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
    #endregion

    #region CustomerAddDTO
    public class CustomerAddDTO
    {
        [Required]
        public UserAddDTO User { get; set; }
    }
    #endregion

    #region DriverAddDTO
    public class DriverAddDTO
    {
        [Required]
        public UserAddDTO User { get; set; }

        [Required]
        public int LocationId { get; set; }

        [Required]
        [RegularExpression("^[A-Z]{2}[0-9]{2}(19|20)\\d{2}\\d{7}$", ErrorMessage = "Enter valid license number")]
        public string DrivingLicenseNumber { get; set; }

        [Required]
        public int VehicleTypeId { get; set; }

        [Required]
        [RegularExpression("^[A-Z]{2}[0-9]{2}[A-Z]{2}[0-9]{4}$", ErrorMessage = "Enter valid vehicle number")]
        public string VehicleNumber { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 4)]
        public string ModelName { get; set; }
    }
    #endregion
}
