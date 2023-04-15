using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiBookingService.DAL.Models
{
    public partial class Role
    {
        public Role()
        {
            Users = new HashSet<User>();
        }

        [Key]
        public int Id { get; set; }

        [Column(TypeName = "varchar(20)")]
        public string RoleType { get; set; }

        public bool Deleted { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
