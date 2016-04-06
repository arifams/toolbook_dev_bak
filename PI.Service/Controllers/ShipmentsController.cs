﻿using PI.Contract.DTOs.RateSheets;
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



namespace PI.Service.Controllers
{
    [RoutePrefix("api/shipments")]
    public class ShipmentsController : BaseApiController
    {
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
            string carrier= currentShipment.CarrierInformation.CarrierName;
            string trackingNumber = currentShipment.GeneralInformation.TrackingNumber;
            string codeShipment = currentShipment.GeneralInformation.ShipmentCode;
            string environment = "taleus";
            ShipmentsManagement shipment = new ShipmentsManagement();
            return shipment.GetLocationHistoryInfoForShipment(carrier,trackingNumber,codeShipment,environment);
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
            ShipmentsManagement shipment = new ShipmentsManagement();
            return shipment.SendShipmentDetails(sendShipmentDetails);
        }

        

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        [Route("GetTrackAndTraceInfo")]
        public StatusHistoryResponce GetTrackAndTraceInfo(string career, string trackingNumber)
        {            
            ShipmentsManagement shipment = new ShipmentsManagement();
            return shipment.GetTrackAndTraceInfo(career, trackingNumber);
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        //[Authorize]
        [HttpPost]
        [Route("UploadDocumentsForShipment")]
        public async Task UploadDocumentsForShipment(FileUploadDto fileUpload)
        {
            try
            {
                var provider = GetMultipartProvider();
                var result = await Request.Content.ReadAsMultipartAsync(provider);



                HttpPostedFileBase assignmentFile = fileUpload.Attachment;
                var fileName = fileUpload.Attachment.FileName;

                var imageFileNameInFull = string.Format("{0}_{1}", System.Guid.NewGuid().ToString(), fileName);

                fileUpload.ClientFileName = fileName;
                fileUpload.UploadedFileName = imageFileNameInFull;

                AzureFileManager media = new AzureFileManager();
                media.InitializeStorage(fileUpload.TenantId.ToString(), DocumentType.Shipment.ToString());
                var result1 = await media.Upload(assignmentFile, imageFileNameInFull);
                
                // Insert document record to DB.
                ShipmentsManagement shipmentManagement = new ShipmentsManagement();
                shipmentManagement.InsertShipmentDocument(fileUpload);

            }
            catch (Exception ex)
            {
                //throw;
            }
        }

        // You could extract these two private methods to a separate utility class since
        // they do not really belong to a controller class but that is up to you
        private MultipartFormDataStreamProvider GetMultipartProvider()
        {
            // IMPORTANT: replace "(tilde)" with the real tilde character
            // (our editor doesn't allow it, so I just wrote "(tilde)" instead)
            var uploadFolder = "(tilde)/App_Data/Tmp/FileUploads"; // you could put this to web.config
            var root = HttpContext.Current.Server.MapPath(uploadFolder);
            Directory.CreateDirectory(root);
            return new MultipartFormDataStreamProvider(root);
        }

    }
}
