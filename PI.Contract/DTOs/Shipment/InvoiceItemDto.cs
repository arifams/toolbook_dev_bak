using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.DTOs.Shipment
{
    public class InvoiceItemDto
    {
        public IList<InvoiceItemLineDto> LineItems { get; set; }
    }
}
