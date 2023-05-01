using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiBookingService.DAL.Models;
using TaxiBookingService.DAL.RepositoriesContract;

namespace TaxiBookingService.DAL.Repositories
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly TbsDbContext db;

        #region Constructor
        public VehicleRepository(TbsDbContext db)
        {
            this.db = db;
        }
        #endregion

        #region GetVehicleType
        public async Task<VehicleType> GetVehicleType(int VehicleTypeId)
        {
            return await db.VehicleTypes.FirstOrDefaultAsync(o => o.Id == VehicleTypeId);
        }
        #endregion

        #region VehicleByID
        public async Task<Vehicle> VehicleById(int VehicleId)
        {
            return await db.Vehicles.FirstOrDefaultAsync(o => o.Id == VehicleId);
        }
        #endregion
    }
}
