using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiBookingService.Common;

namespace TaxiBookingServices.API.Customer.CustomerServiceContract
{
    #region CustomerViewRideResponse
    public class CustomerViewRideResponseDTO : ResponseBase
    {
        public int RideId { get; set; }
        public int DriverId { get; set; }
        public DateTime? ETA { get; set; }
    }
    #endregion

    #region BookRideResponse
    public class BookRideResponseDTO : ResponseBase { }
    #endregion

    #region CancelRideResponse
    public class CancelRideResponseDTO : ResponseBase { }
    #endregion


}
