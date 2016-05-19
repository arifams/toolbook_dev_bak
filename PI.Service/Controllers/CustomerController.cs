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

namespace PI.Service.Controllers
{

    //[CustomAuthorize]
    [RoutePrefix("api/Customer")]
    public class CustomerController : BaseApiController
    {
        IInvoiceMangement invoiceMangement = new InvoiceMangement();

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
            var subject = "Dispute invoice :" + invoice.ShipmentReference.ToString() + "_" + invoice.InvoiceNumber.ToString();

            //sending the dispute infomation email to the admin
            if (result == InvoiceStatus.Disputed)
            {
                string emailTemplate = invoiceMangement.GetDisputeInvoiceEmailTemplate(invoice);
                
                var adminUser = AppUserManager.FindByEmail("sriparcel@outlook.com");

                if (adminUser != null && !string.IsNullOrWhiteSpace(emailTemplate))
                {
                    AppUserManager.SendEmail(adminUser.Id, subject, emailTemplate);
                }

            }
            return result;
        }
    }
}