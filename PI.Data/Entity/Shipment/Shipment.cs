using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity
{
    public class Shipment : BaseEntity
    {
        // General Information
        public string Name { get; set; }
        public long DivisionId { get; set; }
        public long CostCenterId { get; set; }
        public short ShipmentModeId { get; set; }
        public short ShipmentTypeId { get; set; }
        public short ShipmentTermId { get; set; }

        // 
        public long ConsignorId { get; set; }
        public long ConsigneeId { get; set; }
        public long ShipmentPackageId { get; set; }

        #region Navigation properties

        // General Information Navigations
        [ForeignKey("DivisionId")]
        public virtual Division Division { get; set; }

        [ForeignKey("CostCenterId")]
        public virtual CostCenter CostCenter { get; set; }

        [ForeignKey("ShipmentModeId")]
        public virtual ShipmentMode ShipmentMode { get; set; }

        [ForeignKey("ShipmentTypeId")]
        public virtual ShipmentType ShipmentType { get; set; }

        [ForeignKey("ShipmentTermId")]
        public virtual ShipmentTerm ShipmentTerm { get; set; }

        //
        [ForeignKey("ConsignorId")]
        public virtual ShipmentAddress ConsignorAddress { get; set; }

        [ForeignKey("ConsigneeId")]
        public virtual ShipmentAddress ConsigneeAddress { get; set; }

        [ForeignKey("ShipmentPackageId")]
        public virtual ShipmentPackage ShipmentPackage { get; set; }

        #endregion
    }   
}
