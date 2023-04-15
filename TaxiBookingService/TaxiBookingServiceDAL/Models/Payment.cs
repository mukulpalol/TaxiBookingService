using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiBookingService.DAL.Models
{
    public partial class Payment
    {
        public Payment()
        {
            Rides = new HashSet<Ride>();
        }

        [Key]
        public int Id { get; set; }

        [Column(TypeName = "money")]
        public decimal Amount { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime PaymentTimeStamp { get; set; }

        public virtual ICollection<Ride> Rides { get; set; }
    }
}
