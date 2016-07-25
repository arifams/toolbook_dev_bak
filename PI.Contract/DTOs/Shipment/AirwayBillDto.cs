using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.DTOs.Shipment
{
    public class AirwayBillDto
    {
        public long ShipmentId { get; set; }
        public ConsignerAndConsigneeInformationDto AddressInformation { get; set; }
        public PackageDetailsDto PackageDetails { get; set; }
        public string ShipmentReferenceName { get; set; }
        public string CreatedDate { get; set; }
        public string InvoiceNo { get; set; }
        public string VatNo { get; set; }
        public string CustomerNo { get; set; }
        public string ShipTo { get; set; }
        public string InvoiceTo { get; set; }
        public string ShipmentServices { get; set; }
        public string TermsOfPayment { get; set; }
        public string CountryOfOrigin { get; set; }
        public string CountryOfDestination { get; set; }
        public string ModeOfTransport { get; set; }
        public string ImportBroker { get; set; }
        public string Note { get; set; }
        public short ValueCurrency { get; set; }
        public InvoiceItemDto Item { get; set; }
    }
}
