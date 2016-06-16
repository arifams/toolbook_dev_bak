using PI.Business;
using PI.Contract.Business;
using PI.Contract.DTOs.Common;
using PI.Contract.DTOs.Invoice;
using PI.Contract.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using Microsoft.AspNet.Identity;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using Newtonsoft.Json.Linq;
using PI.Contract.DTOs.Customer;

namespace PI.Service.Controllers
{

    //[CustomAuthorize]
    [RoutePrefix("api/Customer")]
    public class CustomerController : BaseApiController
    {
        IInvoiceMangement invoiceMangement = new InvoiceMangement();
        ICustomerManagement customerManagement = new CustomerManagement();

        //public CustomerController(IInvoiceMangement invoiceMangement)
        //{
        //    this.invoiceMangement = invoiceMangement;
        //}

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpGet]
        [Route("GetAllInvoicesByCustomer")]
        public PagedList GetAllInvoicesByCustomer(string userId, string status = null, DateTime? startDate = null,
                                                  DateTime? endDate = null, string shipmentNumber = null, string invoiceNumber = null)
        {
            var pagedRecord = new PagedList();
            return pagedRecord = invoiceMangement.GetAllInvoicesByCustomer(status, userId, startDate, endDate, shipmentNumber, invoiceNumber);
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpPost]
        [Route("PayInvoice")]
        public InvoiceStatus PayInvoice([FromBody] InvoiceDto invoice)
        {
            return invoiceMangement.PayInvoice(invoice.Id);
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpPost]
        [Route("DisputeInvoice")]
        public InvoiceStatus DisputeInvoice([FromBody] InvoiceDto invoice)
        {
            var result = invoiceMangement.DisputeInvoice(invoice);
            var subject = "Dispute invoice :" + invoice.ShipmentReference + "_" + invoice.InvoiceNumber;

            //sending the dispute infomation email to the admin
            if (result == InvoiceStatus.Disputed)
            {
                string emailTemplate = invoiceMangement.GetDisputeInvoiceEmailTemplate(invoice);

                var adminUser = AppUserManager.FindByEmail("support@parcelinternational.com");

                if (adminUser != null && !string.IsNullOrWhiteSpace(emailTemplate))
                {
                    AppUserManager.SendEmail(adminUser.Id, subject, emailTemplate);
                }

            }
            return result;
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpPost]
        [Route("ExportInvoiceReport")]
        public HttpResponseMessage ExportInvoiceReport([FromBody]List<InvoiceDto> invoiceList)
        {
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new ByteArrayContent(invoiceMangement.ExportInvoiceReport(invoiceList));

            result.Content.Headers.Add("x-filename", "MyInvoiceReport.xlsx");
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            return result;          
        }
        

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpGet]
        [Route("GetThemeColour")]
        public string GetThemeColour ([FromUri] string loggedInUserId)
        {
            return customerManagement.GetThemeColour(loggedInUserId);
        }
        
        
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpGet]
        [Route("GetCustomerByCompanyId")]
        public CustomerDto GetCustomerByCompanyId(int companyid)
        {
            return customerManagement.GetCustomerByCompanyId(companyid);
        }

    }
}