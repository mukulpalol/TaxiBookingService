using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiBookingService.DAL.Models
{
    public partial class Driver
    {
        public Driver()
        {
            Rides = new HashSet<Ride>();
            RidesDeclined = new HashSet<RidesDeclined>();
        }

        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        [ForeignKey("Location")]
        public int LocationId { get; set; }

        [Column(TypeName = "varchar(20)")]
        public string DrivingLicenseNumber { get; set; }

        [ForeignKey("Vehicle")]
        public int VehicleId { get; set; }

        [Column(TypeName = "decimal(2,1)")]
        public decimal Rating { get; set; }

        public int TotalRides { get; set; }

        public bool Available { get; set; }

        public bool Deleted { get; set; }
        
        public virtual Location Location { get; set; }
        public virtual User User { get; set; }
        public virtual Vehicle Vehicle { get; set; }
        public virtual ICollection<Ride> Rides { get; set; }
        public virtual ICollection<RidesDeclined> RidesDeclined { get; set; }
    }
}
