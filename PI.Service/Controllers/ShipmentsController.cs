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
using System.Net.Http.Headers;
using PI.Contract.DTOs.Report;
using PI.Contract.DTOs.Carrier;
using PI.Contract.DTOs.Dashboard;

namespace PI.Service.Controllers
{
    //[CustomAuthorize]
    //[RequireHttps]
    [RoutePrefix("api/shipments")]
    public class ShipmentsController : BaseApiController
    {

        readonly ICompanyManagement comapnyManagement;
        readonly IShipmentManagement shipmentManagement;
        readonly IAddressBookManagement addressManagement;
        readonly CommonLogic commonLogic;

        public ShipmentsController(ICompanyManagement comapnyManagement, IShipmentManagement shipmentManagement, IAddressBookManagement addressManagement)
        {
            this.comapnyManagement = comapnyManagement;
            this.shipmentManagement = shipmentManagement;
            this.addressManagement = addressManagement;
            commonLogic = new CommonLogic(); // TODO : H - Need to initialize from DI
        }

        public string RequestForQuoteEmail
        {
            get
            {
                return ConfigurationManager.AppSettings["RequestForQuoteEmail"].ToString();
            }
        }

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
            return shipmentManagement.GetLocationHistoryInfoForShipment(carrier, trackingNumber, codeShipment, environment);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        [Route("UpdateShipmentStatusesByJob")]
        public bool UpdateShipmentStatusesByJob()
        {
            string environment = "taleus";
            IList<ShipmentDto> shipments = shipmentManagement.GetAllShipmentsForAdmins();
            foreach (var shipment in shipments)
            {
                shipmentManagement.GetLocationHistoryInfoForShipment(shipment.CarrierInformation.CarrierName, shipment.GeneralInformation.TrackingNumber, shipment.GeneralInformation.ShipmentCode, environment);
            }
            return true;
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("SaveShipment")]
        public ShipmentOperationResult SaveShipment([FromBody]ShipmentDto addShipment)
        {
            return shipmentManagement.SaveShipment(addShipment);
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
            return shipmentManagement.GetHashForPayLane(payLaneDto);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        //[Authorize]
        [HttpGet]
        [Route("GetAllShipments")]
        public PagedList GetAllShipments(string status = null, string userId = null, DateTime? startDate = null, DateTime? endDate = null,
                                         string number = null, string source = null, string destination = null, bool viaDashboard = false)
        {
            return shipmentManagement.GetAllShipmentsbyUser(status, userId, startDate, endDate,
                                                                          number, source, destination, viaDashboard);

        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        //[Authorize]
        [HttpGet]
        [Route("GetAllShipmentByCompanyId")]
        public PagedList GetAllShipmentByCompanyId(string companyId)
        {
            return shipmentManagement.GetAllShipmentByCompanyId(companyId);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        //[Authorize]
        [HttpGet]
        [Route("GetBusinessOwneridbyCompanyId")]
        public string GetBusinessOwneridbyCompanyId(string companyId)
        {
            return comapnyManagement.GetBusinessOwneridbyCompanyId(companyId);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        //[Authorize]
        [HttpGet]
        [Route("loadAllShipmentsFromCompanyAndSearch")]
        public PagedList loadAllShipmentsFromCompanyAndSearch(string companyId, string status = null, DateTime? startDate = null, DateTime? endDate = null,
                                         string number = null, string source = null, string destination = null)
        {
            return shipmentManagement.loadAllShipmentsFromCompanyAndSearch(companyId, status, startDate, endDate, number, source, destination);

        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        //[Authorize]
        [HttpGet]
        [Route("GetAllshipmentsForManifest")]
        public List<ShipmentDto> GetAllshipmentsForManifest(string userId, string createdDate, string carreer, string reference)
        {
            return shipmentManagement.GetAllshipmentsForManifest(userId, createdDate, carreer, reference);
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        //[Authorize]
        [HttpGet]
        [Route("GetAllPendingShipments")]
        public PagedList GetAllPendingShipments(string userId = null, DateTime? startDate = null, DateTime? endDate = null,
                                                string number = null)
        {
            return shipmentManagement.GetAllPendingShipmentsbyUser(userId, startDate, endDate, number);
        }



        [EnableCors(origins: "*", headers: "*", methods: "*")]
        //[Authorize]
        [HttpGet]
        [Route("GetShipmentbyId")]
        public ShipmentDto GetShipmentbyId([FromUri] string shipmentCode, long shipmentId = 0)
        {
            return shipmentManagement.GetshipmentById(shipmentCode, shipmentId);
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
        public ShipmentOperationResult SendShipmentDetails(SendShipmentDetailsDto sendShipmentDetails)
        {
            ShipmentOperationResult operationResult = new ShipmentOperationResult();

            // Make payment and send shipment to SIS.
            operationResult = shipmentManagement.SendShipmentDetails(sendShipmentDetails);

            #region  Add shipment label to azure storage

            AzureFileManager media = new AzureFileManager();
            long tenantId = commonLogic.GetTenantIdByUserId(sendShipmentDetails.UserId);
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


            return operationResult;
        }



        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        [Route("GetTrackAndTraceInfo")]
        public StatusHistoryResponce GetTrackAndTraceInfo(string career, string trackingNumber)
        {
            return shipmentManagement.GetTrackAndTraceInfo(career, trackingNumber);
        }


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
            string imageFileNameInFull = null;
            // Make absolute link
            string baseUrl = ConfigurationManager.AppSettings["PIBlobStorage"];

            var tenantId = commonLogic.GetTenantIdByUserId(fileDetails.UserId);
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
            CommercialInvoiceDto currentshipment = shipmentManagement.GetshipmentByShipmentCodeForInvoice(shipmentCode);
            return currentshipment;
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        //[Authorize]
        [HttpGet]
        [Route("GetshipmentByShipmentCodeForAirwayBill")]
        public AirwayBillDto GetshipmentByShipmentCodeForAirwayBill(string shipmentCode)
        {
            AirwayBillDto currentshipment = shipmentManagement.GetshipmentByShipmentCodeForAirwayBill(shipmentCode);
            return currentshipment;
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("DeleteFile")]
        public void DeleteFile(FileUploadDto fileDetails)
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
                //throw;
            }
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("SaveCommercialInvoice")]
        public ShipmentOperationResult SaveCommercialInvoice([FromBody]CommercialInvoiceDto addShipment)
        {
            return shipmentManagement.SaveCommercialInvoice(addShipment);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("RequestForQuote")]
        public ShipmentOperationResult RequestForQuote(ShipmentDto addShipment)
        {
            string quoteTemplate = shipmentManagement.RequestForQuote(addShipment);
            string requestForQuoteEmail = RequestForQuoteEmail;
            var adminUser = AppUserManager.FindByEmail(requestForQuoteEmail);    //support@parcelinternational.com

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
        // [Authorize]
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
        public List<CarrierDto> LoadAllCarriers()
        {
            return shipmentManagement.LoadAllCarriers();
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        [Route("GetShipmentForCompanyAndSyncWithSIS")]
        public PagedList GetShipmentForCompanyAndSyncWithSIS(long companyId)
        {
            return shipmentManagement.GetShipmentForCompanyAndSyncWithSIS(companyId);
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("ToggleShipmentFavourites")]
        public bool ToggleShipmentFavourites(ShipmentDto shipment)
        {
            return shipmentManagement.ToggleShipmentFavourites(shipment);
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        [Route("GetShipmentStatusCounts")]
        public DashboardShipments GetShipmentStatusCounts(string userId)
        {
            return shipmentManagement.GetShipmentStatusCounts(userId);
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        [Route("SearchShipmentsById")]
        public PagedList SearchShipmentsById(string number)
        {
            return shipmentManagement.SearchShipmentsById(number);
        }

    }

}



