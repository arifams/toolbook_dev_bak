using NUnit.Framework;
using PI.Business;
using PI.Contract.DTOs;
using PI.Contract.DTOs.AuditTrail;
using PI.Contract.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Business.Tests
{
    [TestFixture]
    public class AdministrationManagmentTests
    {
        AdministrationManagment admin = null;

        public AdministrationManagmentTests()
        {
            admin = new AdministrationManagment();
        }

        [Test]
        public void ImportRateSheetExcelTest()
        {
            string URI = "";
            OperationResult response = admin.ImportRateSheetExcel(URI);
            Assert.AreEqual(response.Status, Status.Success);
        }

        [Test]
        public void ManageInvoicePaymentSettingTest()
        {
            long comapnyId = 1;
            bool response = admin.ManageInvoicePaymentSetting(comapnyId);
            Assert.AreEqual(response, true);
        }

        [Test]
        public void GetAuditTrailsForCustomerTest()
        {
            string userId = "";
            List<AuditTrailDto> response = admin.GetAuditTrailsForCustomer(userId);
            Assert.AreNotEqual(response.Count, 0);
        }
    }
}