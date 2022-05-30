using CosmosDB.Data;
using CosmosDB.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmosDB.Service
{
    public delegate void WriteLine(string text = "", bool highlight = false, bool isException = false);
    public class CosmosService
    {
        private readonly IDbContextFactory<CosmosDBContext> contextFactory;
        private readonly WriteLine writeLine;
        #region setup

        public CosmosService(IDbContextFactory<CosmosDBContext> ContextFactory,WriteLine WriteLine)
        {
            this.contextFactory = ContextFactory?? throw new ArgumentNullException(nameof(ContextFactory));
            this.writeLine = WriteLine?? throw new ArgumentNullException(nameof(WriteLine));
        }

        private async Task ReCreateDatabase()
        {
            using var _context = await contextFactory.CreateDbContextAsync();

            await _context.Database.EnsureDeletedAsync();
            await _context.Database.EnsureCreatedAsync();
        }

        #endregion

        public async void RunSample() {

            await ReCreateDatabase();
            await AddItems();
        
        }

        private async Task AddItems() 
        {
            writeLine();
            writeLine("Adding Items...");

            using var _context = await contextFactory.CreateDbContextAsync();

            _context.Add(
                new Address 
                {
                    AddressId = $"{nameof(Address)}-1",
                    City = "Salt Lake City",
                    State = "Utah",
                    Street = "Course Road",
                    HouseNumber = "1234"
                } );

            _context.Add(
                new Driver {
                    DriverId = $"{nameof(Driver)}-1",
                    FirstName = "Jurgen",
                    LastName = "Kevelaers",
                    EmploymentBeginUtc = new DateTime(2022, 1, 17, 9, 0, 0, DateTimeKind.Utc)
                });

            _context.Add(new Vehicle {
                VehicleId = $"{nameof(Vehicle)}-1",
                Make = "Pluralsight",
                Model = "Buggy",
                Year = 2018,
                LicensePlate = "2GAT123",
                Mileage = 12800,
                PassengerSeatCount = 6
            });

            _context.Add(
                new Trip {
                    TripId = $"{nameof(Trip)}-1",
                    BeginUtc = new DateTime(2022, 3, 23, 10, 45, 0, DateTimeKind.Utc),
                    EndUtc = new DateTime(2022, 3, 23, 11, 17, 0, DateTimeKind.Utc),
                    PassengerCount = 2
                });

           await _context.SaveChangesAsync();
            writeLine("Save changes successfully..");
        
        }
    }
}
