using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity
{
    public class Shipment : LongIdBaseEntity
    {
        // General Information
        public string ShipmentName { get; set; }
        public string ShipmentCode { get; set; }
        public long? DivisionId { get; set; }
        public long? CostCenterId { get; set; }
        public string ShipmentMode { get; set; }
        public string ShipmentTypeCode { get; set; }        
        public string ShipmentTermCode { get; set; }
        public string CarrierName { get; set; }
        public string Status { get; set; }
        public string TrackingNumber { get; set; }

        // Consignor and Consignee Information
        public long ConsignorId { get; set; }
        public long ConsigneeId { get; set; }

        // Package details
        public long ShipmentPackageId { get; set; }

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

        #endregion
    }   
}
