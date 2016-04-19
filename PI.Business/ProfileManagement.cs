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
            ProfileDto currentProfile = new ProfileDto();
            currentProfile.CustomerDetails = new CustomerDto();
            currentProfile.CustomerDetails.CustomerAddress = new AddressDto();

            // Address currentAddress;
            //AccountSettings currentAccountSettings;
            //NotificationCriteria currentnotificationCriteria;

            Customer currentCustomer = this.GetCustomerByUserId(username);
            ApplicationUser applicationUser = this.GetUserById(username);

            Tenant currentTenant = null;
            Company currentCompany = null;
            IQueryable<CostCenter> currentCostCenters = null;
            CostCenter currentCostCenter = null;

            if (applicationUser != null)
            {
                currentTenant = this.GetTenantById(applicationUser.TenantId);
            }

            if (currentTenant != null)
            {
                currentCompany = this.GetCompanyByTenantId(currentTenant.Id);
            }

            if (currentCompany != null)
            {
                currentCostCenters = this.GetCostCenterByCompanyId(currentCompany.Id);

                if (currentCostCenters != null && currentCostCenters.Count() == 1)
                {
                    currentCostCenter = currentCostCenters.FirstOrDefault();
                }
            }

            //assigning basic customer details to Dto
            currentProfile.CustomerDetails.Id = currentCustomer.Id;
            currentProfile.CustomerDetails.UserId = currentCustomer.UserId;
            currentProfile.CustomerDetails.Salutation = currentCustomer.Salutation;
            currentProfile.CustomerDetails.FirstName = currentCustomer.FirstName;
            currentProfile.CustomerDetails.MiddleName = currentCustomer.MiddleName;
            currentProfile.CustomerDetails.LastName = currentCustomer.LastName;
            currentProfile.CustomerDetails.Email = currentCustomer.Email;
            currentProfile.CustomerDetails.SecondaryEmail = currentCustomer.SecondaryEmail;
            currentProfile.CustomerDetails.PhoneNumber = currentCustomer.PhoneNumber;
            currentProfile.CustomerDetails.MobileNumber = currentCustomer.MobileNumber;
            currentProfile.CustomerDetails.JobCapacity = currentCustomer.JobCapacity;
            currentProfile.CustomerDetails.AddressId = currentCustomer.AddressId;
          
            //currentProfile.CustomerDetails.UserName = currentCustomer.UserName;
            //currentProfile.CustomerDetails.Password = currentCustomer.Password;
            currentProfile.CustomerDetails.IsCorpAddressUseAsBusinessAddress = currentCustomer.IsCorpAddressUseAsBusinessAddress;

            if (currentCompany != null)
            {
                currentProfile.CompanyDetails = new CompanyDto();
                currentProfile.CompanyDetails.Id = currentCompany.Id;
                currentProfile.CompanyDetails.TenantId = currentCompany.TenantId;
                currentProfile.CompanyDetails.COCNumber = currentCompany.COCNumber;
                currentProfile.CompanyDetails.VATNumber = currentCompany.VATNumber;
                currentProfile.CompanyDetails.Name = currentCompany.Name;
                currentProfile.CustomerDetails.CompanyCode = currentCompany.CompanyCode;

            }

            if (currentTenant != null)
            {
                currentProfile.CustomerDetails.IsCorporateAccount = currentTenant.IsCorporateAccount;
            }

            //Assign Account setting values to the Profile Dto
            //if (currentAccountSettings != null)
            //{
            //    currentProfile.DefaultLanguageId = currentAccountSettings.DefaultLanguageId;
            //    currentProfile.DefaultCurrencyId = currentAccountSettings.DefaultCurrencyId;
            //    currentProfile.DefaultTimeZoneId = currentAccountSettings.DefaultTimeZoneId;
            //}

            //Assign Notofication criteria to the Profile Dto
            //if (currentnotificationCriteria != null)
            //{
            //    currentProfile.BookingConfirmation = currentnotificationCriteria.BookingConfirmation;
            //    currentProfile.PickupConfirmation = currentnotificationCriteria.PickupConfirmation;
            //    currentProfile.ShipmentDelay = currentnotificationCriteria.ShipmentDelay;
            //    currentProfile.ShipmentException = currentnotificationCriteria.ShipmentException;
            //    currentProfile.NotifyNewSolution = currentnotificationCriteria.NotifyNewSolution;
            //    currentProfile.NotifyDiscountOffer = currentnotificationCriteria.NotifyDiscountOffer;
            //}
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
            CostCenter currentCostCenter = null;
            IQueryable<CostCenter> currentCostCenters = null;
            bool updateUserName = false;


            if (updatedProfile == null)
            {
                return 0;
            }

            currentCustomer = this.GetCustomerByUserId(updatedProfile.CustomerDetails.UserId);


            if (currentCustomer == null)
            {
                return 0;
            }

            using (PIContext context = new PIContext())
            {
                currntUser = context.Users.SingleOrDefault(c => c.Id == currentCustomer.UserId);
                if (currntUser == null)
                {
                    return 0;
                }

                currntUser.Salutation = updatedProfile.CustomerDetails.Salutation;
                currntUser.FirstName = updatedProfile.CustomerDetails.FirstName;
                currntUser.MiddleName = updatedProfile.CustomerDetails.MiddleName;
                currntUser.LastName = updatedProfile.CustomerDetails.LastName;
                context.SaveChanges();

                //check if there any users who has same email
                if (currntUser.UserName != updatedProfile.CustomerDetails.Email)
                {                    
                        ApplicationUser existingUser = this.GetUserbyUserName(updatedProfile.CustomerDetails.Email);
                        ApplicationUser updatedUser = new ApplicationUser();
                        if (existingUser != null)
                        {
                            return -2;
                        }
                        else
                        {
                            var user = context.Users.SingleOrDefault(c => c.Id == currentCustomer.UserId);
                            user.UserName = updatedProfile.CustomerDetails.Email;
                        user.Email = updatedProfile.CustomerDetails.Email;
                            user.EmailConfirmed = false;
                        }
                        context.SaveChanges();
                    updateUserName = true;

                }

            }            

            currentTenant = this.GetTenantById(currntUser.TenantId);

            if (currentTenant == null)
            {
                return 0;
            }

            curentCompany = this.GetCompanyByTenantId(currentTenant.Id);

            if (curentCompany == null)
            {
                return 0;
            }

            currentCostCenters = this.GetCostCenterByCompanyId(curentCompany.Id);

            if (currentCostCenters != null && currentCostCenters.Count() == 1)
            {
                currentCostCenter = currentCostCenters.FirstOrDefault();
            }

            using (PIContext context = PIContext.Get())
            {
                //updating basic customer details
                currentCustomer.Salutation = updatedProfile.CustomerDetails.Salutation;
                currentCustomer.FirstName = updatedProfile.CustomerDetails.FirstName;
                currentCustomer.MiddleName = updatedProfile.CustomerDetails.MiddleName;
                currentCustomer.LastName = updatedProfile.CustomerDetails.LastName;
                currentCustomer.Email = updatedProfile.CustomerDetails.Email;
                currentCustomer.SecondaryEmail = updatedProfile.CustomerDetails.SecondaryEmail;
                currentCustomer.PhoneNumber = updatedProfile.CustomerDetails.PhoneNumber;
                currentCustomer.MobileNumber = updatedProfile.CustomerDetails.MobileNumber;
                currentCustomer.UserName = updatedProfile.CustomerDetails.UserName;
                currentCustomer.Password = updatedProfile.CustomerDetails.Password;
                currentCustomer.IsCorpAddressUseAsBusinessAddress = updatedProfile.CustomerDetails.IsCorpAddressUseAsBusinessAddress;
                currentCustomer.JobCapacity = updatedProfile.CustomerDetails.JobCapacity;
                //set customer entity state as modified
                //context.Customers.Add(currentCustomer);
                context.SaveChanges();

                if (currentTenant != null)
                {
                    currentTenant.IsCorporateAccount = updatedProfile.CustomerDetails.IsCorporateAccount;
                    context.SaveChanges();
                    //context.Tenants.Attach(currentTenant);
                    //context.Entry(currentTenant).State = System.Data.Entity.EntityState.Modified;
                }

                if (curentCompany != null)
                {
                    curentCompany.COCNumber = updatedProfile.CompanyDetails.COCNumber;
                    curentCompany.VATNumber = updatedProfile.CompanyDetails.VATNumber;
                    curentCompany.Name = updatedProfile.CompanyDetails.Name;
                    curentCompany.CompanyCode = updatedProfile.CustomerDetails.CompanyCode;

                    context.SaveChanges();
                }

                if (currentCostCenter != null)
                {
                    //  context.CostCenters.Attach(currentCostCenter);
                    var costCentercurrent = GetCostCenterById(currentCostCenter.Id);
                    costCentercurrent.PhoneNumber = updatedProfile.CompanyDetails.CostCenter.PhoneNumber;
                    context.SaveChanges();

                    BusinessAddress = this.GetAddressbyId(currentCostCenter.BillingAddressId);
                    if (BusinessAddress != null && updatedProfile.CompanyDetails.CostCenter != null &&
                        updatedProfile.CompanyDetails.CostCenter.BillingAddress != null)
                    {
                        BusinessAddress.Number = updatedProfile.CompanyDetails.CostCenter.BillingAddress.Number;
                        BusinessAddress.StreetAddress1 = updatedProfile.CompanyDetails.CostCenter.BillingAddress.StreetAddress1;
                        BusinessAddress.StreetAddress2 = updatedProfile.CompanyDetails.CostCenter.BillingAddress.StreetAddress2;
                        BusinessAddress.City = updatedProfile.CompanyDetails.CostCenter.BillingAddress.City;
                        BusinessAddress.State = updatedProfile.CompanyDetails.CostCenter.BillingAddress.State;
                        BusinessAddress.ZipCode = updatedProfile.CompanyDetails.CostCenter.BillingAddress.ZipCode;
                        BusinessAddress.Country = updatedProfile.CompanyDetails.CostCenter.BillingAddress.Country;

                        context.SaveChanges();

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
                        newBusinessAddress.Country = updatedProfile.CompanyDetails.CostCenter.BillingAddress.Country;
                        currentCostCenter.BillingAddressId = newBusinessAddress.Id;

                        context.Addresses.Add(newBusinessAddress);
                        context.SaveChanges();
                        //context.Entry(newBusinessAddress).State = System.Data.Entity.EntityState.Modified;

                        //context.CostCenters.Attach(currentCostCenter);
                        //context.Entry(currentCostCenter).State = System.Data.Entity.EntityState.Modified;
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
                    context.SaveChanges();
                }
                //Assign Account setting values to the Profile Dto
                if (!updatedProfile.DoNotUpdateAccountSettings && currentAccountSettings != null)
                {
                    currentAccountSettings.DefaultLanguageId = updatedProfile.DefaultLanguageId;
                    currentAccountSettings.DefaultCurrencyId = updatedProfile.DefaultCurrencyId;
                    currentAccountSettings.DefaultTimeZoneId = updatedProfile.DefaultTimeZoneId;

                    //set account settings entity as modidied                   
                    context.SaveChanges();
                }

                else
                {
                    AccountSettings newAccountSettings = new AccountSettings();
                    newAccountSettings.CustomerId = currentCustomer.Id;
                    newAccountSettings.DefaultLanguageId = updatedProfile.DefaultLanguageId;
                    newAccountSettings.DefaultCurrencyId = updatedProfile.DefaultCurrencyId;
                    newAccountSettings.DefaultTimeZoneId = updatedProfile.DefaultTimeZoneId;
                    newAccountSettings.CreatedDate = DateTime.Now;

                    //set account settings entity as modidied
                    context.AccountSettings.Add(newAccountSettings);
                    context.SaveChanges(); 

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
                    currentNotificationCriteria.CreatedDate = DateTime.Now;
                    //set notification criteria entity as modified

                    context.SaveChanges(); 
                }
                else
                {
                    NotificationCriteria newNotificationCriteria = new NotificationCriteria();
                    newNotificationCriteria.CustomerId = currentCustomer.Id;
                    newNotificationCriteria.BookingConfirmation = updatedProfile.BookingConfirmation;
                    newNotificationCriteria.PickupConfirmation = updatedProfile.PickupConfirmation;
                    newNotificationCriteria.ShipmentDelay = updatedProfile.ShipmentDelay;
                    newNotificationCriteria.ShipmentException = updatedProfile.ShipmentException;
                    newNotificationCriteria.NotifyNewSolution = updatedProfile.NotifyNewSolution;
                    newNotificationCriteria.NotifyDiscountOffer = updatedProfile.NotifyDiscountOffer;
                    newNotificationCriteria.CreatedDate = DateTime.Now;
                    //set notification criteria entity as modified
                    context.NotificationCriterias.Add(newNotificationCriteria);
                    context.SaveChanges(); //TODO:

                }

                //saving changes of updated profile
                //context.SaveChanges();
            }
            if (updateUserName)
            {
                return 3;
            }
            else
            {
                return 1;
            }
           

        }

        //check wheteher the updated user name is using by another user
        public ApplicationUser GetUserbyUserName(string UserName)
        {
            using (PIContext context = new PIContext())
            {
                return context.Users.SingleOrDefault(c => c.UserName == UserName);
            }
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
            using (PIContext context = PIContext.Create())
            {
                return context.Users.SingleOrDefault(c => c.Id == userId);
            }

        }

        //get address details by Id
        public Address GetAddressbyId(long addressId)
        {
            using (PIContext context = new PIContext())
            {
                return context.Addresses.SingleOrDefault(a => a.Id == addressId);
            }
        }

        //get Account Settings by customer Id
        public AccountSettings GetAccountSettingByCustomerId(long customerId)
        {
            using (PIContext context = new PIContext())
            {
                return context.AccountSettings.SingleOrDefault(s => s.CustomerId == customerId);
            }
        }

        //get the notofication criterias bt customer Id
        public NotificationCriteria GetNotificationCriteriaByCustomerId(long customerId)
        {
            using (PIContext context = new PIContext())
            {
                return context.NotificationCriterias.SingleOrDefault(n => n.CustomerId == customerId);
            }
        }

        public Company GetCompanyByTenantId(long TenantId)
        {
            using (PIContext context = new PIContext())
            {
                return context.Companies.SingleOrDefault(n => n.TenantId == TenantId);
            }

        }

        public Tenant GetTenantById(long TenantId)
        {
            using (PIContext context = PIContext.Get())
            {
                return context.Tenants.SingleOrDefault(n => n.Id == TenantId);
            }
        }

        //get costcenter by company ID
        public IQueryable<CostCenter> GetCostCenterByCompanyId(long companyId)
        {
            using (PIContext context = new PIContext())
            {
                // return context.CostCenters.Include("BillingAddress").(n => n.CompanyId == companyId);

                var costCenters = from c in context.CostCenters
                                  where c.CompanyId == companyId
                                  && c.IsDelete != true
                                  select c;
                                  
                return costCenters;
            }

        }

        public CostCenter GetCostCenterById(long CostCenterId)
        {
            using (PIContext context = PIContext.Get())
            {
                return context.CostCenters.SingleOrDefault(n => n.Id == CostCenterId);
            }
        }


        public ProfileDto GetCustomerAddressDetails(long cusomerAddressId, long companyId)
        {
            ProfileDto currentProfile = new ProfileDto();
            currentProfile.CustomerDetails = new CustomerDto();
            currentProfile.CompanyDetails = new CompanyDto();

            Address currentAddress = this.GetAddressbyId(cusomerAddressId);

            //assign address values to the  Profile Dto
            if (currentAddress != null)
            {
                currentProfile.CustomerDetails.CustomerAddress = new AddressDto();
                currentProfile.CustomerDetails.CustomerAddress.Id = currentAddress.Id;
                currentProfile.CustomerDetails.CustomerAddress.Country = currentAddress.Country;
                currentProfile.CustomerDetails.CustomerAddress.ZipCode = currentAddress.ZipCode;
                currentProfile.CustomerDetails.CustomerAddress.Number = currentAddress.Number;
                currentProfile.CustomerDetails.CustomerAddress.StreetAddress1 = currentAddress.StreetAddress1;
                currentProfile.CustomerDetails.CustomerAddress.StreetAddress2 = currentAddress.StreetAddress2;
                currentProfile.CustomerDetails.CustomerAddress.City = currentAddress.City;
                currentProfile.CustomerDetails.CustomerAddress.State = currentAddress.State;
            }

            IQueryable<CostCenter> currentCostCenters = this.GetCostCenterByCompanyId(companyId);
            CostCenter currentCostCenter = null;

            if (currentCostCenters != null && currentCostCenters.Count() == 1)
            {
                currentCostCenter = currentCostCenters.FirstOrDefault();
            }

            if (currentCostCenter != null)
            {
                currentProfile.CompanyDetails.CostCenter = new CostCenterDto();
                currentProfile.CompanyDetails.CostCenter.Id = currentCostCenter.Id;
                currentProfile.CompanyDetails.CostCenter.BillingAddressId = currentCostCenter.BillingAddressId;
                currentProfile.CompanyDetails.CostCenter.PhoneNumber = currentCostCenter.PhoneNumber;

                currentProfile.CompanyDetails.CostCenter.BillingAddress = new AddressDto();
                currentProfile.CompanyDetails.CostCenter.BillingAddress.Id = currentCostCenter.BillingAddress.Id;
                currentProfile.CompanyDetails.CostCenter.BillingAddress.City = currentCostCenter.BillingAddress.City;
                currentProfile.CompanyDetails.CostCenter.BillingAddress.StreetAddress1 = currentCostCenter.BillingAddress.StreetAddress1;
                currentProfile.CompanyDetails.CostCenter.BillingAddress.StreetAddress2 = currentCostCenter.BillingAddress.StreetAddress2;
                currentProfile.CompanyDetails.CostCenter.BillingAddress.Number = currentCostCenter.BillingAddress.Number;
                currentProfile.CompanyDetails.CostCenter.BillingAddress.ZipCode = currentCostCenter.BillingAddress.ZipCode;
                currentProfile.CompanyDetails.CostCenter.BillingAddress.State = currentCostCenter.BillingAddress.State;
                currentProfile.CompanyDetails.CostCenter.BillingAddress.Country = currentCostCenter.BillingAddress.Country;

            }

            return currentProfile;
        }


        public ProfileDto GetAccountSettings(long customerId)
        {
            ProfileDto profileDetails = new ProfileDto();
            AccountSettingsDto accountSettings = new AccountSettingsDto();

            AccountSettings dbSettings = GetAccountSettingByCustomerId(customerId);

            if (dbSettings != null)
            {
                accountSettings.DefaultCurrencyId = dbSettings.DefaultCurrencyId;
                accountSettings.DefaultLanguageId = dbSettings.DefaultLanguageId;
                accountSettings.DefaultTimeZoneId = dbSettings.DefaultTimeZoneId;
            }

            accountSettings.Languages = GetAllLanguages().ToList();
            accountSettings.Currencies = GetAllCurrencies().ToList();
            accountSettings.TimeZones = GetAllTimeZones().ToList();

            profileDetails.AccountSettings = accountSettings;
            NotificationCriteria notifications = this.GetNotificationCriteriaByCustomerId(customerId);


            if (notifications != null)
            {
                profileDetails.BookingConfirmation = notifications.BookingConfirmation;
                profileDetails.PickupConfirmation = notifications.PickupConfirmation;
                profileDetails.ShipmentDelay = notifications.ShipmentDelay;
                profileDetails.ShipmentException = notifications.ShipmentException;
                profileDetails.NotifyNewSolution = notifications.NotifyNewSolution;
                profileDetails.NotifyDiscountOffer = notifications.NotifyDiscountOffer;
            }

            return profileDetails;
        }


        //Get Account Setting Details
        //retrieve all languages
        private IQueryable<LanguageDto> GetAllLanguages()
        {
            using (PIContext context = PIContext.Get())
            {
                var languages = from l in context.Languages
                                select new LanguageDto()
                                {
                                    Id = l.Id,
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
                                     Id = c.Id,
                                     CurrencyCode = c.CurrencyCode,
                                     CurrencyName = c.CurrencyName
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
                                    Id = t.Id,
                                    TimeZoneCode = t.TimeZoneCode,
                                    CountryName = t.CountryName
                                };
                return timeZones;
            }
        }

    }
}
