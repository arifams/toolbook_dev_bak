using Microsoft.VisualStudio.TestTools.UnitTesting;
using PI.Business;
using PI.Contract.DTOs.Address;
using PI.Contract.DTOs.Customer;
using PI.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Business.Tests
{
    [TestClass()]
    public class CustomerManagementTests
    {
        CustomerManagement customer = null;

        [TestInitialize]
        public void Initialize()
        {
            customer = new CustomerManagement();

        }

        [TestMethod()]
        public void SaveCustomerTest()
        {
            CustomerDto customerDetail = new CustomerDto()
            {
                FirstName = "FirstName",
                MiddleName = "MiddleName",
                LastName = "LastName",
                Salutation = "Mr",
                Email = "test@t.com",
                PhoneNumber = "1312312312",
                MobileNumber = "1231231233",               
                UserName = "test@t.com",
                Password = "pass",
                UserId = "",
                //CompanyId = customer.CompanyId,
                AddressId = 0,
                CustomerAddress = new AddressDto()
                {
                    Country = "US",
                    ZipCode = "234",
                    Number = "1234",
                    StreetAddress1 = "StreetAddress1",
                    StreetAddress2 = "StreetAddress2",
                    City = "City",
                    State = "State"              
                }
            };
            int response = customer.SaveCustomer(customerDetail, false);
            Assert.AreEqual(response, 1);
        }

        [TestMethod()]
        public void GetCustomerByIdTest()
        {
            long custId = 1;
            Customer response = customer.GetCustomerById(custId);
            Assert.AreNotEqual(response, null);
        }
        

        [TestMethod()]
        public void VerifyUserLoginTest()
        {
            CustomerDto customerDetail = new CustomerDto()
            {
                FirstName = "FirstName",
                MiddleName = "MiddleName",
                LastName = "LastName",
                Salutation = "Mr",
                Email = "test@t.com",
                PhoneNumber = "1312312312",
                MobileNumber = "1231231233",
                UserName = "test@t.com",
                Password = "pass",
                UserId = "",
                //CompanyId = customer.CompanyId,
                AddressId = 0,
                CustomerAddress = new AddressDto()
                {
                    Country = "US",
                    ZipCode = "234",
                    Number = "1234",
                    StreetAddress1 = "StreetAddress1",
                    StreetAddress2 = "StreetAddress2",
                    City = "City",
                    State = "State"
                }
            };
            int response = customer.VerifyUserLogin(customerDetail);
            Assert.AreEqual(response, 1);
        }

        [TestMethod()]
        public void GetJwtTokenTest()
        {
            string userid = "";
            string role = "";
            string tenantId = "";
            string userName = "";
            string companyId = "";

            string response=customer.GetJwtToken(userid, role, tenantId, userName, companyId);
            Assert.AreNotEqual(response, string.Empty);
        }

        [TestMethod()]
        public void GetBytesTest()
        {
            string input = "";
            byte[] response = customer.GetBytes(input);
            Assert.AreNotEqual(response, null);
        }

        [TestMethod()]
        public void GetThemeColourTest()
        {
            string loggedInUserId = "";
            string response = customer.GetThemeColour(loggedInUserId);
            Assert.AreNotEqual(response, null);
        }

        [TestMethod()]
        public void GetCustomerByCompanyIdTest()
        {
            int companyId = 1;
            CustomerDto response = customer.GetCustomerByCompanyId(companyId);
            Assert.AreNotEqual(response, null);
        }
    }
}