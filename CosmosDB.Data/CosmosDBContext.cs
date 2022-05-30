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
          base.OnModelCreating(modelBuilder);

            modelBuilder.HasManualThroughput(400);
            modelBuilder.HasDefaultContainer("AllInOne");

            modelBuilder.Entity<Address>()
                .Property(address => address.HouseNumber)
                .ToJsonProperty("StreetHouseNumber");

            modelBuilder.Entity<Address>().HasNoDiscriminator().ToContainer(nameof(Address)).HasPartitionKey(address => address.State);
            modelBuilder.Entity<Driver>().HasNoDiscriminator().ToContainer(nameof(Driver));
            modelBuilder.Entity<Vehicle>().HasNoDiscriminator().ToContainer(nameof(Vehicle)).HasPartitionKey(vehicle=>vehicle.Make);
            modelBuilder.Entity<Trip>().HasNoDiscriminator().ToContainer(nameof(Trip));

        }

    }
}
