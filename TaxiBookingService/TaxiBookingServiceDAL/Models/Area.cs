using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiBookingService.DAL.Models
{
    public partial class Area
    {
        public Area()
        {
            Locations = new HashSet<Location>();
        }

        [Key]
        public int Id { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string Name { get; set; }

        [ForeignKey("City")]
        public int CityId { get; set; }

        public bool Deleted { get; set; }

        public virtual City City { get; set; }

        public virtual ICollection<Location> Locations { get; set; }
    }
}
