using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity
{
    public class ShipmentStatusHistory : LongIdBaseEntity
    {
        public long ShipmentId { get; set; }

        public string OldStatus { get; set; }

        public string NewStatus { get; set; }


        #region Navigation properties

        [ForeignKey("ShipmentId")]
        public virtual Shipment Shipment { get; set; }

        #endregion 
    }
}
