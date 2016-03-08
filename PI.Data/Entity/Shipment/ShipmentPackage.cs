using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity
{
    public class ShipmentPackage : BaseEntity
    {
        public string PackageDescription { get; set; }
        public decimal TotalWeight { get; set; }
        public string WeightMetric { get; set; }
        public decimal TotalVolume { get; set; }
        public string VolumeMetric { get; set; }
        public string HSCOde { get; set; }
        public DateTime CollectionDate { get; set; }
        public string CarrierInstruction { get; set; }
        public bool IsInsurance { get; set; }
        public decimal InsuranceDeclaredValue { get; set; }
        public string InsuranceCurrencyType { get; set; }

        // Skipped dangerous good section.

        public DateTime EarliestPickupDate { get; set; }
        public DateTime EstDeliveryDate { get; set; }

        public virtual IList<PackageProduct> PackageProducts { get; set; }
    }
}
