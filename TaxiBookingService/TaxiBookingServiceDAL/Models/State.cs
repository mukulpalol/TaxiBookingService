using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiBookingService.DAL.Models
{
    public partial class State
    {
        public State()
        {
            Cities = new HashSet<City>();
        }

        [Key]
        public int Id { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string Name { get; set; }

        public bool Deleted { get; set; }

        public virtual ICollection<City> Cities { get; set; }
    }
}
