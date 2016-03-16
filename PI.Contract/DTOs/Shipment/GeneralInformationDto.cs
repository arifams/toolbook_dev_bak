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

        public string ShipmentMode { get; set; }

        public string ShipmentTypeCode { get; set; }
        public string ShipmentTermCode { get; set; }

        public string shipmentModeName { get; set; }
        public string shipmentTypeName { get; set; }
        
       
    }
}
