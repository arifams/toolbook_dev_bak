using Microsoft.VisualStudio.TestTools.UnitTesting;
using PI.Business;
using PI.Contract.DTOs.Address;
using PI.Contract.DTOs.Company;
using PI.Contract.DTOs.CostCenter;
using PI.Contract.DTOs.Customer;
using PI.Contract.DTOs.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Business.Tests
{
    [TestClass()]
    public class ProfileManagementTests
    {

        ProfileManagement profile = null;

        [TestInitialize]
        public void Initialize()
        {
            profile = new ProfileManagement();

        }

        [TestMethod()]
        public void getProfileByUserNameTest()
        {
            string username = "";
            ProfileDto response = profile.getProfileByUserName(username);
            Assert.AreNotEqual(response, null);
        }

        [TestMethod()]
        public void getProfileByUserNameForShipmentTest()
        {
            string username = "";
            ProfileDto response = profile.getProfileByUserNameForShipment(username);
            Assert.AreNotEqual(response, null);
        }

        [TestMethod()]
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

        [TestMethod()]
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

        [TestMethod()]
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

        [TestMethod()]
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

        [TestMethod()]
        public void UpdateSetupWizardBillingAddressTest()
        {
            new NotImplementedException();
        }

        [TestMethod()]
        public void UpdateProfileLoginDetailsTest()
        {
            new NotImplementedException();
        }

        [TestMethod()]
        public void UpdateProfileAccountSettingsTest()
        {
            
        }

        [TestMethod()]
        public void UpdateThemeColourTest()
        {
            
        }

        [TestMethod()]
        public void GetUserbyUserNameTest()
        {
            new NotImplementedException();
        }

        [TestMethod()]
        public void GetCustomerByUserIdTest()
        {
            
        }

        [TestMethod()]
        public void GetLanguageCodeByUserIdTest()
        {
            new NotImplementedException();
        }

        [TestMethod()]
        public void GetCustomerByUserEmailTest()
        {
            new NotImplementedException();
        }

        [TestMethod()]
        public void GetUserByIdTest()
        {
            new NotImplementedException();
        }

        [TestMethod()]
        public void GetAddressbyIdTest()
        {
            
        }

        [TestMethod()]
        public void GetAccountSettingByCustomerIdTest()
        {
            
        }

        [TestMethod()]
        public void GetNotificationCriteriaByCustomerIdTest()
        {
            
        }

        [TestMethod()]
        public void GetCompanyByTenantIdTest()
        {
            
        }

        [TestMethod()]
        public void GetTenantByIdTest()
        {
            
        }

        [TestMethod()]
        public void GetCostCenterByCompanyIdTest()
        {
            
        }

        [TestMethod()]
        public void GetCostCenterByIdTest()
        {
            
        }

        [TestMethod()]
        public void GetCustomerAddressDetailsTest()
        {
            
        }

        [TestMethod()]
        public void GetAccountSettingsTest()
        {
            
        }

        [TestMethod()]
        public void GetRoleNameByIdTest()
        {
            
        }

        [TestMethod()]
        public void GetAllCurrenciesTest()
        {
            
        }

        [TestMethod()]
        public void GetAllTimeZonesTest()
        {
            
        }
    }
}