using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmosDB.Domain
{
    public class Trip
    {
        public string TripId { get; set; }
        public DateTime BeginUtc { get; set; }
        public DateTime? EndUtc { get; set; }
        public short PassengerCount { get; set; }

        public string DriverId { get; set; }
        public Driver Driver { get; set; }
        public string VehicleId { get; set; }
        public Vehicle Vehicle { get; set; }
        //public string FromAddressId { get; set; }
        public Address FromAddress { get; set; }
        //public string ToAddressId { get; set; }
        public Address ToAddress { get; set; }
    }
}
