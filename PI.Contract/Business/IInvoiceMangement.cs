using PI.Contract.DTOs.Common;
using PI.Contract.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.Business
{
    public interface IInvoiceMangement
    {

        /// <summary>
        /// Get all invoices by customer
        /// </summary>
        /// <param name="status"></param>
        /// <param name="userId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="refNumber"></param>
        /// <returns></returns>
        PagedList GetAllInvoicesByCustomer(string status, string userId, DateTime? startDate, DateTime? endDate,
                                           string shipmentNumber, string invoiceNumber);


        /// <summary>
        /// Get all invoices for Admin management
        /// </summary>
        /// <param name="status"></param>
        /// <param name="userId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="shipmentnumber"></param>
        /// <param name="businessowner"></param>
        /// <param name="invoicenumber"></param>
        /// <returns></returns>
        PagedList GetAllInvoices(string status, string userId, DateTime? startDate, DateTime? endDate,
                                       string shipmentnumber, string businessowner, string invoicenumber);


        /// <summary>
        /// Pay an invoice
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        InvoiceStatus PayInvoice(long invoiceId);


        /// <summary>
        /// Dispute a particular invoice
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        InvoiceStatus DisputeInvoice(long invoiceId);

    }
}
