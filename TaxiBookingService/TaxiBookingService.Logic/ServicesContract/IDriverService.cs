using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiBookingServices.API.Driver.DriverServiceContract;

namespace TaxiBookingService.Logic.ServicesContract
{
    public interface IDriverService
    {
        Task<UpdateLocationResponseDTO> UpdateLocation(UpdateLocationRequestDTO updateLocation);
        Task<UpdateAvailabilityResponseDTO> UpdateAvailability(bool availability);
        Task<BookRideDriverResponseDTO> GetRideRequest();
    }
}
