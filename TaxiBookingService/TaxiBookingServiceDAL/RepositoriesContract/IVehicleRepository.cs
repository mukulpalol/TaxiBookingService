using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiBookingService.DAL.Models;

namespace TaxiBookingService.DAL.RepositoriesContract
{
    public interface IVehicleRepository
    {
        Task<VehicleType> GetVehicleType(int VehicleTypeId);
        Task<Vehicle> VehicleById(int VehicleId);
    }
}
