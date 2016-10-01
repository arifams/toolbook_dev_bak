﻿using OfficeOpenXml;
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

namespace PI.Business
{
    public class InvoiceMangement : IInvoiceMangement
    {
        private PIContext context;
        private ILogger logger;
        IShipmentManagement shipmentManagement;
        IPaymentManager paymentManager;

        public InvoiceMangement(ILogger logger, IShipmentManagement shipmentManagement, IPaymentManager paymentManager,  PIContext _context = null)
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
            string role = context.GetUserRoleById(userId);
            Company company = context.GetCompanyByUserId(userId);

            
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
            payment.CreatedDate = DateTime.Now;
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
                    CreatedDate = DateTime.Now,
                    URL = invoiceDetails.URL,
                    DueDate= Convert.ToDateTime(invoiceDetails.DueDate)                    

                };


                context.Invoices.Add(invoice);
                context.SaveChanges();
                invoiceSaved = true;

            }
            catch (Exception e)
            {

                throw;
            }

                

               

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
        

        public async Task<bool> FetchInvoiceDetailsfromPdf(string pdfUrl)
        {
            InvoiceDto invoiceDetails = new InvoiceDto();
            ShipmentDto shipmentDetails = new ShipmentDto();
            AzureFileManager media = new AzureFileManager();
            string baseUrl = ConfigurationManager.AppSettings["PIBlobStorage"];

            //generating a random number for invoice name
            Random generator = new Random();
            string code = generator.Next(1000000, 9999999).ToString("D7");
            string invoicename = "PI_" + DateTime.Now.Year.ToString() + "_" + code;
            
            var url = pdfUrl;
            string filename = "";
            string trackingNo = "";
            string invoiceNumber = "";
            string createdDate = "";
            string duedate = "";
            string terms = "";
            
            string pathToPdf = url;
            string xml_path = System.Web.HttpContext.Current.Server.MapPath("\\Pdf\\invoice.xml");
            string pathToXml = xml_path;

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
                trackingNo = this.GetBetween(doc.SelectSingleNode("document/page/table/row/cell[contains(text(),'AWB#')]").InnerText, "AWB#:", "Reference").Replace(" ", "");
                invoiceNumber = doc.SelectSingleNode("document/page/table/row/cell[text()='INVOICE #']").NextSibling.InnerText;
                createdDate= doc.SelectSingleNode("document/page/table/row/cell[text()='DATE']").NextSibling.InnerText;
                duedate=doc.SelectSingleNode("document/page/table/row/cell[text()='DUE DATE']").NextSibling.InnerText;
                terms=doc.SelectSingleNode("document/page/table/row/cell[text()='TERMS']").NextSibling.InnerText;                
            }

            f.ClosePdf();
            if (!string.IsNullOrEmpty(trackingNo))
            {
                shipmentDetails= shipmentManagement.GetShipmentDetailsByTrackingNo(trackingNo);
            }
            //get tenantId 
            var tenantId = context.GetTenantIdByUserId(shipmentDetails.GeneralInformation.CreatedBy);

                //saving invoice details fetched from the Pdf
                WebClient myclient = new WebClient();
            using (Stream savedPdf = new MemoryStream(myclient.DownloadData(pdfUrl)))
            {
                filename = string.Format("{0}_{1}", System.Guid.NewGuid().ToString(), invoicename + ".pdf");
                media.InitializeStorage(tenantId.ToString(), Utility.GetEnumDescription(DocumentType.Invoice));
                await media.Upload(savedPdf, filename);
            }            
                
            //uploaded Url
            var returnData = baseUrl + "TENANT_" + tenantId + "/" + Utility.GetEnumDescription(DocumentType.Invoice)+ "/" + filename;

            //saving fetched details from Pdf
             invoiceDetails.InvoiceNumber = invoiceNumber;
             invoiceDetails.ShipmentId =Convert.ToInt16(shipmentDetails.GeneralInformation.ShipmentId);
             invoiceDetails.InvoiceDate =createdDate;
             invoiceDetails.DueDate = duedate;
             invoiceDetails.CreatedOn = createdDate;
             invoiceDetails.Terms = terms;
             invoiceDetails.InvoiceValue = Convert.ToDecimal(shipmentDetails.PackageDetails.CarrierCost);
             invoiceDetails.URL = returnData;
             invoiceDetails.InvoiceStatus = InvoiceStatus.Paid.ToString();
            invoiceDetails.CreatedBy = shipmentDetails.GeneralInformation.CreatedBy;


            this.SaveInvoiceDetails(invoiceDetails);

            //deleting pdf file saved in tenant0 space
            
            media.InitializeStorage("0", "Invoice_Temp");

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
