﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity
{
    public class ShipmentPackage : LongIdBaseEntity
    {
        [MaxLength(100)]
        public string PackageDescription { get; set; }
        public decimal TotalWeight { get; set; }
        public short WeightMetricId { get; set; }
        public decimal TotalVolume { get; set; }
        public short VolumeMetricId { get; set; }
        [MaxLength(50)]
        public string HSCode { get; set; }
        public DateTime CollectionDate { get; set; }
        public string CarrierInstruction { get; set; }
        public bool IsInsured { get; set; }
        public decimal InsuranceDeclaredValue { get; set; }
        public short InsuranceCurrencyType { get; set; }
        public bool IsDG { get; set; }
        public bool Accessibility { get; set; }
        [MaxLength(20)]
        public string DGType { get; set; }

        // Package payments
        public decimal CarrierCost { get; set; }
        public decimal InsuranceCost { get; set; }
        public short PaymentTypeId { get; set; }

        // Skipped dangerous good section.

        public DateTime? EarliestPickupDate { get; set; }
        public DateTime? EstDeliveryDate { get; set; }

        #region Navigation properties

        // Navigation of package.
        public virtual IList<PackageProduct> PackageProducts { get; set; }

        //[ForeignKey("PaymentTypeId")]
        //public PaymentType PaymentType { get; set; }

        [ForeignKey("WeightMetricId")]
        public virtual WeightMetric WeightMetric { get; set; }

        [ForeignKey("VolumeMetricId")]
        public virtual VolumeMetric VolumeMetric { get; set; }

        [ForeignKey("InsuranceCurrencyType")]
        public virtual Currency Currency { get; set; }

        #endregion
    }
}
