using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PI.Contract.DTOs.Shipment
{
   public class ShipmentStatusHistoryDto
    {
        public long ShipmentId { get; set; }

        public string OldStatus { get; set; }

        public string NewStatus { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
