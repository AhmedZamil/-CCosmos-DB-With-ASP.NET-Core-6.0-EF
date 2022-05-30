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
    public delegate void WaitForNext(string actionName);
    public class CosmosService
    {
        private readonly IDbContextFactory<CosmosDBContext> contextFactory;
        private readonly WriteLine writeLine;
        private readonly WaitForNext waitForNext;
        #region setup

        public CosmosService(IDbContextFactory<CosmosDBContext> contextFactory, WriteLine writeLine, WaitForNext waitForNext)
        {
            this.contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            this.writeLine = writeLine ?? throw new ArgumentNullException(nameof(writeLine));
            this.waitForNext = waitForNext ?? throw new ArgumentNullException(nameof(waitForNext));
        }

        private async Task ReCreateDatabase()
        {
            using var _context = await contextFactory.CreateDbContextAsync();

            await _context.Database.EnsureDeletedAsync();
            await _context.Database.EnsureCreatedAsync();
        }

        #endregion

        public async void RunSample()
        {

            await ReCreateDatabase();
            using var defaultContext = await contextFactory.CreateDbContextAsync();

            await AddItemsFromDefaultContext(defaultContext);

            waitForNext(nameof(GetTripFromDefaultContext));
            await GetTripFromDefaultContext(defaultContext);

            waitForNext(nameof(GetTripFromOtherContext));
            await GetTripFromOtherContext();

            waitForNext(nameof(GetTripFromOtherContextWithExplicitLoading));
            await GetTripFromOtherContextWithExplicitLoading();

            waitForNext(nameof(UpdateVehicleFromOtherContext));
            await UpdateVehicleFromOtherContext();

            //await AddItems();

        }
        public void WriteTripInfo(Trip trip)
        {
            writeLine($"  From address instance available on trip: {(trip.FromAddress == null ? "no" : "yes")}");
            writeLine($"  To address instance available on trip: {(trip.ToAddress == null ? "no" : "yes")}");
            writeLine($"  Driver instance available on trip: {(trip.Driver == null ? "no" : "yes")}");
            writeLine($"  Vehicle instance available on trip: {(trip.Vehicle == null ? "no" : "yes")}");

            if (trip.Driver?.Trips != null)
            {
                writeLine($"  Driver has {trip.Driver.Trips.Count} trip instance(s)");
            }

            if (trip.Vehicle?.Trips != null)
            {
                writeLine($"  Vehicle has {trip.Vehicle.Trips.Count} trip instance(s)");
            }
        }

        public async Task AddItemsFromDefaultContext(CosmosDBContext defaultContext)
        {
            writeLine();
            writeLine("Adding vehicle and driver from DEFAULT context...");

            defaultContext.Add(
              new Vehicle
              {
                  VehicleId = $"{nameof(Vehicle)}-1",
                  Make = "Pluralsight",
                  Model = "Buggy",
                  Year = 2018,
                  LicensePlate = "2GAT123",
                  Mileage = 12800,
                  PassengerSeatCount = 6,
                  TechnicalSpecifications = new Dictionary<string, string>
                {
            {
              "Maximum Horsepower", "275"
            },
            {
              "Maximum Torque", "262"
            },
            {
              "Fuel Capacity", "25.1"
            },
            {
              "Length (inches)", "219.9"
            },
            {
              "Width (inches)", "81.3"
            }
                },
                  CheckUpUtcs = new List<DateTime>
                {
            new DateTime(2019, 2, 12, 11, 0, 0, DateTimeKind.Utc),
            new DateTime(2020, 2, 19, 9, 0, 0, DateTimeKind.Utc),
            new DateTime(2021, 2, 14, 16, 0, 0, DateTimeKind.Utc)
                }
              });

            defaultContext.Add(
              new Driver
              {
                  DriverId = $"{nameof(Driver)}-1",
                  FirstName = "Jurgen",
                  LastName = "Kevelaers",
                  EmploymentBeginUtc = new DateTime(2022, 1, 17, 9, 0, 0, DateTimeKind.Utc),
                  Address = new Address
                  {
                      AddressId = $"{nameof(Address)}-1",
                      City = "Draper",
                      State = "Utah",
                      Street = "Class Street",
                      HouseNumber = "98"
                  }
              });

            var trip = new Trip
            {
                TripId = $"{nameof(Trip)}-1",
                BeginUtc = new DateTime(2022, 2, 23, 10, 45, 0, DateTimeKind.Utc),
                EndUtc = new DateTime(2022, 2, 23, 11, 17, 0, DateTimeKind.Utc),
                PassengerCount = 2,
                DriverId = $"{nameof(Driver)}-1",
                VehicleId = $"{nameof(Vehicle)}-1",
                FromAddress = new Address
                {
                    AddressId = $"{nameof(Address)}-2",
                    City = "Salt Lake City",
                    State = "Utah",
                    Street = "Course Road",
                    HouseNumber = "1234"
                },
                ToAddress = new Address
                {
                    AddressId = $"{nameof(Address)}-3",
                    City = "Rock Springs",
                    State = "Wyoming",
                    Street = "Lecture Lane",
                    HouseNumber = "42"
                }
            };

            defaultContext.Add(trip);

            WriteTripInfo(trip);

            await defaultContext.SaveChangesAsync();

            writeLine("Save successful");
        }

        public async Task GetTripFromDefaultContext(CosmosDBContext defaultContext)
        {
            writeLine();
            writeLine("Getting trip from DEFAULT context...");

            var trip = await defaultContext.Trips.FindAsync($"{nameof(Trip)}-1");

            WriteTripInfo(trip);
        }

        public async Task GetTripFromOtherContext()
        {
            writeLine();
            writeLine("Getting trip from OTHER context...");

            using var otherContext = await contextFactory.CreateDbContextAsync();

            var trip = await otherContext.Trips.FindAsync($"{nameof(Trip)}-1");

            WriteTripInfo(trip);

            // Include is not supported:

            // var trip = otherContext.Trips
            //   .Include(trip => trip.Driver)
            //   .FirstAsync(trip => trip.TripId == $"{nameof(Trip)}-1");
        }

        public async Task GetTripFromOtherContextWithExplicitLoading()
        {
            writeLine();
            writeLine("Getting trip from OTHER context with explicit loading...");

            using var otherContext = await contextFactory.CreateDbContextAsync();

            var trip = await otherContext.Trips.FindAsync($"{nameof(Trip)}-1");

            var tripEntry = otherContext.Entry(trip);

            await tripEntry
              .Reference(trip => trip.Driver)
              .LoadAsync();

            await tripEntry
              .Reference(trip => trip.Vehicle)
              .LoadAsync();

            await tripEntry
              .Reference(trip => trip.FromAddress)
              .LoadAsync();

            await tripEntry
              .Reference(trip => trip.ToAddress)
              .LoadAsync();

            WriteTripInfo(trip);

            var driver = await otherContext.Drivers.FindAsync($"{nameof(Driver)}-1");

            var driverEntry = otherContext.Entry(driver);

            await driverEntry
              .Collection(driver => driver.Trips)
              .LoadAsync();

            writeLine();
            writeLine($"Found {driver.Trips.Count} trip(s) on the driver instance");
        }

        public async Task UpdateVehicleFromOtherContext()
        {
            writeLine();
            writeLine("Updating vehicle dictionary from OTHER context...");

            using var otherContext = await contextFactory.CreateDbContextAsync();

            var vehicle = await otherContext.Vehicles.FindAsync($"{nameof(Vehicle)}-1");

            vehicle.Mileage += 100;

            vehicle.TechnicalSpecifications["Fuel Capacity"] = "26";
            vehicle.TechnicalSpecifications["Wheelbase (inches)"] = "130";

            vehicle.CheckUpUtcs.Add(new DateTime(2022, 2, 10, 13, 0, 0, DateTimeKind.Utc));

            await otherContext.SaveChangesAsync();

            writeLine("Save successful");
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
                });

            _context.Add(
                new Driver
                {
                    DriverId = $"{nameof(Driver)}-1",
                    FirstName = "Jurgen",
                    LastName = "Kevelaers",
                    EmploymentBeginUtc = new DateTime(2022, 1, 17, 9, 0, 0, DateTimeKind.Utc)
                });

            _context.Add(new Vehicle
            {
                VehicleId = $"{nameof(Vehicle)}-1",
                Make = "Pluralsight",
                Model = "Buggy",
                Year = 2018,
                LicensePlate = "2GAT123",
                Mileage = 12800,
                PassengerSeatCount = 6
            });

            _context.Add(
                new Trip
                {
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
