using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.DTOs.Postmen
{
    public class ShipmentRequestDto
    {
        public bool async { get; set; }
        public bool return_shipment { get; set; }
        public bool is_document { get; set; }
        public string service_type { get; set; }
        public string paper_size { get; set; }        
        public PMbilling billing { get; set; }
        public PMcustoms customs { get; set; }
        public List<PMReference> references { get; set; }
        public PMShipment shipment { get; set; }
        public PMShipperAccount shipper_account { get; set; }

    }
}
