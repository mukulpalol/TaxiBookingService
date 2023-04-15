using Microsoft.EntityFrameworkCore;
using TaxiBookingService.DAL.Models;

namespace TaxiBookingService.DAL
{
    public class TbsDbContext : DbContext
    {
        public TbsDbContext(DbContextOptions<TbsDbContext> options) : base(options)
        {
        }

        public virtual DbSet<Area> Areas { get; set; }
        public virtual DbSet<CancelReason> CancelReasons { get; set; }
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Driver> Drivers { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<Ride> Rides { get; set; }
        public virtual DbSet<RidesDeclined> RidesDeclined { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<State> States { get; set; }
        public virtual DbSet<Status> Statuses { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Vehicle> Vehicles { get; set; }
        public virtual DbSet<VehicleType> VehicleTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<State>().HasData(
                new State { Id = 1, Name = "Karnataka" },
                new State { Id = 2, Name = "Maharashtra" },
                new State { Id = 3, Name = "Puducherry" },
                new State { Id = 4, Name = "Rajasthan" }
            );

            modelBuilder.Entity<City>().HasData(
                new City { Id = 1, Name = "Bangalore", StateId = 1 },
                new City { Id = 2, Name = "Mumbai", StateId = 2 },
                new City { Id = 3, Name = "Jaipur", StateId = 4 },
                new City { Id = 4, Name = "Udaipur", StateId = 4 },
                new City { Id = 5, Name = "Puducherry", StateId = 3 }
                );

            modelBuilder.Entity<Area>().HasData(
                new Area { Id = 1, Name = "Vidhyadhar Nagar", CityId = 3 },
                new Area { Id = 2, Name = "Sirsi Road", CityId = 3 },
                new Area { Id = 3, Name = "Jagatpura", CityId = 3 },
                new Area { Id = 4, Name = "Pratap Nagar", CityId = 4 },
                new Area { Id = 5, Name = "Ganpati Nagar", CityId = 4 }
                );

            modelBuilder.Entity<Location>().HasData(
                new Location { Id = 1, StreetName = "39 Sector 2 Road", Latitude = 26.955022M, Longitude = 75.773914M, AreaId = 1 },
                new Location { Id = 2, StreetName = "RIICO Industrial Area", Latitude = 26.927076M, Longitude = 75.702112M, AreaId = 2 },
                new Location { Id = 3, StreetName = "Ram Nagariya Road", Latitude = 26.822239M, Longitude = 75.864820M, AreaId = 3 },
                new Location { Id = 4, StreetName = "B4 Central Spine Road", Latitude = 26.967003M, Longitude = 75.782395M, AreaId = 1 },
                new Location { Id = 5, StreetName = "6 Dhaka Nagar", Latitude = 26.924471M, Longitude = 75.697313M, AreaId = 2 }
                );

            modelBuilder.Entity<VehicleType>().HasData(
                new VehicleType { Id = 1, Type = "Hatchback", AverageSpeed = 30, Capacity = 3, FareFactor = 17 },
                new VehicleType { Id = 2, Type = "Sedan", AverageSpeed = 40, Capacity = 4, FareFactor = 19 },
                new VehicleType { Id = 3, Type = "SUV", AverageSpeed = 45, Capacity = 6, FareFactor = 21 }
                );

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, RoleType = "Admin" },
                new Role { Id = 2, RoleType = "Customer" },
                new Role { Id = 3, RoleType = "Driver" }
                );

            modelBuilder.Entity<CancelReason>().HasData(
                new CancelReason { Id = 1, CancelReasons = "Driver denied duty", ValidReason = true },
                new CancelReason { Id = 2, CancelReasons = "Entered wrong drop location", ValidReason = true },
                new CancelReason { Id = 3, CancelReasons = "Driver arrived too early", ValidReason = false },
                new CancelReason { Id = 4, CancelReasons = "Change of plans", ValidReason = false }
                );

            modelBuilder.Entity<Status>().HasData(
                new Status { Id = 1, StatusType = "Searching" },
                new Status { Id = 2, StatusType = "Booked" },
                new Status { Id = 3, StatusType = "Ride Started" },
                new Status { Id = 4, StatusType = "Ride Completed" },
                new Status { Id = 5, StatusType = "Ride Cancelled" },
                new Status { Id = 6, StatusType = "No Drivers Available" }
                );

            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, FirstName = "admin", Email = "admin@example.com", Password = "password", Dob = DateTime.Parse("22/05/1992"), Gender = "male", PhoneNumber = "9000000000", RoleId = 1 }
                );

            modelBuilder.Entity<Ride>(entity =>
            {
                entity.HasOne(d => d.Drop)
                    .WithMany(p => p.RideDrops)
                    .HasForeignKey(d => d.DropId);
            });
        }
    }
}
