using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity
{
   public class ShipmentLocationHistory: LongIdBaseEntity
    {

        public long ShipmentId { get; set; }        

        public string Country { get; set; }

        public string City { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public IList<LocationActivity> LocaionActivities { get; set; }

        #region Navigation

        [ForeignKey("ShipmentId")]
        public virtual Shipment Shipment { get; set; }

        #endregion

    }
}
