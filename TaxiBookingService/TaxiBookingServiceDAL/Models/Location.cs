using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiBookingService.DAL.Models
{
    public partial class Location
    {
        public Location()
        {
            Drivers = new HashSet<Driver>();
            RideDrops = new HashSet<Ride>();
            RidePickUps = new HashSet<Ride>();
        }

        [Key]
        public int Id { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string StreetName { get; set; }

        [Column(TypeName = "decimal(8,6)")]
        public decimal Latitude { get; set; }

        [Column(TypeName = "decimal(9,6)")]
        public decimal Longitude { get; set; }

        [ForeignKey("Area")]
        public int AreaId { get; set; }

        public bool Deleted { get; set; }

        public virtual Area Area { get; set; }
        public virtual ICollection<Driver> Drivers { get; set; }
        public virtual ICollection<Ride> RideDrops { get; set; }
        public virtual ICollection<Ride> RidePickUps { get; set; }
    }
}
