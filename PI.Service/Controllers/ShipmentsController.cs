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
using Microsoft.AspNet.Identity;
using System.Text;
using PI.Contract.Business;
using System.Net.Http.Headers;
using PI.Contract.DTOs.Carrier;
using PI.Contract.DTOs.Dashboard;
using PI.Contract.DTOs.Payment;

namespace PI.Service.Controllers
{
    [CustomAuthorize]
    [RoutePrefix("api/shipments")]
    public class ShipmentsController : BaseApiController
    {
        readonly ICompanyManagement companyManagement;
        readonly ICustomerManagement customerManagement;
        readonly IShipmentManagement shipmentManagement;
        readonly IAddressBookManagement addressManagement;
        readonly ProfileManagement profileManagement;   // TODO : H - Change to IProfileManagement

        public ShipmentsController(ICompanyManagement companyManagement, IShipmentManagement shipmentManagement, IAddressBookManagement addressManagement, ProfileManagement profileManagement, ICustomerManagement customerManagement)
        {
            this.companyManagement = companyManagement;
            this.shipmentManagement = shipmentManagement;
            this.addressManagement = addressManagement;
            this.profileManagement = profileManagement;
            this.customerManagement = customerManagement;
        }

        public string RequestForQuoteEmail
        {
            get
            {
                return ConfigurationManager.AppSettings["RequestForQuoteEmail"].ToString();
            }
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("GetRatesforShipment")]
        public IHttpActionResult GetRatesforShipment([FromBody]ShipmentDto currentShipment)
        {
            return Ok(shipmentManagement.GetRateSheet(currentShipment));
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("GetLocationHistoryforShipment")]
        public IHttpActionResult GetLocationHistoryforShipment([FromBody]ShipmentDto currentShipment)
        {
            string carrier = currentShipment.CarrierInformation.CarrierName;
            string trackingNumber = currentShipment.GeneralInformation.TrackingNumber;
            string codeShipment = currentShipment.GeneralInformation.ShipmentCode;
            string environment = "taleus";

            return Ok(shipmentManagement.GetLocationHistoryInfoForShipment(carrier, trackingNumber, 
                                                                           codeShipment, environment));
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        [Route("UpdateShipmentStatusesByJob")]
        public IHttpActionResult UpdateShipmentStatusesByJob()
        {
            string environment = "taleus";
            IList<ShipmentDto> shipments = shipmentManagement.GetAllShipmentsForAdmins();
            foreach (var shipment in shipments)
            {
                shipmentManagement.GetLocationHistoryInfoForShipment(shipment.CarrierInformation.CarrierName, shipment.GeneralInformation.TrackingNumber, shipment.GeneralInformation.ShipmentCode, environment);
            }

            return Ok(true);
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("SaveShipment")]
        public IHttpActionResult SaveShipment([FromBody]ShipmentDto addShipment)
        {
            return Ok(shipmentManagement.SaveShipment(addShipment));
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("UpdateshipmentStatusManually")]
        public int UpdateshipmentStatusManually([FromBody]ShipmentDto addShipment)
        {
            return shipmentManagement.UpdateshipmentStatusManually(addShipment.GeneralInformation.ShipmentCode, addShipment.GeneralInformation.Status);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        [Route("GetAllCurrencies")]
        public IHttpActionResult GetAllCurrencies()
        {
            ProfileManagement userprofile = new ProfileManagement();

            return Ok(userprofile.GetAllCurrencies());
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("GetHashForPayLane")]
        public IHttpActionResult GetHashForPayLane(PayLaneDto payLaneDto)
        {
            return Ok(shipmentManagement.GetHashForPayLane(payLaneDto));
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        //[Authorize]
        [HttpGet]
        [Route("GetAllShipments")]
        public IHttpActionResult GetAllShipments(string status = null, string userId = null, DateTime? startDate = null, DateTime? endDate = null,
                                         string number = null, string source = null, string destination = null, bool viaDashboard = false)
        {
            return Ok(shipmentManagement.GetAllShipmentsbyUser(status, userId, startDate, endDate,
                                                               number, source, destination, viaDashboard));

        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        //[Authorize]
        [HttpGet]
        [Route("GetAllShipmentByCompanyId")]
        public IHttpActionResult GetAllShipmentByCompanyId(string companyId)
        {
            return Ok(shipmentManagement.GetAllShipmentByCompanyId(companyId));
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        //[Authorize]
        [HttpGet]
        [Route("GetBusinessOwneridbyCompanyId")]
        public IHttpActionResult GetBusinessOwneridbyCompanyId(string companyId)
        {
            return Ok(companyManagement.GetBusinessOwneridbyCompanyId(companyId));
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        //[Authorize]
        [HttpGet]
        [Route("loadAllShipmentsFromCompanyAndSearch")]
        public IHttpActionResult loadAllShipmentsFromCompanyAndSearch(string companyId, string status = null, DateTime? startDate = null, DateTime? endDate = null,
                                         string number = null, string source = null, string destination = null)
        {
            return Ok(shipmentManagement.loadAllShipmentsFromCompanyAndSearch(companyId, status, startDate, 
                                                                              endDate, number, source, destination));

        }



        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        [Route("GetFilteredShipmentsExcel")]
        public HttpResponseMessage GetFilteredShipmentsExcel(string status = null, string userId = null, DateTime? startDate = null, DateTime? endDate = null,
                                         string number = null, string source = null, string destination = null, bool viaDashboard = false)
        {
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new ByteArrayContent(shipmentManagement.loadAllShipmentsForExcel(status,  userId ,  startDate, endDate,
                                          number ,  source, destination, viaDashboard));
            result.Content.Headers.Add("x-filename", "ShipmentDetails.xlsx");
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            return result;


        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        //[Authorize]
        [HttpGet]
        [Route("GetAllshipmentsForManifest")]
        public IHttpActionResult GetAllshipmentsForManifest(string userId, string createdDate, string carreer, string reference)
        {
            return Ok(shipmentManagement.GetAllshipmentsForManifest(userId, createdDate, carreer, reference));
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        //[Authorize]
        [HttpGet]
        [Route("GetAllPendingShipments")]
        public IHttpActionResult GetAllPendingShipments(string userId = null, DateTime? startDate = null, DateTime? endDate = null,
                                                string number = null)
        {
            return Ok(shipmentManagement.GetAllPendingShipmentsbyUser(userId, startDate, endDate, number));
        }

        
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        //[Authorize]
        [HttpGet]
        [Route("GetShipmentbyId")]
        public IHttpActionResult GetShipmentbyId([FromUri] string shipmentCode, long shipmentId = 0)
        {
            return Ok(shipmentManagement.GetshipmentById(shipmentCode, shipmentId));
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        //[Authorize]
        [HttpGet]
        [Route("DeleteShipment")]
        public int DeleteShipment([FromUri]string shipmentCode, [FromUri]string trackingNumber, [FromUri]string carrierName, bool isAdmin, [FromUri]long shipmentId)
        {
            return shipmentManagement.DeleteShipment(shipmentCode, trackingNumber, carrierName, isAdmin, shipmentId);
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("SendShipmentDetails")]
        public IHttpActionResult SendShipmentDetails(SendShipmentDetailsDto sendShipmentDetails)
        {
            ShipmentOperationResult operationResult = new ShipmentOperationResult();

            // Make payment and send shipment to SIS.
            operationResult = shipmentManagement.SendShipmentDetails(sendShipmentDetails);

            #region  Add shipment label to azure storage

            AzureFileManager media = new AzureFileManager();
            long tenantId = companyManagement.GetTenantIdByUserId(sendShipmentDetails.UserId);
            media.InitializeStorage(tenantId.ToString(), Utility.GetEnumDescription(DocumentType.ShipmentLabel));
            var result = media.UploadFromFileURL(operationResult.LabelURL, operationResult.ShipmentId.ToString() + ".pdf");

            #endregion


            #region Send Booking Confirmaion Email to customer.

            if (operationResult.Status == Status.Success)
            {
                StringBuilder emailbody = new StringBuilder(sendShipmentDetails.TemplateLink);

                emailbody
                    .Replace("<OrderReference>", operationResult.ShipmentDto.GeneralInformation.ShipmentName)
                    .Replace("<PickupDate>", operationResult.ShipmentDto.CarrierInformation.PickupDate != null ? Convert.ToDateTime(operationResult.ShipmentDto.CarrierInformation.PickupDate).ToShortDateString() : string.Empty)
                    .Replace("<ShipmentMode>", operationResult.ShipmentDto.GeneralInformation.shipmentModeName)
                    .Replace("<ShipmentType>", operationResult.ShipmentDto.GeneralInformation.ShipmentServices)
                    .Replace("<Carrier>", operationResult.ShipmentDto.CarrierInformation.CarrierName)
                    .Replace("<ShipmentPrice>", operationResult.ShipmentDto.PackageDetails.ValueCurrencyString + " " + operationResult.ShipmentDto.CarrierInformation.Price.ToString())
                    .Replace("<PaymentType>", operationResult.ShipmentDto.GeneralInformation.ShipmentPaymentTypeName);

                StringBuilder productList = new StringBuilder();
                decimal totalVol = 0;

                foreach (var product in operationResult.ShipmentDto.PackageDetails.ProductIngredients)
                {
                    productList.Append("<tr>");

                    productList.Append("<td style='width:290px;text-align:center;color:#fff'>");
                    productList.Append(product.ProductType);
                    productList.Append("</td>");

                    productList.Append("<td style='width:290px;text-align:center;color:#fff;'>");
                    productList.Append(product.Quantity);
                    productList.Append("</td>");

                    productList.Append("<td style='width:290px;text-align:center;color:#fff;'>");
                    productList.Append(product.Weight.ToString("n2"));
                    productList.Append("</td>");

                    totalVol = product.Length * product.Width * product.Height * product.Quantity;
                    productList.Append("<td style='width:290px;text-align:center;color:#fff;'>");
                    productList.Append(totalVol.ToString("n2"));
                    productList.Append("</td>");

                    productList.Append("</tr>");
                }

                emailbody
                    .Replace("<tableRecords>", productList.ToString());

                AppUserManager.SendEmail(sendShipmentDetails.UserId, "Order Confirmation", emailbody.ToString());
            }

            #endregion


            return Ok(operationResult);
        }



        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        [Route("GetTrackAndTraceInfo")]
        public IHttpActionResult GetTrackAndTraceInfo(string career, string trackingNumber)
        {
            return Ok(shipmentManagement.GetTrackAndTraceInfo(career, trackingNumber));
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [Route("UploadAddressBook")]
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

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [Route("Upload")]
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
                    catch (Exception ex)
                    {
                    }
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

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("UploadAddressBook")]
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

       
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        [Route("GetAvailableFilesForShipment")]
        public IHttpActionResult GetAvailableFilesForShipment(string shipmentCode, string userId)
        {
            return Ok(shipmentManagement.GetAvailableFilesForShipmentbyTenant(shipmentCode, userId));
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        [Route("GetshipmentByShipmentCodeForInvoice")]
        public IHttpActionResult GetshipmentByShipmentCodeForInvoice(string shipmentCode)
        {
            return Ok(shipmentManagement.GetshipmentByShipmentCodeForInvoice(shipmentCode));
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        [Route("GetshipmentByShipmentCodeForAirwayBill")]
        public IHttpActionResult GetshipmentByShipmentCodeForAirwayBill(string shipmentCode)
        {
            return Ok(shipmentManagement.GetshipmentByShipmentCodeForAirwayBill(shipmentCode));
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("DeleteFile")]
        public IHttpActionResult DeleteFile(FileUploadDto fileDetails)
        {
            try
            {
                AzureFileManager media = new AzureFileManager();
                media.InitializeStorage(fileDetails.TenantId.ToString(), "SHIPMENT_DOCUMENTS");//Utility.GetEnumDescription(fileDetails.DocumentType));
                var result = media.Delete(fileDetails.FileAbsoluteURL);

                shipmentManagement.DeleteFileInDB(fileDetails);

            }
            catch (Exception ex)
            {
                return BadRequest();
            }

            return Ok();
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("SaveCommercialInvoice")]
        public IHttpActionResult SaveCommercialInvoice([FromBody]CommercialInvoiceDto addShipment)
        {
            return Ok(shipmentManagement.SaveCommercialInvoice(addShipment));
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("RequestForQuote")]
        public IHttpActionResult RequestForQuote(ShipmentDto addShipment)
        {
            string quoteTemplate = shipmentManagement.RequestForQuote(addShipment);
            string requestForQuoteEmail = RequestForQuoteEmail;
            var adminUser = AppUserManager.FindByEmail(requestForQuoteEmail);    //support@parcelinternational.com
            ShipmentOperationResult oResult = new ShipmentOperationResult();

            if (adminUser != null && !string.IsNullOrWhiteSpace(quoteTemplate))
            {
                AppUserManager.SendEmail(adminUser.Id, "Request for Quote", quoteTemplate);

                oResult.Status = Status.Success;               
            }
            else
            {
                oResult.Status = Status.Error;
            }

            return Ok(oResult);
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        [Route("GetShipmentDetails")]
        public HttpResponseMessage GetShipmentDetails(string userId, short carrierId = 0, long companyId = 0, DateTime? startDate = null,
                                                      DateTime? endDate = null, short status = 0, string countryOfOrigin = null, string countryOfDestination = null, short product = 0, short packageType = 0)
        {
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new ByteArrayContent(shipmentManagement.ShipmentReportForExcel(userId, carrierId,
                                                                                            companyId, startDate, endDate, status, countryOfOrigin, countryOfDestination, product, packageType));
            result.Content.Headers.Add("x-filename", "ShipmentDetailsReport.xlsx");
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            return result;
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        [Route("LoadAllCarriers")]
        public IHttpActionResult LoadAllCarriers()
        {
            return Ok(shipmentManagement.LoadAllCarriers());
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        [Route("GetShipmentForCompanyAndSyncWithSIS")]
        public IHttpActionResult GetShipmentForCompanyAndSyncWithSIS(long companyId)
        {
            return Ok(shipmentManagement.GetShipmentForCompanyAndSyncWithSIS(companyId));
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("ToggleShipmentFavourites")]
        public IHttpActionResult ToggleShipmentFavourites(ShipmentDto shipment)
        {
            return Ok(shipmentManagement.ToggleShipmentFavourites(shipment));
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        [Route("GetShipmentStatusCounts")]
        public IHttpActionResult GetShipmentStatusCounts(string userId)
        {
            return Ok(shipmentManagement.GetShipmentStatusCounts(userId));
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        [Route("SearchShipmentsById")]
        public IHttpActionResult SearchShipmentsById(string number)
        {
            return Ok(shipmentManagement.SearchShipmentsById(number));
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("PaymentCharge")]
        public IHttpActionResult PaymentCharge(PaymentDto payment)
        {
            return Ok(shipmentManagement.PaymentCharge(payment));
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


        private string GetFileName(MultipartFileData fileData)
        {
            return fileData.Headers.ContentDisposition.FileName;
        }

        #endregion

    }

}



