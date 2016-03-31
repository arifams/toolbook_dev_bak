using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity
{
    public class ShipmentPayment : BaseEntity
    {
        [Key, ForeignKey("Shipment")]
        public long ShipmentId { get; set; }

        public long SaleId { get; set; }
        public string Status { get; set; }

        #region Navigation

        public virtual Shipment Shipment { get; set; }

        #endregion
    }
}
