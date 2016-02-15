using PI.Contract.DTOs.AccountSettings;
using PI.Contract.DTOs.Customer;
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
       
        //get the profile details
        public ProfileDto getProfileByUserName(string username)
        {
            ProfileDto currentProfile=new ProfileDto();
            currentProfile.CustomerDetails = new CustomerDto();
            currentProfile.CustomerDetails.CustomerAddress = new Contract.DTOs.Address.AddressDto();

            Address currentAddress;
            AccountSettings currentAccountSettings;
            NotificationCriteria currentnotificationCriteria;

           Customer currentCustomer = this.GetCustomerByUserId(username);
           

            if (currentCustomer==null)
            {
                return null;
            }

            Company currentCompany = this.GetCompanyById(currentCustomer.CompanyId);
            Tenant currentTenant = null;

            if (currentCompany != null)
            {
                currentTenant = this.GetTenantById(currentCompany.TenantId);

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

            if (currentCompany!=null)
            {
                currentProfile.COCNumber = currentCompany.COCNumber;
                currentProfile.VATNumber = currentCompany.VATNumber;
            }
            if (currentTenant!=null)
            {
                currentProfile.CustomerDetails.IsCorporateAccount = currentTenant.IsCorporateAccount;
            }            
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
                currentProfile.DefaultTimeZoneId = currentAccountSettings.DefaultTimeZoneId;
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

        public int updateProfileData(ProfileDto updatedProfile)
        {
            Customer currentCustomer;
            Address currentAddress;
            AccountSettings currentAccountSettings;
            NotificationCriteria currentNotificationCriteria;
            Company curentCompany;
            Tenant currentTenant;


            if (updatedProfile == null)
            {
                return 0;
            }
            currentCustomer = this.GetCustomerByUserName(updatedProfile.CustomerDetails.Email);

            if (currentCustomer == null)
            {
                return 0;
            }

            using (PIContext context = PIContext.Get())
            {
                //updating basic customer details
                currentCustomer.Salutation = updatedProfile.CustomerDetails.Salutation;
                currentCustomer.FirstName = updatedProfile.CustomerDetails.FirstName;
                currentCustomer.MiddleName = updatedProfile.CustomerDetails.MiddleName;
                currentCustomer.LastName = updatedProfile.CustomerDetails.LastName;
                currentCustomer.Email = updatedProfile.CustomerDetails.Email;
                currentCustomer.PhoneNumber = updatedProfile.CustomerDetails.PhoneNumber;
                currentCustomer.MobileNumber = updatedProfile.CustomerDetails.MobileNumber;
                currentCustomer.UserName = updatedProfile.CustomerDetails.UserName;
                currentCustomer.Password = updatedProfile.CustomerDetails.Password;
                //set customer entity state as modified
                context.Customers.Attach(currentCustomer);
                context.Entry(currentCustomer).State = System.Data.Entity.EntityState.Modified;

                currentAddress = this.GetAddressbyId(currentCustomer.AddressId);
                currentAccountSettings = this.GetAccountSettingByCustomerId(currentCustomer.Id);
                currentNotificationCriteria = this.GetNotificationCriteriaByCustomerId(currentCustomer.Id);

                if (currentAddress != null)
                {
                    currentAddress.Country = updatedProfile.CustomerDetails.CustomerAddress.Country;
                    currentAddress.ZipCode = updatedProfile.CustomerDetails.CustomerAddress.ZipCode;
                    currentAddress.Number = updatedProfile.CustomerDetails.CustomerAddress.Number;
                    currentAddress.StreetAddress1 = updatedProfile.CustomerDetails.CustomerAddress.StreetAddress1;
                    currentAddress.StreetAddress2 = updatedProfile.CustomerDetails.CustomerAddress.StreetAddress2;
                    currentAddress.City = updatedProfile.CustomerDetails.CustomerAddress.City;
                    currentAddress.State = updatedProfile.CustomerDetails.CustomerAddress.State;
                    //set address entity state as modified
                    context.Addresses.Attach(currentAddress);
                    context.Entry(currentAddress).State = System.Data.Entity.EntityState.Modified;
                }
                //Assign Account setting values to the Profile Dto
                if (currentAccountSettings != null)
                {
                    currentAccountSettings.DefaultLanguageId = updatedProfile.DefaultLanguageId;
                    currentAccountSettings.DefaultCurrencyId = updatedProfile.DefaultCurrencyId;
                    currentAccountSettings.DefaultTimeZoneId = updatedProfile.DefaultTimeZoneId;
                    //set account settings entity as modidied
                    context.AccountSettings.Attach(currentAccountSettings);
                    context.Entry(currentAccountSettings).State = System.Data.Entity.EntityState.Modified;
                }

                else
                {
                    AccountSettings newAccountSettings = new AccountSettings();
                    newAccountSettings.CustomerId = currentCustomer.Id;
                    newAccountSettings.DefaultLanguageId = updatedProfile.DefaultLanguageId;
                    newAccountSettings.DefaultCurrencyId = updatedProfile.DefaultCurrencyId;
                    newAccountSettings.DefaultTimeZoneId = updatedProfile.DefaultTimeZoneId;
                    //set account settings entity as modidied
                    context.AccountSettings.Attach(newAccountSettings);
                    context.Entry(newAccountSettings).State = System.Data.Entity.EntityState.Modified;

                }

                //Assign Notofication criteria to the Profile Dto
                if (currentNotificationCriteria != null)
                {
                    currentNotificationCriteria.BookingConfirmation = updatedProfile.BookingConfirmation;
                    currentNotificationCriteria.PickupConfirmation = updatedProfile.PickupConfirmation;
                    currentNotificationCriteria.ShipmentDelay = updatedProfile.ShipmentDelay;
                    currentNotificationCriteria.ShipmentException = updatedProfile.ShipmentException;
                    currentNotificationCriteria.NotifyNewSolution = updatedProfile.NotifyNewSolution;
                    currentNotificationCriteria.NotifyDiscountOffer = updatedProfile.NotifyDiscountOffer;
                    //set notification criteria entity as modified
                    context.NotificationCriterias.Attach(currentNotificationCriteria);
                    context.Entry(currentNotificationCriteria).State = System.Data.Entity.EntityState.Modified;
                }
                else
                {
                    NotificationCriteria newNotificationCriteria = new NotificationCriteria();
                    newNotificationCriteria.CustomerId= currentCustomer.Id;
                    newNotificationCriteria.BookingConfirmation = updatedProfile.BookingConfirmation;
                    newNotificationCriteria.PickupConfirmation = updatedProfile.PickupConfirmation;
                    newNotificationCriteria.ShipmentDelay = updatedProfile.ShipmentDelay;
                    newNotificationCriteria.ShipmentException = updatedProfile.ShipmentException;
                    newNotificationCriteria.NotifyNewSolution = updatedProfile.NotifyNewSolution;
                    newNotificationCriteria.NotifyDiscountOffer = updatedProfile.NotifyDiscountOffer;
                    //set notification criteria entity as modified
                    context.NotificationCriterias.Attach(newNotificationCriteria);
                    context.Entry(newNotificationCriteria).State = System.Data.Entity.EntityState.Modified;

                }

                //saving changes of updated profile
                context.SaveChanges();
            }
            return 1;

        }

       

        //get the customer details by username(email)
        public Customer GetCustomerByUserId(string userId)
        {
            using (PIContext context = PIContext.Get())
            {
                return context.Customers.SingleOrDefault(c => c.UserId == userId);
            }
        }

        //get address details by Id
        public Address GetAddressbyId(long addressId)
        {
            using (PIContext context = PIContext.Get())
            {
                return context.Addresses.SingleOrDefault(a => a.Id == addressId);
            }
        }

        //get Account Settings by customer Id
        public AccountSettings GetAccountSettingByCustomerId(long customerId)
        {
            using (PIContext context = PIContext.Get())
            {
                return context.AccountSettings.SingleOrDefault(s => s.CustomerId == customerId);
            }
        }

        //get the notofication criterias bt customer Id
        public NotificationCriteria GetNotificationCriteriaByCustomerId(long customerId)
        {
            using (PIContext context = PIContext.Get())
            {
                return context.NotificationCriterias.SingleOrDefault(n => n.CustomerId == customerId);
            }
        }

        public Company GetCompanyById(long CustomerId)
        {
            using (PIContext context = PIContext.Get())
            {
                return context.Companies.SingleOrDefault(n => n.Id == CustomerId);
            }

        }

        public Tenant GetTenantById(long TenantId)
        {
            using (PIContext context= PIContext.Get())
            {
                return context.Tenants.SingleOrDefault(n => n.Id == TenantId);
            }
        }

       



        //Get Account Setting Details
        //retrieve all languages
        public IQueryable<LanguageDto> GetAllLanguages()
        {
            using (PIContext context = PIContext.Get())
            {
                var languages = from l in context.Languages
                                select new LanguageDto()
                                {
                                    Id=l.Id,
                                    LanguageCode = l.LanguageCode,
                                    LanguageName = l.LanguageName
                                };
                return languages;
            }           
        }

        //retrieve all currencies
        public IQueryable<CurrencyDto> GetAllCurrencies()
        {
            using (PIContext context = PIContext.Get())
            {
                var currencies = from c in context.Currencies
                                 select new CurrencyDto()
                                 {
                                     Id=c.Id,
                                     CurrencyCode=c.CurrencyCode,
                                     CurrencyName=c.CurrencyName
                                 };
                return currencies;
            }
        }
        
        //retrieve all TimeZones
        public IQueryable<TimeZoneDto> GetAllTimeZones()
        {
            using (PIContext context = PIContext.Get())
            {
                var timeZones = from t in context.TimeZones
                                select new TimeZoneDto()
                                {
                                    Id=t.Id,
                                    TimeZoneCode=t.TimeZoneCode,
                                    CountryName=t.CountryName
                                };
                return timeZones;
            }
        }

    }
}
