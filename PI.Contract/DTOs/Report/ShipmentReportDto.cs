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
       
        public string UserId { get; set; }

        // General
        #region General

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

        public long CreatedBy { get; set; }

        public string CreatedDate { get; set; }

        public short ShipmentPaymentTypeId { get; set; }

        public string ShipmentLabelBLOBURL { get; set; }

        public bool IsEnableEdit { get; set; }

        public bool IsEnableDelete { get; set; }

        public string ShipmentReferenceName { get; set; }

        public string ManualStatusUpdatedDate { get; set; }

        #endregion


        #region Consignor
        public string ConsignorFirstName { get; set; }
        public string ConsignorLastName { get; set; }
        public string ConsignorCountry { get; set; }
        public string ConsignorPostalcode { get; set; }
        public string ConsignorNumber { get; set; }
        public string ConsignorAddress1 { get; set; }
        public string ConsignorAddress2 { get; set; }
        public string ConsignorCity { get; set; }
        public string ConsignorState { get; set; }
        public string ConsignorEmail { get; set; }
        public string ConsignorContactNumber { get; set; }
        public string ConsignorContactName { get; set; }
        public string ConsignorDetails { get; set; }
        #endregion


        #region Consignee

        public string ConsigneeFirstName { get; set; }
        public string ConsigneeLastName { get; set; }
        public string ConsigneeCountry { get; set; }
        public string ConsigneePostalcode { get; set; }
        public string ConsigneeNumber { get; set; }
        public string ConsigneeAddress1 { get; set; }
        public string ConsigneeAddress2 { get; set; }
        public string ConsigneeCity { get; set; }
        public string ConsigneeState { get; set; }
        public string ConsigneeEmail { get; set; }
        public string ConsigneeContactNumber { get; set; }
        public string ConsigneeContactName { get; set; }
        public string ConsigneeDetails { get; set; }

        #endregion


        #region Package Details

        public string ShipmentDescription { get; set; }
        public decimal TotalWeight { get; set; }
        public bool CmLBS { get; set; }
        public string productTypes { get; set; }
        public decimal TotalVolume { get; set; }
        public bool VolumeCMM { get; set; }
        public List<ProductIngredientsDto> ProductIngredients { get; set; }
        public string HsCode { get; set; }
        public string PreferredCollectionDate { get; set; }
        public string Instructions { get; set; }
        public string IsInsuared { get; set; }
        public int ValueCurrency { get; set; }
        public decimal DeclaredValue { get; set; }
        public long Count { get; set; }
        public bool IsDG { get; set; }
        public string DGType { get; set; }
        public bool Accessibility { get; set; }

        public short PaymentTypeId { get; set; }
        #endregion


        #region Carrier Details

        public decimal Insurance { get; set; }
        public string CarrierName { get; set; }
        public string PickupDate { get; set; }
        public string DeliveryTime { get; set; }
        public decimal Price { get; set; }
        public string serviceLevel { get; set; }
        public string tariffText { get; set; }
        public string tarriffType { get; set; }
        public string currency { get; set; }

        #endregion




    }
}
