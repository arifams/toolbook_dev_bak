using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.DTOs.Shipment
{
    public class InvoiceItemLineDto
    {
        public string Description { get; set; }
        public short Quantity { get; set; }
        public decimal PricePerPiece { get; set; }
        public string HSCode { get; set; }
    }
}
