using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.DTOs.Shipment
{
    public class PayLaneDto
    {
        public string Description { get; set; }
        public string Currency { get; set; }
        public double Amount { get; set; }
        public string TransactionType { get; set; }
    }
}
