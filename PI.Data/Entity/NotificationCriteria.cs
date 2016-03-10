using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity
{
    public class NotificationCriteria : LongIdBaseEntity
    {
        public bool BookingConfirmation { get; set; }
        public bool PickupConfirmation { get; set; }
        public bool ShipmentDelay { get; set; }
        public bool ShipmentException { get; set; }

        public bool NotifyNewSolution { get; set; }
        public bool NotifyDiscountOffer { get; set; }

        public long CustomerId { get; set; }

        #region Navigation Property

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }

        #endregion
    }
}
