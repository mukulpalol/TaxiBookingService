using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiBookingServices.API.Driver.DriverServiceContract
{
    #region UpdateLocationRequest
    public class UpdateLocationRequestDTO
    {
        [Required]
        [RegularExpression("^[1-9][0-9]*$", ErrorMessage = "Enter valid location id")]
        public int LocationId { get; set; }
    }
    #endregion

    #region RideIdRequest
    public class RideIdRequestDTO
    {
        [Required]
        [RegularExpression("^[0-9]{1,}$", ErrorMessage = "Enter valid ride id")]
        public int RideId { get; set; }
    }
    #endregion


}
