using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.DTOs.Shipment
{
    public class CarrierInformationDto
    {
        public double Insurance { get; set; }
        public string CarrierName { get; set; }
        public DateTime PickupDate { get; set; }
        public DateTime DeliveryTime { get; set; }
        public string Price { get; set; }

    }
}
