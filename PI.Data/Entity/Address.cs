using PI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity
{
    public class Address:BaseEntity
    {
        public Country Country { get; set; } 
        public string ZipCode { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string StreetAddress1 { get; set; }
        public string StreetAddress2 { get; set; }

        // Navigation property
        public IList<Customer> Customers { get; set; }
    }
}
