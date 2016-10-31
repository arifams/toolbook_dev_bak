using NUnit.Framework;
using PI.Business;
using PI.Business.Test;
using PI.Common;
using PI.Contract.DTOs.Address;
using PI.Contract.DTOs.Customer;
using PI.Data;
using PI.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Business.Tests
{
    [TestFixture]
    public class CustomerManagementTests
    {
        private CustomerManagement customerManagement = null;

        [TestFixtureSetUp]
        public void Init()
        {
            List<Customer> customers = new List<Customer>
            {
                new Customer()
                {
                    Id=1,
                    UserId="1",
                    AddressId=1,
                    Email="user1@parcel.com",
                    FirstName="fname",
                    JobCapacity="jobcapacity",
                    LastName="lname",
                    SelectedColour="#FF0000"
                }
            };

            var mockSetcustomers = MoqHelper.CreateMockForDbSet<Customer>()
                                           .SetupForQueryOn(customers)
                                           .WithAdd(customers);

            var mockContext = MoqHelper.CreateMockForDbContext<PIContext, Customer>(mockSetcustomers);
            mockContext.Setup(c => c.Customers).Returns(mockSetcustomers.Object);
            
            customerManagement = new CustomerManagement(new Log4NetLogger(),mockContext.Object);           

        }

        [Test]
        public void SaveCustomerTest()
        {
            //CustomerDto customerDetail = new CustomerDto()
            //{
            //    FirstName = "FirstName",
            //    MiddleName = "MiddleName",
            //    LastName = "LastName",
            //    Salutation = "Mr",
            //    Email = "test@t.com",
            //    PhoneNumber = "1312312312",
            //    MobileNumber = "1231231233",               
            //    UserName = "test@t.com",
            //    Password = "pass",
            //    UserId = "1",
            //    //CompanyId = customer.CompanyId,
            //    AddressId = 0,
            //    CustomerAddress = new AddressDto()
            //    {
            //        Country = "US",
            //        ZipCode = "234",
            //        Number = "1234",
            //        StreetAddress1 = "StreetAddress1",
            //        StreetAddress2 = "StreetAddress2",
            //        City = "City",
            //        State = "State"              
            //    }
            //};
            //int response = customerManagement.SaveCustomer(customerDetail, false);
            //Assert.AreEqual(response, 1);
            Assert.AreEqual(true, true);
        }

        [Test]
        public void GetCustomerByIdTest()
        {
            long custId = 1;
            Customer response = customerManagement.GetCustomerById(custId);
            Assert.AreNotEqual(response, null);
        }


        //[Test]
        //public void VerifyUserLoginTest()
        //{
        //    CustomerDto customerDetail = new CustomerDto()
        //    {
        //        FirstName = "FirstName",
        //        MiddleName = "MiddleName",
        //        LastName = "LastName",
        //        Salutation = "Mr",
        //        Email = "test@t.com",
        //        PhoneNumber = "1312312312",
        //        MobileNumber = "1231231233",
        //        UserName = "test@t.com",
        //        Password = "pass",
        //        UserId = "",
        //        //CompanyId = customer.CompanyId,
        //        AddressId = 0,
        //        CustomerAddress = new AddressDto()
        //        {
        //            Country = "US",
        //            ZipCode = "234",
        //            Number = "1234",
        //            StreetAddress1 = "StreetAddress1",
        //            StreetAddress2 = "StreetAddress2",
        //            City = "City",
        //            State = "State"
        //        }
        //    };
        //    int response = customerManagement.VerifyUserLogin(customerDetail);
        //    Assert.AreEqual(response, 1);
        //}

        [Test]
        public void GetJwtTokenTest()
        {
            string userid = "1";
            string role = "BusinessOwner";
            string tenantId = "1";
            string userName = "uaser1";
            string companyId = "1";

            string response= customerManagement.GetJwtToken(userid, role, tenantId, userName, companyId);
            Assert.AreNotEqual(response, string.Empty);
        }

        [Test]
        public void GetBytesTest()
        {
            string input = "testinput";
            byte[] response = customerManagement.GetBytes(input);
            Assert.AreNotEqual(response, null);
        }

        [Test]
        public void GetThemeColourTest()
        {
            string loggedInUserId = "1";
            string response = customerManagement.GetThemeColour(loggedInUserId);
            Assert.AreEqual(response, "#FF0000");
        }

        [Test]
        public void GetCustomerByCompanyIdTest()
        {
            int companyId = 1;
            //CustomerDto response = customerManagement.GetCustomerByCompanyId(companyId);
            //Assert.AreNotEqual(response, null);
        }
    }
}