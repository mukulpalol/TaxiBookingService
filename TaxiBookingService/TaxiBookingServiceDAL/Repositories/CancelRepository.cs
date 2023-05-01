using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiBookingService.Common;
using TaxiBookingService.DAL.Models;
using TaxiBookingService.DAL.RepositoriesContract;

namespace TaxiBookingService.DAL.Repositories
{
    public class CancelRepository : ICancelRepository
    {
        private readonly TbsDbContext db;

        #region Constructor
        public CancelRepository(TbsDbContext db)
        {
            this.db = db;
        }
        #endregion

        #region GetCancelReason
        public async Task<CancelReason> GetCancelReason(int cancelReasonId)
        {
            var cancelReason = await db.CancelReasons.FirstOrDefaultAsync(c => c.Id == cancelReasonId);
            return cancelReason;
        }
        #endregion

        #region GetCancellationFactor
        public async Task<decimal> GetCancellationFactor()
        {
            var setting = await db.Settings.FirstOrDefaultAsync(s => s.Id == (int)SettingsEnum.CancellationFactor);
            return setting.Value;
        }
        #endregion
    }
}
