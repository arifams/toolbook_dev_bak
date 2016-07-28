using Microsoft.VisualStudio.TestTools.UnitTesting;
using PI.Business;
using PI.Contract.DTOs.Common;
using PI.Contract.DTOs.Invoice;
using PI.Contract.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Business.Tests
{
    [TestClass()]
    public class InvoiceMangementTests
    {
        InvoiceMangement invoice = null;

        [TestInitialize]
        public void Initialize()
        {
            invoice = new InvoiceMangement();

        }

        [TestMethod()]
        public void GetAllInvoicesByCustomerTest()
        {
            string status = "";
            string userId = "";
            DateTime? startDate = DateTime.Now;
            DateTime? endDate = DateTime.Now;
            string shipmentNumber = "";
            string invoiceNumber = "";

            PagedList response = invoice.GetAllInvoicesByCustomer(status, userId, startDate, endDate,
                                                   shipmentNumber, invoiceNumber);

            Assert.AreNotEqual(response.TotalRecords, 0);

        }

        [TestMethod()]
        public void UpdateInvoiceStatusTest()
        {
            InvoiceDto invoiceDto = new InvoiceDto()
            {
             Id=1,
             InvoiceStatus="1"  
            };
            InvoiceStatus response=invoice.UpdateInvoiceStatus(invoiceDto);
            Assert.AreNotEqual(response, invoiceDto.InvoiceStatus);
        }

        [TestMethod()]
        public void PayInvoiceTest()
        {
            long invoiceId = 1;
            InvoiceStatus response = invoice.PayInvoice(invoiceId);
            Assert.AreEqual(response, InvoiceStatus.Paid);
        }

        [TestMethod()]
        public void DisputeInvoiceTest()
        {
            InvoiceDto invoicedto = new InvoiceDto()
            {
                Id=1,
                DisputeComment="Dispute Test",
                CreatedBy="1"
            };
            InvoiceStatus response = invoice.DisputeInvoice(invoicedto);
            Assert.AreEqual(response, InvoiceStatus.Disputed);
        }

        [TestMethod()]
        public void GetDisputeInvoiceEmailTemplateTest()
        {
            InvoiceDto invoiceDto = new InvoiceDto()
            {
                Id = 1,
                InvoiceStatus = "1",
                ShipmentReference="ref1234",
                InvoiceNumber="12312",
                InvoiceDate=DateTime.Now.ToString(),
                DisputeComment="Dispute comment"
            };

            string response = invoice.GetDisputeInvoiceEmailTemplate(invoiceDto);
            Assert.AreNotEqual(response, null);

        }

        [TestMethod()]
        public void GetAllInvoicesTest()
        {
            string status = "";
            string userId = "";
            DateTime? startDate = DateTime.Now;
            DateTime? endDate = DateTime.Now;
            string shipmentnumber = "";
            string businessowner = "";
            string invoicenumber = "";
            PagedList response = invoice.GetAllInvoices(status, userId, startDate, endDate,
                                       shipmentnumber, businessowner, invoicenumber);

            Assert.AreNotEqual(response.TotalRecords, 0);
        }

        [TestMethod()]
        public void SaveInvoiceDetailsTest()
        {
            InvoiceDto invoiceDto = new InvoiceDto()
            {
                InvoiceNumber = "",
                ShipmentId = 1,
                InvoiceValue = 100,
                CreatedBy = "1",
                InvoiceStatus = InvoiceStatus.Pending.ToString(),               
                URL = "invoice_url"
            };

            bool response = invoice.SaveInvoiceDetails(invoiceDto);
            Assert.AreEqual(response, true);

        }

        [TestMethod()]
        public void SaveCreditNoteDetailsTest()
        {
            InvoiceDto invoiceDto = new InvoiceDto()
            {
                InvoiceNumber="",
                Id=1,
                InvoiceValue=100,
                CreatedBy="",                
                URL ="URL"
            };
            bool response = invoice.SaveCreditNoteDetails(invoiceDto);
            Assert.AreEqual(response, true);
        }

        [TestMethod()]
        public void ExportInvoiceReportTest()
        {
            
        }
    }
}