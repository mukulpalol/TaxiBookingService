using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiBookingService.DAL.Models
{
    public partial class User
    {
        public User()
        {
            Customers = new HashSet<Customer>();
            Drivers = new HashSet<Driver>();
        }

        [Key]
        public int Id { get; set; }

        [Column(TypeName = "varchar(25)")]
        public string FirstName { get; set; }

        [Column(TypeName = "varchar(25)")]
        public string? LastName { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string Email { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string Password { get; set; }

        [Column(TypeName = "date")]
        public DateTime Dob { get; set; }

        [Column(TypeName = "varchar(6)")]
        public string Gender { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string PhoneNumber { get; set; }

        public bool Deleted { get; set; }

        [ForeignKey("Role")]
        public int RoleId { get; set; }

        public virtual Role Role { get; set; }
        public virtual ICollection<Customer> Customers { get; set; }
        public virtual ICollection<Driver> Drivers { get; set; }
    }
}
