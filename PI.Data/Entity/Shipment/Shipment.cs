﻿using PI.Contract.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity
{
    public class Shipment : LongIdBaseEntity
    {
        // General Information
        [MaxLength(100)]
        public string ShipmentName { get; set; }
        [MaxLength(100)]
        public string ShipmentReferenceName { get; set; }
        [MaxLength(100)]
        public string ShipmentCode { get; set; }
        public long? DivisionId { get; set; }
        public long? CostCenterId { get; set; }
        public CarrierType ShipmentMode { get; set; }
        public short ShipmentService { get; set; }

        public long MainShipment { get; set; }
        //public string ShipmentTypeCode { get; set; }        
        //public string ShipmentTermCode { get; set; }

        //public string CarrierName { get; set; }
        [MaxLength(50)]
        public string TarriffType { get; set; }
        [MaxLength(50)]
        public string TariffText { get; set; }
        [MaxLength(100)]
        public string CarrierDescription { get; set; }

        [MaxLength(20)]
        public string ServiceLevel {get;set;}

        public DateTime? PickUpDate { get; set; }
        public short Status { get; set; }
        [MaxLength(50)]
        public string TrackingNumber { get; set; }
        
        [MaxLength(50)]
        public string Provider { get; set; }

        [MaxLength(100)]
        public string PickupConfirmationNumber { get; set; }

        // Consignor and Consignee Information
        public long ConsignorId { get; set; }
        public long ConsigneeId { get; set; }

        // Package details
        public long ShipmentPackageId { get; set; }

        public long? ParentShipmentId { get; set; }
        public bool IsParent { get; set; }

        public short ShipmentPaymentTypeId { get; set; }

        public DateTime? ManualStatusUpdatedDate { get; set; }

        public bool IsFavourite { get; set; }

        #region Navigation properties

        // General Information - Navigations
        [ForeignKey("DivisionId")]
        public virtual Division Division { get; set; }

        [ForeignKey("CostCenterId")]
        public virtual CostCenter CostCenter { get; set; }
        
        // Consignor and Consignee Information - Navigations
        [ForeignKey("ConsignorId")]
        public virtual ShipmentAddress ConsignorAddress { get; set; }

        [ForeignKey("ConsigneeId")]
        public virtual ShipmentAddress ConsigneeAddress { get; set; }

        // Package details
        [ForeignKey("ShipmentPackageId")]
        public virtual ShipmentPackage ShipmentPackage { get; set; }

        [ForeignKey("ParentShipmentId")]
        public virtual Shipment ParentShipment { get; set; }

        public virtual CommercialInvoice CommercialInvoice { get; set; }

        public short CarrierId { get; set; }
        public virtual Carrier Carrier { get; set; }



        #endregion
    }   
}
