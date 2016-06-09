using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.DTOs.Shipment
{
    public class CarrierInformationDto
    {
        public decimal Insurance { get; set; }
        public string CarrierName { get; set; }
        public DateTime? PickupDate { get; set; }
        public DateTime? DeliveryTime { get; set; }
        public decimal Price { get; set; }
        public string serviceLevel { get; set; }
        public string tariffText { get; set; }
        public string tarriffType { get; set; }
        public string currency { get; set; }
        public string CountryCodeByTarrifText { get; set; }
    }
}
