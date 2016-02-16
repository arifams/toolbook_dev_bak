using PI.Contract.DTOs.AccountSettings;
using PI.Contract.DTOs.Address;
using PI.Contract.DTOs.Company;
using PI.Contract.DTOs.CostCenter;
using PI.Contract.DTOs.Customer;
using PI.Contract.DTOs.Profile;
using PI.Data;
using PI.Data.Entity;
using PI.Data.Entity.Identity;
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
            currentProfile.CustomerDetails.CustomerAddress = new AddressDto();

            Address currentAddress;
            AccountSettings currentAccountSettings;
            NotificationCriteria currentnotificationCriteria;

           Customer currentCustomer = this.GetCustomerByUserId(username);
           ApplicationUser applicationUser = this.GetUserById(username);

            Tenant currentTenant = null;
            Company currentCompany = null;
            CostCenter currentCostCenter = null;

            if (applicationUser != null)
            {
                currentTenant = this.GetTenantById(applicationUser.TenantId);
            }

            if (currentTenant!=null)
            {
                currentCompany = this.GetCompanyByTenantId(currentTenant.Id);
            }

            if (currentCompany!=null)
            {
                currentCostCenter = this.GetCostCenterByCompanyId(currentCompany.Id);
            }

                  
            //assigning basic customer details to Dto
            currentProfile.CustomerDetails.Salutation = currentCustomer.Salutation;
            currentProfile.CustomerDetails.FirstName = currentCustomer.FirstName;
            currentProfile.CustomerDetails.MiddleName = currentCustomer.MiddleName;
            currentProfile.CustomerDetails.LastName = currentCustomer.LastName;
            currentProfile.CustomerDetails.Email = currentCustomer.Email;
            currentProfile.CustomerDetails.PhoneNumber =currentCustomer.PhoneNumber;
            currentProfile.CustomerDetails.MobileNumber = currentCustomer.MobileNumber;
            //currentProfile.CustomerDetails.UserName = currentCustomer.UserName;
            //currentProfile.CustomerDetails.Password = currentCustomer.Password;
            currentProfile.CustomerDetails.IsCorpAddressUseAsBusinessAddress = currentCustomer.IsCorpAddressUseAsBusinessAddress;
            
            if (currentCompany!=null)
            {
                currentProfile.CompanyDetails = new CompanyDto();
                currentProfile.CompanyDetails.COCNumber = currentCompany.COCNumber;
                currentProfile.CompanyDetails.VATNumber = currentCompany.VATNumber;
                currentProfile.CompanyDetails.Name = currentCompany.Name;
                currentProfile.CustomerDetails.CompanyCode = currentCompany.CompanyCode;

            }

            if (currentTenant!=null)
            {
                currentProfile.CustomerDetails.IsCorporateAccount = currentTenant.IsCorporateAccount;
            }            
            currentAddress = this.GetAddressbyId(currentCustomer.AddressId);
            currentAccountSettings = this.GetAccountSettingByCustomerId(currentCustomer.Id);
            currentnotificationCriteria = this.GetNotificationCriteriaByCustomerId(currentCustomer.Id);

            if (currentCostCenter!=null && currentCustomer.IsCorpAddressUseAsBusinessAddress)
            {
                currentProfile.CompanyDetails.CostCenter = new CostCenterDto();
                currentProfile.CompanyDetails.CostCenter.BillingAddress = new AddressDto(); 

                currentProfile.CompanyDetails.CostCenter.BillingAddress.City = currentCostCenter.BillingAddress.City;
                currentProfile.CompanyDetails.CostCenter.BillingAddress.StreetAddress1 = currentCostCenter.BillingAddress.StreetAddress1;
                currentProfile.CompanyDetails.CostCenter.BillingAddress.StreetAddress2 = currentCostCenter.BillingAddress.StreetAddress2;
                currentProfile.CompanyDetails.CostCenter.BillingAddress.Number = currentCostCenter.BillingAddress.Number;
                currentProfile.CompanyDetails.CostCenter.BillingAddress.ZipCode = currentCostCenter.BillingAddress.ZipCode;
                currentProfile.CompanyDetails.CostCenter.BillingAddress.State = currentCostCenter.BillingAddress.State;                
                
            }
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
        

        //Update user profile detils
        public int updateProfileData(ProfileDto updatedProfile)
        {
            Customer currentCustomer;
            Address currentAddress;
            Address BusinessAddress;
            AccountSettings currentAccountSettings;
            NotificationCriteria currentNotificationCriteria;
            ApplicationUser currntUser;
            Company curentCompany;
            Tenant currentTenant;
            CostCenter currentCostCenter;


            if (updatedProfile == null)
            {
                return 0;
            }

            currentCustomer = this.GetCustomerByUserEmail(updatedProfile.CustomerDetails.UserId);          


            if (currentCustomer == null)
            {
                return 0;
            }

            currntUser = this.GetUserById(currentCustomer.UserId);

            if (currntUser==null)
            {
                return 0;
            }

            currentTenant = this.GetTenantById(currntUser.TenantId);

            if (currentTenant==null)
            {
                return 0;
            }

            curentCompany = this.GetCompanyByTenantId(currentTenant.Id);

            if (curentCompany==null)
            {
                return 0;
            }

            currentCostCenter = this.GetCostCenterByCompanyId(curentCompany.Id);

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
                currentCustomer.IsCorpAddressUseAsBusinessAddress = updatedProfile.CustomerDetails.IsCorpAddressUseAsBusinessAddress;
                //set customer entity state as modified
                context.Customers.Attach(currentCustomer);
                context.Entry(currentCustomer).State = System.Data.Entity.EntityState.Modified;

                if (currentTenant!=null)
                {
                    currentTenant.IsCorporateAccount = updatedProfile.CustomerDetails.IsCorporateAccount;

                    context.Tenants.Attach(currentTenant);
                    context.Entry(currentTenant).State = System.Data.Entity.EntityState.Modified;
                }

                if (curentCompany!=null)
                {
                    curentCompany.COCNumber = updatedProfile.CompanyDetails.COCNumber;
                    curentCompany.VATNumber = updatedProfile.CompanyDetails.VATNumber;
                    curentCompany.Name = updatedProfile.CompanyDetails.Name;
                    curentCompany.CompanyCode = updatedProfile.CustomerDetails.CompanyCode;

                    context.Companies.Attach(curentCompany);
                    context.Entry(curentCompany).State = System.Data.Entity.EntityState.Modified;
                }

                if (currentCostCenter!=null)
                {
                    BusinessAddress = this.GetAddressbyId(currentCostCenter.BillingAddressId);
                    if (BusinessAddress!=null)
                    {
                        BusinessAddress.Number = updatedProfile.CompanyDetails.CostCenter.BillingAddress.Number;
                        BusinessAddress.StreetAddress1 = updatedProfile.CompanyDetails.CostCenter.BillingAddress.StreetAddress1;
                        BusinessAddress.StreetAddress2 = updatedProfile.CompanyDetails.CostCenter.BillingAddress.StreetAddress2;
                        BusinessAddress.City = updatedProfile.CompanyDetails.CostCenter.BillingAddress.City;
                        BusinessAddress.State = updatedProfile.CompanyDetails.CostCenter.BillingAddress.State;
                        BusinessAddress.ZipCode = updatedProfile.CompanyDetails.CostCenter.BillingAddress.ZipCode;

                        context.Addresses.Attach(BusinessAddress);
                        context.Entry(BusinessAddress).State = System.Data.Entity.EntityState.Modified;

                    }
                    else
                    {
                        Address newBusinessAddress = new Address();
                        newBusinessAddress.Number = updatedProfile.CompanyDetails.CostCenter.BillingAddress.Number;
                        newBusinessAddress.StreetAddress1 = updatedProfile.CompanyDetails.CostCenter.BillingAddress.StreetAddress1;
                        newBusinessAddress.StreetAddress2 = updatedProfile.CompanyDetails.CostCenter.BillingAddress.StreetAddress2;
                        newBusinessAddress.City = updatedProfile.CompanyDetails.CostCenter.BillingAddress.City;
                        newBusinessAddress.State = updatedProfile.CompanyDetails.CostCenter.BillingAddress.State;
                        newBusinessAddress.ZipCode = updatedProfile.CompanyDetails.CostCenter.BillingAddress.ZipCode;

                        currentCostCenter.BillingAddressId = newBusinessAddress.Id;

                        context.Addresses.Attach(newBusinessAddress);
                        context.Entry(newBusinessAddress).State = System.Data.Entity.EntityState.Modified;

                        context.CostCenters.Attach(currentCostCenter);
                        context.Entry(currentCostCenter).State = System.Data.Entity.EntityState.Modified;


                    }


                }

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

       

        //get the customer details by userId
        public Customer GetCustomerByUserId(string userId)
        {
            using (PIContext context = PIContext.Get())
            {
                return context.Customers.SingleOrDefault(c => c.UserId == userId);
            }
        }

        //get the customer by user name
        public Customer GetCustomerByUserEmail(string username)
        {
            using (PIContext context = PIContext.Get())
            {
                return context.Customers.SingleOrDefault(c => c.Email == username);
            }
        }

        //get the user by ID
        public ApplicationUser GetUserById(string userId)
        {
            using (ApplicationDbContext context = ApplicationDbContext.Create())
            {
                return context.Users.SingleOrDefault(c => c.Id == userId);
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

        public Company GetCompanyByTenantId(long TenantId)
        {
            using (PIContext context = PIContext.Get())
            {
                return context.Companies.SingleOrDefault(n => n.TenantId == TenantId);
            }

        }

        public Tenant GetTenantById(long TenantId)
        {
            using (PIContext context= PIContext.Get())
            {
                return context.Tenants.SingleOrDefault(n => n.Id == TenantId);
            }
        }

        //get costcenter by company ID
        public CostCenter GetCostCenterByCompanyId(long companyId)
        {
            using (PIContext context = PIContext.Get())
            {
                return context.CostCenters.SingleOrDefault(n => n.CompanyId == companyId);
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
