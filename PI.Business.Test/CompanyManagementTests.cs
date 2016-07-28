using Microsoft.VisualStudio.TestTools.UnitTesting;
using PI.Business;
using PI.Contract.DTOs.Address;
using PI.Contract.DTOs.Common;
using PI.Contract.DTOs.CostCenter;
using PI.Contract.DTOs.Customer;
using PI.Contract.DTOs.Division;
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
           // Assert.AreNotEqual(response.Count, 0);
        }

        [TestMethod()]
        public void GetCostCentersbyDivisionTest()
        {
            string companyId = "10";
            var response = company.GetCostCentersbyDivision(companyId);
           // Assert.AreNotEqual(response.Count, 0);
        }

        [TestMethod()]
        public void GetAllCostCentersTest()
        {
            long divId = 0;
            string userId = "";          
            PagedList pageRecords = company.GetAllCostCenters(divId, "Active", userId,"", 1, 10, "Id", "asc");
            //Assert.AreNotEqual(pageRecords.TotalRecords, 0);
        }

        [TestMethod()]
        public void GetCostCentersByIdTest()
        {
            string userId = "";
            long id = 0;
            CostCenterDto response = company.GetCostCentersById(id, userId);
            //Assert.AreNotEqual(response, null);

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
          //  Assert.AreEqual(response, 1);
        }

        [TestMethod()]
        public void DeleteCostCenterTest()
        {
            long id = 10;
            int response = company.DeleteCostCenter(id);
           // Assert.AreEqual(response, 1);
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

        }

        [TestMethod()]
        public void GetDivisionByIdTest()
        {
            
        }

        [TestMethod()]
        public void SaveDivisionTest()
        {
            
        }

        [TestMethod()]
        public void DeleteDivisionTest()
        {
            
        }

        [TestMethod()]
        public void IsLoggedInAsBusinessOwnerTest()
        {
            
        }

        [TestMethod()]
        public void GetLoggedInUserNameTest()
        {
            
        }

        [TestMethod()]
        public void UpdateLastLoginTimeAndAduitTrailTest()
        {
            
        }

        [TestMethod()]
        public void GetAllActiveChildRolesTest()
        {
            
        }

        [TestMethod()]
        public void GetUserByIdTest()
        {
            
        }

        [TestMethod()]
        public void SaveUserTest()
        {
            
        }

        [TestMethod()]
        public void LoadUserManagementTest()
        {
            
        }

        [TestMethod()]
        public void GetAllUsersTest()
        {
            
        }

        [TestMethod()]
        public void GetRoleNameTest()
        {
            
        }

        [TestMethod()]
        public void GetAccountTypeTest()
        {
            
        }

        [TestMethod()]
        public void GetAllComapniesTest()
        {
            
        }

        [TestMethod()]
        public void GetAllComapniesForAdminSearchTest()
        {
            
        }

        [TestMethod()]
        public void ChangeCompanyStatusTest()
        {
            
        }

        [TestMethod()]
        public void GetCompanyByUserIDTest()
        {
            
        }

        [TestMethod()]
        public void GetBusinessOwneridbyCompanyIdTest()
        {
            
        }

        [TestMethod()]
        public void UpdateCompanyLogoTest()
        {
            
        }
    }
}