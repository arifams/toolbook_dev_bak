using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.DTOs.Shipment
{
    public class ConsignerAndConsigneeInformationDto
    {
        public ConsignerDto Consigner { get; set; }
        public ConsigneeDto Consignee { get; set; }
    }
}
