using PI.Contract.DTOs.Division;
using PI.Contract.DTOs.Payment;
using PI.Contract.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.DTOs.Shipment
{
    public class ShipmentDto
    {
        public long Id { get; set; }
        public GeneralInformationDto GeneralInformation { get; set; }
        public ConsignerAndConsigneeInformationDto AddressInformation { get; set; }
        public PackageDetailsDto PackageDetails { get; set; }
        public CarrierInformationDto CarrierInformation { get; set; }
        public string UserId { get; set; }
        public string CreatedBy { get; set; }
        public string SISCompanyCode { get; set; }
        public DivisionDto Division { get; set; }

        public string TemplateLink { get; set; }
        public bool isSaveAsDraft { get; set; }

        public PaymentDto PaymentDto { get; set; }

        public string InvoiceUrl { get; set; }
        public string LabelUrl { get; set; }
        public bool HasShipmentAdded { get; set; }

        public UserDto InvokingUserDetails { get; set; }
    }
}
