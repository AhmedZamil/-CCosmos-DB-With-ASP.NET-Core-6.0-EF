using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmosDB.Domain
{
    public class Vehicle
    {
        public string VehicleId { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public short Year { get; set; }
        public string LicensePlate { get; set; }
        public double Mileage { get; set; }
        public byte PassengerSeatCount { get; set; }
    }
}
