using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiBookingService.DAL.Models
{
    public partial class Customer
    {
        public Customer()
        {
            Rides = new HashSet<Ride>();
        }

        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        [Column(TypeName = "money")]
        public decimal AmountDue { get; set; }

        [Column(TypeName = "decimal(2,1)")]
        public decimal Rating { get; set; }

        public int TotalRides { get; set; }

        public bool Deleted { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<Ride> Rides { get; set; }
    }
}
