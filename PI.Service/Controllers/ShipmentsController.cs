using PI.Contract.DTOs.RateSheets;
using PI.Contract.DTOs.Shipment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using PI.Business;
using PI.Contract.DTOs.AccountSettings;
using PI.Contract.DTOs.Common;
using System.Web;
using System.Threading.Tasks;
using AzureMediaManager;
using PI.Common;
using PI.Contract.Enums;
using PI.Contract.DTOs.FileUpload;
using System.IO;
using Newtonsoft.Json;
using System.Configuration;
using PI.Contract.DTOs.AddressBook;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Microsoft.AspNet.Identity;
using System.Text;
using PI.Contract.Business;

namespace PI.Service.Controllers
{
    //[CustomAuthorize]
    [RoutePrefix("api/shipments")]
    public class ShipmentsController : BaseApiController
    {

        ICompanyManagement comapnyManagement = new CompanyManagement();
        IShipmentManagement shipmentManagement = new ShipmentsManagement();
        IAddressBookManagement addressManagement = new AddressBookManagement();


        //public ShipmentsController(ICompanyManagement companymanagement, IShipmentManagement shipmentmanagement, IAddressBookManagement addressmanagement)
        //{
        //    this.comapnyManagement = companymanagement;
        //    this.shipmentManagement = shipmentmanagement;
        //    this.addressManagement = addressmanagement;
        //}

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("GetRatesforShipment")]
        public ShipmentcostList GetRatesforShipment([FromBody]ShipmentDto currentShipment)
        {
          //  ShipmentsManagement shipment = new ShipmentsManagement();
            return shipmentManagement.GetRateSheet(currentShipment);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("GetLocationHistoryforShipment")]
        public StatusHistoryResponce GetLocationHistoryforShipment([FromBody]ShipmentDto currentShipment)
        {
            string carrier = currentShipment.CarrierInformation.CarrierName;
            string trackingNumber = currentShipment.GeneralInformation.TrackingNumber;
            string codeShipment = currentShipment.GeneralInformation.ShipmentCode;
            string environment = "taleus";
            ShipmentsManagement shipment = new ShipmentsManagement();
            return shipment.GetLocationHistoryInfoForShipment(carrier, trackingNumber, codeShipment, environment);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("SaveShipment")]
        public ShipmentOperationResult SaveShipment([FromBody]ShipmentDto addShipment)
        {
            //ShipmentsManagement shipment = new ShipmentsManagement();
            return shipmentManagement.SaveShipment(addShipment);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        [Route("GetAllCurrencies")]
        public List<CurrencyDto> GetAllCurrencies()
        {
            ProfileManagement userprofile = new ProfileManagement();
            List<CurrencyDto> currencies = userprofile.GetAllCurrencies();
            return currencies;
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("GetHashForPayLane")]
        public PayLaneDto GetHashForPayLane(PayLaneDto payLaneDto)
        {
          //  ShipmentsManagement shipment = new ShipmentsManagement();
            return shipmentManagement.GetHashForPayLane(payLaneDto);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        //[Authorize]
        [HttpGet]
        [Route("GetAllShipments")]
        public PagedList GetAllShipments(string status = null, string userId = null, DateTime? startDate = null, DateTime? endDate = null,
                                         string number = null, string source = null, string destination = null)
        {
           // ShipmentsManagement shipmentManagement = new ShipmentsManagement();
            var pagedRecord = new PagedList();
            return pagedRecord = shipmentManagement.GetAllShipmentsbyUser(status, userId, startDate, endDate, number, source, destination);

        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        //[Authorize]
        [HttpGet]
        [Route("GetAllShipmentByCompanyId")]
        public PagedList GetAllShipmentByCompanyId(string companyId)
        {
            var pagedRecord = new PagedList();
            return pagedRecord = shipmentManagement.GetAllShipmentByCompanyId(companyId);
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        //[Authorize]
        [HttpGet]
        [Route("loadAllShipmentsFromCompanyAndSearch")]
        public PagedList loadAllShipmentsFromCompanyAndSearch(string companyId, string status = null,DateTime? startDate = null, DateTime? endDate = null,
                                         string number = null, string source = null, string destination = null)
        {
            var pagedRecord = new PagedList();
            return pagedRecord = shipmentManagement.loadAllShipmentsFromCompanyAndSearch(companyId,status, startDate, endDate, number, source, destination);

        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        //[Authorize]
        [HttpGet]
        [Route("GetAllshipmentsForManifest")]
        public List<ShipmentDto> GetAllshipmentsForManifest(string userId, string createdDate, string carreer, string reference)
        {
           // ShipmentsManagement shipmentManagement = new ShipmentsManagement();
            var pagedRecord = new List<ShipmentDto>();
            return pagedRecord = shipmentManagement.GetAllshipmentsForManifest(userId, createdDate, carreer, reference);

        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        //[Authorize]
        [HttpGet]
        [Route("GetAllPendingShipments")]
        public PagedList GetAllPendingShipments(string userId = null, DateTime? startDate = null, DateTime? endDate = null,
                                                string number = null)
        {
           // ShipmentsManagement shipmentManagement = new ShipmentsManagement();
            var pagedRecord = new PagedList();
            return pagedRecord = shipmentManagement.GetAllPendingShipmentsbyUser(userId, startDate, endDate, number);

        }



        [EnableCors(origins: "*", headers: "*", methods: "*")]
        //[Authorize]
        [HttpGet]
        [Route("GetShipmentbyId")]
        public ShipmentDto GetShipmentbyId([FromUri] string shipmentId)
        {
           // ShipmentsManagement shipmentManagement = new ShipmentsManagement();
            ShipmentDto currentshipment = shipmentManagement.GetshipmentById(shipmentId);
            return currentshipment;
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        //[Authorize]
        [HttpGet]
        [Route("DeleteShipment")]
        public int DeleteShipment([FromUri]string shipmentCode, [FromUri]string trackingNumber, [FromUri]string carrierName, bool isAdmin)
        {
           // ShipmentsManagement shipmentManagement = new ShipmentsManagement();
            return shipmentManagement.DeleteShipment(shipmentCode, trackingNumber, carrierName, isAdmin);

        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("SendShipmentDetails")]
        public ShipmentOperationResult SendShipmentDetails(SendShipmentDetailsDto sendShipmentDetails)
        {
            ShipmentOperationResult operationResult = new ShipmentOperationResult();

            // Make payment and send shipment to SIS.
           // ShipmentsManagement shipment = new ShipmentsManagement();
            operationResult = shipmentManagement.SendShipmentDetails(sendShipmentDetails);

            #region  Add shipment label to azure storage

            AzureFileManager media = new AzureFileManager();
            long tenantId = comapnyManagement.GettenantIdByUserId(sendShipmentDetails.UserId);
            media.InitializeStorage(tenantId.ToString(), Utility.GetEnumDescription(DocumentType.ShipmentLabel));
            var result = media.UploadFromFileURL(operationResult.LabelURL, operationResult.ShipmentId.ToString() + ".pdf");

            #endregion


            #region For Email Confirmaion

            StringBuilder emailbody = new StringBuilder("user.TemplateLink");
            //emailbody.Replace("FirstName", user.FirstName).Replace("LastName", user.LastName).Replace("Salutation", user.Salutation + ".")
            //                            .Replace("ActivationURL", "<a href=\"" + callbackUrl + "\">here</a>");

            AppUserManager.SendEmailAsync(sendShipmentDetails.UserId, "Your account has been provisioned!", emailbody.ToString());

            #endregion


            return operationResult;
        }



        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        [Route("GetTrackAndTraceInfo")]
        public StatusHistoryResponce GetTrackAndTraceInfo(string career, string trackingNumber)
        {
            //ShipmentsManagement shipment = new ShipmentsManagement();
            return shipmentManagement.GetTrackAndTraceInfo(career, trackingNumber);
        }


        //[EnableCors(origins: "*", headers: "*", methods: "*")]
        ////[Authorize]
        //[HttpPost]
        //[Route("UploadDocumentsForShipment")]
        //public async Task UploadDocumentsForShipment(FileUploadDto fileUpload)
        //{
        //    try
        //    {
        //        var provider = GetMultipartProvider();
        //        var result = await Request.Content.ReadAsMultipartAsync(provider);



        //        HttpPostedFileBase assignmentFile = fileUpload.Attachment;
        //        var fileName = fileUpload.Attachment.FileName;

        //        var imageFileNameInFull = string.Format("{0}_{1}", System.Guid.NewGuid().ToString(), fileName);

        //        fileUpload.ClientFileName = fileName;
        //        fileUpload.UploadedFileName = imageFileNameInFull;

        //        AzureFileManager media = new AzureFileManager();
        //        media.InitializeStorage(fileUpload.TenantId.ToString(), DocumentType.Shipment.ToString());
        //        var result1 = await media.Upload(assignmentFile, imageFileNameInFull);

        //        // Insert document record to DB.
        //        ShipmentsManagement shipmentManagement = new ShipmentsManagement();
        //        shipmentManagement.InsertShipmentDocument(fileUpload);

        //    }
        //    catch (Exception ex)
        //    {
        //        //throw;
        //    }
        //}
        [HttpPost] // This is from System.Web.Http, and not from System.Web.Mvc
        public async Task<HttpResponseMessage> UploadAddressBook(String userId)
        {
            var responce = await UploadAddressBook();

            var urlJson = await responce.Content.ReadAsStringAsync();

            Result result = null;
            result = JsonConvert.DeserializeObject<Result>(urlJson);

            addressManagement.UpdateAddressBookDatafromExcel(result.returnData, userId);

            return this.Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPost] // This is from System.Web.Http, and not from System.Web.Mvc
        public async Task<HttpResponseMessage> Upload()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                this.Request.CreateResponse(HttpStatusCode.UnsupportedMediaType);
            }

            var provider = GetMultipartProvider();
            var result = await Request.Content.ReadAsMultipartAsync(provider);

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
            //CompanyManagement companyManagement = new CompanyManagement();
            string imageFileNameInFull = null;
            // Make absolute link
            string baseUrl = ConfigurationManager.AppSettings["PIBlobStorage"];

            var tenantId = comapnyManagement.GettenantIdByUserId(fileDetails.UserId);
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
                catch (Exception ex) { }

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
            else
            {
                imageFileNameInFull = string.Format("{0}_{1}", System.Guid.NewGuid().ToString(), originalFileName);
                fileDetails.ClientFileName = originalFileName;
                fileDetails.UploadedFileName = imageFileNameInFull;
            }

            media.InitializeStorage(fileDetails.TenantId.ToString(), Utility.GetEnumDescription(fileDetails.DocumentType));
            var opResult = await media.Upload(stream, imageFileNameInFull);


            if (fileDetails.DocumentType != DocumentType.AddressBook)
            {
                // Insert document record to DB.
               // ShipmentsManagement shipmentManagement = new ShipmentsManagement();
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



        [HttpPost]
        public async Task<HttpResponseMessage> UploadAddressBook()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                this.Request.CreateResponse(HttpStatusCode.UnsupportedMediaType);
            }

            var provider = GetMultipartProvider();
            var result = await Request.Content.ReadAsMultipartAsync(provider);

            string returnData = result.FileData.First().LocalFileName;

            return this.Request.CreateResponse(HttpStatusCode.OK, new { returnData });
        }

        public MultipartFormDataStreamProvider GetMultipartProvider()
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

                if (fileUploadDto.DocumentType != DocumentType.AddressBook)
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

        public string GetFileName(MultipartFileData fileData)
        {
            return fileData.Headers.ContentDisposition.FileName;
        }


        // [EnableCors(origins: "*", headers: "*", methods: "*")]
        ////[Authorize]
        //[HttpPost]
        //[Route("UploadDocumentsForShipment")]
        //public async Task UploadDocumentsForShipment(FileUploadDto fileUpload)
        //{
        //    try
        //    {                               
        //        HttpPostedFileBase assignmentFile = fileUpload.Attachment;
        //        var fileName = fileUpload.Attachment.FileName;
        //        var imageFileNameInFull = string.Format("{0}_{1}", System.Guid.NewGuid().ToString(), fileName);
        //        fileUpload.ClientFileName = fileName;
        //        fileUpload.UploadedFileName = imageFileNameInFull;

        //        AzureFileManager media = new AzureFileManager();
        //        CompanyManagement companyManagement = new CompanyManagement();
        //        var tenantId = companyManagement.GettenantIdByUserId(fileUpload.UserId);
        //        fileUpload.TenantId = tenantId;

        //        media.InitializeStorage(fileUpload.TenantId.ToString(), Utility.GetEnumDescription(DocumentType.Shipment));
        //        var result = await media.Upload(assignmentFile, imageFileNameInFull);

        //        // Insert document record to DB.
        //        ShipmentsManagement shipmentManagement = new ShipmentsManagement();
        //        shipmentManagement.InsertShipmentDocument(fileUpload);
        //    }
        //    catch (Exception ex)
        //    {
        //        //throw;
        //    }
        //}


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        [Route("GetAvailableFilesForShipment")]
        public List<FileUploadDto> GetAvailableFilesForShipment(string shipmentCode, string userId)
        {
            return shipmentManagement.GetAvailableFilesForShipmentbyTenant(shipmentCode, userId);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        //[Authorize]
        [HttpGet]
        [Route("GetshipmentByShipmentCodeForInvoice")]
        public CommercialInvoiceDto GetshipmentByShipmentCodeForInvoice(string shipmentCode)
        {
           // ShipmentsManagement shipmentManagement = new ShipmentsManagement();
            CommercialInvoiceDto currentshipment = shipmentManagement.GetshipmentByShipmentCodeForInvoice(shipmentCode);
            return currentshipment;
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("DeleteFile")]
        public void DeleteFile(FileUploadDto fileDetails)
        {
            try
            {
                //shipmentManagement.(fileDelete.Id);

                AzureFileManager media = new AzureFileManager();
                media.InitializeStorage(fileDetails.TenantId.ToString(), "SHIPMENT_DOCUMENTS");//Utility.GetEnumDescription(fileDetails.DocumentType));
                var result = media.Delete(fileDetails.FileAbsoluteURL);

                shipmentManagement.DeleteFileInDB(fileDetails);

            }
            catch (Exception ex)
            {
                //throw;
            }
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("SaveCommercialInvoice")]
        public ShipmentOperationResult SaveCommercialInvoice([FromBody]CommercialInvoiceDto addShipment)
        {
            //ShipmentsManagement shipment = new ShipmentsManagement();
            return shipmentManagement.SaveCommercialInvoice(addShipment);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("RequestForQuote")]
        public ShipmentOperationResult RequestForQuote(ShipmentDto addShipment)
        {
           // ShipmentsManagement shipment = new ShipmentsManagement();
            string quoteTemplate = shipmentManagement.RequestForQuote(addShipment);
            // TODO: H - Change the staff user.
            var adminUser = AppUserManager.FindByEmail("sriparcel@outlook.com");

            if (adminUser != null && !string.IsNullOrWhiteSpace(quoteTemplate))
            {
                AppUserManager.SendEmail(adminUser.Id, "Request for Quote", quoteTemplate);

                return new ShipmentOperationResult()
                {
                    Status = Status.Success
                };
            }
            else
            {
                return new ShipmentOperationResult()
                {
                    Status = Status.Error
                };
            }

        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        [Route("ShipmentReport")]
        public string ShipmentReport(string userId, string languageId, ReportType reportType, short carrierId = 0, long companyId = 0, DateTime? startDate = null, DateTime? endDate = null)
        {
            shipmentManagement.ShipmentReport(userId, languageId, reportType, carrierId, companyId, startDate, endDate);

            return "";
        }
    }
}



