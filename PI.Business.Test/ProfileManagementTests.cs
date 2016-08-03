using NUnit.Framework;
using PI.Business;
using PI.Contract.DTOs.AccountSettings;
using PI.Contract.DTOs.Address;
using PI.Contract.DTOs.Company;
using PI.Contract.DTOs.CostCenter;
using PI.Contract.DTOs.Customer;
using PI.Contract.DTOs.Profile;
using PI.Data.Entity;
using PI.Data.Entity.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Business.Tests
{
    [TestFixture]
    public class ProfileManagementTests
    {

        ProfileManagement profile = null;

      
        public ProfileManagementTests()
        {
            profile = new ProfileManagement();

        }

        [Test]
        public void getProfileByUserNameTest()
        {
            string username = "";
            ProfileDto response = profile.getProfileByUserName(username);
            Assert.AreNotEqual(response, null);
        }

        [Test]
        public void getProfileByUserNameForShipmentTest()
        {
            string username = "";
            ProfileDto response = profile.getProfileByUserNameForShipment(username);
            Assert.AreNotEqual(response, null);
        }

        [Test]
        public void updateProfileDataTest()
        {
            ProfileDto updatedProfile = new ProfileDto()
            {
                CustomerDetails = new CustomerDto()
                {
                    UserId="",
                    Salutation="",
                    FirstName="",
                    MiddleName="",
                    LastName="",
                    Email=""
                }
            };
            int response = profile.updateProfileData(updatedProfile);
            Assert.AreEqual(response, 1);
        }

        [Test]
        public void UpdateProfileGeneralTest()
        {
            ProfileDto updatedProfile = new ProfileDto()
            {
                CustomerDetails = new CustomerDto()
                {
                    UserId = "",
                    UserName= "UserName",
                    Salutation = "Mr",
                    FirstName = "FirstName",
                    MiddleName = "MiddleName",
                    LastName = "LastName",
                    Email = "Email@E.com",
                    JobCapacity= "JobCapacity",
                    IsCorporateAccount=true
                }
            };
            int response = profile.UpdateProfileGeneral(updatedProfile);
            Assert.AreEqual(response, 1);

        }

        [Test]
        public void UpdateProfileAddressTest()
        {
            ProfileDto updatedProfile = new ProfileDto()
            {
                CustomerDetails = new CustomerDto()
                {
                    UserId = "",
                    UserName = "UserName",
                    Salutation = "Mr",
                    FirstName = "FirstName",
                    MiddleName = "MiddleName",
                    LastName = "LastName",
                    Email = "Email@E.com",
                    JobCapacity = "JobCapacity",
                    IsCorporateAccount = true
                }
            };
            int response = profile.UpdateProfileAddress(updatedProfile);
        }

        [Test]
        public void UpdateProfileBillingAddressTest()
        {
            ProfileDto profileDto = new ProfileDto()
            {
                CompanyDetails=new CompanyDto
                {
                   CostCenter= new CostCenterDto
                   {
                    BillingAddress= new AddressDto
                    {
                        Number="123",
                        StreetAddress1= "StreetAddress1",
                        StreetAddress2= "StreetAddress2",
                        City= "City",
                        State= "State",
                        ZipCode= "ZipCode",
                        Country= "Country"
                    }  
                }
                }
            };

            int response = profile.UpdateProfileBillingAddress(profileDto);
            Assert.AreEqual(response, 1);
           
        }

        [Test]
        public void UpdateSetupWizardBillingAddressTest()
        {
            ProfileDto profileDto = new ProfileDto()
            {
                CustomerDetails = new CustomerDto()
                {
                    UserId = "",
                    UserName = "UserName",
                    Salutation = "Mr",
                    FirstName = "FirstName",
                    MiddleName = "MiddleName",
                    LastName = "LastName",
                    Email = "Email@E.com",
                    JobCapacity = "JobCapacity",
                    IsCorporateAccount = true
                }
            };

            int response = profile.UpdateSetupWizardBillingAddress(profileDto);
            Assert.AreEqual(response, 1)
;        }

        [Test]
        public void UpdateProfileLoginDetailsTest()
        {
            ProfileDto profileDto = new ProfileDto()
            {
                NewPassword= "NewPassword",
                CustomerDetails = new CustomerDto()
                {
                    UserId = "",
                    UserName = "UserName"                    
                }              

            };

            int response = profile.UpdateProfileLoginDetails(profileDto);
            Assert.AreEqual(response, 1);
        }

        [Test]
        public void UpdateProfileAccountSettingsTest()
        {
            ProfileDto profileDto = new ProfileDto()
            {
                NewPassword = "NewPassword",
                DefaultLanguageId=1,
                DefaultCurrencyId=1,
                DefaultTimeZoneId=1,
                DefaultWeightMetricId=1,
                DefaultVolumeMetricId=1,

                BookingConfirmation=true,
                PickupConfirmation=true,
                ShipmentDelay=true,
                ShipmentException=true,
                NotifyNewSolution=true,
                NotifyDiscountOffer=true,

                CustomerDetails = new CustomerDto()
                {
                    UserId = "",
                    UserName = "UserName"
                }

            };

            int response = profile.UpdateProfileAccountSettings(profileDto);
            Assert.AreEqual(response, 1);
        }

        [Test]
        public void UpdateThemeColourTest()
        {
            ProfileDto profileDto = new ProfileDto()
            {
                SelectedColour= "#396485",
                CustomerDetails = new CustomerDto()
                {
                    UserId = "",
                    UserName = "UserName"
                }

            };
            int response = profile.UpdateThemeColour(profileDto);

        }

        [Test]
        public void GetUserbyUserNameTest()
        {
            string UserName = "info@parcelinternational.com";
            ApplicationUser response = profile.GetUserbyUserName(UserName);
            Assert.AreNotEqual(response, null);
        }

        [Test]
        public void GetCustomerByUserIdTest()
        {
            string userId = "";
            Customer response = profile.GetCustomerByUserId(userId);
            Assert.AreNotEqual(response, null);
        }

        [Test]
        public void GetLanguageCodeByUserIdTest()
        {
            string userId = "";
            string response=profile.GetLanguageCodeByUserId(userId);
            Assert.AreNotEqual(response, null);
        }

        [Test]
        public void GetCustomerByUserEmailTest()
        {
            string username = "";
            Customer response = profile.GetCustomerByUserEmail(username);
            Assert.AreNotEqual(response, null);
        }

        [Test]
        public void GetUserByIdTest()
        {
            string userId = "";
            ApplicationUser response = profile.GetUserById(userId);
            Assert.AreNotEqual(response, null);
        }

        [Test]
        public void GetAddressbyIdTest()
        {
            long addressId = 1;
            Address response = profile.GetAddressbyId(addressId);
            Assert.AreNotEqual(response, null);
        }

        [Test]
        public void GetAccountSettingByCustomerIdTest()
        {
            long customerId = 1;
            AccountSettings response = profile.GetAccountSettingByCustomerId(customerId);
            Assert.AreNotEqual(response, 1);
        }

        [Test]
        public void GetNotificationCriteriaByCustomerIdTest()
        {
            long customerId = 1;
            NotificationCriteria response = profile.GetNotificationCriteriaByCustomerId(customerId);
            Assert.AreNotEqual(response, null);
        }

        [Test]
        public void GetCompanyByTenantIdTest()
        {
            long TenantId = 1;
            Company response = profile.GetCompanyByTenantId(TenantId);
            Assert.AreNotEqual(response, null);
        }

        [Test]
        public void GetTenantByIdTest()
        {
            long TenantId = 1;
            Tenant response = profile.GetTenantById(TenantId);
            Assert.AreNotEqual(response, null);
            
        }

        [Test]
        public void GetCostCenterByCompanyIdTest()
        {
            long companyId = 1;
            IQueryable<CostCenter> response = profile.GetCostCenterByCompanyId(companyId);
            Assert.AreNotEqual(response.Count(), 0);
        }

        [Test]
        public void GetCostCenterByIdTest()
        {
            long CostCenterId = 1;
            CostCenter response = profile.GetCostCenterById(CostCenterId);
            Assert.AreNotEqual(response, null);
        }

        [Test]
        public void GetCustomerAddressDetailsTest()
        {
            long cusomerAddressId=1;
            long companyId = 1;
            ProfileDto response = profile.GetCustomerAddressDetails(cusomerAddressId, companyId);
            Assert.AreNotEqual(response, null);
        }

        [Test]
        public void GetAccountSettingsTest()
        {
            long customerId = 1;
            ProfileDto response = profile.GetAccountSettings(customerId);
            Assert.AreNotEqual(response, null);
        }

        [Test]
        public void GetRoleNameByIdTest()
        {
            string id = "";
            string response = profile.GetRoleNameById(id);
            Assert.AreNotEqual(response, string.Empty);
        }

        [Test]
        public void GetAllCurrenciesTest()
        {
            List<CurrencyDto> response = profile.GetAllCurrencies();
            Assert.AreNotEqual(response, null);
        }

        [Test]
        public void GetAllTimeZonesTest()
        {
            List<TimeZoneDto> response = profile.GetAllTimeZones();
            Assert.AreNotEqual(response, null);
        }
    }
}