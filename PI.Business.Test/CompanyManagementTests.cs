using Microsoft.VisualStudio.TestTools.UnitTesting;
using PI.Business;
using PI.Contract.DTOs.Address;
using PI.Contract.DTOs.Customer;
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
            Assert.AreNotEqual(response, 0);
        }

        [TestMethod()]
        public void GetAllCostCentersForCompanyTest()
        {
            //string userId=""
            //var response = company.GetAllCostCentersForCompany();
            //Assert.AreNotEqual(response, null);
        }

        [TestMethod()]
        public void GetCostCentersbyDivisionTest()
        {
            
        }

        [TestMethod()]
        public void GetAllCostCentersTest()
        {
            
        }

        [TestMethod()]
        public void GetCostCentersByIdTest()
        {
            
        }

        [TestMethod()]
        public void SaveCostCenterTest()
        {
            
        }

        [TestMethod()]
        public void DeleteCostCenterTest()
        {
            
        }

        [TestMethod()]
        public void GetAllActiveDivisionsForCompanyTest()
        {
            
        }

        [TestMethod()]
        public void GetAllActiveDivisionsOfUserTest()
        {
            
        }

        [TestMethod()]
        public void GetAllDivisionsForCompanyTest()
        {
            
        }

        [TestMethod()]
        public void GetAssignedDivisionsTest()
        {
            
        }

        [TestMethod()]
        public void GetAllDivisionsTest()
        {
            
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