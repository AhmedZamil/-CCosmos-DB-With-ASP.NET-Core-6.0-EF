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
    }
}
