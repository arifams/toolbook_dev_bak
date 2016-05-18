using PI.Contract.Business;
using PI.Contract.DTOs;
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

namespace PI.Business
{
    public class InvoiceMangement : IInvoiceMangement
    {
        CommonLogic genericMethods = new CommonLogic();


        /// <summary>
        /// Get all invoices by customer
        /// </summary>
        /// <param name="status"></param>
        /// <param name="userId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="refNumber"></param>
        /// <returns></returns>
        public PagedList GetAllInvoicesByCustomer(string status, string userId, DateTime? startDate, DateTime? endDate,
                                                  string shipmentNumber, string invoiceNumber)
        {
            var pagedRecord = new PagedList();
            IList<Invoice> invoiceList = new List<Invoice>();
            pagedRecord.Content = new List<InvoiceDto>();
            InvoiceStatus invoiceStatus = InvoiceStatus.None;

            if (status != null)
            {
                invoiceStatus = (InvoiceStatus)Enum.Parse(typeof(InvoiceStatus), status, true);
            }
            string role = genericMethods.GetUserRoleById(userId);
            Company company = genericMethods.GetCompanyByUserId(userId);

            
            using (var context = new PIContext())
            {
                if (role == "BusinessOwner" || role == "Manager")
                {
                    // Business Owners
                    invoiceList = context.Invoices.Where(x => x.Shipment.Division.CompanyId == company.Id).ToList();
                }
                else if (role == "Supervisor")
                {
                    // Supervises
                    invoiceList = context.Invoices.Where(x => x.Shipment.Division.UserInDivisions.Any(u => u.UserId == userId)).ToList();

                }
                else
                {
                    // Operators
                    invoiceList = context.Invoices.Where(x => x.Shipment.CreatedBy == userId).ToList();
                }

                var content =  invoiceList.Where(i=> i.IsDelete == false &&
                                               (string.IsNullOrEmpty(shipmentNumber) || i.Shipment.ShipmentCode.Contains(shipmentNumber)) &&
                                               (string.IsNullOrEmpty(invoiceNumber) || i.InvoiceNumber.Contains(invoiceNumber)) &&
                                               (string.IsNullOrEmpty(status) || i.InvoiceStatus == invoiceStatus) &&
                                               (startDate == null || i.CreatedDate >= startDate && i.CreatedDate <= endDate)).ToList();
               
                foreach (var item in content)
                {
                    pagedRecord.Content.Add(new InvoiceDto
                    {
                        Id = item.Id,
                        ShipmentReference = item.Shipment.ShipmentCode,
                        InvoiceDate = item.CreatedDate.ToString("dd/MM/yyyy"),
                        InvoiceNumber = item.InvoiceNumber,
                        InvoiceValue = item.InvoiceValue,
                        InvoiceStatus = (short)item.InvoiceStatus,
                        URL = item.URL
                    });
                }

                return pagedRecord;
            }
        }

        /// <summary>
        /// Pay an invoice
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        public InvoiceStatus PayInvoice(long invoiceId)
        {            
            using (var context = new PIContext())
            {
              var invoice = context.Invoices.Where(i => i.Id == invoiceId).SingleOrDefault();
              invoice.InvoiceStatus = InvoiceStatus.Paid;

              context.SaveChanges();

              return invoice.InvoiceStatus;
            }
        }


        /// <summary>
        /// Dispute a particular invoice
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        public InvoiceStatus DisputeInvoice(long invoiceId)
        {
            using (var context = new PIContext())
            {
                var invoice = context.Invoices.Where(i => i.Id == invoiceId).SingleOrDefault();
                invoice.InvoiceStatus = InvoiceStatus.Disputed;

                context.SaveChanges();

                return invoice.InvoiceStatus;
            }
        }
    }
}
