using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiBookingService.DAL.Models
{
    public partial class Ride
    {
        public Ride()
        {
            RidesDeclined = new HashSet<RidesDeclined>();
        }

        [Key]
        public int Id { get; set; }

        [ForeignKey("Customer")]
        public int CustomerId { get; set; }

        [ForeignKey("Driver")]
        public int? DriverId { get; set; }

        [ForeignKey("PickUp")]
        public int PickUpId { get; set; }

        [ForeignKey("Drop")]
        public int DropId { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? PickUpTime { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? DropTime { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? EstimatedPickUpTime { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? EstimatedDropTime { get; set; }

        public TimeSpan? Duration { get; set; }

        [Column(TypeName = "decimal(5,1)")]
        public decimal Distance { get; set; }

        [Column(TypeName = "money")]
        public decimal? Amount { get; set; }

        [ForeignKey("Payment")]
        public int? PaymentId { get; set; }

        [ForeignKey("Status")]
        public int StatusId { get; set; }

        [Column(TypeName = "decimal(2,1)")]   
        public decimal? DriverRating { get; set; }

        [Column(TypeName = "decimal(2,1)")]
        public decimal? CustomerRating { get; set; }

        [ForeignKey("CancelReason")]
        public int? CancelReasonId { get; set; }

        public bool Deleted { get; set; }

        public virtual CancelReason? CancelReason { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual Driver? Driver { get; set; }
        public virtual Location Drop { get; set; }
        public virtual Payment? Payment { get; set; }
        public virtual Location PickUp { get; set; }
        public virtual Status Status { get; set; }
        public virtual ICollection<RidesDeclined> RidesDeclined { get; set; }
    }
}
