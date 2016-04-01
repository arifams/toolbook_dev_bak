using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.DTOs.Shipment
{
    public class ShipmentOperationResult : OperationResult
    {
        public string AddShipmentXML { get; set; }
        public long ShipmentId { get; set; }
        public string LabelURL { get; set; }
    }
}
