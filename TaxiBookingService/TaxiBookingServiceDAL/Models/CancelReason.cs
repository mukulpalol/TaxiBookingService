using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiBookingService.DAL.Models
{
    public partial class CancelReason
    {
        public CancelReason()
        {
            Rides = new HashSet<Ride>();
        }

        [Key]
        public int Id { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string CancelReasons { get; set; }

        public bool ValidReason { get; set; }

        public bool Deleted { get; set; }

        public virtual ICollection<Ride> Rides { get; set; }
    }
}
