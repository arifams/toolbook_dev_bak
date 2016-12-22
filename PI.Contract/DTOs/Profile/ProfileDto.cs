using PI.Contract.DTOs.AccountSettings;
using PI.Contract.DTOs.Company;
using PI.Contract.DTOs.CostCenter;
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
        public short DefaultLanguageId { get; set; }
        public short DefaultCurrencyId { get; set; }
        public short DefaultTimeZoneId { get; set; }

        public short DefaultVolumeMetricId { get; set; }
        public short DefaultWeightMetricId { get; set; }
        
        public bool DoNotUpdateAccountSettings { get; set; }

        public bool IsInvoicePaymentEnabled { get; set; }

        public string SelectedColour { get; set; }

        /// user settings
        public string NewPassword { get; set; }       
        public string OldPassword { get; set; }      

        public CustomerDto CustomerDetails { get; set; }
        public CompanyDto CompanyDetails { get; set; }

        public AccountSettingsDto AccountSettings { get; set; }

        public string LoggedUserId { get; set; }
    }
}
