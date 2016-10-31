using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity
{
    public class LocationActivity: LongIdBaseEntity
    {
        [MaxLength(50)]
        public string Status { get; set; }
        public DateTime Date { get; set; }
        public DateTime Time { get; set; }

        public long ShipmentLocationHistoryId { get; set; }

        #region Navigation

        [ForeignKey("ShipmentLocationHistoryId")]
        public virtual ShipmentLocationHistory ShipmentLocationHistory { get; set; }

        #endregion

    }
}
