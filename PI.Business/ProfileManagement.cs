using PI.Contract.DTOs.Profile;
using PI.Data;
using PI.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Business
{
    public class ProfileManagement
    {
        PIContext context = PIContext.Get();
        //get the profile details
        public ProfileDto getProfileByUserName(string username)
        {
            ProfileDto currentProfile=new ProfileDto();
            Address currentAddress;
            AccountSettings currentAccountSettings;
            NotificationCriteria currentnotificationCriteria;

           Customer currentCustomer = this.GetCustomerByUserName(username);

            if (currentCustomer==null)
            {
                return null;
            }

            //assigning basic customer details to Dto
            currentProfile.CustomerDetails.Salutation = currentCustomer.Salutation;
            currentProfile.CustomerDetails.FirstName = currentCustomer.FirstName;
            currentProfile.CustomerDetails.MiddleName = currentCustomer.MiddleName;
            currentProfile.CustomerDetails.LastName = currentCustomer.LastName;
            currentProfile.CustomerDetails.Email = currentCustomer.Email;
            currentProfile.CustomerDetails.PhoneNumber =currentCustomer.PhoneNumber;
            currentProfile.CustomerDetails.MobileNumber = currentCustomer.MobileNumber;
            currentProfile.CustomerDetails.UserName = currentCustomer.UserName;
            currentProfile.CustomerDetails.Password = currentCustomer.Password;


            currentAddress = this.GetAddressbyId(currentCustomer.AddressId);
            currentAccountSettings = this.GetAccountSettingByCustomerId(currentCustomer.Id);
            currentnotificationCriteria = this.GetNotificationCriteriaByCustomerId(currentCustomer.Id);

            //assign address values to the  Profile Dto
            if (currentAddress!=null)
            {
                currentProfile.CustomerDetails.CustomerAddress.Country = currentAddress.Country;
                currentProfile.CustomerDetails.CustomerAddress.ZipCode = currentAddress.ZipCode;
                currentProfile.CustomerDetails.CustomerAddress.Number = currentAddress.Number;
                currentProfile.CustomerDetails.CustomerAddress.StreetAddress1 = currentAddress.StreetAddress1;
                currentProfile.CustomerDetails.CustomerAddress.StreetAddress2 = currentAddress.StreetAddress2;
                currentProfile.CustomerDetails.CustomerAddress.City = currentAddress.City;
                currentProfile.CustomerDetails.CustomerAddress.State = currentAddress.State;
            }
            //Assign Account setting values to the Profile Dto
            if (currentAccountSettings!=null)
            {
                currentProfile.DefaultLanguageId = currentAccountSettings.DefaultLanguageId;
                currentProfile.DefaultCurrencyId = currentAccountSettings.DefaultCurrencyId;
                currentProfile.DefaultTimeZoneId = currentAccountSettings.DefaultCurrencyId;
            }

            //Assign Notofication criteria to the Profile Dto
            if (currentnotificationCriteria != null)
            {
                currentProfile.BookingConfirmation = currentnotificationCriteria.BookingConfirmation;
                currentProfile.PickupConfirmation = currentnotificationCriteria.PickupConfirmation;
                currentProfile.ShipmentDelay = currentnotificationCriteria.ShipmentDelay;
                currentProfile.ShipmentException = currentnotificationCriteria.ShipmentException;
                currentProfile.NotifyNewSolution = currentnotificationCriteria.NotifyNewSolution;
                currentProfile.NotifyDiscountOffer= currentnotificationCriteria.NotifyDiscountOffer;
             }       

            
            return currentProfile;

        }

        //get the customer details by username(email)
        public Customer GetCustomerByUserName(string username)
        {
            using (context)
            {
                return context.Customers.Single(c => c.Email == username);
            }
        }

        //get address details by Id
        public Address GetAddressbyId(long addressId)
        {
            using (context)
            {
                return context.Addresses.Single(a => a.Id == addressId);
            }
        }

        //get Account Settings by customer Id
        public AccountSettings GetAccountSettingByCustomerId(long customerId)
        {
            using (context)
            {
                return context.AccountSettings.Single(s => s.CustomerId == customerId);
            }
        }

        //get the notofication criterias bt customer Id
        public NotificationCriteria GetNotificationCriteriaByCustomerId(long customerId)
        {
            using (context)
            {
                return context.NotificationCriterias.Single(n => n.CustomerId == customerId);
            }
        }

    }
}
