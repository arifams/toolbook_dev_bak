using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.DTOs.Shipment
{
    public class GeneralInformationDto
    {
        public string ShipmentName { get; set; }
        public long DivisionId { get; set; }
        public long CostCenterId { get; set; }

        public bool Express { get; set; }
        public bool AirFreight { get; set; }
        public bool SeaFreight { get; set; }
        public bool RoadFreight { get; set; }

        public bool All { get; set; }
        public long SelectedShipmentType { get; set; }
        public long SelectedShipmentTerm { get; set; }
        public string shipmentMode { get; set; }
        public string shipmentType { get; set; }
        
       
    }
}
