using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.DTOs.Shipment
{
    public class GeneralInformationDto
    {
        public string ShipmentId { get; set; }

        public string ShipmentCode { get; set; }

        public string ShipmentName { get; set; }

        public long DivisionId { get; set; }

        public long CostCenterId { get; set; }        

        public string ShipmentMode { get; set; }

        public string ShipmentTypeCode { get; set; }

        public string ShipmentTermCode { get; set; }

        public string shipmentModeName { get; set; }

        public string shipmentTypeName { get; set; }

        public string ShipmentServices { get; set; }

        public string TrackingNumber { get; set; }

        public string Status { get; set; }

        public string CreatedBy { get; set; }

        public string CreatedUser { get; set; }

        public string CreatedDate { get; set; }

        public short ShipmentPaymentTypeId { get; set; }

        public string ShipmentPaymentTypeName { get; set; }

        public string ShipmentLabelBLOBURL { get; set; }

        public IList<string> ShipmentLabelBLOBURLList { get; set; }

        public bool IsEnableEdit { get; set; }

        public bool IsEnableDelete { get; set; }

        public string ShipmentReferenceName { get; set; }

        public string ManualStatusUpdatedDate { get; set; }

        public bool IsFavourite { get; set; }

        public string CompanyName { get; set; }

        public string Owner { get; set; }

        public string ErrorUrl { get; set; }

        //public string UserId { get; set; }

    }
}
