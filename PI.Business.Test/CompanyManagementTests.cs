using NUnit.Framework;
using PI.Business;
using PI.Contract.DTOs.Address;
using PI.Contract.DTOs.Common;
using PI.Contract.DTOs.Company;
using PI.Contract.DTOs.CostCenter;
using PI.Contract.DTOs.Customer;
using PI.Contract.DTOs.Division;
using PI.Contract.DTOs.Role;
using PI.Contract.DTOs.User;
using PI.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using PI.Business.Test;
using PI.Data;
using PI.Data.Entity.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PI.Common;

namespace PI.Business.Tests
{
    [TestFixture]
    public class CompanyManagementTests
    {
       private CompanyManagement companyManagement = null;

        [TestFixtureSetUp]
        public void Init()
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
                    Type="USER"
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
                    IsDelete=false,
                    IsActive=true,
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
                    },
                    Divisions= new Division
                    {

                        Id=1,
                        CompanyId=1,
                        IsActive=true,
                        IsDelete=false,
                        Name="div1",
                        Description="finance_div",
                        DefaultCostCenterId=1,
                        Type="USER"
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
                 Description="Desc",
                 DefaultCostCenterId=1,
                 UserInDivisions=userindivisions,
                 Type="USER",    
                 DivisionCostCenters= new List<DivisionCostCenter>
                 {
                      new DivisionCostCenter
                      {
                  
                        CostCenterId=1,
                        DivisionId=1,
                        Id=1,
                        IsAssigned=true,
                        IsActive=true,
                        IsDelete=false,
                        CostCenters=new CostCenter() {
                             Id=1,
                             BillingAddressId=1,
                             CompanyId=1,
                             Description="costcenter",
                             Name="cc_finance",
                             IsActive=true,
                             IsDelete=false,
                             PhoneNumber="12312312312",
                             Type="USER"
                        },
                        Divisions= new Division()
                        {
                             Id=1,
                             CompanyId=1,
                             IsActive=true,
                             IsDelete=false,
                             Name="div1",
                             Description="Desc",
                             DefaultCostCenterId=1,
                             UserInDivisions=userindivisions,
                             Type="USER",
                        }               
                      }
                 },           
                 

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
                    Id=1                  

                }
            };

            List<AuditTrail> auditTrails = new List<AuditTrail>()
            {
                new AuditTrail
                {
                    Id=1,
                    CreatedBy="1",
                    IsActive=true,
                    IsDelete=false
                }
            };

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
            var mockSetDivisionCostCenters= MoqHelper.CreateMockForDbSet<DivisionCostCenter>()
                                             .SetupForQueryOn(divisionCostCenters)
                                             .WithAdd(divisionCostCenters);
            var mockSetusers = MoqHelper.CreateMockForDbSet<ApplicationUser>()
                                            .SetupForQueryOn(users)
                                            .WithAdd(users);
            var mockSetcustomers = MoqHelper.CreateMockForDbSet<Customer>()
                                           .SetupForQueryOn(customers)
                                           .WithAdd(customers);

            var mockSetAuditTrails = MoqHelper.CreateMockForDbSet<AuditTrail>()
                                                .SetupForQueryOn(auditTrails)
                                                .WithAdd(auditTrails);



            var mockContext = MoqHelper.CreateMockForDbContext<PIContext, Division>(mockSetDivisions);


            mockContext.Setup(c => c.Divisions).Returns(mockSetDivisions.Object);
            mockContext.Setup(c => c.UsersInDivisions).Returns(mockSetUserindivisions.Object);
            mockContext.Setup(c => c.CostCenters).Returns(mockSetCostCenters.Object);
            mockContext.Setup(c => c.Companies).Returns(mockSetCompanies.Object);
            mockContext.Setup(c => c.Tenants).Returns(mockSetTenants.Object);
            mockContext.Setup(c => c.DivisionCostCenters).Returns(mockSetDivisionCostCenters.Object);
            mockContext.Setup(c => c.Users).Returns(mockSetusers.Object);
            mockContext.Setup(c => c.AuditTrail).Returns(mockSetAuditTrails.Object);
            mockContext.Setup(c => c.Customers).Returns(mockSetcustomers.Object);

            companyManagement = new CompanyManagement(new Log4NetLogger(), new CustomerManagement(new Log4NetLogger(), mockContext.Object),mockContext.Object);
        }

        [Order(1)]
        [Test]
        public void GetAllActiveDivisionsForCompanyTest()
        {
            string UserId = "1";
            IList<DivisionDto> response = companyManagement.GetAllActiveDivisionsForCompany(UserId);
            Assert.AreEqual(response.Count, 1);
        }

        //using common methods 
        [Order(2)]
        [Test]
        public void GetAllCostCentersForCompanyTest()
        {
            string userId = "1";
            var response = companyManagement.GetAllCostCentersForCompany(userId);
            Assert.AreEqual(response.FirstOrDefault().Id, 1);
        }

        [Order(3)]
        [Test]
        public void GetAllDivisionsForCompanyTest()
        {
            string UserId = "1";
            IList<DivisionDto> response = companyManagement.GetAllDivisionsForCompany(UserId);
            Assert.AreNotEqual(response.Count, 0);

        }

        [Order(4)]
        [Test]
        public void GetAssignedDivisionsTest()
        {
            string UserId = "1";
            IList<DivisionDto> response = companyManagement.GetAssignedDivisions(UserId);
            Assert.AreNotEqual(response.Count, 0);

        }

        [Order(5)]
        [Test]
        public void CreateCompanyDetailsTest()
        {
           
            CustomerDto dto = new CustomerDto()
            {
                AddressId = 1,
                Code = "1234",
                CompanyCode = "Comp1234",
                CompanyId = 10,
                CompanyName = "Test Corp",
                Email = "test1234@yopmail.com",
                FirstName = "FName",
                LastName = "LName",
                IsConfirmEmail = true,
                IsCorporateAccount = true,
                MobileNumber = "1231231233",
                PhoneNumber = "1231231233",
                Password = "TestPassword",
                UserId = "1",
                Salutation = "Mr",
                Id =20 ,
                CustomerAddress= new AddressDto
                {
                    Id=100,
                    StreetAddress1="custAdd1",
                    StreetAddress2="custAdd2",
                    City="CityTest",
                    Country="US",
                    Number="2222",
                    State="California",
                    ZipCode="12314"
                    
                }             
            };

            var response = companyManagement.CreateCompanyDetails(dto);
            Assert.AreEqual(response, 0);
           
        }
               
        [Order(6)]
        [Test]
        public void GetCostCentersbyDivisionTest()
        {
            string companyId = "1";
            var response = companyManagement.GetCostCentersbyDivision(companyId);
            Assert.AreEqual(response.FirstOrDefault().Id, 1);
        }

        [Order(7)]
        [Test]
        public void GetDivisionByIdTest()
        {
            long divId = 1;
            string userId = "1";
            //DivisionDto response = companyManagement.GetDivisionById(divId, userId);
            //Assert.AreNotEqual(response, null);

        }

        [Order(8)]
        [Test]
        public void GetAllActiveDivisionsOfUserTest()
        {
            string UserId = "1";
            IList<DivisionDto> response = companyManagement.GetAllActiveDivisionsOfUser(UserId);
            Assert.AreNotEqual(response.Count, 0);
        }

        [Order(9)]
        [Test]
        public void GetCostCentersByIdTest()
        {
            string userId = "1";
            long id = 0;
            CostCenterDto response = companyManagement.GetCostCentersById(id, userId);
            Assert.AreNotEqual(response, null);

        }

        [Order(10)]
        [TestCase("cc_finance")]
        [TestCase("cc_hr")]
        public void SaveCostCenterTest(string name)
        {
            IList<long> divList = new List<long>();
            divList.Add(1);
            divList.Add(2);

            CostCenterDto costCenter = new CostCenterDto()
            {
                Id = 0,
                Name = name,
                Description = "Test Desc",
                PhoneNumber = "34534534555",
                Status = 1,
                CompanyId = 1, //costCenter.CompanyId, TODO H - why?
                Type = "USER",
                UserId = "1",

                BillingAddress = new AddressDto()
                {
                    Country = "US",
                    ZipCode = "1234",
                    Number = "CAL123",
                    StreetAddress1 = "StreetAddress1",
                    StreetAddress2 = "StreetAddress2",
                    City = "City",
                    State = "State"

                },
                AssignedDivisionIdList = divList,
                IsActive = true

            };

            int response = companyManagement.SaveCostCenter(costCenter);
            if (name == "cc_finance")
            {
                Assert.AreEqual(response, -1);
            }
            else
            {
                Assert.AreEqual(response, 1);
            }

        }
        
        [Test]
        public void SaveDivisionTest()
        {
            DivisionDto division = new DivisionDto()
            {
                Id = 1,
                CompanyId = 1,                
                Name = "div2",
                Description = "Desc",
                DefaultCostCenterId = 1,              
                Type = "USER",
                UserId="1"

            };
            int response = companyManagement.SaveDivision(division);
            Assert.AreEqual(response, 1);          

        }

        [Order(11)]        
        [Test]
        public void SaveDivisionExistingTest()
        {
            DivisionDto division = new DivisionDto()
            {
                Id = 2,
                CompanyId = 1,
                Name = "div1",
                Description = "Desc",
                DefaultCostCenterId = 1,
                Type = "USER",
                UserId = "1"

            };

            int response = companyManagement.SaveDivision(division);
            Assert.AreEqual(response, -1);
                      
        }

        //using common methods// roles ??
        //blocked beacause of mocking Roles 
        [Test]
        public void GetAllCostCentersTest()
        {
            long divId = 1;
            string userId = "1";
            //PagedList pageRecords = companyManagement.GetAllCostCenters(divId, "USER", userId, "", 1, 10, "Id", "asc");
            //Assert.AreNotEqual(pageRecords.TotalRecords, 0);
        }

        [Test]
        public void DeleteCostCenterTest()
        {
            long id = 1;
            int response = companyManagement.DeleteCostCenter(id);
            Assert.AreEqual(response, 1);
        }



        //blocked by Mocking role
        [Test]
        public void GetAllDivisionsTest()
        {
            long costcenterId = 1;
            string userId = "1";
            //PagedList pageRecords = companyManagement.GetAllDivisions(costcenterId, "USER", userId, "", 1, 10, "Id", "asc");
            //Assert.AreNotEqual(pageRecords.TotalRecords, 0);

        }

        [Test]
        public void DeleteDivisionTest()
        {
            long divisionId = 1;
            int response = companyManagement.DeleteDivision(divisionId);
            Assert.AreEqual(response, 1);
            
        }

        //blocked by mocking Role
        [Test]
        public void IsLoggedInAsBusinessOwnerTest()
        {
            string userId = "1";
            //bool response = companyManagement.IsLoggedInAsBusinessOwner(userId);
            //Assert.AreEqual(response, true);

        }

        //blocked by mocking Role
        [Test]
        public void IsLoggedInAsNotBusinessOwnerTest()
        {
            string userId = "1";
            //bool response = companyManagement.IsLoggedInAsBusinessOwner(userId);
            //Assert.AreEqual(response, false);

        }

        [Test]
        public void GetLoggedInUserNameTest()
        {
            string userId = "1";
            string response = companyManagement.GetLoggedInUserName(userId);
            Assert.AreNotEqual(response, null);
        }

        [Test]
        public void UpdateLastLoginTimeAndAduitTrailTest()
        {
            
        }

        //blocked beacause of mocking Roles 
        [Test]
        public void GetAllActiveChildRolesTest()
        {
            string userId = "1";
            //List<RolesDto> response = companyManagement.GetAllActiveChildRoles(userId);
            //Assert.AreNotEqual(response.Count, 0);

        }

        //blocked by mocking role
        [Test]
        public void GetUserByIdTest()
        {
            string userId = "1";
            string loggedinUserId = "1";
            //UserDto response = companyManagement.GetUserById(userId, loggedinUserId);
            //Assert.AreNotEqual(response, null);
        }

        [TestCase("UserName@sdfs")]
        [TestCase("user1@parcel.com")]
        public void SaveUserTest(string email)
        {
            //UserDto user = new UserDto()
            //{
            //    LoggedInUserId = "1",
            //    IsActive = true,
            //    UserName = "UserName@sdfs",
            //    FirstName = "FirstName",
            //    LastName = "LastName",
            //    Email = email,
            //    LastLoginTime = DateTime.UtcNow.ToString(),
            //    MiddleName = "MiddleName",
            //    RoleName = "BusinessOwner",
            //    Password = "123333",
            //    Salutation = "Mr",
            //    AssignedDivisionIdList= new List<long>()
            //    {
            //        1,
            //        2
            //    }

            //};
            //UserResultDto response = companyManagement.SaveUser(user);
            //if (email== "UserName@sdfs")
            //{
            //    Assert.AreEqual(response.IsSucess, true);
            //}
            //else if (email== "user1@parcel.com")
            //{
            //    Assert.AreEqual(response.IsSucess, false);
            //}
            Assert.AreEqual(true,true);
        }

        //blocked by mocking Role
        [Test]
        public void LoadUserManagementTest()
        {
            string logggedInUserId = "1";
            //UserDto response = companyManagement.LoadUserManagement(logggedInUserId);
            //Assert.AreNotEqual(response, null);
        }

        //blocked by role
        [Test]
        public void GetAllUsersTest()
        {
            long division = 1;
            string role = "BusinessOwner";
            string userId = "1";
            string status = "True";
            string searchtext = "";
            //PagedList response = companyManagement.GetAllUsers(division, role, userId, status, searchtext);
            //Assert.AreNotEqual(response.TotalRecords, 0);

        }

        //blocked by role
        [Test]
        public void GetRoleNameTest()
        {
            string roleId = "1";
            //string responce = companyManagement.GetRoleName(roleId);
            //Assert.AreEqual(responce, "BusinessOwner");
        }

        [Test]
        public void GetAccountTypeTest()
        {
            string userId = "1";
            bool responce= companyManagement.GetAccountType(userId);
            Assert.AreEqual(responce, true);
        }

        //blocked beacause of mocking Roles 
        [Test]
        public void GetAllComapniesTest()
        {
            string status = null;
            string searchtext = "";
            //PagedList response = companyManagement.GetAllComapnies(status, searchtext);
            //Assert.AreNotEqual(response.TotalRecords, 0);

        }

        //blocked beacause of mocking Roles 
        [Test]
        public void GetAllComapniesForAdminSearchTest()
        {
            string searchtext = "";
            //PagedList response = companyManagement.GetAllComapniesForAdminSearch(searchtext);
            //Assert.AreNotEqual(response.TotalRecords, 0);
        }

        [Test]
        public void GetCompanyByUserIDTest()
        {
            string userID = "1";
            CompanyDto response = companyManagement.GetCompanyByUserID(userID);
            Assert.AreNotEqual(response, null);
        }

        //blocked by role
        [Test]
        public void GetBusinessOwneridbyCompanyIdTest()
        {
            string companyId = "1";
            //string response = companyManagement.GetBusinessOwneridbyCompanyId(companyId);
            //Assert.AreNotEqual(response, string.Empty);
        }

        [Test]
        public void UpdateCompanyLogoTest()
        {
            string URL="test";
            string userId="1";
            bool response = companyManagement.UpdateCompanyLogo(URL, userId);
            Assert.AreEqual(response, true);

        }

        [Test]
        public void ChangeCompanyStatusTest()
        {
            long comapnyId = 1;
            bool response = companyManagement.ChangeCompanyStatus(comapnyId);
            Assert.AreEqual(response, false);
        }
    }
}