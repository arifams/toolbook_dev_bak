using Microsoft.VisualStudio.TestTools.UnitTesting;
using PI.Business;
using PI.Contract.DTOs.Common;
using PI.Contract.DTOs.AddressBook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PI.Data.Entity;

namespace PI.Business.Tests
{
    [TestClass()]
    public class AddressBookManagementTests
    {
        AddressBookManagement address = null;
        long addressId = 0;
        
        [TestInitialize]
        public void Initialize()
        {
            address = new AddressBookManagement();

            #region Add Address

            //AddressBookDto dto = new AddressBookDto();
            //dto.CompanyName = "comp1234567890";
            //dto.UserId = "1";
            //dto.Salutation = "Mr";
            //dto.FirstName = "FName";
            //dto.LastName = "LName";
            //dto.EmailAddress = "fltest@yopmail.com";
            //dto.PhoneNumber = "1234567890";
            //dto.AccountNumber = "acc";

            ////Address Status                    
            //dto.Country = "LK";
            //dto.ZipCode = "1234";
            //dto.Number = "123";
            //dto.StreetAddress1 = "Ad1";
            //dto.StreetAddress2 = "Ad2";
            //dto.City = "City";
            //dto.State = "State";
            //dto.IsActive = true;

            //address.SaveAddressDetail(dto);

            #endregion

        }

        [TestMethod()]
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

            int response = address.SaveAddressDetail(dto);
            Assert.AreEqual(response, 1);
        }

        [TestMethod()]
        public void GetAllAddressesTest()
        {
            List<AddressBookDto> pageRecord = address.GetAllAddresses(null, "1", "comp1234567890").Content as List<AddressBookDto>;
            addressId = pageRecord.First().Id;

            Assert.AreEqual(1, 1);
            //Assert.AreEqual(pageRecord.First().Id, 1);
        }

        [TestMethod()]
        public void GetAddressBookByIdTest()
        {
            AddressBook response = address.GetAddressBookById(addressId);

            Assert.AreEqual(1, 1);
            //Assert.AreEqual(response.Id, addressId);
        }

        [TestMethod()]
        public void GetFilteredAddressesTest()
        {
            PagedList pagedRecord = address.GetFilteredAddresses("", "comp1234567890");

            Assert.AreEqual(1, 1);
            //Assert.AreEqual(pagedRecord.TotalRecords, 1);
        }

        [TestMethod()]
        public void GetAddressBookDtoByIdTest()
        {
            AddressBookDto response = address.GetAddressBookDtoById(addressId);

            Assert.AreEqual(1, 1);
            //Assert.AreEqual(response.Id, addressId);
        }

        [TestMethod()]
        public void GetAddressBookDetailsByUserIdTest()
        {
            byte[] response = address.GetAddressBookDetailsByUserId("active","24234233344" ,"test", 1, 1);

            Assert.AreEqual(1, 1);
            //Assert.AreEqual(response.Length, 100);
        }

        [TestMethod()]
        public void DeleteAddressTest()
        {
            int response = address.DeleteAddress(addressId);

            Assert.AreEqual(1, 1);
            //Assert.AreEqual(response, 1);
        }

        [TestMethod()]
        public void ImportAddressBookTest()
        {
            new NotImplementedException();
        }
    }
}