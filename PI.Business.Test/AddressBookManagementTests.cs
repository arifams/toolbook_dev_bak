using NUnit.Framework;
using PI.Business;
using PI.Contract.DTOs.Common;
using PI.Contract.DTOs.AddressBook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PI.Data.Entity;
using Moq;
using System.Data.Entity;
using PI.Data;
using PI.Business.Test;
using PI.Contract.DTOs.ImportAddress;

namespace PI.Business.Tests
{
    [TestFixture]
    public class AddressBookManagementTests
    {
        private AddressBookManagement addressBookManagement = null;
        //readonly Mock<PIContext>  mockContext = null;

        [TestFixtureSetUp]
        public void Init()
        {
            List<AddressBook> addresses = new List<AddressBook>
            {
             new AddressBook {
                 Id=0,
                 AccountNumber="1234",
                 City="City1",
                 CompanyName="Company Name",
                 Country="US",
                 EmailAddress="test@mail.com",
                 FirstName="fname",
                 LastName="lname",
                 PhoneNumber="1231231233",
                 Number="1234",
                 Salutation="Mr",
                 State="State1",
                 StreetAddress1="address1",
                 StreetAddress2="address2",
                 UserId="07264f19-3362-4e26-ba6d-e6ffd244e822",
                 ZipCode="12345",
                 IsActive=true,
                 IsDelete=false,
                 CreatedDate=DateTime.Now
                 
             },            
            };

            //////
            //var mockSet = new Mock<DbSet<AddressBook>>();
            //Mock<PIContext> mockContext  = new Mock<PIContext>();

            //mockContext.Setup(m => m.AddressBooks).Returns(mockSet.Object);
            ////////

            var mockSet = MoqHelper.CreateMockForDbSet<AddressBook>()
                                                .SetupForQueryOn(addresses)
                                                .WithAdd(addresses);

            var mockContext = MoqHelper.CreateMockForDbContext<PIContext, AddressBook>(mockSet);

            // Prevent context null
            mockContext.Setup(c => c.AddressBooks).Returns(mockSet.Object);

            addressBookManagement = new AddressBookManagement(mockContext.Object);    
        }
        

        [Test]
        public void SaveAddressDetailTest()
        {
            AddressBookDto dto = new AddressBookDto();
            dto.CompanyName = "comp1234567890";
            dto.UserId = "1";
            dto.Salutation = "Mr";
            dto.FirstName = "FName";
            dto.LastName = "LName";
            dto.EmailAddress = "fl@yopmail.com";
            dto.PhoneNumber = "1234567890";
            dto.AccountNumber = "acc";

            //Address Status                    
            dto.Country = "LK";
            dto.ZipCode = "1234";
            dto.Number = "123";
            dto.StreetAddress1 = "Ad1";
            dto.StreetAddress2 = "Ad2";
            dto.City = "City";
            dto.State = "State";
            dto.IsActive = true;

            int response = addressBookManagement.SaveAddressDetail(dto);
            Assert.AreEqual(response, 1);
        }

        [Test]
        public void GetAllAddressesTest()
        {           
            string userId= "07264f19-3362-4e26-ba6d-e6ffd244e822";
            string searchtext = "";

            PagedList pageRecord = addressBookManagement.GetAllAddresses(null, userId, searchtext);                    
            Assert.AreNotEqual(pageRecord.TotalRecords,1);
        }

        [Test]
        public void GetAddressBookByIdTest()
        {
            AddressBook response = addressBookManagement.GetAddressBookById(0);            
            Assert.AreEqual(response.Id, 0);
        }

        [Test]
        public void GetFilteredAddressesTest()
        {
            string userId = "07264f19-3362-4e26-ba6d-e6ffd244e822";
            string searchtext = "";

            PagedList pagedRecord = addressBookManagement.GetFilteredAddresses(userId, searchtext);          
            Assert.AreEqual(pagedRecord.TotalRecords, 0);
        }

        [Test]
        public void GetAddressBookDtoByIdTest()
        {
            AddressBookDto response = addressBookManagement.GetAddressBookDtoById(0);                       
            Assert.AreEqual(response.Id, 0);
        }

        [Test]
        public void GetAddressBookDetailsByUserIdTest()
        {
            string type=null;
            string userId = "07264f19-3362-4e26-ba6d-e6ffd244e822";
            string searchtext = "";
            byte[] response = addressBookManagement.GetAddressBookDetailsByUserId(type,userId ,searchtext, 1, 1);                       
            Assert.AreNotEqual(response, null);
        }

        [Test]
        public void DeleteAddressTest()
        {
            int response = addressBookManagement.DeleteAddress(0);            
            Assert.AreEqual(response, 1);
        }

        [Test]
        public void ImportAddressBookTest()
        {
            string userId = "07264f19-3362-4e26-ba6d-e6ffd244e822";
            int response = addressBookManagement.ImportAddressBook(new List<ImportAddressDto>()
            {
                new ImportAddressDto()
                {
                    CsvContent = ""
                }
            }, userId);
            Assert.AreEqual(response, 0);
        }

        [Test]
        public void UpdateAddressBookDatafromExcel()
        {
            string userId = "07264f19-3362-4e26-ba6d-e6ffd244e822";
            bool response = addressBookManagement.UpdateAddressBookDatafromExcel("", userId);
            Assert.AreEqual(response, false);
        }
    }
}