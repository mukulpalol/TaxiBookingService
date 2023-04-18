using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiBookingService.DAL.Models
{
    public partial class VehicleType
    {
        public VehicleType()
        {
            Vehicles = new HashSet<Vehicle>();
        }

        [Key]
        public int Id { get; set; }

        [Column(TypeName = "varchar(25)")]
        public string Type { get; set; }

        public int Capacity { get; set; }
        
        public int AverageSpeed { get; set; }

        public int FareFactor { get; set; }

        public bool Deleted { get; set; }
        public virtual ICollection<Vehicle> Vehicles { get; set; }
    }
}
