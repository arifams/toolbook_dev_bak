using AzureMediaManager;
using Newtonsoft.Json;
using PI.Business;
using PI.Common;
using PI.Contract.Business;
using PI.Contract.DTOs;
using PI.Contract.DTOs.AddressBook;
using PI.Contract.DTOs.AuditTrail;
using PI.Contract.DTOs.Common;
using PI.Contract.DTOs.Company;
using PI.Contract.DTOs.FileUpload;
using PI.Contract.DTOs.Invoice;
using PI.Contract.Enums;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace PI.Service.Controllers
{

    [CustomAuthorize(Roles = "Admin,1st Line Support,2nd Line Support")]
    //[CustomAuthorize(Roles = "Admin")]
    [RoutePrefix("api/Admin")]
    public class AdminController : BaseApiController
    {
        readonly IAdministrationManagment adminManagement;
        readonly IInvoiceMangement invoiceMangement;
        readonly ICompanyManagement companyManagement;
        readonly IShipmentManagement shipmentManagement;
        readonly ICustomerManagement customerManagement;

        public AdminController(IAdministrationManagment adminManagement, IInvoiceMangement invoiceMangement, 
                               ICompanyManagement companyManagement, IShipmentManagement shipmentManagement, ICustomerManagement customerManagement)
        {
            this.adminManagement = adminManagement;
            this.invoiceMangement = invoiceMangement;
            this.companyManagement = companyManagement;
            this.shipmentManagement = shipmentManagement;
            this.customerManagement = customerManagement;
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpPost]
        // This is from System.Web.Http, and not from System.Web.Mvc
        [Route("UploadRateSheet")]
        public async Task<HttpResponseMessage> UploadRateSheet(string userId)
        {
            OperationResult opResult = new OperationResult();
            HttpResponseMessage uploadResult = new HttpResponseMessage();
            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                {
                    this.Request.CreateResponse(HttpStatusCode.UnsupportedMediaType);
                }

                var provider = GetMultipartProvider();
                var result = await Request.Content.ReadAsMultipartAsync(provider);
                uploadResult = await this.Upload(result);

                string returnData = result.FileData.First().LocalFileName;
                var response = this.Request.CreateResponse(HttpStatusCode.OK, new { returnData });

                var urlJson = await response.Content.ReadAsStringAsync();

                Result deSelizalizedObject = null;
                deSelizalizedObject = JsonConvert.DeserializeObject<Result>(urlJson);

                opResult = adminManagement.ImportRateSheetExcel(deSelizalizedObject.returnData);
            }
            catch (Exception ex)
            {
                opResult.Status = Status.Error;
                opResult.Message = "Controller  :" + ex.ToString();
            }

            return this.Request.CreateResponse(opResult.Status == Status.Success && uploadResult.StatusCode == HttpStatusCode.OK ? HttpStatusCode.OK : HttpStatusCode.InternalServerError, opResult.Message);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost] // This is from System.Web.Http, and not from System.Web.Mvc
        [Route("Upload")]
        public async Task<HttpResponseMessage> Upload(MultipartFormDataStreamProvider results)
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                this.Request.CreateResponse(HttpStatusCode.UnsupportedMediaType);
            }

            var result = results;
            // On upload, files are given a generic name like "BodyPart_26d6abe1-3ae1-416a-9429-b35f15e6e5d5"
            // so this is how you can get the original file name
            var originalFileName = GetDeserializedFileName(result.FileData.First());

            // uploadedFileInfo object will give you some additional stuff like file length,
            // creation time, directory name, a few filesystem methods etc..
            var uploadedFileInfo = new FileInfo(result.FileData.First().LocalFileName);

            // Remove this line as well as GetFormData method if you're not
            // sending any form data with your upload request
            var fileDetails = GetFormData<FileUploadDto>(result);

            // Convert to stream            
            Stream stream = File.OpenRead(uploadedFileInfo.FullName);

            AzureFileManager media = new AzureFileManager();
            string imageFileNameInFull = null;
            // Make absolute link
            string baseUrl = ConfigurationManager.AppSettings["PIBlobStorage"];

            var tenantId = companyManagement.GetTenantIdByUserId(fileDetails.UserId);
            fileDetails.TenantId = tenantId;

            if (fileDetails.DocumentType == DocumentType.AddressBook)
            {
                var fileNameSplitByDot = originalFileName.Split(new char[1] { '.' });
                string fileExtention = fileNameSplitByDot[fileNameSplitByDot.Length - 1];

                imageFileNameInFull = string.Format("{0}.{1}", fileDetails.UserId, fileExtention);
                fileDetails.UploadedFileName = imageFileNameInFull;
                try
                {
                    // Delete if a file already exists from the same userId
                    await media.Delete(baseUrl + "TENANT_" + fileDetails.TenantId + "/" + Utility.GetEnumDescription(fileDetails.DocumentType)
                                        + "/" + (fileDetails.UserId + ".xls"));
                }
                catch (Exception ex)
                {
                }

                try
                {
                    // Delete if a file already exists from the same userId
                    try
                    {
                        await media.Delete(baseUrl + "TENANT_" + fileDetails.TenantId + "/" + Utility.GetEnumDescription(fileDetails.DocumentType)
                                                 + "/" + (fileDetails.UserId + ".xlsx"));
                    }
                    catch (Exception ex) { }
                }
                catch (Exception)
                {
                    //to do
                }

            }
            else if (fileDetails.DocumentType == DocumentType.Logo)
            {
                var fileNameSplitByDot = originalFileName.Split(new char[1] { '.' });
                string fileExtention = fileNameSplitByDot[fileNameSplitByDot.Length - 1];

                imageFileNameInFull = System.Guid.NewGuid().ToString() + "logo." + fileExtention;
                fileDetails.ClientFileName = originalFileName;
                fileDetails.UploadedFileName = imageFileNameInFull;
            }

            else
            {
                imageFileNameInFull = string.Format("{0}_{1}", System.Guid.NewGuid().ToString(), originalFileName);
                fileDetails.ClientFileName = originalFileName;
                fileDetails.UploadedFileName = imageFileNameInFull;
            }

            media.InitializeStorage(fileDetails.TenantId.ToString(), Utility.GetEnumDescription(fileDetails.DocumentType));
            await media.Upload(stream, imageFileNameInFull);


            if (fileDetails.DocumentType != DocumentType.AddressBook && fileDetails.DocumentType != DocumentType.RateSheet && fileDetails.DocumentType != DocumentType.Logo)
            {
                // Insert document record to DB.
                shipmentManagement.InsertShipmentDocument(fileDetails);

                //Delete the temporary saved file.
                if (File.Exists(uploadedFileInfo.FullName))
                {
                    System.IO.File.Delete(uploadedFileInfo.FullName);
                }
            }

            // Through the request response you can return an object to the Angular controller
            // You will be able to access this in the .success callback through its data attribute
            // If you want to send something to the .error callback, use the HttpStatusCode.BadRequest instead
            var returnData = baseUrl + "TENANT_" + fileDetails.TenantId + "/" + Utility.GetEnumDescription(fileDetails.DocumentType)
                             + "/" + fileDetails.UploadedFileName;


            return this.Request.CreateResponse(HttpStatusCode.OK, new { returnData });
        }


        //[EnableCors(origins: "*", headers: "*", methods: "*")]
        //[System.Web.Http.AcceptVerbs("GET", "POST")]
        //[System.Web.Http.HttpPost]
        //[Route("UploadInvoice")]
        //public async Task<HttpResponseMessage> UploadInvoice()
        //{
        //    if (!Request.Content.IsMimeMultipartContent())
        //    {
        //        this.Request.CreateResponse(HttpStatusCode.UnsupportedMediaType);
        //    }

        //    var provider = GetMultipartProvider();
        //    var result = await Request.Content.ReadAsMultipartAsync(provider);

        //    foreach (var invoice in result.FileData)
        //    {
        //        // On upload, files are given a generic name like "BodyPart_26d6abe1-3ae1-416a-9429-b35f15e6e5d5"
        //        // so this is how you can get the original file name
        //        var originalFileName = GetDeserializedFileName(invoice);

        //        string[] invoiceDetails = (Path.GetFileNameWithoutExtension(originalFileName)).Split('_');

        //        // uploadedFileInfo object will give you some additional stuff like file length,
        //        // creation time, directory name, a few filesystem methods etc..
        //        var uploadedFileInfo = new FileInfo(invoice.LocalFileName);

        //        // Remove this line as well as GetFormData method if you're not
        //        // sending any form data with your upload request
        //        var fileDetails = GetFormData<FileUploadDto>(result);

        //        // Convert to stream            
        //        Stream stream = File.OpenRead(uploadedFileInfo.FullName);

        //        AzureFileManager media = new AzureFileManager();
        //        string imageFileNameInFull = null;
        //        // Make absolute link
        //        string baseUrl = ConfigurationManager.AppSettings["PIBlobStorage"];

        //        var codeshipment = invoiceDetails[0];
        //        var currentShipment = shipmentManagement.GetShipmentByCodeShipment(codeshipment);

        //        if (currentShipment != null)
        //        {
        //            var tenantId = currentShipment.Division.Company.TenantId;

        //            fileDetails.TenantId = tenantId;

        //            imageFileNameInFull = string.Format("{0}_{1}", System.Guid.NewGuid().ToString(), originalFileName);
        //            fileDetails.ClientFileName = originalFileName;
        //            fileDetails.UploadedFileName = imageFileNameInFull;

        //            media.InitializeStorage(fileDetails.TenantId.ToString(), Utility.GetEnumDescription(fileDetails.DocumentType));
        //            await media.Upload(stream, imageFileNameInFull);

        //            //Delete the temporary saved file.
        //            if (File.Exists(uploadedFileInfo.FullName))
        //            {
        //                System.IO.File.Delete(uploadedFileInfo.FullName);
        //            }
        //            // Through the request response you can return an object to the Angular controller
        //            // You will be able to access this in the .success callback through its data attribute
        //            // If you want to send something to the .error callback, use the HttpStatusCode.BadRequest instead
        //            var returnData = baseUrl + "TENANT_" + fileDetails.TenantId + "/" + Utility.GetEnumDescription(fileDetails.DocumentType)
        //                             + "/" + imageFileNameInFull;


        //            InvoiceDto invoiceDetail = new InvoiceDto()
        //            {
        //                ShipmentId = currentShipment.Id,
        //                InvoiceNumber = invoiceDetails[1],
        //                InvoiceValue = decimal.Parse(invoiceDetails[2]),
        //                InvoiceStatus = InvoiceStatus.Pending.ToString(),
        //                CreatedBy = fileDetails.UserId,
        //                URL = returnData
        //            };

        //            if (fileDetails.DocumentType == DocumentType.Invoice)
        //            {
        //                if (!invoiceMangement.SaveInvoiceDetails(invoiceDetail))
        //                {
        //                    return this.Request.CreateResponse(HttpStatusCode.InternalServerError);
        //                }
        //            }
        //            else if (fileDetails.DocumentType == DocumentType.CreditNote)
        //            {
        //                invoiceDetail.Id = long.Parse(fileDetails.CodeReference);

        //                if (!invoiceMangement.SaveCreditNoteDetails(invoiceDetail))
        //                {
        //                    return this.Request.CreateResponse(HttpStatusCode.InternalServerError);
        //                }
        //            }

        //        }

        //    }

        //    return this.Request.CreateResponse(HttpStatusCode.OK);
        //}

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpPost]
        [Route("UploadInvoice")]
        public async Task<HttpResponseMessage> UploadInvoice()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                this.Request.CreateResponse(HttpStatusCode.UnsupportedMediaType);
            }

            var provider = GetMultipartProvider();
            var result = await Request.Content.ReadAsMultipartAsync(provider);

            foreach (var invoice in result.FileData)
            {
                // On upload, files are given a generic name like "BodyPart_26d6abe1-3ae1-416a-9429-b35f15e6e5d5"
                // so this is how you can get the original file name
                var originalFileName = GetDeserializedFileName(invoice);

              //  string[] invoiceDetails = (Path.GetFileNameWithoutExtension(originalFileName)).Split('_');

                // uploadedFileInfo object will give you some additional stuff like file length,
                // creation time, directory name, a few filesystem methods etc..
                var uploadedFileInfo = new FileInfo(invoice.LocalFileName);

                // Remove this line as well as GetFormData method if you're not
                // sending any form data with your upload request
                var fileDetails = GetFormData<FileUploadDto>(result);

                // Convert to stream            
                Stream stream = File.OpenRead(uploadedFileInfo.FullName);

                AzureFileManager media = new AzureFileManager();
                string imageFileNameInFull = null;
                // Make absolute link
                string baseUrl = ConfigurationManager.AppSettings["PIBlobStorage"];

              //  var codeshipment = invoiceDetails[0];
              //  var currentShipment = shipmentManagement.GetShipmentByCodeShipment(codeshipment);

                //if (currentShipment != null)
                //{
                  //  var tenantId = currentShipment.Division.Company.TenantId;

                    //fileDetails.TenantId = tenantId;

                    imageFileNameInFull = string.Format("{0}_{1}", System.Guid.NewGuid().ToString(), originalFileName);
                  //  fileDetails.ClientFileName = originalFileName;
                   // fileDetails.UploadedFileName = imageFileNameInFull;

                    media.InitializeStorage("0", "Invoice_Temp");
                    await media.Upload(stream, imageFileNameInFull);

                    //Delete the temporary saved file.
                    if (File.Exists(uploadedFileInfo.FullName))
                    {
                        System.IO.File.Delete(uploadedFileInfo.FullName);
                    }
                    // Through the request response you can return an object to the Angular controller
                    // You will be able to access this in the .success callback through its data attribute
                    // If you want to send something to the .error callback, use the HttpStatusCode.BadRequest instead
                    var returnData = baseUrl + "TENANT_" + 0 + "/" + "Invoice_Temp"
                                     + "/" + imageFileNameInFull;

                   await invoiceMangement.FetchInvoiceDetailsfromPdf(returnData);

                //}

            }

            return this.Request.CreateResponse(HttpStatusCode.OK);
        }



        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("ManageInvoicePaymentSetting")]
        public IHttpActionResult ManageInvoicePaymentSetting([FromBody] CompanyDto copmany)
        {
            return Ok(adminManagement.ManageInvoicePaymentSetting(copmany.Id));
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet]
        [Route("GetAllInvoices")]
        public IHttpActionResult GetAllInvoices(PagedList pageList)
        {
            return Ok(invoiceMangement.GetAllInvoicesForAdmin(pageList));
            //return Ok(invoiceMangement.GetAllInvoicesForAdmin(status, userId, startDate, endDate, searchValue));
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        [Route("ExportInvoiceReport")]
        public HttpResponseMessage ExportInvoiceReport(string status = null, string userId = null, DateTime? startDate = null, DateTime? endDate = null,
                                                string searchValue = null)
        {
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new ByteArrayContent(invoiceMangement.GetAllInvoicesForAdminExport(status, userId, startDate, endDate, searchValue));
            result.Content.Headers.Add("x-filename", "InvoiceReport.xlsx");
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            return result;
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("UpdateInvoiceStatus")]
        public IHttpActionResult UpdateInvoiceStatus([FromBody] InvoiceDto invoice)
        {
            var status = invoiceMangement.UpdateInvoiceStatus(invoice);
            return Ok(status.ToString());
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        [Route("GetAuditTrailsForCustomer")]
        public IHttpActionResult GetAuditTrailsForCustomer(string userId)
        {
            return Ok(adminManagement.GetAuditTrailsForCustomer(userId));
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        //[Authorize]
        [HttpGet]
        [Route("loadAllShipmentsForAdmin")]
        public IHttpActionResult loadAllShipmentsForAdmin(string status = null, DateTime? startDate = null, DateTime? endDate = null, string searchValue = null, int currentPage = 0, int pageSize = 10)
        {
            return Ok(shipmentManagement.loadAllShipmentsForAdmin(status, startDate, endDate, searchValue, currentPage, pageSize));

        }

        
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        //[Authorize]
        [HttpGet]
        [Route("loadAllShipmentsForAdminExcelExport")]
        public HttpResponseMessage loadAllShipmentsForAdminExcelExport(string status = null, DateTime? startDate = null, DateTime? endDate = null,
                                                                      string number = null, string source = null, string destination = null)
        {
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new ByteArrayContent(shipmentManagement.loadAllShipmentsForAdminExcelExport(status, startDate, endDate, number, source, destination));
            result.Content.Headers.Add("x-filename", "ShipmentDetails.xlsx");
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            return result;
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        //[CustomAuthorize(Roles = "Admin")]
        [HttpPost]
        [Route("GetAllComapnies")]
        public IHttpActionResult GetAllComapnies(PagedList pageList)
        {
            return Ok(companyManagement.GetAllComapnies(pageList));
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpGet]
        [Route("GetAllComapniesForAdminSearch")]
        public IHttpActionResult GetAllComapniesForAdmin(string searchText = null)
        {
            return Ok(companyManagement.GetAllComapniesForAdminSearch(searchText));
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpPost]
        [Route("ChangeCompanyStatus")]
        public IHttpActionResult ChangeCompanyStatus([FromBody] CompanyDto copmany)
        {
            return Ok(companyManagement.ChangeCompanyStatus(copmany.Id));
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        [Route("GetCustomerByCompanyId")]
        public IHttpActionResult GetCustomerByCompanyId(int companyid)
        {
            return Ok(customerManagement.GetCustomerByCompanyId(companyid));
        }

        #region Private methods

        private MultipartFormDataStreamProvider GetMultipartProvider()
        {
            var uploadFolder = "~/App_Data/Tmp/FileUploads"; // you could put this to web.config
            var root = HttpContext.Current.Server.MapPath(uploadFolder);
            Directory.CreateDirectory(root);
            return new MultipartFormDataStreamProvider(root);
        }


        // Extracts Request FormatData as a strongly typed model
        private FileUploadDto GetFormData<T>(MultipartFormDataStreamProvider result)
        {
            FileUploadDto fileUploadDto = new FileUploadDto();

            if (result.FormData.HasKeys())
            {
                fileUploadDto.UserId = Uri.UnescapeDataString(result.FormData.GetValues(0).FirstOrDefault());
                var docType = Uri.UnescapeDataString(result.FormData.GetValues(1).FirstOrDefault());
                fileUploadDto.DocumentType = Utility.GetValueFromDescription<DocumentType>(docType);

                if (fileUploadDto.DocumentType != DocumentType.AddressBook && fileUploadDto.DocumentType != DocumentType.RateSheet && fileUploadDto.DocumentType != DocumentType.Logo)
                {
                    fileUploadDto.CodeReference = Uri.UnescapeDataString(result.FormData.GetValues(2).FirstOrDefault());
                }
            }

            return fileUploadDto;
        }


        private string GetDeserializedFileName(MultipartFileData fileData)
        {
            var fileName = GetFileName(fileData);
            return JsonConvert.DeserializeObject(fileName).ToString();
        }


        private string GetFileName(MultipartFileData fileData)
        {
            return fileData.Headers.ContentDisposition.FileName;
        }

        #endregion

    }
}
