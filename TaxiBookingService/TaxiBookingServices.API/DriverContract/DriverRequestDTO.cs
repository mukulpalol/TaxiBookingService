using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiBookingServices.API.DriverContract
{
    #region UpdateLocationRequest
    public class UpdateLocationRequestDTO
    {
        [Required]
        [RegularExpression("^[1-9][0-9]*$", ErrorMessage = "Enter valid location id")]
        public int LocationId { get; set; }
    }
    #endregion

    #region UpdateAvailabilityRequest
    public class UpdateAvailabilityRequestDTO
    {
        [Required]
        [RegularExpression("^([t][r][u][e]|[f][a][l][s][e])", ErrorMessage = "Enter valid availability")]
        public bool Available { get; set; }
    }
    #endregion

    #region RideAcceptRequest
    public class RideAcceptRequestDTO
    {
        [Required]
        [RegularExpression("^[0-9]{1,}$", ErrorMessage = "Enter valid ride id")]
        public int RideId { get; set; }

        [Required]
        [RegularExpression("^([t][r][u][e]|[f][a][l][s][e])", ErrorMessage = "Enter valid availability")]
        public bool Accept { get; set; }
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
