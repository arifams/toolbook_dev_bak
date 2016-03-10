using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.DTOs.Shipment
{
    public class ConsigneeDto
    {
        public string Name { get; set; }
        public string Country { get; set; }
        public string Postalcode{ get; set; }
        public string Number { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public string Details { get; set; }
    }
}
