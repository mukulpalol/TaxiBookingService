using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaxiBookingService.DAL.Models
{
    public partial class City
    {
        public City()
        {
            Areas = new HashSet<Area>();
        }

        [Key]
        public int Id { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string Name { get; set; }

        [ForeignKey("State")]
        public int StateId { get; set; }

        public bool Deleted { get; set; }

        public virtual State State { get; set; }

        public virtual ICollection<Area> Areas { get; set; }
    }
}
