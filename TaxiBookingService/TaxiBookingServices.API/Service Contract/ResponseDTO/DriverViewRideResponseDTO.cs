using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiBookingService.Common;

namespace TaxiBookingServices.API.Service_Contract
{
    public class DriverViewRideResponseDTO : ResponseBase
    {
        public int RideId { get; set; }
        public int PickUpId { get; set; }
        public int DropId { get; set; }
    }
}
