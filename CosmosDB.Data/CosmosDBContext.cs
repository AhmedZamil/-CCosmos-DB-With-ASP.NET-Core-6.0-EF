using CosmosDB.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmosDB.Data
{
    public class CosmosDBContext : DbContext
    {
        public CosmosDBContext(DbContextOptions<CosmosDBContext> Options):base(Options)
        {

        }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasManualThroughput(600);

            modelBuilder.Entity<Address>()
              .Property(address => address.HouseNumber)
              .ToJsonProperty("StreetHouseNumber");

            // implicit:

            // modelBuilder.Entity<Driver>()
            //   .OwnsOne(driver => driver.Address);

            // modelBuilder.Entity<Driver>()
            //   .OwnsMany(driver => driver.Trips);

            modelBuilder.Entity<Address>()
              .HasNoDiscriminator()
              .ToContainer(nameof(Address))
              .HasPartitionKey(address => address.State)
              .HasKey(address => address.AddressId);

            modelBuilder.Entity<Driver>()
              .HasNoDiscriminator()
              .ToContainer(nameof(Driver))
              .HasKey(driver => driver.DriverId);

            modelBuilder.Entity<Vehicle>()
              .HasNoDiscriminator()
              .ToContainer(nameof(Vehicle))
              .HasPartitionKey(vehicle => vehicle.Make)
              .HasKey(vehicle => vehicle.VehicleId);

            modelBuilder.Entity<Trip>()
              .HasNoDiscriminator()
              .ToContainer(nameof(Trip))
              .HasPartitionKey(trip => trip.VehicleId)
              .HasKey(trip => trip.TripId);

        }

    }
}
