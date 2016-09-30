using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity
{
    public class ShipmentLocationHistory : LongIdBaseEntity
    {
        public long ShipmentId { get; set; }
        [MaxLength(2)]
        public string Country { get; set; }
        [MaxLength(100)]
        public string City { get; set; }

        [MaxLength(100)]
        public string State { get; set; }

        [MaxLength(100)]
        public string Zip { get; set; }

        [MaxLength(100)]
        public string Message { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public DateTime DateTime { get; set; }

        [MaxLength(100)]
        public string Status { get; set; }

        // public IList<LocationActivity> LocaionActivities { get; set; }

        #region Navigation

        [ForeignKey("ShipmentId")]
        public virtual Shipment Shipment { get; set; }

        #endregion

    }
}
