using OfficeOpenXml;
using OfficeOpenXml.Style;
using PI.Contract;
using PI.Contract.Business;
using PI.Contract.DTOs;
using PI.Contract.DTOs.Common;
using PI.Contract.DTOs.Invoice;
using PI.Contract.Enums;
using PI.Data;
using PI.Data.Entity;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Business
{
    public class InvoiceMangement : IInvoiceMangement
    {
        CommonLogic genericMethods = new CommonLogic();

        private PIContext context;
        private ILogger logger;

        public InvoiceMangement(ILogger logger, PIContext _context = null)
        {
            context = _context ?? PIContext.Get();
            this.logger = logger;
        }

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

            
            //using (var context = PIContext.Get())
            //{
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
                        InvoiceStatus =item.InvoiceStatus.ToString(),
                        URL = item.URL
                    });
                }

                return pagedRecord;
           // }
        }


        /// <summary>
        /// Update invoice status
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        public InvoiceStatus UpdateInvoiceStatus(InvoiceDto invoiceDto)
        {
            //using (var context = PIContext.Get())
            //{
                var invoice = context.Invoices.Where(i => i.Id == invoiceDto.Id).SingleOrDefault();
                invoice.InvoiceStatus = (InvoiceStatus)Enum.Parse( typeof(InvoiceStatus), invoiceDto.InvoiceStatus,true);

                context.SaveChanges();

                return invoice.InvoiceStatus;
           // }
        }


        /// <summary>
        /// Pay an invoice
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        public InvoiceStatus PayInvoice(long invoiceId)
        {            
            //using (var context = PIContext.Get())
            //{
              var invoice = context.Invoices.Where(i => i.Id == invoiceId).SingleOrDefault();
              invoice.InvoiceStatus = InvoiceStatus.Paid;

              context.SaveChanges();

              return invoice.InvoiceStatus;
           // }
        }


        /// <summary>
        /// Dispute a particular invoice
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        public InvoiceStatus DisputeInvoice(InvoiceDto invoice)
        {
            //using (var context = PIContext.Get())
            //{
                var currentinvoice = context.Invoices.Where(i => i.Id == invoice.Id).SingleOrDefault();
                currentinvoice.InvoiceStatus = InvoiceStatus.Disputed;

                InvoiceDisputeHistory history = new InvoiceDisputeHistory()
                {
                    InvoiceId = invoice.Id,
                    DisputeComment = invoice.DisputeComment,
                    CreatedDate = DateTime.Now,
                    CreatedBy= invoice.CreatedBy

                };

                context.InvoiceDisputeHistories.Add(history);
                context.SaveChanges();

                return currentinvoice.InvoiceStatus;
            //}
        }


        public string GetDisputeInvoiceEmailTemplate(InvoiceDto invoice)
        {
            StringBuilder strTemplate = new StringBuilder();
            string keyValueHtmlTemplate = "<span> <span class='name'>{0}:</span> <span class='value'>{1}</span> </span> <br>";
            // Start the document
            strTemplate.Append("<!DOCTYPE html><html><head><title></title><meta charset='utf-8' /> <style> .name{ width:200px;display:inline-block;font-weight:600;font-size:medium } .value{ font-style:italic; } table { border-collapse: collapse; width: 100%; } th, td { text-align: left; padding: 8px; } tr:nth-child(even){background-color: #f2f2f2} th {background-color: lightblue;color: white;} </style></head><body>");

            // General 
            strTemplate.Append("<h3>Dear Admin</h3>");
            strTemplate.AppendFormat(keyValueHtmlTemplate, "Shipment Reference: ", invoice.ShipmentReference);
            strTemplate.AppendFormat(keyValueHtmlTemplate, "Invoice Number: ", invoice.InvoiceNumber);
            strTemplate.AppendFormat(keyValueHtmlTemplate, "Invoiced Date: ", invoice.InvoiceDate);
            strTemplate.Append("<br>");
            strTemplate.Append("<h3>Customer responce/reason for dispute: </h3>");
            strTemplate.Append("<span class='value'>" + invoice.DisputeComment.ToString() +"</span> </span>");
            strTemplate.Append("<br>");

            strTemplate.Append("</body></html>");
            // End the document

            return strTemplate.ToString();
        }


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
        public PagedList GetAllInvoices(string status, string userId, DateTime? startDate, DateTime? endDate,
                                       string shipmentnumber, string businessowner, string invoicenumber)
        {
            var pagedRecord = new PagedList();
            int page = 1;
            int pageSize = 10;
            pagedRecord.Content = new List<InvoiceDto>();

            //using (PIContext context = PIContext.Get())
            //{            
                string BusinessOwnerId = context.Roles.Where(r => r.Name == "BusinessOwner").Select(r => r.Id).FirstOrDefault();

                var content = (from customer in context.Customers
                                 join comapny in context.Companies on customer.User.TenantId equals comapny.TenantId
                                 join invoice in context.Invoices on comapny.Id equals invoice.Shipment.Division.CompanyId
                                 where customer.User.Roles.Any(r => r.RoleId == BusinessOwnerId) &&
                                 customer.IsDelete == false &&
                                 (string.IsNullOrEmpty(businessowner) || customer.FirstName.Contains(businessowner) || customer.LastName.Contains(businessowner)) &&
                                 (string.IsNullOrEmpty(status) || status == invoice.InvoiceStatus.ToString()) &&
                                 (string.IsNullOrEmpty(invoicenumber) || invoice.InvoiceNumber.Contains(invoicenumber)) &&
                                 (string.IsNullOrEmpty(shipmentnumber) || invoice.Shipment.ShipmentCode.Contains(shipmentnumber)) &&
                                 (startDate == null || (invoice.CreatedDate >= startDate && invoice.CreatedDate <= endDate))
                                 select new
                                 {
                                     Customer = customer,
                                     Company = comapny,
                                     Invoice = invoice
                                 }).ToList();

                //removing unmatched company invoices according to the business owners
                foreach (var item in content)
                {                    
                        pagedRecord.Content.Add(new InvoiceDto
                        {
                            Id = item.Invoice.Id,
                            InvoiceNumber = item.Invoice.InvoiceNumber,
                            InvoiceStatus = item.Invoice.InvoiceStatus.ToString(),
                            InvoiceValue = item.Invoice.InvoiceValue,
                            ShipmentId = item.Invoice.ShipmentId,
                            ShipmentReference=item.Invoice.Shipment.ShipmentCode,                            
                            URL = item.Invoice.URL,
                            BusinessOwner = item.Customer.FirstName+ " " + item.Customer.LastName,
                            CompanyName = item.Company.Name,
                            InvoiceDate = item.Invoice.CreatedDate.ToString("dd/MM/yyyy"),
                            CreditNoteURL = item.Invoice.creditNoteList.Count == 0 ? null :
                                            item.Invoice.creditNoteList.OrderByDescending(x=> x.CreatedDate).FirstOrDefault().URL
                        });                    
                }

          //  }

            pagedRecord.TotalRecords = pagedRecord.Content.Count;
            pagedRecord.CurrentPage = page;
            pagedRecord.PageSize = pageSize;
            pagedRecord.TotalPages = (int)Math.Ceiling((decimal)pagedRecord.TotalRecords / pagedRecord.PageSize);

            return pagedRecord;


        }


        /// <summary>
        /// Save uploaded invoice details
        /// </summary>
        /// <param name="invoiceDetails"></param>
        /// <returns></returns>
        public bool SaveInvoiceDetails(InvoiceDto invoiceDetails)
        {
            bool invoiceSaved = false;
            //using (PIContext context = PIContext.Get())
            //{

                Invoice invoice = new Invoice()
                {
                    InvoiceNumber = invoiceDetails.InvoiceNumber,
                    ShipmentId = invoiceDetails.ShipmentId,
                    InvoiceValue = invoiceDetails.InvoiceValue,
                    CreatedBy = invoiceDetails.CreatedBy.ToString(),
                    InvoiceStatus = (InvoiceStatus)Enum.Parse(typeof(InvoiceStatus), invoiceDetails.InvoiceStatus, true),
                    CreatedDate = DateTime.Now,
                    URL = invoiceDetails.URL
                };

                context.Invoices.Add(invoice);
                context.SaveChanges();
                invoiceSaved = true;

            //}
            return invoiceSaved;

        }


        /// <summary>
        /// Save uploaded Credit Note details
        /// </summary>
        /// <param name="creditNoteDetails"></param>
        /// <returns></returns>
        public bool SaveCreditNoteDetails(InvoiceDto creditNoteDetails)
        {
            //using (PIContext context = PIContext.Get())
            //{
                try
                {
                    CreditNote creditNote = new CreditNote()
                    {
                        CreditNoteNumber = creditNoteDetails.InvoiceNumber,
                        InvoiceId = creditNoteDetails.Id,
                        CreditNoteValue = creditNoteDetails.InvoiceValue,
                        CreatedBy = creditNoteDetails.CreatedBy,
                        CreatedDate = DateTime.Now,
                        URL = creditNoteDetails.URL
                    };

                    context.CreditNotes.Add(creditNote);
                    context.SaveChanges();

                    // Update Invoice status and value.
                    var invoice = context.Invoices.Where(x => x.Id == creditNoteDetails.Id).SingleOrDefault();

                    invoice.InvoiceStatus = (invoice.InvoiceValue == creditNote.CreditNoteValue) ?
                                           InvoiceStatus.Paid : InvoiceStatus.Pending;

                    invoice.InvoiceValue = (invoice.InvoiceValue - creditNote.CreditNoteValue);
                  
                    context.SaveChanges();

                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }

           // }
        }


        public byte[] ExportInvoiceReport(List<InvoiceDto> invoiceList, bool isAdmin = false)
        {
            byte[] stream = this.generateExcelSheetForInvoiceReport(invoiceList, isAdmin);
            return stream;
        }



        private byte[] generateExcelSheetForInvoiceReport(List<InvoiceDto> invoiceList, bool isAdmin)
        {
            using (ExcelPackage excel = new ExcelPackage())
            {
                //Create the worksheet
                ExcelWorksheet ws = excel.Workbook.Worksheets.Add("Invoices");

                //Merging cells and create a center heading for out table
                ws.Cells[2, 1].Value = "Invoice Report";
                ws.Cells[2, 1, 2, 8].Merge = true;
                ws.Cells[2, 1, 2, 8].Style.Font.Bold = true;
                ws.Cells[2, 1, 2, 8].Style.Font.Size = 15;
                ws.Cells[2, 1, 2, 8].Style.Font.Name = "Calibri";
                ws.Cells[2, 1, 2, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                // Set headings.
                ws.Cells["A6"].Value = "INVOICE NUMBER";
                ws.Cells["B6"].Value = "INVOICE DATE";
                ws.Cells["C6"].Value = "SHIPMENT REFERENCE";
                ws.Cells["D6"].Value = "INVOICE VALUE";
                ws.Cells["E6"].Value = "INVOICE STATUS";              
                ws.Cells["F6"].Value = isAdmin ? "BUSINESS OWNER" : null;
                ws.Cells["G6"].Value = isAdmin ? "CORPORATE NAME" : null;

                string endingCell = isAdmin ? "G6" : "E6";

                //Format the header for columns.
                using (ExcelRange rng = ws.Cells["A6:" + endingCell])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid;                      //Set Pattern for the background to Solid
                    rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189));  //Set color to dark blue
                    rng.Style.Font.Color.SetColor(Color.White);
                }

                //ws.Cells["A6:H6"].AutoFitColumns();

                // Set data.
                int rowIndex = 6;

                if (invoiceList != null)
                {
                    foreach (var invoice in invoiceList) // Adding Data into rows
                    {
                        rowIndex++;

                        var cell = ws.Cells[rowIndex, 1];
                        cell.Value = invoice.InvoiceNumber;

                        cell = ws.Cells[rowIndex, 2];
                        cell.Value = invoice.InvoiceDate;

                        cell = ws.Cells[rowIndex, 3];
                        cell.Value = invoice.ShipmentReference;

                        cell = ws.Cells[rowIndex, 4];
                        cell.Value = invoice.InvoiceValue;

                        cell = ws.Cells[rowIndex, 5];
                        cell.Value = invoice.InvoiceStatus;

                        cell = ws.Cells[rowIndex, 6];
                        cell.Value = invoice.BusinessOwner;

                        cell = ws.Cells[rowIndex, 7];
                        cell.Value = invoice.CompanyName;            

                        ws.Row(rowIndex).Height = 25;
                    }

                    // Set width
                    for (int i = 1; i < 8; i++)
                    {
                        ws.Column(i).Width = 25;
                    }
                }
                return excel.GetAsByteArray();
            }
        }


    }
}
