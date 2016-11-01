using PI.Contract.DTOs;
using PI.Contract.DTOs.Common;
using PI.Contract.DTOs.Invoice;
using PI.Contract.DTOs.Shipment;
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
        PagedList GetAllInvoicesByCustomer(string status, string userId, DateTime? startDate, DateTime? endDate, string searchValue);


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
        PagedList GetAllInvoicesForAdmin(PagedList pageList);


        /// <summary>
        /// Pay an invoice
        /// </summary>
        /// <param name="invoiceDto"></param>
        /// <returns></returns>
        OperationResult PayInvoice(InvoiceDto invoiceDto);


        /// <summary>
        /// Dispute a particular invoice
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        InvoiceStatus DisputeInvoice(InvoiceDto invoice);


        /// <summary>
        /// Save uploaded invoice details
        /// </summary>
        /// <param name="invoiceDetails"></param>
        /// <returns></returns>
        bool SaveInvoiceDetails(InvoiceDto invoiceDetails);


        /// <summary>
        /// Save uploaded Credit Note details
        /// </summary>
        /// <param name="creditNoteDetails"></param>
        /// <returns></returns>
        bool SaveCreditNoteDetails(InvoiceDto creditNoteDetails);

        /// <summary>
        /// Get disputed invoice confirmation email to admin
        /// </summary>
        /// <param name="invoice
        string GetDisputeInvoiceEmailTemplate(InvoiceDto invoice);

        byte[] ExportInvoiceReport(List<InvoiceDto> invoiceList, bool isAdmin = false);

        /// <summary>
        /// Update invoice status
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        InvoiceStatus UpdateInvoiceStatus(InvoiceDto invoiceDto);


        /// <summary>
        /// Fetch InvoiceDetails fromPdf
        /// </summary>
        /// <param name="pdfUrl"></param>
        /// <returns></returns>
        Task<bool> FetchInvoiceDetailsfromPdf(string pdfUrl);

        /// <summary>
        /// Get All Invoices For Admin Export
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        byte[] GetAllInvoicesForAdminExport(string status, string userId, DateTime? startDate, DateTime? endDate, string searchValue);


        byte[] GetAllInvoicesByAdminForExport(string status, string userId, DateTime? startDate, DateTime? endDate, string searchValue);

    }
}
