using Microsoft.AspNet.Identity.EntityFramework;
using NUnit.Framework;
using PI.Business;
using PI.Business.Test;
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

namespace PI.Business.Tests
{
    [TestFixture]
    public class ProfileManagementTests
    {

        ProfileManagement profileManagement = null;

      
        public ProfileManagementTests()
        {
            List<IdentityRole> roles = new List<IdentityRole>()
            {
                new IdentityRole
                {
                    Id="1",
                    Name="BusinessOwner",

                }
            };

            List<IdentityUserRole> userroles = new List<IdentityUserRole>()
            {
                new IdentityUserRole
                {
                   RoleId="1",
                   UserId="1"

                }
            };


            List<ApplicationUser> users = new List<ApplicationUser>()
            {
                new ApplicationUser()
                {
                        Id="1",
                        UserName="user1@parcel.com",
                        Email="user1@parcel.com",
                        FirstName="fname",
                        LastName="lname",
                        IsActive=true,
                        IsDeleted=false,
                        Salutation="Mr",
                        TenantId=1,
                        PhoneNumber="1231231233",

                },
                 new ApplicationUser()
                {
                        Id="2",
                        UserName="user2@parcel.com",
                        Email="user2@parcel.com",
                        FirstName="fname",
                        LastName="lname",
                        IsActive=true,
                        IsDeleted=false,
                        Salutation="Mr",
                        TenantId=1,
                        PhoneNumber="1231231233",

                }
            };

            List<DivisionCostCenter> divisionCostCenters = new List<DivisionCostCenter>()
            {
                new DivisionCostCenter
                {
                    CostCenterId=1,
                    DivisionId=1,
                    Id=1,
                    IsAssigned=true,
                    IsActive=true,
                    IsDelete=false,
                    CostCenters=new CostCenter(),
                    Divisions= new Division()

                }
            };

            List<CostCenter> costCenters = new List<CostCenter>()
            {
               new CostCenter
                {
                    Id = 1,
                    BillingAddressId = 1,
                    CompanyId = 1,
                    Description = "costcenter",
                    Name = "cc_finance",
                    IsActive = true,
                    IsDelete = false,
                    PhoneNumber = "12312312312",
                    Type="USER",
                    BillingAddress = new Address
                    {
                         Id=1,
                         City="City",
                         Country="US",
                         Number="123",
                         State="State",
                         StreetAddress1="add1",
                         StreetAddress2="add2",
                         ZipCode="zipCode",
                         IsActive=true,
                         IsDelete=true
                    }
                   // DivisionCostCenters= divisionCostCenters
                   
,
                },
            };

            List<Company> companies = new List<Company>()
            {
                new Company
                {
                    Id=1,
                     Name="comp1",
                     IsActive=true,
                     IsDelete=false,
                     CompanyCode="comp1",
                     LogoUrl="http://parcelinternational.com",
                     TenantId=1,
                     COCNumber="1234",
                     VATNumber="vat123",
                     IsInvoiceEnabled=true,
                }
            };



            List<Tenant> tenants = new List<Tenant>()
            {
              new Tenant
              {
                    Id = 1,
                    TenancyName = "tenant1",
                    IsCorporateAccount = true,
                    IsActive = true,
                    IsDelete = false
              }
            };


            List<UserInDivision> userindivisions = new List<UserInDivision>()
            {
                new UserInDivision
                {
                    UserId="1",
                    DivisionId=1,
                    User= new ApplicationUser
                    {
                        Id="1",
                        UserName="user1",
                        Email="user1@parcel.com",
                        FirstName="fname",
                        LastName="lname",
                        IsActive=true,
                        IsDeleted=false,
                        Salutation="Mr",
                        TenantId=1,
                        PhoneNumber="1231231233",
                    }
                }
            };

            List<Division> divisions = new List<Division>
            {

             new Division {

                 Id=1,
                 CompanyId=1,
                 IsActive=true,
                 IsDelete=false,
                 Name="div1",
                 Description="finance_div",
                 DefaultCostCenterId=1,
                 UserInDivisions=userindivisions,
                 Type="USER",


                 CostCenter= new CostCenter
                 {
                     Id=1,
                     BillingAddressId=1,
                     CompanyId=1,
                     Description="costcenter",
                     Name="cc_finance",
                     IsActive=true,
                     IsDelete=false,
                     PhoneNumber="12312312312",
                     Type="USER"
,                 },

                 Company= new Company
                 {
                     Id=1,
                     Name="comp1",
                     IsActive=true,
                     IsDelete=false,
                     CompanyCode="comp1",
                     LogoUrl="http://parcelinternational.com",
                     TenantId=1,
                     COCNumber="1234",
                     VATNumber="vat123",
                     IsInvoiceEnabled=true,


                     Tenant= new Tenant
                     {
                         Id=1,
                         TenancyName="tenant1",
                         IsCorporateAccount=true,
                         IsActive=true,
                         IsDelete=false
                     }
                 }
             },

            };

            List<AccountSettings> accountSettings = new List<AccountSettings>()
            {
                new AccountSettings
                {
                    Id=1,
                    CustomerId=1,
                    DefaultCurrencyId=1,
                    DefaultLanguageId=1,
                    DefaultTimeZoneId=1,
                    VolumeMetricId=1,
                    WeightMetricId=1
                }
            };

            List<Language> languages = new List<Language>()
            {
                new Language
                {
                    Id=1,
                    LanguageCode="EN",
                    LanguageName="English"
                },
                new Language
                {
                    Id=2,
                    LanguageCode="DT",
                    LanguageName="Dutch"
                }

            };

            List<Currency> currencies = new List<Currency>
            {
                new Currency
                {
                    Id=1,
                    CurrencyCode="EUR",
                    CurrencyName="Euro"
                }
            };

            List<Data.Entity.TimeZone> timeZones = new List<Data.Entity.TimeZone>()
            {
                 new Data.Entity.TimeZone
                 {
                      Id=1,
                      TimeZoneCode="123",
                      CountryName="US"
                 }
            };

            List<Customer> customers = new List<Customer>()
            {
                new Customer
                {
                    UserId="1",
                    AddressId=1,
                    Email="user1@parcel.com",
                    FirstName="fname",
                    JobCapacity="jobcapacity",
                    LastName="lname",
                    Id=1,
                    User= new ApplicationUser
                    {
                        Id="1",
                        UserName="user1",
                        Email="user1@parcel.com",
                        FirstName="fname",
                        LastName="lname",
                        IsActive=true,
                        IsDeleted=false,
                        Salutation="Mr",
                        TenantId=1,
                        PhoneNumber="1231231233",
                    }

                },
                
            };

            List<NotificationCriteria> notifications = new List<NotificationCriteria>()
            {
                new NotificationCriteria
                {
                     Id=1,
                     CustomerId=1,
                     NotifyDiscountOffer=true,
                     BookingConfirmation=true,
                     NotifyNewSolution=true,
                     PickupConfirmation=true,
                     ShipmentDelay=true,
                     ShipmentException=true,
                     IsActive= true,
                     IsDelete= false
                            
                }
            };

            List<Address> addresses = new List<Address>()
            {
                new Address
                {
                    Id=1,
                    City="City",
                    Country="US",
                    Number="123",
                    State="State",
                    StreetAddress1="add1",
                    StreetAddress2="add2",
                    ZipCode="zipCode",
                    IsActive=true,
                    IsDelete=true                  
                },
                 new Address
                {
                    Id=2,
                    City="City",
                    Country="US",
                    Number="123",
                    State="State",
                    StreetAddress1="add1",
                    StreetAddress2="add2",
                    ZipCode="zipCode",
                    IsActive=true,
                    IsDelete=true
                }
            };

            var mockSetCustomers = MoqHelper.CreateMockForDbSet<Customer>()
                                               .SetupForQueryOn(customers)
                                               .WithAdd(customers);

            var mockSetDivisions = MoqHelper.CreateMockForDbSet<Division>()
                                                .SetupForQueryOn(divisions)
                                                .WithAdd(divisions);
            var mockSetUserindivisions = MoqHelper.CreateMockForDbSet<UserInDivision>()
                                               .SetupForQueryOn(userindivisions)
                                               .WithAdd(userindivisions);
            var mockSetCostCenters = MoqHelper.CreateMockForDbSet<CostCenter>()
                                             .SetupForQueryOn(costCenters)
                                             .WithAdd(costCenters);
            var mockSetCompanies = MoqHelper.CreateMockForDbSet<Company>()
                                             .SetupForQueryOn(companies)
                                             .WithAdd(companies);
            var mockSetTenants = MoqHelper.CreateMockForDbSet<Tenant>()
                                             .SetupForQueryOn(tenants)
                                             .WithAdd(tenants);
            var mockSetDivisionCostCenters = MoqHelper.CreateMockForDbSet<DivisionCostCenter>()
                                             .SetupForQueryOn(divisionCostCenters)
                                             .WithAdd(divisionCostCenters);
            var mockSetusers = MoqHelper.CreateMockForDbSet<ApplicationUser>()
                                            .SetupForQueryOn(users)
                                            .WithAdd(users);
            var mockSetAccountSettings= MoqHelper.CreateMockForDbSet<AccountSettings>()
                                          .SetupForQueryOn(accountSettings)
                                            .WithAdd(accountSettings);

            var mockSetCurrencies = MoqHelper.CreateMockForDbSet<Currency>()
                                         .SetupForQueryOn(currencies)
                                           .WithAdd(currencies);

            var mockSetLanguages = MoqHelper.CreateMockForDbSet<Language>()
                                         .SetupForQueryOn(languages)
                                           .WithAdd(languages);

            var mockSetTimeZones = MoqHelper.CreateMockForDbSet <Data.Entity.TimeZone> ()
                                         .SetupForQueryOn(timeZones)
                                           .WithAdd(timeZones);
            var mockSetNotoficationCriterias = MoqHelper.CreateMockForDbSet<NotificationCriteria>()
                                         .SetupForQueryOn(notifications)
                                           .WithAdd(notifications);

            var mockSetAddresses = MoqHelper.CreateMockForDbSet<Address>()
                                        .SetupForQueryOn(addresses)
                                          .WithAdd(addresses);


            var mockContext = MoqHelper.CreateMockForDbContext<PIContext, Division>(mockSetDivisions);


            mockContext.Setup(c => c.Divisions).Returns(mockSetDivisions.Object);
            mockContext.Setup(c => c.UsersInDivisions).Returns(mockSetUserindivisions.Object);
            mockContext.Setup(c => c.CostCenters).Returns(mockSetCostCenters.Object);
            mockContext.Setup(c => c.Companies).Returns(mockSetCompanies.Object);
            mockContext.Setup(c => c.Tenants).Returns(mockSetTenants.Object);
            mockContext.Setup(c => c.DivisionCostCenters).Returns(mockSetDivisionCostCenters.Object);
            mockContext.Setup(c => c.Users).Returns(mockSetusers.Object);
            mockContext.Setup(c => c.Customers).Returns(mockSetCustomers.Object);

            mockContext.Setup(c => c.AccountSettings).Returns(mockSetAccountSettings.Object);
            mockContext.Setup(c => c.Languages).Returns(mockSetLanguages.Object);
            mockContext.Setup(c => c.TimeZones).Returns(mockSetTimeZones.Object);
            mockContext.Setup(c => c.Currencies).Returns(mockSetCurrencies.Object);
            mockContext.Setup(c => c.NotificationCriterias).Returns(mockSetNotoficationCriterias.Object);

            mockContext.Setup(c => c.Addresses).Returns(mockSetAddresses.Object);
            profileManagement = new ProfileManagement(mockContext.Object);

        }

        [Test]
        public void getProfileByUserNameTest()
        {
            string username = "1";
            ProfileDto response = profileManagement.getProfileByUserName(username);
            Assert.AreEqual(response.CustomerDetails.Id, 1);
        }

        [Test]
        public void getProfileByUserNameForShipmentTest()
        {
            string username = "1";
            ProfileDto response = profileManagement.getProfileByUserNameForShipment(username);
            Assert.AreEqual(response.CustomerDetails.Id, 1);
        }

        [Test]
        public void updateProfileDataTest()
        {
            ProfileDto updatedProfile = new ProfileDto()
            {
                 DefaultLanguageId=1,
                 DefaultCurrencyId=1,
                 DefaultTimeZoneId=1,
                 BookingConfirmation=true,
                 PickupConfirmation=true,
                 ShipmentDelay=true,
                 ShipmentException=true,
                 NotifyNewSolution=true,
                 NotifyDiscountOffer=true,

            CustomerDetails = new CustomerDto()
                {
                    UserId="1",
                    Salutation="Mrs",
                    FirstName= "FirstName",
                    MiddleName= "MiddleName",
                    LastName= "LastName",
                    Email= "user1@parcel.com",
                    CustomerAddress= new AddressDto
                    {
                        Number = "123",
                        StreetAddress1 = "StreetAddress1",
                        StreetAddress2 = "StreetAddress2",
                        City = "City",
                        State = "State",
                        ZipCode = "ZipCode",
                        Country = "Country"

                    }
                },
                CompanyDetails = new CompanyDto
                {
                    Id=1,
                    VATNumber="123",
                    CompanyCode="comp123",
                    COCNumber="123",
                    TenantId=1,
                    Name="comp1",
                    CostCenter= new CostCenterDto
                    {
                        Id=1,
                        PhoneNumber="32342342342",
                        UserId="1",
                        BillingAddress= new AddressDto
                        {
                            Id=1,
                            City="city",
                            Country="US",
                             Number="123",
                             State="state",
                             StreetAddress1="add1",
                             StreetAddress2="add2",
                             ZipCode="zip"
                             
                        }
                        
                    }
                }
            };
            int response = profileManagement.updateProfileData(updatedProfile);
            Assert.AreEqual(response, 1);
        }

        [Test]
        public void updateProfileDataExistingUserTest()
        {
            ProfileDto updatedProfile = new ProfileDto()
            {
                DefaultLanguageId = 1,
                DefaultCurrencyId = 1,
                DefaultTimeZoneId = 1,
                BookingConfirmation = true,
                PickupConfirmation = true,
                ShipmentDelay = true,
                ShipmentException = true,
                NotifyNewSolution = true,
                NotifyDiscountOffer = true,

                CustomerDetails = new CustomerDto()
                {
                    UserId = "1",
                    Salutation = "Mrs",
                    FirstName = "FirstName",
                    MiddleName = "MiddleName",
                    LastName = "LastName",
                    Email = "user2@parcel.com",
                    CustomerAddress = new AddressDto
                    {
                        Number = "123",
                        StreetAddress1 = "StreetAddress1",
                        StreetAddress2 = "StreetAddress2",
                        City = "City",
                        State = "State",
                        ZipCode = "ZipCode",
                        Country = "Country"

                    }
                },
                CompanyDetails = new CompanyDto
                {
                    Id = 1,
                    VATNumber = "123",
                    CompanyCode = "comp123",
                    COCNumber = "123",
                    TenantId = 1,
                    Name = "comp1",
                    CostCenter = new CostCenterDto
                    {
                        Id = 1,
                        PhoneNumber = "32342342342",
                        UserId = "1",
                        BillingAddress = new AddressDto
                        {
                            Id = 1,
                            City = "city",
                            Country = "US",
                            Number = "123",
                            State = "state",
                            StreetAddress1 = "add1",
                            StreetAddress2 = "add2",
                            ZipCode = "zip"

                        }

                    }
                }
            };
            int response = profileManagement.updateProfileData(updatedProfile);
            Assert.AreEqual(response, -2);
        }

        [Test]
        public void UpdateProfileGeneralTest()
        {
            ProfileDto updatedProfile = new ProfileDto()
            {
                CustomerDetails = new CustomerDto()
                {
                    UserId = "1",
                    UserName= "user1@parcel.com",
                    Salutation = "Mr",
                    FirstName = "FirstName",
                    MiddleName = "MiddleName",
                    LastName = "LastName",
                    Email = "user1@parcel.com",
                    JobCapacity= "JobCapacity",
                    IsCorporateAccount=true
                }
            };
            int response = profileManagement.UpdateProfileGeneral(updatedProfile);
            Assert.AreEqual(response, 1);

        }

        [Test]
        public void UpdateProfileGeneralUpdateUserNameTest()
        {
            ProfileDto updatedProfile = new ProfileDto()
            {
                CustomerDetails = new CustomerDto()
                {
                    UserId = "1",
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
            int response = profileManagement.UpdateProfileGeneral(updatedProfile);
            Assert.AreEqual(response, 3);

        }

        [Test]
        public void UpdateProfileAddressTest()
        {
            ProfileDto updatedProfile = new ProfileDto()
            {
                CustomerDetails = new CustomerDto()
                {
                    UserId = "1",
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
            int response = profileManagement.UpdateProfileAddress(updatedProfile);
        }

        [Test]
        public void UpdateProfileBillingAddressTest()
        {
            ProfileDto profileDto = new ProfileDto()
            {
                CustomerDetails= new CustomerDto
                {
                    Id=1,
                    UserId="1",
                    SecondaryEmail="test@test",
                    IsCorpAddressUseAsBusinessAddress=false
                    
                },

                CompanyDetails=new CompanyDto
                {
                    Id=1,
                   CostCenter= new CostCenterDto
                   {
                       UserId="1",
                       Id=1,
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

            int response = profileManagement.UpdateProfileBillingAddress(profileDto);
            Assert.AreEqual(response, 1);
           
        }

        [TestCase("1")]
        [TestCase("10")]
        public void UpdateSetupWizardBillingAddressTest(string userId)
        {
            ProfileDto profileDto = new ProfileDto()
            {
                CustomerDetails = new CustomerDto()
                {
                    UserId = userId,
                    UserName = "user1@parcel.com",
                    Salutation = "Mr",
                    FirstName = "FirstName",
                    MiddleName = "MiddleName",
                    LastName = "LastName",
                    Email = "user1@parcel.com",
                    JobCapacity = "JobCapacity",
                    IsCorporateAccount = true,                     
                    SecondaryEmail= "Email@E.com",
                    CustomerAddress= new AddressDto
                    {
                        Number = "123",
                        StreetAddress1 = "StreetAddress1",
                        StreetAddress2 = "StreetAddress2",
                        City = "City",
                        State = "State",
                        ZipCode = "ZipCode",
                        Country = "Country"
                    }
                }
            };

            int response = profileManagement.UpdateSetupWizardBillingAddress(profileDto);
            if (userId=="1")
            {
                Assert.AreEqual(response, 1);
            }
            if (userId == "10")
            {
                Assert.AreEqual(response, 0);
            }
;        }

        [Test]
        public void UpdateProfileLoginDetailsTest()
        {
            ProfileDto profileDto = new ProfileDto()
            {
                NewPassword= "NewPassword",
                CustomerDetails = new CustomerDto()
                {
                    UserId = "1",
                    UserName = "UserName"                    
                }              

            };

            int response = profileManagement.UpdateProfileLoginDetails(profileDto);
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
                    UserId = "1",
                    UserName = "UserName"
                }

            };

            int response = profileManagement.UpdateProfileAccountSettings(profileDto);
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
                    UserId = "1",
                    UserName = "UserName"
                }

            };
            int response = profileManagement.UpdateThemeColour(profileDto);

        }

        [Test]
        public void GetUserbyUserNameTest()
        {
            string UserName = "user1@parcel.com";
            ApplicationUser response = profileManagement.GetUserbyUserName(UserName);
            Assert.AreEqual(response.Email, "user1@parcel.com");
        }

        [Test]
        public void GetCustomerByUserIdTest()
        {
            string userId = "1";
            Customer response = profileManagement.GetCustomerByUserId(userId);
            Assert.AreNotEqual(response, null);
        }

        [Test]
        public void GetLanguageCodeByUserIdTest()
        {
            string userId = "1";
            string response= profileManagement.GetLanguageCodeByUserId(userId);
            Assert.AreNotEqual(response, null);
        }

        [Test]
        public void GetCustomerByUserEmailTest()
        {
            string username ="user1@parcel.com";
            Customer response = profileManagement.GetCustomerByUserEmail(username);
            Assert.AreNotEqual(response, null);
        }

        [Test]
        public void GetUserByIdTest()
        {
            string userId = "1";
            ApplicationUser response = profileManagement.GetUserById(userId);
            Assert.AreNotEqual(response, null);
        }

        [Test]
        public void GetAddressbyIdTest()
        {
            long addressId = 1;
            Address response = profileManagement.GetAddressbyId(addressId);
            Assert.AreNotEqual(response, null);
        }

        [Test]
        public void GetAccountSettingByCustomerIdTest()
        {
            long customerId = 1;
            AccountSettings response = profileManagement.GetAccountSettingByCustomerId(customerId);
            Assert.AreNotEqual(response, 1);
        }

        [Test]
        public void GetNotificationCriteriaByCustomerIdTest()
        {
            long customerId = 1;
            NotificationCriteria response = profileManagement.GetNotificationCriteriaByCustomerId(customerId);
            Assert.AreNotEqual(response, null);
        }

        [Test]
        public void GetCompanyByTenantIdTest()
        {
            long TenantId = 1;
            Company response = profileManagement.GetCompanyByTenantId(TenantId);
            Assert.AreNotEqual(response, null);
        }

        [Test]
        public void GetTenantByIdTest()
        {
            long TenantId = 1;
            Tenant response = profileManagement.GetTenantById(TenantId);
            Assert.AreNotEqual(response, null);
            
        }

        [Test]
        public void GetCostCenterByCompanyIdTest()
        {
            long companyId = 1;
            IQueryable<CostCenter> response = profileManagement.GetCostCenterByCompanyId(companyId);
            Assert.AreNotEqual(response.Count(), 0);
        }

        [Test]
        public void GetCostCenterByIdTest()
        {
            long CostCenterId = 1;
            CostCenter response = profileManagement.GetCostCenterById(CostCenterId);
            Assert.AreNotEqual(response, null);
        }

        [Test]
        public void GetCustomerAddressDetailsTest()
        {
            long cusomerAddressId=1;
            long companyId = 1;
            ProfileDto response = profileManagement.GetCustomerAddressDetails(cusomerAddressId, companyId);
            Assert.AreNotEqual(response, null);
        }

        [Test]
        public void GetAccountSettingsTest()
        {
            long customerId = 1;
            ProfileDto response = profileManagement.GetAccountSettings(customerId);
            Assert.AreNotEqual(response, null);
        }

        [Test]
        public void GetRoleNameByIdTest()
        {
            string id = "1";
            string response = profileManagement.GetRoleNameById(id);
            Assert.AreNotEqual(response, string.Empty);
        }

        [Test]
        public void GetAllCurrenciesTest()
        {
            List<CurrencyDto> response = profileManagement.GetAllCurrencies();
            Assert.AreNotEqual(response, null);
        }

        [Test]
        public void GetAllTimeZonesTest()
        {
            List<TimeZoneDto> response = profileManagement.GetAllTimeZones();
            Assert.AreNotEqual(response, null);
        }
    }
}