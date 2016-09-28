using PI.Contract.DTOs.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.DTOs.Shipment
{
    public class SendShipmentDetailsDto
    {
        public long ShipmentId { get; set; }

        public OperationResult PaymentResult { get; set; }

        public string UserId { get; set; }

        public string TemplateLink { get; set; }
    }
}
