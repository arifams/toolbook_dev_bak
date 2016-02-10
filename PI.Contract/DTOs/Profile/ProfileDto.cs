using PI.Contract.DTOs.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.DTOs.Profile
{
    public class ProfileDto
    {
        public bool BookingConfirmation { get; set; }
        public bool PickupConfirmation { get; set; }
        public bool ShipmentDelay { get; set; }
        public bool ShipmentException { get; set; }
        public bool NotifyNewSolution { get; set; }
        public bool NotifyDiscountOffer { get; set; }

        //Default Account settings
        public long DefaultLanguageId { get; set; }
        public long DefaultCurrencyId { get; set; }
        public long DefaultTimeZoneId { get; set; }

        public CustomerDto CustomerDetails { get; set; }

    }
}
