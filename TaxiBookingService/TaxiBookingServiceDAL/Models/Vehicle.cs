using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiBookingService.DAL.Models
{
    public partial class Vehicle
    {
        public Vehicle()
        {
            Drivers = new HashSet<Driver>();
        }

        [Key]
        public int Id { get; set; }

        [ForeignKey("VehicleType")]
        public int VehicleTypeId { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string VehicleNumber { get; set; }

        [Column(TypeName = "varchar(20)")]
        public string ModelName { get; set; }

        public bool Deleted { get; set; }

        public virtual VehicleType VehicleType { get; set; }
        public virtual ICollection<Driver> Drivers { get; set; }
    }
}
