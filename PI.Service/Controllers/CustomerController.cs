using PI.Business;
using PI.Contract.Business;
using PI.Contract.DTOs.Invoice;
using PI.Contract.Enums;
using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Cors;
using Microsoft.AspNet.Identity;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using PI.Contract.DTOs.FileUpload;
using System.Threading.Tasks;

namespace PI.Service.Controllers
{
    [CustomAuthorize]
    [RoutePrefix("api/Customer")]
    public class CustomerController : BaseApiController
    {
        readonly IInvoiceMangement invoiceMangement;
        readonly ICustomerManagement customerManagement;

        public CustomerController(IInvoiceMangement invoiceMangement, ICustomerManagement customerManagement)
        {
            this.invoiceMangement = invoiceMangement;
            this.customerManagement = customerManagement;
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        [Route("GetAllInvoicesByCustomer")]
        public IHttpActionResult GetAllInvoicesByCustomer(string userId, string status = null, DateTime? startDate = null,
                                                          DateTime? endDate = null, string searchValue = null)
        {
            return Ok(invoiceMangement.GetAllInvoicesByCustomer(status, userId, startDate, endDate, searchValue));
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("PayInvoice")]
        public IHttpActionResult PayInvoice([FromBody] InvoiceDto invoice)
        {
            return Ok(invoiceMangement.PayInvoice(invoice));
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("DisputeInvoice")]
        public IHttpActionResult DisputeInvoice([FromBody] InvoiceDto invoice)
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

            return Ok(result);
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        [Route("ExportInvoiceReport")]
        public HttpResponseMessage ExportInvoiceReport(string userId, string status = null, DateTime? startDate = null,
                                                          DateTime? endDate = null, string searchValue = null)
        {
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new ByteArrayContent(invoiceMangement.GetAllInvoicesByAdminForExport(status, userId, startDate, endDate, searchValue));
            result.Content.Headers.Add("x-filename", "MyInvoiceReport.xlsx");
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            return result;          
        }
        

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        [Route("GetThemeColour")]
        public IHttpActionResult GetThemeColour ([FromUri] string loggedInUserId)
        {
            return Ok(customerManagement.GetThemeColour(loggedInUserId));
        }

        
    }
}