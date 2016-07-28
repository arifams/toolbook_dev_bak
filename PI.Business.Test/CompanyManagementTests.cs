using Microsoft.VisualStudio.TestTools.UnitTesting;
using PI.Business;
using PI.Contract.DTOs.Address;
using PI.Contract.DTOs.Common;
using PI.Contract.DTOs.Company;
using PI.Contract.DTOs.CostCenter;
using PI.Contract.DTOs.Customer;
using PI.Contract.DTOs.Division;
using PI.Contract.DTOs.Role;
using PI.Contract.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Business.Tests
{
    [TestClass()]
    public class CompanyManagementTests
    {
        CompanyManagement company = null;

        [TestInitialize]
        public void Initialize()
        {
            company = new CompanyManagement();

        }

        [TestMethod()]
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

            var response = company.CreateCompanyDetails(dto);
           
        }

        [TestMethod()]
        public void GetAllCostCentersForCompanyTest()
        {
            string userId = "cdf30573-1fba-412e-972f-ec867b02d07e";
            var response = company.GetAllCostCentersForCompany(userId);
            Assert.AreNotEqual(response.Count, 0);
        }

        [TestMethod()]
        public void GetCostCentersbyDivisionTest()
        {
            string companyId = "10";
            var response = company.GetCostCentersbyDivision(companyId);
            Assert.AreNotEqual(response.Count, 0);
        }

        [TestMethod()]
        public void GetAllCostCentersTest()
        {
            long divId = 0;
            string userId = "";          
            PagedList pageRecords = company.GetAllCostCenters(divId, "Active", userId,"", 1, 10, "Id", "asc");
            Assert.AreNotEqual(pageRecords.TotalRecords, 0);
        }

        [TestMethod()]
        public void GetCostCentersByIdTest()
        {
            string userId = "";
            long id = 0;
            CostCenterDto response = company.GetCostCentersById(id, userId);
            Assert.AreNotEqual(response, null);

        }

        [TestMethod()]
        public void SaveCostCenterTest()
        {
            IList<long> divList = new List<long>();
            divList.Add(1);
            divList.Add(2);

            CostCenterDto costCenter = new CostCenterDto()
            {
                Id=0,
                Name = "TestCost Center",
                Description = "Test Desc",
                PhoneNumber = "34534534555",
                Status = 1,
                CompanyId =1, //costCenter.CompanyId, TODO H - why?
                Type = "USER",            
                    
                BillingAddress = new AddressDto()
                {
                    Country = "US",
                    ZipCode = "1234",
                    Number = "CAL123",
                    StreetAddress1 = "StreetAddress1",
                    StreetAddress2 ="StreetAddress2",
                    City ="City",
                    State = "State"
                  
                },
                AssignedDivisionIdList= divList,
                IsActive =true

            };

            int response = company.SaveCostCenter(costCenter);
            Assert.AreEqual(response, 1);
        }

        [TestMethod()]
        public void DeleteCostCenterTest()
        {
            long id = 10;
            int response = company.DeleteCostCenter(id);
            Assert.AreEqual(response, 1);
        }

        [TestMethod()]
        public void GetAllActiveDivisionsForCompanyTest()
        {
            string UserId = "";
            IList<DivisionDto> response = company.GetAllActiveDivisionsForCompany(UserId);
            Assert.AreNotEqual(response.Count, 0);
        }

        [TestMethod()]
        public void GetAllActiveDivisionsOfUserTest()
        {
            string UserId = "";
            IList<DivisionDto> response = company.GetAllActiveDivisionsOfUser(UserId);
            Assert.AreNotEqual(response.Count, 0);
        }

        [TestMethod()]
        public void GetAllDivisionsForCompanyTest()
        {
            string UserId = "";
            IList<DivisionDto> response = company.GetAllDivisionsForCompany(UserId);
            Assert.AreNotEqual(response.Count, 0);

        }

        [TestMethod()]
        public void GetAssignedDivisionsTest()
        {
            string UserId = "";
            IList<DivisionDto> response = company.GetAssignedDivisions(UserId);
            Assert.AreNotEqual(response.Count, 0);

        }

        [TestMethod()]
        public void GetAllDivisionsTest()
        {
            long costcenterId = 0;
            string userId = "";
            PagedList pageRecords = company.GetAllDivisions(costcenterId, "Active", userId, "", 1, 10, "Id", "asc");
            Assert.AreNotEqual(pageRecords.TotalRecords, 0);

        }

        [TestMethod()]
        public void GetDivisionByIdTest()
        {
            long divId = 1;
            string userId = "";
            DivisionDto response = company.GetDivisionById(divId, userId);
            Assert.AreNotEqual(response, null);

        }

        [TestMethod()]
        public void SaveDivisionTest()
        {
            DivisionDto division = new DivisionDto()
            {
                Id=1,
                CompanyId=1,
                DefaultCostCenterId=1,
                Description="Desc",
                Name="Division1",
                NumberOfUsers=1,
                Status=1,
                Type="USER",
                UserId= "cdf30573-1fba-412e-972f-ec867b02d07e"

            };

            int response = company.SaveDivision(division);
            Assert.AreEqual(response, 1);

        }

        [TestMethod()]
        public void DeleteDivisionTest()
        {
            long divisionId = 1;
            int response = company.DeleteDivision(divisionId);
            Assert.AreEqual(response, 1);
            
        }

        [TestMethod()]
        public void IsLoggedInAsBusinessOwnerTest()
        {
            string userId = "";
            bool response = company.IsLoggedInAsBusinessOwner(userId);
            Assert.AreEqual(response, true);
            
        }

        [TestMethod()]
        public void IsLoggedInAsNotBusinessOwnerTest()
        {
            string userId = "";
            bool response = company.IsLoggedInAsBusinessOwner(userId);
            Assert.AreEqual(response, false);

        }

        [TestMethod()]
        public void GetLoggedInUserNameTest()
        {
            string userId = "";
            string response = company.GetLoggedInUserName(userId);
            Assert.AreEqual(response, null);
        }

        [TestMethod()]
        public void UpdateLastLoginTimeAndAduitTrailTest()
        {
            
        }

        [TestMethod()]
        public void GetAllActiveChildRolesTest()
        {
            string userId = "";
            List<RolesDto> response = company.GetAllActiveChildRoles(userId);
            Assert.AreNotEqual(response.Count, 0);

        }

        [TestMethod()]
        public void GetUserByIdTest()
        {
            string userId = "";
            string loggedinUserId = "";
            UserDto response = company.GetUserById(userId, loggedinUserId);
            Assert.AreNotEqual(response, null);
        }

        [TestMethod()]
        public void SaveUserTest()
        {
            UserDto user = new UserDto()
            {
                LoggedInUserId = "",
                IsActive = true,
                UserName = "",
                FirstName = "",
                LastName = "",
                Email = "",
                LastLoginTime = DateTime.Now.ToString(),
                MiddleName = "",
                RoleName = "BusinessOwner",
                Password = "",
                Salutation = "Mr"

            };
            UserResultDto response = company.SaveUser(user);
            Assert.AreNotEqual(response, null);
        }

        [TestMethod()]
        public void LoadUserManagementTest()
        {
            string logggedInUserId = "";
            UserDto response = company.LoadUserManagement(logggedInUserId);
            Assert.AreNotEqual(response, null);
        }

        [TestMethod()]
        public void GetAllUsersTest()
        {
            long division=1;
            string role = "BusinessOwner";
            string userId = "";
            string status = "1";
            string searchtext = "";
            PagedList response = company.GetAllUsers(division,role,userId,status,searchtext);
            Assert.AreNotEqual(response.TotalRecords, 0);

        }

        [TestMethod()]
        public void GetRoleNameTest()
        {
            string roleId = "af354b00-317f-4f45-8b10-3671de73d918";
            string responce = company.GetRoleName(roleId);
            Assert.AreEqual(responce, "BusinessOwner");
        }

        [TestMethod()]
        public void GetAccountTypeTest()
        {
            string userId = "af354b00-317f-4f45-8b10-3671de73d918";
            bool responce=company.GetAccountType(userId);
            Assert.AreEqual(responce, true);
        }

        [TestMethod()]
        public void GetAllComapniesTest()
        {
            string status="";
            string searchtext = "";
            PagedList response = company.GetAllComapnies(status, searchtext);
            Assert.AreNotEqual(response.TotalRecords, 0);
            
        }

        [TestMethod()]
        public void GetAllComapniesForAdminSearchTest()
        {
            string searchtext = "";
            PagedList response = company.GetAllComapniesForAdminSearch(searchtext);
            Assert.AreNotEqual(response.TotalRecords, 0);
        }

        [TestMethod()]
        public void ChangeCompanyStatusTest()
        {
            long comapnyId = 0;
            bool response = company.ChangeCompanyStatus(comapnyId);
            Assert.AreEqual(response, true);
        }

        [TestMethod()]
        public void GetCompanyByUserIDTest()
        {
            string userID = "";
            CompanyDto response = company.GetCompanyByUserID(userID);
            Assert.AreNotEqual(response, null);
        }

        [TestMethod()]
        public void GetBusinessOwneridbyCompanyIdTest()
        {
            string companyId = "";
            string response = company.GetBusinessOwneridbyCompanyId(companyId);
            Assert.AreNotEqual(response, string.Empty);
        }

        [TestMethod()]
        public void UpdateCompanyLogoTest()
        {
            string URL="";
            string userId="";
            bool response = company.UpdateCompanyLogo(URL, userId);
            Assert.AreEqual(response, true);

        }
    }
}