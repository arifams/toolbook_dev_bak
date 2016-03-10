using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity
{
    public class PackageProduct : LongIdBaseEntity
    {
        public short ProductTypeId { get; set; }
        public short Quantity { get; set; }
        public decimal Weight { get; set; }
        public decimal Height { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public long ShipmentPackageId { get; set; }

        #region Navigation properties

        [ForeignKey("ShipmentPackageId")]
        public virtual ShipmentPackage ShipmentPackage { get; set; }

        [ForeignKey("ProductTypeId")]
        public virtual PackageProductType PackageProductType { get; set; }

        #endregion
    }
}
