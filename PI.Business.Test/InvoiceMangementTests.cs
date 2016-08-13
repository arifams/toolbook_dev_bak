using NUnit.Framework;
using PI.Business;
using PI.Business.Test;
using PI.Common;
using PI.Contract.DTOs.Common;
using PI.Contract.DTOs.Invoice;
using PI.Contract.Enums;
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
    public class InvoiceMangementTests
    {
       private InvoiceMangement invoiceManagement = null;

        [TestFixtureSetUp]
        public void Init()
        {
            List<Invoice> invoices = new List<Invoice>()
            {
                new Invoice()
                {
                    Id=1,
                    InvoiceStatus=InvoiceStatus.Pending,
                    InvoiceNumber="inv123",
                    InvoiceValue=100,                  
                    Shipment= new Shipment
                    {
                        Id=1,
                        CarrierId=1,
                        
                    }
                }
            };

            List<InvoiceDisputeHistory> invoiceDisputeHistories = new List<InvoiceDisputeHistory>()
            {
                 new InvoiceDisputeHistory
                 {
                     Id=1,
                     DisputeComment="disputed",
                     InvoiceId=1,
                     IsActive=true,
                     IsDelete=false,
                     CreatedBy="1",
                     CreatedDate=DateTime.Now,
                     Invoice= new Invoice
                     {
                         Id=1,
                         InvoiceNumber="1",
                         InvoiceValue=100,
                         ShipmentId=1,
                         InvoiceStatus=InvoiceStatus.Pending,
                         URL="url",
                         IsActive=true,
                         IsDelete=false,
                         creditNoteList= new List<CreditNote>
                         {
                             new CreditNote
                             {
                                 Id=1,
                                 InvoiceId=1,                                 
                             }
                         }
                     }
                 }
            };
            List<CreditNote> creditNotes = new List<CreditNote>()
            {
                new CreditNote
                {
                    Id=1,
                    InvoiceId=1,
                    IsActive=true,
                    CreditNoteNumber="10",
                    URL="URl",
                    CreditNoteValue=100,
                    IsDelete=false,
                    CreatedBy="1"

                }
            };

            var mockSetinvoices = MoqHelper.CreateMockForDbSet<Invoice>()
                                           .SetupForQueryOn(invoices)
                                           .WithAdd(invoices);

            var mockSetinvoiceDisputeHistories = MoqHelper.CreateMockForDbSet<InvoiceDisputeHistory>()
                                           .SetupForQueryOn(invoiceDisputeHistories)
                                           .WithAdd(invoiceDisputeHistories);

            var mockSetinvoiceCreditNotes = MoqHelper.CreateMockForDbSet<CreditNote>()
                                           .SetupForQueryOn(creditNotes)
                                           .WithAdd(creditNotes);

            var mockContext = MoqHelper.CreateMockForDbContext<PIContext, Invoice>(mockSetinvoices);

            mockContext.Setup(c => c.Invoices).Returns(mockSetinvoices.Object);
            mockContext.Setup(c => c.InvoiceDisputeHistories).Returns(mockSetinvoiceDisputeHistories.Object);
            mockContext.Setup(c => c.CreditNotes).Returns(mockSetinvoiceCreditNotes.Object);


            invoiceManagement = new InvoiceMangement(new Log4NetLogger(), mockContext.Object);

        }

        ////blocked by mocking role
        [Test]
        public void GetAllInvoicesByCustomerTest()
        {
            string status = "";
            string userId = "1";
            DateTime? startDate = DateTime.Now;
            DateTime? endDate = DateTime.Now;
            string shipmentNumber = "";
            string invoiceNumber = "";

            //PagedList response = invoiceManagement.GetAllInvoicesByCustomer(status, userId, startDate, endDate,
            //                                       shipmentNumber, invoiceNumber);

            //Assert.AreNotEqual(response.TotalRecords, 0);
        }

        [Test]
        public void UpdateInvoiceStatusTest()
        {
            InvoiceDto invoiceDto = new InvoiceDto()
            {
             Id=1,
             InvoiceStatus= "1"
            };
            InvoiceStatus response= invoiceManagement.UpdateInvoiceStatus(invoiceDto);
            Assert.AreNotEqual(response, invoiceDto.InvoiceStatus);
        }

        [Test]
        public void PayInvoiceTest()
        {
            long invoiceId = 1;
            InvoiceStatus response = invoiceManagement.PayInvoice(invoiceId);
            Assert.AreEqual(response, InvoiceStatus.Paid);
        }

        [Test]
        public void DisputeInvoiceTest()
        {
            InvoiceDto invoicedto = new InvoiceDto()
            {
                Id=1,
                DisputeComment="Dispute Test",
                CreatedBy="1"
            };
            InvoiceStatus response = invoiceManagement.DisputeInvoice(invoicedto);
            Assert.AreEqual(response, InvoiceStatus.Disputed);
        }

        [Test]
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

            string response = invoiceManagement.GetDisputeInvoiceEmailTemplate(invoiceDto);
            Assert.AreNotEqual(response, null);

        }

        //blocked by role
        [Test]
        public void GetAllInvoicesTest()
        {
            string status = "";
            string userId = "";
            DateTime? startDate = DateTime.Now;
            DateTime? endDate = DateTime.Now;
            string shipmentnumber = "";
            string businessowner = "";
            string invoicenumber = "";
            //PagedList response = invoiceManagement.GetAllInvoices(status, userId, startDate, endDate,
            //                           shipmentnumber, businessowner, invoicenumber);

            //Assert.AreNotEqual(response.TotalRecords, 0);
        }

        [Test]
        public void SaveInvoiceDetailsTest()
        {
            InvoiceDto invoiceDto = new InvoiceDto()
            {
                InvoiceNumber = "1",
                ShipmentId = 1,
                InvoiceValue = 100,
                CreatedBy = "1",
                InvoiceStatus = InvoiceStatus.Pending.ToString(),               
                URL = "invoice_url"
            };

            bool response = invoiceManagement.SaveInvoiceDetails(invoiceDto);
            Assert.AreEqual(response, true);

        }

        [Test]
        public void SaveCreditNoteDetailsTest()
        {
            InvoiceDto invoiceDto = new InvoiceDto()
            {
                InvoiceNumber="1",
                Id=1,
                InvoiceValue=100,
                CreatedBy="",                
                URL ="URL"
            };
            bool response = invoiceManagement.SaveCreditNoteDetails(invoiceDto);
            Assert.AreEqual(response, true);
        }

       
        [Test]
        public void ExportInvoiceReportTest()
        {
            List<InvoiceDto> invoiceList = new List<InvoiceDto>()
            {

                new InvoiceDto
                {
                    BusinessOwner="owner",
                    CompanyName="companyName",
                    CreatedBy="1",
                    CreditNoteURL="URL",
                    DisputeComment="coment",
                    Id=1,
                    InvoiceDate=DateTime.Now.ToString(),
                    InvoiceNumber="1",
                    InvoiceStatus=InvoiceStatus.Pending.ToString(),
                    InvoiceValue=1000,
                    ShipmentId=1,
                    ShipmentReference="ship123",
                    URL="URL"
                    
                }
            };
            bool isAdmin = true;
            byte[] response = invoiceManagement.ExportInvoiceReport(invoiceList, isAdmin);
            Assert.AreNotEqual(response, null);
        }
    }
}