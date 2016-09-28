using PI.Contract.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity
{
    public class ShipmentPayment : LongIdBaseEntity
    {
        [ForeignKey("Shipment")]
        public long ShipmentId { get; set; }

        public string PaymentId { get; set; }
        
        public Status Status { get; set; }

        public string StatusCode { get; set; }

        #region Navigation

        public virtual Shipment Shipment { get; set; }

        #endregion
    }
}
