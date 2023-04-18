using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiBookingService.DAL.Models
{
    public partial class Status
    {
        public Status()
        {
            Rides = new HashSet<Ride>();
        }

        [Key]
        public int Id { get; set; }

        [Column(TypeName = "varchar(20)")]
        public string StatusType { get; set; }

        public bool Deleted { get; set; }

        public virtual ICollection<Ride> Rides { get; set; }
    }
}
