using PI.Contract.DTOs.Shipment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.DTOs.Report
{
    public class ShipmentReportDto
    {
        public GeneralInformationDto GeneralInformation { get; set; }
        public ConsignerAndConsigneeInformationDto AddressInformation { get; set; }
        public PackageDetailsDto PackageDetails { get; set; }
        public CarrierInformationDto CarrierInformation { get; set; }
        public string UserId { get; set; }
    }
}
