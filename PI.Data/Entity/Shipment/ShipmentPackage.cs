﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity
{
    public class ShipmentPackage : LongIdBaseEntity
    {
        public string PackageDescription { get; set; }
        public decimal TotalWeight { get; set; }
        public string WeightMetric { get; set; }
        public decimal TotalVolume { get; set; }
        public string VolumeMetric { get; set; }
        public string HSCode { get; set; }
        public DateTime CollectionDate { get; set; }
        public string CarrierInstruction { get; set; }
        public bool IsInsurance { get; set; }
        public decimal InsuranceDeclaredValue { get; set; }
        public string InsuranceCurrencyType { get; set; }

        // Package payments
        public decimal CarrierCost { get; set; }
        public decimal InsuranceCost { get; set; }
        public short PaymentTypeId { get; set; }

        // Skipped dangerous good section.

        public DateTime EarliestPickupDate { get; set; }
        public DateTime EstDeliveryDate { get; set; }

        #region Navigation properties

        // Navigation of package.
        public virtual IList<PackageProduct> PackageProducts { get; set; }

        [ForeignKey("PaymentTypeId")]
        public PaymentType PaymentType { get; set; }

        #endregion
    }
}
