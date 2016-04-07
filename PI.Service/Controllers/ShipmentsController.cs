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

namespace PI.Service.Controllers
{
    [RoutePrefix("api/shipments")]
    public class ShipmentsController : BaseApiController
    {
        CompanyManagement comapnyManagement = new CompanyManagement();

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("GetRatesforShipment")]
        public ShipmentcostList GetRatesforShipment([FromBody]ShipmentDto currentShipment)
        {
            ShipmentsManagement shipment = new ShipmentsManagement();
            return shipment.GetRateSheet(currentShipment);
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
            ShipmentsManagement shipment = new ShipmentsManagement();
            return shipment.SaveShipment(addShipment);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        [Route("GetAllCurrencies")]
        public IQueryable<CurrencyDto> GetAllCurrencies()
        {
            ProfileManagement userprofile = new ProfileManagement();
            IQueryable<CurrencyDto> currencies = userprofile.GetAllCurrencies();
            return currencies;
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("GetHashForPayLane")]
        public PayLaneDto GetHashForPayLane(PayLaneDto payLaneDto)
        {
            ShipmentsManagement shipment = new ShipmentsManagement();
            return shipment.GetHashForPayLane(payLaneDto);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        //[Authorize]
        [HttpGet]
        [Route("GetAllShipments")]
        public PagedList GetAllShipments(string status = null, string userId = null, DateTime? startDate = null, DateTime? endDate = null,
                                         string number = null, string source = null, string destination = null)
        {
            ShipmentsManagement shipmentManagement = new ShipmentsManagement();
            var pagedRecord = new PagedList();
            return pagedRecord = shipmentManagement.GetAllShipmentsbyUser(status, userId, startDate, endDate, number, source, destination);

        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        //[Authorize]
        [HttpGet]
        [Route("GetAllPendingShipments")]
        public PagedList GetAllPendingShipments(string userId = null, DateTime? startDate = null, DateTime? endDate = null,
                                                string number = null)
        {
            ShipmentsManagement shipmentManagement = new ShipmentsManagement();
            var pagedRecord = new PagedList();
            return pagedRecord = shipmentManagement.GetAllPendingShipmentsbyUser(userId, startDate, endDate, number);

        }



        [EnableCors(origins: "*", headers: "*", methods: "*")]
        //[Authorize]
        [HttpGet]
        [Route("GetShipmentbyId")]
        public ShipmentDto GetShipmentbyId([FromUri] string shipmentId)
        {
            ShipmentsManagement shipmentManagement = new ShipmentsManagement();
            ShipmentDto currentshipment = shipmentManagement.GetshipmentById(shipmentId);
            return currentshipment;
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        //[Authorize]
        [HttpGet]
        [Route("DeleteShipment")]
        public int DeleteShipment(string shipmentCode, string trackingNumber, string carrierName)
        {
            ShipmentsManagement shipmentManagement = new ShipmentsManagement();
            return shipmentManagement.DeleteShipment(shipmentCode, trackingNumber, carrierName);

        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("SendShipmentDetails")]
        public ShipmentOperationResult SendShipmentDetails(SendShipmentDetailsDto sendShipmentDetails)
        {
            ShipmentOperationResult operationResult = new ShipmentOperationResult();

            // Make payment and send shipment to SIS.
            ShipmentsManagement shipment = new ShipmentsManagement();
            operationResult = shipment.SendShipmentDetails(sendShipmentDetails);

            // Add shipment label to azure storage.
            AzureFileManager media = new AzureFileManager();
            long tenantId = comapnyManagement.GettenantIdByUserId(sendShipmentDetails.UserId);
            media.InitializeStorage(tenantId.ToString(), Utility.GetEnumDescription(DocumentType.ShipmentLabel));

            var result = media.UploadFromFileURL(operationResult.LabelURL, operationResult.ShipmentId.ToString() + ".pdf");

            return operationResult;
        }



        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        [Route("GetTrackAndTraceInfo")]
        public StatusHistoryResponce GetTrackAndTraceInfo(string career, string trackingNumber)
        {
            ShipmentsManagement shipment = new ShipmentsManagement();
            return shipment.GetTrackAndTraceInfo(career, trackingNumber);
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
            CompanyManagement companyManagement = new CompanyManagement();

            var tenantId = companyManagement.GettenantIdByUserId(fileDetails.UserId);
            fileDetails.TenantId = tenantId;

            var imageFileNameInFull = string.Format("{0}_{1}", System.Guid.NewGuid().ToString(), originalFileName);
            fileDetails.ClientFileName = originalFileName;
            fileDetails.UploadedFileName = imageFileNameInFull;

            media.InitializeStorage(fileDetails.TenantId.ToString(), Utility.GetEnumDescription(DocumentType.Shipment));
            var opResult = await media.Upload(stream, imageFileNameInFull);


            // Insert document record to DB.
            ShipmentsManagement shipmentManagement = new ShipmentsManagement();
            shipmentManagement.InsertShipmentDocument(fileDetails);

            // Through the request response you can return an object to the Angular controller
            // You will be able to access this in the .success callback through its data attribute
            // If you want to send something to the .error callback, use the HttpStatusCode.BadRequest instead
            var returnData = "ReturnTest";
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
                fileUploadDto.ReferenceId = long.Parse(Uri.UnescapeDataString(result.FormData.GetValues(0).FirstOrDefault()));
                fileUploadDto.UserId = Uri.UnescapeDataString(result.FormData.GetValues(1).FirstOrDefault());
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

        public List<FileUploadDto> GetAvailableFilesForShipment(int shipmentId, string userId)
        {
            ShipmentsManagement shipmentManagement = new ShipmentsManagement();
            return shipmentManagement.GetAvailableFilesForShipmentbyTenant(shipmentId, userId);
        }

    }
}



