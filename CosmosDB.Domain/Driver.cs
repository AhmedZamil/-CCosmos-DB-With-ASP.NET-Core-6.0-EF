using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmosDB.Domain
{
    public class Driver
    {
        public string DriverId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime EmploymentBeginUtc { get; set; }
        public DateTime? EmploymentEndUtc { get; set; }
    }
}
