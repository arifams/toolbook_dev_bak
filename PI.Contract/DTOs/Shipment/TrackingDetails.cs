using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.DTOs.Shipment
{
    public class TrackingDetails
    {

        public long ShipmentId { get; set; }
        
        public string Country { get; set; }
       
        public string City { get; set; }
       
        public string State { get; set; }
        
        public string Zip { get; set; }
       
        public string Message { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string DateTime { get; set; }
       
        public string Status { get; set; }
    }
}
