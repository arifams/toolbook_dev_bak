using PI.Contract.Business;
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
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Business
{
    public class ProfileManagement : IProfileManagement
    {
        CommonLogic commonLogics = new CommonLogic();

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


            //Assigning Address Details

            if (currentCustomer.CustomerAddress!=null)
            {
                currentProfile.CustomerDetails.CustomerAddress.ZipCode = currentCustomer.CustomerAddress.ZipCode;
                currentProfile.CustomerDetails.CustomerAddress.StreetAddress1 = currentCustomer.CustomerAddress.StreetAddress1;
                currentProfile.CustomerDetails.CustomerAddress.StreetAddress2 = currentCustomer.CustomerAddress.StreetAddress2;
                currentProfile.CustomerDetails.CustomerAddress.Number = currentCustomer.CustomerAddress.Number;
                currentProfile.CustomerDetails.CustomerAddress.City = currentCustomer.CustomerAddress.City;
                currentProfile.CustomerDetails.CustomerAddress.State = currentCustomer.CustomerAddress.State;
                currentProfile.CustomerDetails.CustomerAddress.Country = currentCustomer.CustomerAddress.Country;
            }


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
                currentProfile.CompanyDetails.CompanyCode = currentCompany.CompanyCode;


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

        public ProfileDto getProfileByUserNameForShipment(string username)
        {
            ProfileDto currentProfile = new ProfileDto();
            currentProfile.CustomerDetails = new CustomerDto();
            currentProfile.CustomerDetails.CustomerAddress = new AddressDto();

            Address currentAddress;

            Customer currentCustomer = this.GetCustomerByUserId(username);
            ApplicationUser applicationUser = this.GetUserById(username);


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
            currentAddress = this.GetAddressbyId(currentCustomer.AddressId);

            //assign address values to the  Profile Dto
            if (currentAddress != null)
            {
                currentProfile.CustomerDetails.CustomerAddress.Id = currentAddress.Id;
                currentProfile.CustomerDetails.CustomerAddress.Country = currentAddress.Country;
                currentProfile.CustomerDetails.CustomerAddress.ZipCode = currentAddress.ZipCode;
                currentProfile.CustomerDetails.CustomerAddress.Number = currentAddress.Number;
                currentProfile.CustomerDetails.CustomerAddress.StreetAddress1 = currentAddress.StreetAddress1;
                currentProfile.CustomerDetails.CustomerAddress.StreetAddress2 = currentAddress.StreetAddress2;
                currentProfile.CustomerDetails.CustomerAddress.City = currentAddress.City;
                currentProfile.CustomerDetails.CustomerAddress.State = currentAddress.State;
            }

            var curentCompany = this.GetCompanyByTenantId(currentCustomer.User.TenantId);

            currentProfile.IsInvoicePaymentEnabled = curentCompany.IsInvoiceEnabled;
            currentProfile.CompanyDetails = new CompanyDto
            {
                Name = curentCompany.Name
            };
                
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

            using (PIContext context = new PIContext())
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

        public int UpdateProfileGeneral(ProfileDto updatedProfile)
        {
            bool updateUserName = false;

            using (PIContext context = new PIContext())
            {
                Customer currentCustomer = context.Customers.SingleOrDefault(c => c.UserId == updatedProfile.CustomerDetails.UserId);
                if (currentCustomer == null)
                {
                    // No customer found by user Id.
                    return 0;
                }

                ApplicationUser currentUser = context.Users.SingleOrDefault(c => c.Id == currentCustomer.UserId);
                if (currentUser == null)
                {
                    return 0;
                }

                // Check user change email address.
                if (updatedProfile.CustomerDetails.Email!=null && currentUser.UserName != updatedProfile.CustomerDetails.Email)
                {
                    // Check if there any users who has same email
                    ApplicationUser existingUser = this.GetUserbyUserName(updatedProfile.CustomerDetails.Email);
                    if (existingUser != null)
                    {
                        // This email is already registered.
                        return -2;
                    }
                    else
                    {
                        // Save in Users table.
                        currentUser.UserName = updatedProfile.CustomerDetails.Email;
                        currentUser.Email = updatedProfile.CustomerDetails.Email;
                        currentUser.EmailConfirmed = false;
                        updateUserName = true;
                    }
                }

                // Save in Users table.
                currentUser.Salutation = updatedProfile.CustomerDetails.Salutation;
                currentUser.FirstName = updatedProfile.CustomerDetails.FirstName;
                currentUser.LastName = updatedProfile.CustomerDetails.LastName;
                context.SaveChanges();

                // Save in Customer table.                
                currentCustomer.Salutation = updatedProfile.CustomerDetails.Salutation;
                currentCustomer.FirstName = updatedProfile.CustomerDetails.FirstName;
                currentCustomer.LastName = updatedProfile.CustomerDetails.LastName;


                //this section added for updating profile details for the first time user Login
                if (updatedProfile.CustomerDetails.UserName!=null)
                {
                    currentCustomer.UserName = updatedProfile.CustomerDetails.UserName;
                }
                if (updatedProfile.CustomerDetails.Email!=null)
                {
                    currentCustomer.Email = updatedProfile.CustomerDetails.Email;
                }
                if (updatedProfile.CustomerDetails.JobCapacity!=null)
                {
                    currentCustomer.JobCapacity = updatedProfile.CustomerDetails.JobCapacity;
                }              
              
                context.SaveChanges();

                Tenant currentTenant = context.Tenants.SingleOrDefault(n => n.Id == currentUser.TenantId);
                if (currentTenant == null)
                {
                    return 0;
                }

                Company currentCompany = context.Companies.SingleOrDefault(n => n.TenantId == currentTenant.Id);
                if (currentCompany == null)
                {
                    return 0;
                }

                // Update the company
                if (updatedProfile.CompanyDetails!=null && updatedProfile.CompanyDetails.Name!=null)
                {
                    currentCompany.Name = updatedProfile.CompanyDetails.Name;
                    context.SaveChanges();
                }               

                currentTenant.IsCorporateAccount = updatedProfile.CustomerDetails.IsCorporateAccount;
                context.SaveChanges();
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

        public int UpdateProfileAddress(ProfileDto updatedProfile)
        {
            using (PIContext context = new PIContext())
            {
                Customer currentCustomer = context.Customers.SingleOrDefault(c => c.UserId == updatedProfile.CustomerDetails.UserId);
                if (currentCustomer == null)
                {
                    return 0;
                }

                // Update customer details.
                currentCustomer.PhoneNumber = updatedProfile.CustomerDetails.PhoneNumber;
                currentCustomer.MobileNumber = updatedProfile.CustomerDetails.MobileNumber;
                context.SaveChanges();

                ApplicationUser currentUser = context.Users.SingleOrDefault(c => c.Id == currentCustomer.UserId);
                if (currentUser == null)
                {
                    return 0;
                }

                Tenant currentTenant = this.GetTenantById(currentUser.TenantId);
                if (currentTenant == null)
                {
                    return 0;
                }

                Company currentCompany = context.Companies.SingleOrDefault(n => n.TenantId == currentTenant.Id);
                if (currentCompany == null)
                {
                    return 0;
                }

                CostCenter currentCostCenter = null;
                IList<CostCenter> currentCostCenters = this.GetCostCenterByCompanyId(currentCompany.Id).ToList();
                if (currentCostCenters != null && currentCostCenters.Count() == 1)
                {
                    currentCostCenter = currentCostCenters.Where(c => c.Type == "SYSTEM").FirstOrDefault();
                }

                if (currentCompany != null && updatedProfile.CompanyDetails!= null)
                {
                    if (updatedProfile.CompanyDetails.COCNumber!=null)
                    {
                        currentCompany.COCNumber = updatedProfile.CompanyDetails.COCNumber;
                    }
                    if (updatedProfile.CompanyDetails.VATNumber!=null)
                    {
                        currentCompany.VATNumber = updatedProfile.CompanyDetails.VATNumber;
                    }

                    if (updatedProfile.CompanyDetails.CompanyCode!=null)
                    {
                        currentCompany.CompanyCode = updatedProfile.CompanyDetails.CompanyCode;
                    }
                    if (updatedProfile.CompanyDetails.Name!=null)
                    {
                        currentCompany.Name = updatedProfile.CompanyDetails.Name;
                    }
                    context.SaveChanges();
                }

                if (currentCostCenter != null)
                {
                    var costCentercurrent = context.CostCenters.SingleOrDefault(n => n.Id == currentCostCenter.Id);
                    costCentercurrent.PhoneNumber = updatedProfile.CustomerDetails.PhoneNumber;
                    context.SaveChanges();
                }

                Address currentAddress = context.Addresses.SingleOrDefault(a => a.Id == currentCustomer.AddressId);

                if (currentAddress != null)
                {
                    currentAddress.Country = updatedProfile.CustomerDetails.CustomerAddress.Country;
                    currentAddress.ZipCode = updatedProfile.CustomerDetails.CustomerAddress.ZipCode;
                    currentAddress.Number = updatedProfile.CustomerDetails.CustomerAddress.Number;
                    currentAddress.StreetAddress1 = updatedProfile.CustomerDetails.CustomerAddress.StreetAddress1;
                    currentAddress.StreetAddress2 = updatedProfile.CustomerDetails.CustomerAddress.StreetAddress2;
                    currentAddress.City = updatedProfile.CustomerDetails.CustomerAddress.City;
                    currentAddress.State = updatedProfile.CustomerDetails.CustomerAddress.State;

                    context.SaveChanges();
                }
            }

            return 1;
        }

        public int UpdateProfileBillingAddress(ProfileDto updatedProfile)
        {
            Customer currentCustomer;
            using (PIContext context = new PIContext())
            {
                currentCustomer = context.Customers.SingleOrDefault(c => c.UserId == updatedProfile.CustomerDetails.UserId);
                if (currentCustomer == null)
                {
                    return 0;
                }

                // Updating basic customer details
                currentCustomer.SecondaryEmail = updatedProfile.CustomerDetails.SecondaryEmail;
                currentCustomer.IsCorpAddressUseAsBusinessAddress = updatedProfile.CustomerDetails.IsCorpAddressUseAsBusinessAddress;
                context.SaveChanges();
            }

            ApplicationUser currentUser;
            using (PIContext context = new PIContext())
            {
                currentUser = context.Users.SingleOrDefault(c => c.Id == currentCustomer.UserId);
                if (currentUser == null)
                {
                    return 0;
                }
            }

            Tenant currentTenant = this.GetTenantById(currentUser.TenantId);
            if (currentTenant == null)
            {
                return 0;
            }

            Company curentCompany = this.GetCompanyByTenantId(currentTenant.Id);
            if (curentCompany == null)
            {
                return 0;
            }

            using (PIContext context = new PIContext())
            {
                CostCenter currentCostCenter = (from c in context.CostCenters
                                                where c.CompanyId == curentCompany.Id && !c.IsDelete && c.Type == "SYSTEM"
                                                select c).FirstOrDefault();

                if (currentCostCenter != null)
                {
                    Address BusinessAddress = context.Addresses.SingleOrDefault(a => a.Id == currentCostCenter.BillingAddressId);

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
                    }
                }
            }
            return 1;
        }


        //update billing address as corporate address
        public int UpdateSetupWizardBillingAddress(ProfileDto updatedProfile)
        {
            Customer currentCustomer;
            using (PIContext context = new PIContext())
            {
                currentCustomer = context.Customers.SingleOrDefault(c => c.UserId == updatedProfile.CustomerDetails.UserId);
                if (currentCustomer == null)
                {
                    return 0;
                }

                // Updating basic customer details
                currentCustomer.SecondaryEmail = updatedProfile.CustomerDetails.SecondaryEmail;
                currentCustomer.IsCorpAddressUseAsBusinessAddress = updatedProfile.CustomerDetails.IsCorpAddressUseAsBusinessAddress;
                context.SaveChanges();
            }

            ApplicationUser currentUser;
            using (PIContext context = new PIContext())
            {
                currentUser = context.Users.SingleOrDefault(c => c.Id == currentCustomer.UserId);
                if (currentUser == null)
                {
                    return 0;
                }
            }

            Tenant currentTenant = this.GetTenantById(currentUser.TenantId);
            if (currentTenant == null)
            {
                return 0;
            }

            Company curentCompany = this.GetCompanyByTenantId(currentTenant.Id);
            if (curentCompany == null)
            {
                return 0;
            }

            using (PIContext context = new PIContext())
            {
                CostCenter currentCostCenter = (from c in context.CostCenters
                                                where c.CompanyId == curentCompany.Id && !c.IsDelete && c.Type == "SYSTEM"
                                                select c).FirstOrDefault();

                if (currentCostCenter != null)
                {
                    Address BusinessAddress = context.Addresses.SingleOrDefault(a => a.Id == currentCostCenter.BillingAddressId);

                    if (BusinessAddress != null  &&
                        updatedProfile.CustomerDetails.CustomerAddress!= null)
                    {
                        BusinessAddress.Number = updatedProfile.CustomerDetails.CustomerAddress.Number;
                        BusinessAddress.StreetAddress1 = updatedProfile.CustomerDetails.CustomerAddress.StreetAddress1;
                        BusinessAddress.StreetAddress2 = updatedProfile.CustomerDetails.CustomerAddress.StreetAddress2;
                        BusinessAddress.City = updatedProfile.CustomerDetails.CustomerAddress.City;
                        BusinessAddress.State = updatedProfile.CustomerDetails.CustomerAddress.State;
                        BusinessAddress.ZipCode = updatedProfile.CustomerDetails.CustomerAddress.ZipCode;
                        BusinessAddress.Country = updatedProfile.CustomerDetails.CustomerAddress.Country;
                        context.SaveChanges();

                    }
                    else
                    {
                        Address newBusinessAddress = new Address();
                        newBusinessAddress.Number = updatedProfile.CustomerDetails.CustomerAddress.Number;
                        newBusinessAddress.StreetAddress1 = updatedProfile.CustomerDetails.CustomerAddress.StreetAddress1;
                        newBusinessAddress.StreetAddress2 = updatedProfile.CustomerDetails.CustomerAddress.StreetAddress2;
                        newBusinessAddress.City = updatedProfile.CustomerDetails.CustomerAddress.City;
                        newBusinessAddress.State = updatedProfile.CustomerDetails.CustomerAddress.State;
                        newBusinessAddress.ZipCode = updatedProfile.CustomerDetails.CustomerAddress.ZipCode;
                        newBusinessAddress.Country = updatedProfile.CustomerDetails.CustomerAddress.Country;

                        currentCostCenter.BillingAddressId = newBusinessAddress.Id;

                        context.Addresses.Add(newBusinessAddress);
                        context.SaveChanges();
                    }
                }
            }
            return 1;
        }

        public int UpdateProfileLoginDetails(ProfileDto updatedProfile)
        {
            using (PIContext context = new PIContext())
            {
                Customer currentCustomer = context.Customers.SingleOrDefault(c => c.UserId == updatedProfile.CustomerDetails.UserId);
                currentCustomer.Password = updatedProfile.NewPassword;
                context.SaveChanges();
            }

            return 1;
        }

        public int UpdateProfileAccountSettings(ProfileDto updatedProfile)
        {
            Customer currentCustomer = this.GetCustomerByUserId(updatedProfile.CustomerDetails.UserId);
            if (currentCustomer == null)
            {
                return 0;
            }

            using (PIContext context = new PIContext())
            {
                AccountSettings currentAccountSettings = context.AccountSettings.SingleOrDefault(s => s.CustomerId == currentCustomer.Id);

                //Assign Account setting values to the Profile Dto
                if (!updatedProfile.DoNotUpdateAccountSettings && currentAccountSettings != null)
                {
                    currentAccountSettings.DefaultLanguageId = updatedProfile.DefaultLanguageId;
                    currentAccountSettings.DefaultCurrencyId = updatedProfile.DefaultCurrencyId;
                    currentAccountSettings.DefaultTimeZoneId = updatedProfile.DefaultTimeZoneId;

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

                    context.AccountSettings.Add(newAccountSettings);
                    context.SaveChanges();
                }

                NotificationCriteria currentNotificationCriteria = context.NotificationCriterias.SingleOrDefault(n => n.CustomerId == currentCustomer.Id);

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

                    context.NotificationCriterias.Add(newNotificationCriteria);
                    context.SaveChanges();
                }
            }

            return 1;
        }


        /// <summary>
        /// Update Theme Colour
        /// </summary>
        /// <param name="updatedProfile"></param>
        /// <returns></returns>
        public int UpdateThemeColour(ProfileDto updatedProfile)
        {
            try
            {
                using (PIContext context = new PIContext())
                {
                    Customer currentCustomer = context.Customers.SingleOrDefault(c => c.UserId == updatedProfile.CustomerDetails.UserId);
                    if (currentCustomer == null)
                    {
                        return 0;
                    }

                    currentCustomer.SelectedColour = updatedProfile.SelectedColour;
                    context.SaveChanges();
                }

                return 1;
            }
            catch (Exception ex)
            {
                return 0;
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
            using (PIContext context = new PIContext())
            {
                return context.Customers.SingleOrDefault(c => c.UserId == userId);
            }
        }


        //get the profile language code
        public string GetLanguageCodeByUserId(string userId)
        {
            var customer = this.GetCustomerByUserId(userId);
            if (customer==null)
            {
                return null;
            }
            var accountsettings = this.GetAccountSettingByCustomerId(customer.Id);
            if (accountsettings==null)
            {
                return null;
            }
            return commonLogics.GetLanguageCodeById(accountsettings.DefaultLanguageId);
        }

        //get the customer by user name
        public Customer GetCustomerByUserEmail(string username)
        {
            using (PIContext context = new PIContext())
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
            using (PIContext context = new PIContext())
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
            using (PIContext context = new PIContext())
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

            accountSettings.Languages = GetAllLanguages();
            accountSettings.Currencies = GetAllCurrencies();
            accountSettings.TimeZones = GetAllTimeZones();

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

        //get role name by Id
        public string GetRoleNameById(string id)
        {
            var roleName = "";

            using (PIContext context = new PIContext())
            {
                roleName = (from n in context.Roles
                            where n.Id.Equals(id)
                            select n.Name).FirstOrDefault();
            }
            return roleName;
        }


        //Get Account Setting Details
        //retrieve all languages
        private List<LanguageDto> GetAllLanguages()
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
                return languages.ToList();
            }
        }

        //retrieve all currencies
        public List<CurrencyDto> GetAllCurrencies()
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
                return currencies.ToList();
            }
        }

        //retrieve all TimeZones
        public List<TimeZoneDto> GetAllTimeZones()
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
                return timeZones.ToList();
            }
        }

    }
}
