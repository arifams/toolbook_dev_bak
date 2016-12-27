using OfficeOpenXml;
using OfficeOpenXml.Style;
using PI.Contract;
using PI.Contract.Business;
using PI.Contract.DTOs;
using PI.Contract.DTOs.Common;
using PI.Contract.DTOs.Invoice;
using PI.Contract.DTOs.Shipment;
using PI.Contract.Enums;
using PI.Data;
using PI.Data.Entity;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Net;
using System.IO;
using System.Web;
using PI.Contract.TemplateLoader;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using AzureMediaManager;
using PI.Common;
using System.Configuration;
using SautinSoft;
using System.Xml;
using PI.Contract.DTOs.Payment;
using System.Globalization;

namespace PI.Business
{
    public class InvoiceMangement : IInvoiceMangement
    {
        private PIContext context;
        private ILogger logger;
        IShipmentManagement shipmentManagement;
        IPaymentManager paymentManager;

        public InvoiceMangement(ILogger logger, IShipmentManagement shipmentManagement, IPaymentManager paymentManager, PIContext _context = null)
        {
            context = _context ?? PIContext.Get();
            this.shipmentManagement = shipmentManagement;
            this.logger = logger;
            this.paymentManager = paymentManager;
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
        public PagedList GetAllInvoicesByCustomer(string status, string userId, DateTime? startDate, DateTime? endDate, string searchValue)
        {
            var pagedRecord = new PagedList();
            IList<Invoice> invoiceList = new List<Invoice>();
            pagedRecord.Content = new List<InvoiceDto>();
            InvoiceStatus invoiceStatus = InvoiceStatus.None;

            if (status != null)
            {
                invoiceStatus = (InvoiceStatus)Enum.Parse(typeof(InvoiceStatus), status, true);
            }

            string role = context.GetUserRoleById(userId);
            Company company = context.GetCompanyByUserId(userId);


            //using (var context = PIContext.Get())
            //{
            if (role == "BusinessOwner" || role == "Manager")
            {
                // Business Owners
                invoiceList = context.Invoices.Where(x => x.Shipment.Division.CompanyId == company.Id).ToList();
            }
            //else if (role == "Supervisor")
            //{
            //    // Supervises
            //    invoiceList = context.Invoices.Where(x => x.Shipment.Division.UserInDivisions.Any(u => u.UserId == userId)).ToList();
            //}
            //else
            //{
            //    // Operators
            //    invoiceList = context.Invoices.Where(x => x.Shipment.CreatedBy == userId).ToList();
            //}

            var content = invoiceList.Where(i => i.IsDelete == false &&
                                          (string.IsNullOrEmpty(searchValue) || i.InvoiceNumber.Contains(searchValue)) &&
                                          (string.IsNullOrEmpty(status) || i.InvoiceStatus == invoiceStatus) &&
                                          (startDate == null || i.CreatedDate >= startDate && i.CreatedDate <= endDate)).ToList();

            foreach (var item in content)
            {
                pagedRecord.Content.Add(new InvoiceDto
                {
                    Id = item.Id,
                    ShipmentReference = item.Shipment.ShipmentCode,
                    InvoiceDate = item.CreatedDate.ToString("dd MMM yyyy"),
                    InvoiceNumber = item.InvoiceNumber,
                    InvoiceValue = item.InvoiceValue,
                    InvoiceStatus = item.InvoiceStatus.ToString(),
                    Sum = item.Sum.ToString(),
                    CreditedValue = item.CreditAmount.ToString(),
                    URL = item.URL
                });
            }

            return pagedRecord;
            // }
        }




        public byte[] GetAllInvoicesByAdminForExport(string status, string userId, DateTime? startDate, DateTime? endDate, string searchValue)
        {
            
            IList<Invoice> invoiceList = new List<Invoice>();
            List<InvoiceDto> Content = new List<InvoiceDto>();
            InvoiceStatus invoiceStatus = InvoiceStatus.None;

            if (status != null)
            {
                invoiceStatus = (InvoiceStatus)Enum.Parse(typeof(InvoiceStatus), status, true);
            }

            string role = context.GetUserRoleById(userId);
            Company company = context.GetCompanyByUserId(userId);


            //using (var context = PIContext.Get())
            //{
            if (role == "BusinessOwner" || role == "Manager")
            {
                // Business Owners
                invoiceList = context.Invoices.Where(x => x.Shipment.Division.CompanyId == company.Id).ToList();
            }
            //else if (role == "Supervisor")
            //{
            //    // Supervises
            //    invoiceList = context.Invoices.Where(x => x.Shipment.Division.UserInDivisions.Any(u => u.UserId == userId)).ToList();
            //}
            //else
            //{
            //    // Operators
            //    invoiceList = context.Invoices.Where(x => x.Shipment.CreatedBy == userId).ToList();
            //}

            var content = invoiceList.Where(i => i.IsDelete == false &&
                                          (string.IsNullOrEmpty(searchValue) || i.InvoiceNumber.Contains(searchValue)) &&
                                          (string.IsNullOrEmpty(status) || i.InvoiceStatus == invoiceStatus) &&
                                          (startDate == null || i.CreatedDate >= startDate && i.CreatedDate <= endDate)).ToList();

            foreach (var item in content)
            {
                Content.Add(new InvoiceDto
                {
                    Id = item.Id,
                    ShipmentReference = item.Shipment.ShipmentCode,
                    InvoiceDate = item.CreatedDate.ToString("dd/MM/yyyy"),
                    InvoiceNumber = item.InvoiceNumber,
                    InvoiceValue = item.InvoiceValue,
                    InvoiceStatus = item.InvoiceStatus.ToString(),
                    CreditedValue=item.CreditAmount.ToString(),
                    Sum=item.Sum.ToString(),
                    URL = item.URL
                });
            }

            return this.generateExcelSheetForInvoiceReport(Content, false);
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
            invoice.InvoiceStatus = (InvoiceStatus)Enum.Parse(typeof(InvoiceStatus), invoiceDto.InvoiceStatus, true);

            context.SaveChanges();

            return invoice.InvoiceStatus;
            // }
        }


        /// <summary>
        /// Pay an invoice
        /// </summary>
        /// <param name="invoiceDto"></param>
        /// <returns></returns>
        public OperationResult PayInvoice(InvoiceDto invoiceDto)
        {
            var invoice = context.Invoices.Where(i => i.Id == invoiceDto.Id).SingleOrDefault();

            PaymentDto paymentDto = new PaymentDto()
            {
                CardNonce = invoiceDto.CardNonce,
                ChargeAmount = invoice.InvoiceValue,
                CurrencyType = "USD",   // TODO: change this.
            };

            OperationResult result = paymentManager.Charge(paymentDto);

            var payment = new Payment();
            payment.CreatedBy = invoiceDto.UserId;
            payment.CreatedDate = DateTime.UtcNow;
            payment.IsActive = true;
            payment.PaymentId = result.FieldList["PaymentKey"];
            payment.Status = result.Status;
            payment.PaymentType = Contract.Enums.PaymentType.Invoice;
            payment.ReferenceId = invoiceDto.Id;

            if (result.Status == Status.PaymentError)
            {
                // If failed, due to payment gateway error, then record payment error code.
                payment.StatusCode = result.FieldList["errorCode"];
            }
            else if (result.Status == Status.Success)
            {
                invoice.InvoiceStatus = InvoiceStatus.Paid;
            }

            context.Payments.Add(payment);
            context.SaveChanges();
            return result;
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
                CreatedDate = DateTime.UtcNow,
                CreatedBy = invoice.CreatedBy

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
            strTemplate.Append("<span class='value'>" + invoice.DisputeComment.ToString() + "</span> </span>");
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
        public PagedList GetAllInvoicesForAdmin(PagedList pageList)
        {
            //string status, string userId, DateTime? startDate, DateTime? endDate, string searchValue
            var pagedRecord = new PagedList();
            pagedRecord.Content = new List<InvoiceDto>();

            //
            pageList.DynamicContent = pageList.filterContent;

            string status = pageList.DynamicContent.status.ToString();
            string searchValue = pageList.DynamicContent.searchValue.ToString();
            DateTime? startDate = null, endDate = null;

            if (!string.IsNullOrWhiteSpace(pageList.DynamicContent.startDate.ToString()))
            {
                // Convert to utc
                startDate = Convert.ToDateTime(pageList.DynamicContent.startDate.ToString());
                startDate = startDate.Value.ToUniversalTime();

                endDate = Convert.ToDateTime(pageList.DynamicContent.endDate.ToString());
                endDate = endDate.Value.ToUniversalTime();
            }
            //

         //   string BusinessOwnerRoleId = context.Roles.Where(r => r.Name == "BusinessOwner").Select(r => r.Id).FirstOrDefault();

            var querableContent = (from invoice in context.Invoices
                           join company in context.Companies on invoice.Shipment.Division.CompanyId equals company.Id
                           //join user in context.Users on company.TenantId equals user.TenantId
                           join user in context.Users on invoice.CreatedBy equals user.Id
                           where  //user.Roles.Any(r => r.RoleId == BusinessOwnerRoleId) &&
                           company.IsDelete == false &&
                           (status == "" || invoice.InvoiceStatus.ToString() == status) &&
                           (string.IsNullOrEmpty(searchValue) ||
                             company.Name.Contains(searchValue) ||
                             user.FirstName.Contains(searchValue) || user.LastName.Contains(searchValue) ||
                             invoice.InvoiceNumber.Contains(searchValue)
                           ) &&
                            (startDate == null || (invoice.CreatedDate >= startDate && invoice.CreatedDate <= endDate))
                           select new
                           {
                               User = user,
                               Company = company,
                               Invoice = invoice
                           });

            var contentList = querableContent.OrderBy(d => d.Invoice.CreatedDate).Skip(pageList.CurrentPage).Take(pageList.PageSize).ToList();

            //removing unmatched company invoices according to the business owners
            foreach (var item in contentList)
            {

                //if (item.User.Roles.Any(r => r.RoleId == BusinessOwnerRoleId))
                //{
                    pagedRecord.Content.Add(new InvoiceDto
                    {
                        Id = item.Invoice.Id,
                        InvoiceNumber = item.Invoice.InvoiceNumber,
                        InvoiceStatus = item.Invoice.InvoiceStatus.ToString(),
                        InvoiceValue = item.Invoice.InvoiceValue,
                        ShipmentId = item.Invoice.ShipmentId,
                        ShipmentReference = item.Invoice.Shipment.ShipmentCode,
                        URL = item.Invoice.URL,
                        BusinessOwner = item.User.FirstName + " " + item.User.LastName,
                        CompanyName = item.Company.Name,
                        CompanyId = item.Company.Id.ToString("00000"),
                        InvoiceDate = item.Invoice.CreatedDate.ToString("dd/MM/yyyy"),
                        CreditNoteURL = item.Invoice.creditNoteList.Count == 0 ? null :
                                        item.Invoice.creditNoteList.OrderByDescending(x => x.CreatedDate).FirstOrDefault().URL,
                        Sum=item.Invoice.Sum.ToString(),
                        CreditedValue= item.Invoice.CreditAmount.ToString(),
                    });
               // }
            }

            pagedRecord.TotalRecords = querableContent.Count();
            pagedRecord.PageSize = pageList.PageSize;
            pagedRecord.TotalPages = (int)Math.Ceiling((decimal)pagedRecord.TotalRecords / pagedRecord.PageSize);

            return pagedRecord;
        }



        public byte[] GetAllInvoicesForAdminExport(string status, string userId, DateTime? startDate, DateTime? endDate, string searchValue)
        {
          
            int page = 1;
            int pageSize = 10;
            List<InvoiceDto> Content = new List<InvoiceDto>();

            //using (PIContext context = PIContext.Get())
            //{            
            string BusinessOwnerRoleId = context.Roles.Where(r => r.Name == "BusinessOwner").Select(r => r.Id).FirstOrDefault();

            var content = (from invoice in context.Invoices
                           join company in context.Companies on invoice.Shipment.Division.CompanyId equals company.Id
                           join user in context.Users on company.TenantId equals user.TenantId
                           where user.Roles.Any(r => r.RoleId == BusinessOwnerRoleId) &&
                           company.IsDelete == false &&
                           (status == null || invoice.InvoiceStatus.ToString() == status) &&
                           (string.IsNullOrEmpty(searchValue) ||
                             company.Name.Contains(searchValue) ||
                             user.FirstName.Contains(searchValue) || user.LastName.Contains(searchValue) ||
                             invoice.InvoiceNumber.Contains(searchValue)
                           ) &&
                            (startDate == null || (invoice.CreatedDate >= startDate && invoice.CreatedDate <= endDate))
                           select new
                           {
                               User = user,
                               Company = company,
                               Invoice = invoice
                           }).ToList();

            //removing unmatched company invoices according to the business owners
            foreach (var item in content)
            {

                if (item.User.Roles.Any(r => r.RoleId == BusinessOwnerRoleId))
                {
                    Content.Add(new InvoiceDto
                    {
                        Id = item.Invoice.Id,
                        InvoiceNumber = item.Invoice.InvoiceNumber,
                        InvoiceStatus = item.Invoice.InvoiceStatus.ToString(),
                        InvoiceValue = item.Invoice.InvoiceValue,
                        ShipmentId = item.Invoice.ShipmentId,
                        ShipmentReference = item.Invoice.Shipment.ShipmentCode,
                        URL = item.Invoice.URL,
                        BusinessOwner = item.User.FirstName + " " + item.User.LastName,
                        CompanyName = item.Company.Name,
                        CompanyId = item.Company.Id.ToString("00000"),
                        InvoiceDate = item.Invoice.CreatedDate.ToString("dd/MM/yyyy"),
                        CreditNoteURL = item.Invoice.creditNoteList.Count == 0 ? null :
                                        item.Invoice.creditNoteList.OrderByDescending(x => x.CreatedDate).FirstOrDefault().URL,
                        Sum=item.Invoice.Sum.ToString(),
                        CreditedValue=item.Invoice.CreditAmount.ToString()
                        
                    });
                }
            }
            return this.ExportInvoiceReport(Content, true);


          
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
            ShipmentDto dto = new ShipmentDto();


            try
            {
                Invoice invoice = new Invoice()
                {
                    InvoiceNumber = invoiceDetails.InvoiceNumber,
                    ShipmentId = invoiceDetails.ShipmentId,
                    InvoiceValue = invoiceDetails.InvoiceValue,
                    CreatedBy = invoiceDetails.CreatedBy.ToString(),
                    InvoiceStatus = (InvoiceStatus)Enum.Parse(typeof(InvoiceStatus), invoiceDetails.InvoiceStatus, true),
                    CreatedDate = DateTime.UtcNow,
                    URL = invoiceDetails.URL,
                    DueDate = DateTime.ParseExact(invoiceDetails.DueDate,"MM/dd/yyyy",CultureInfo.InvariantCulture),
                    

                };
                context.Invoices.Add(invoice);
                context.SaveChanges();
                invoiceSaved = true;

            }
            catch (Exception e)
            {

                throw;
            }
              

           

                      


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
                    CreatedDate = DateTime.UtcNow,
                    URL = creditNoteDetails.URL
                };

                context.CreditNotes.Add(creditNote);
                context.SaveChanges();

                // Update Invoice status and value.
                var invoice = context.Invoices.Where(x => x.Id == creditNoteDetails.Id).SingleOrDefault();

                invoice.InvoiceStatus = (invoice.InvoiceValue == creditNote.CreditNoteValue) ?
                                       InvoiceStatus.Paid : InvoiceStatus.Pending;

                invoice.Sum = (invoice.InvoiceValue - creditNote.CreditNoteValue);
                invoice.CreditAmount = creditNote.CreditNoteValue;
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
                ws.Cells["C6"].Value = "SHIPMENT ID";
                ws.Cells["D6"].Value = "INVOICE VALUE";
                ws.Cells["E6"].Value = "INVOICE STATUS";

                ws.Cells["F6"].Value = "SUM";
                ws.Cells["G6"].Value = "CREDITED AMOUNT";
                ws.Cells["H6"].Value = "URL";

                ws.Cells["I6"].Value = isAdmin ? "BUSINESS OWNER" : null;
                ws.Cells["J6"].Value = isAdmin ? "CORPORATE NAME" : null;
                ws.Cells["K6"].Value = isAdmin ? "Company Id" : null;

                string endingCell = isAdmin ? "K6" : "H6";

                //Format the header for columns.
                using (ExcelRange rng = ws.Cells["A6:" + endingCell])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid;                      //Set Pattern for the background to Solid
                    rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));  //Set color to dark blue
                    rng.Style.Font.Color.SetColor(System.Drawing.Color.White);
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
                        cell.Value = invoice.Sum;

                        cell = ws.Cells[rowIndex, 7];
                        cell.Value = invoice.CreditedValue;

                        cell = ws.Cells[rowIndex, 8];
                        cell.Value = invoice.URL;
                        

                        cell = ws.Cells[rowIndex, 9];
                        cell.Value = invoice.BusinessOwner;

                        cell = ws.Cells[rowIndex, 10];
                        cell.Value = invoice.CompanyName;

                        cell = ws.Cells[rowIndex, 11];
                        cell.Value = invoice.CompanyId;

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


        public async Task<bool> FetchInvoiceDetailsfromPdf(string pdfUrl)
        {
            InvoiceDto invoiceDetails = new InvoiceDto();
            //ShipmentDto shipmentDetails = new ShipmentDto();
            AzureFileManager media = new AzureFileManager();
            string baseUrl = ConfigurationManager.AppSettings["PIBlobStorage"];

            //generating a random number for invoice name
            Random generator = new Random();
            string code = generator.Next(1000000, 9999999).ToString("D7");
            string invoicename = "PI_" + DateTime.UtcNow.Year.ToString() + "_" + code;

            var url = pdfUrl;
            string filename = "";
            string trackingNo = "";
            string invoiceNumber = "";
            string createdDate = "";
            string duedate = "";
            string terms = "";
            string invoiceAmount = "";
            XmlNode creditnode = null;


            //for credit note values
            string creditnoteNumber = "";
            string customerNumber = "";
            string totalAmountship = "";
            string totalVat = "";
            string creditAmount = "";
            bool invoice = true;

            string pathToPdf = url;

            var uploadFolder = "~/App_Data/Tmp/FileUploads/invoice.xml";
            string pathToXml = System.Web.HttpContext.Current.Server.MapPath(uploadFolder);
                      
          

            // Convert PDF file to XML file. 
            SautinSoft.PdfFocus f = new SautinSoft.PdfFocus();

            f.XmlOptions.ConvertNonTabularDataToSpreadsheet = true;

           
            try
            {
                Uri uri = new Uri(pathToPdf);
                f.OpenPdf(uri);

            }
            catch (Exception e)
            {
                   
                throw;
            }
            

            if (f.PageCount > 0)
            {
                int result = f.ToXml(pathToXml);
                XmlDocument doc = new XmlDocument();
                doc.Load(pathToXml);
                //fetching details from xml
                XmlNode trackingNode = doc.SelectSingleNode("document/page/table/row/cell[contains(text(),'AWB#')]");
              
                if (trackingNode == null)
                {
                    //set the flag for credit notes
                    invoice = false;

                    creditnoteNumber =doc.SelectSingleNode("document/page/table/row/cell[contains(text(),'Credit number')]").InnerText.Split(new char[0]).Last();
                    customerNumber = this.GetBetween(doc.SelectSingleNode("document/page/table/row/cell[contains(text(),'Customer')]").InnerText, "Customer", "Invoice ").Replace(" ", "");
                    invoiceNumber = this.GetBetween(doc.SelectSingleNode("document/page/table/row/cell[contains(text(),'Invoice number')]").InnerText, "number:", "Credit").Replace(" ", "");
                    totalAmountship = this.GetBetween(doc.SelectSingleNode("document/page/table/row/cell[contains(text(),'Total amount shipment')]").InnerText, "$", "Total").Replace(" ", ""); 


                    totalVat = doc.SelectSingleNode("document/page/table/row/cell[contains(text(),'Total VAT')]").InnerText.Split(new char[0]).Last();
                    creditAmount = doc.SelectSingleNode("document/page/table/row/cell[contains(text(),'Credit amount')]").InnerText.Split(new char[0]).Last().Replace("$","");

                }
                else
                {

                   
                    trackingNo = this.GetBetween(doc.SelectSingleNode("document/page/table/row/cell[contains(text(),'AWB#')]").InnerText, "AWB#:", "Reference").Replace(" ", "");
                    if (string.IsNullOrEmpty(trackingNo)&& doc.SelectSingleNode("document/page/table/row/cell[contains(text(),'AWB#')]").InnerText!=null)
                    {
                        trackingNo = doc.SelectSingleNode("document/page/table/row/cell[contains(text(),'AWB#')]").InnerText.Replace("AWB#:", "").Replace(" ", "");
                    }
                 
                   // invoiceNumber = doc.SelectSingleNode("document/page/table/row/cell[text()='INVOICE #']").NextSibling.InnerText;
                    invoiceNumber = doc.SelectSingleNode("document/page/table/row/cell[contains(text(),'INVOICE #')]").InnerText.Replace(" ", "");
                    invoiceNumber = invoiceNumber.Replace("INVOICE#", "");

                    if (doc.SelectSingleNode("document/page/table/row/cell[contains(text(),'DATE')]").NextSibling==null)
                    {
                        createdDate = doc.SelectSingleNode("document/page/table/row/cell[contains(text(),'DATE')]").InnerText.Replace(" ", "");
                        createdDate = createdDate.Replace("DATE", "");
                    }
                    else
                    {
                        createdDate = doc.SelectSingleNode("document/page/table/row/cell[contains(text(),'DATE')]").NextSibling.InnerText.Replace(" ", "");
                    }


                    if (doc.SelectSingleNode("document/page/table/row/cell[contains(text(),'DUE DATE')]").NextSibling==null)
                    {
                        duedate = doc.SelectSingleNode("document/page/table/row/cell[contains(text(),'DUE DATE')]").InnerText.Replace(" ", "");
                        duedate = duedate.Replace("DUEDATE", "");
                    }
                    else
                    {
                        duedate = doc.SelectSingleNode("document/page/table/row/cell[contains(text(),'DUE DATE')]").NextSibling.InnerText.Replace(" ", "");
                    }

                    if (doc.SelectSingleNode("document/page/table/row/cell[contains(text(),'TERMS')]").NextSibling==null)
                    {
                        terms = doc.SelectSingleNode("document/page/table/row/cell[contains(text(),'TERMS')]").InnerText.Replace(" ", "");
                        terms = terms.Replace("TERMS", "");
                    }
                    else
                    {
                        terms = doc.SelectSingleNode("document/page/table/row/cell[contains(text(),'TERMS')]").NextSibling.InnerText.Replace(" ", "");
                    }

                    if (doc.SelectSingleNode("document/page/table/row/cell[contains(text(),'BALANCE DUE')]").NextSibling==null)
                    {
                        invoiceAmount = doc.SelectSingleNode("document/page/table/row/cell[contains(text(),'BALANCE DUE')]").InnerText.Replace("$", "").Replace(" ", "");
                        invoiceAmount = invoiceAmount.Replace("BALANCE DUE", "");
                    }
                    else if (doc.SelectSingleNode("document/page/table/row/cell[contains(text(),'BALANCE DUE')]").NextSibling.NextSibling == null)
                    {
                        doc.SelectSingleNode("document/page/table/row/cell[contains(text(),'BALANCE DUE')]").NextSibling.InnerText.Replace("$", "").Replace(" ", "");
                    }
                    else
                    {
                        invoiceAmount = doc.SelectSingleNode("document/page/table/row/cell[contains(text(),'BALANCE DUE')]").NextSibling.NextSibling.InnerText.Replace("$", "").Replace(" ", "");
                    }
                   
                    

                }
                

            }

            f.ClosePdf();
            var savedInvoice = this.GetInvoiceByNumber(invoiceNumber);

            Shipment shipmentDetails = null;
            if (!invoice && savedInvoice ==null)
            {
                return false;
            }
            else if(invoice)
            {
                // Invoice upload
                shipmentDetails = context.Shipments.Where(sh => sh.TrackingNumber == trackingNo).FirstOrDefault();
            }
            else
            {
                // Dispute upload
                shipmentDetails = context.Shipments.Where(sh => sh.Id == (long)savedInvoice.ShipmentId).FirstOrDefault();
            }

            //get tenantId 
            if (shipmentDetails==null)
            {
                return false;
            }
            var tenantId = context.GetTenantIdByUserId(shipmentDetails.CreatedBy);

            //saving invoice details fetched from the Pdf
            WebClient myclient = new WebClient();
            using (Stream savedPdf = new MemoryStream(myclient.DownloadData(pdfUrl)))
            {
                filename = string.Format("{0}_{1}", System.Guid.NewGuid().ToString(), invoicename + ".pdf");
                media.InitializeStorage(tenantId.ToString(), Utility.GetEnumDescription(DocumentType.Invoice));
                await media.Upload(savedPdf, filename);
            }

            //uploaded Url
            var returnData = baseUrl + "TENANT_" + tenantId + "/" + Utility.GetEnumDescription(DocumentType.Invoice) + "/" + filename;
            

            if (invoice)
            {
                //saving fetched details from Pdf
                Invoice invoiceToSave = new Invoice()
                {
                    InvoiceNumber = invoiceNumber,
                    CreatedDate = Convert.ToDateTime(createdDate),
                    DueDate = Convert.ToDateTime(duedate),
                    CreatedBy = shipmentDetails.CreatedBy,
                    InvoiceStatus = InvoiceStatus.Pending,
                    InvoiceValue = Convert.ToDecimal(invoiceAmount),
                    IsActive = true,
                    ShipmentId = shipmentDetails.Id,
                    URL = returnData,
                    Terms = terms
                };
                context.Invoices.Add(invoiceToSave);
                context.SaveChanges();

                //invoiceDetails.InvoiceNumber = invoiceNumber;
                //invoiceDetails.Id = savedInvoice.Id;
                //invoiceDetails.InvoiceDate = createdDate;
                //invoiceDetails.DueDate = duedate;
                //invoiceDetails.CreatedOn = createdDate;
                //invoiceDetails.Terms = terms;
                //invoiceDetails.InvoiceValue = Convert.ToDecimal(invoiceAmount);
                //invoiceDetails.URL = returnData;
                //invoiceDetails.InvoiceStatus = InvoiceStatus.Paid.ToString();
                //invoiceDetails.CreatedBy = shipmentDetails.GeneralInformation.CreatedBy;
                //this.SaveCreditNoteDetails(invoiceDetails);
            }
            else
            {
               
                 invoiceDetails.Id = savedInvoice.Id;
                 invoiceDetails.InvoiceNumber = invoiceNumber;
                 invoiceDetails.InvoiceValue = Convert.ToDecimal(creditAmount);
                 invoiceDetails.CreatedBy = savedInvoice.CreatedBy;
                 invoiceDetails.URL = returnData;
                
                //saving credit notes
                this.SaveCreditNoteDetails(invoiceDetails);
            }

           

            //deleting pdf file saved in tenant0 space
            
           // media.InitializeStorage("0", "Invoice_Temp");

            //try
            //{
            //    await media.Delete(pdfUrl);
            //}
            //catch (Exception e)
            //{

            //    var m = e.Message;
            //}
           

            return true;

        }

        //get invoice by number 
        private Invoice GetInvoiceByNumber(string number)
        {

            Invoice invoice = context.Invoices.Where(t => t.InvoiceNumber == number).FirstOrDefault();
            return invoice;           
        }


        private string GetBetween(string value, string a, string b)
        {
            int posA = value.IndexOf(a);
            int posB = value.LastIndexOf(b);
            if (posA == -1)
            {
                return "";
            }
            if (posB == -1)
            {
                return "";
            }
            int adjustedPosA = posA + a.Length;
            if (adjustedPosA >= posB)
            {
                return "";
            }
            return value.Substring(adjustedPosA, posB - adjustedPosA);
        }

    }
}
