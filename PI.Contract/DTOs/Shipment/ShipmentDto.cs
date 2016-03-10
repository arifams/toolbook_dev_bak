using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.DTOs.Shipment
{
    public class ShipmentDto
    {
        public GeneralInformationDto GeneralInformation { get; set; }
        public ConsignerAndConsigneeInformationDto AddressInformation { get; set; }
        public PackageDetailsDto PackageDetails { get; set; }
        public CarrierInformationDto CarrierInformation { get; set; }
    }
}
