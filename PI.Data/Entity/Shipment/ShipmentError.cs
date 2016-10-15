using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity
{
   public class ShipmentError : LongIdBaseEntity
    {
        public long ShipmentId { get; set; }

        public string ErrorMessage { get; set; }

        #region Navigation
        [ForeignKey("ShipmentId")]
        public virtual Shipment Shipment { get; set; }
        #endregion
    }
}
