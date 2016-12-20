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
using Newtonsoft.Json.Linq;
using PI.Data.Entity;
using iTextSharp.text;
using iTextSharp.text.pdf;
using PI.Contract.TemplateLoader;
using HtmlAgilityPack;
using PI.Contract.DTOs.Invoice;
using System.Drawing;
using PI.Contract.DTOs.User;
using PI.Data.Entity.Identity;

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
        readonly IInvoiceMangement invoiceManagement;
        readonly ProfileManagement profileManagement;   // TODO : H - Change to IProfileManagement

        public ShipmentsController(ICompanyManagement companyManagement, IShipmentManagement shipmentManagement, IAddressBookManagement addressManagement, ProfileManagement profileManagement, ICustomerManagement customerManagement, IInvoiceMangement invoiceManagement)
        {
            this.companyManagement = companyManagement;
            this.shipmentManagement = shipmentManagement;
            this.addressManagement = addressManagement;
            this.profileManagement = profileManagement;
            this.customerManagement = customerManagement;
            this.invoiceManagement = invoiceManagement;
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

            return Ok(shipmentManagement.GetLocationHistoryInfoForShipment(carrier, trackingNumber, codeShipment, environment));

        }


        [AllowAnonymous]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("GetAllShipmentsForWebJob")]
        public IHttpActionResult GetAllShipmentsForWebJob([FromBody] UserDto userDetails)
        {
            string roleName = null;
            if (string.IsNullOrEmpty(userDetails.UserName) && string.IsNullOrEmpty(userDetails.Password))
            {
                return Unauthorized();
            }

            var user = AppUserManager.Find(userDetails.UserName, userDetails.Password);

            if (user == null)
            {
                return Unauthorized();
            }

            roleName = companyManagement.GetRoleName(user.Roles.FirstOrDefault().RoleId);

            if (roleName != "Admin")
            {
                var response = Request.CreateResponse(HttpStatusCode.Forbidden);
                return (IHttpActionResult)response;
            }

            var allShipmentList = shipmentManagement.GetAllShipmentsForAdmins();

            return Ok(allShipmentList);

        }


        [AllowAnonymous]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("UpdateAllShipmentsFromWebJob")]
        public IHttpActionResult UpdateAllShipmentsFromWebJob([FromBody] ShipmentDto shipment)

        {
            var userDetails = shipment.InvokingUserDetails;
            string roleName = null;

            if (string.IsNullOrEmpty(userDetails.UserName) && string.IsNullOrEmpty(userDetails.Password))
            {
                return Unauthorized();
            }

            var user = AppUserManager.Find(userDetails.UserName, userDetails.Password);

            if (user == null)
            {
                return Unauthorized();
            }

            roleName = companyManagement.GetRoleName(user.Roles.FirstOrDefault().RoleId);

            if (roleName != "Admin")
            {
                var response = Request.CreateResponse(HttpStatusCode.Forbidden);
                return (IHttpActionResult)response;
            }

            //var allShipmentList = shipmentManagement.GetAllShipmentsForAdmins();

            //foreach (var shipment in allShipmentList)
            //{
            string carrier = shipment.CarrierInformation.CarrierName;
            string trackingNumber = shipment.GeneralInformation.TrackingNumber;
            string codeShipment = shipment.GeneralInformation.ShipmentCode;
            string environment = "taleus";

            // update all shipment details
            var shipmentTracking = shipmentManagement.GetLocationHistoryInfoForShipment(carrier, trackingNumber, codeShipment, environment);

            if (shipmentTracking.info.status == Utility.GetEnumDescription(ShipmentStatus.Exception))
            {
                var profile = profileManagement.getProfileByUserName(shipment.GeneralInformation.CreatedBy);

                var notifications = profileManagement.GetNotificationCriteriaByCustomerId(profile.CustomerDetails.Id);

                if (notifications.ShipmentException == true)
                {
                    string htmlTemplate = "";
                    TemplateLoader templateLoader = new TemplateLoader();

                    //get the email template for invoice
                    HtmlDocument template = templateLoader.getHtmlTemplatebyName("exceptionEmail");
                    htmlTemplate = template.DocumentNode.InnerHtml;

                    //replace strings in Html                       

                    var updatedString = htmlTemplate.Replace("{firstname}", profile.CustomerDetails.FirstName).Replace("{lastname}", profile.CustomerDetails.LastName).Replace("{shipmentCode}", shipment.GeneralInformation.ShipmentCode).Replace("{shipmentreference}", shipment.GeneralInformation.ShipmentReferenceName).Replace("\r", "").Replace("\n", "");

                    ApplicationUser existingUser = AppUserManager.FindByName(profile.CustomerDetails.Email);

                    //sending email
                    AppUserManager.SendEmail(existingUser.Id, "Exception occured in shipment - " + trackingNumber, updatedString);
                }

            }

            return Ok();

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

        [AllowAnonymous]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("HandleSISRequest")]
        public IHttpActionResult HandleSISRequest([FromBody]SISShipmentCreateDto shipmentInfo)
        {
            return Ok(shipmentManagement.HandleSISRequest(shipmentInfo.AddShipmentXml, shipmentInfo.ShipmentReference));

        }



        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("UpdateShipmentReference")]
        public IHttpActionResult UpdateShipmentReference([FromBody]ShipmentDto addShipment)
        {
            return Ok(shipmentManagement.UpdateShipmentReference(addShipment));
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("UpdateshipmentStatusManually")]
        public int UpdateshipmentStatusManually([FromBody]ShipmentDto addShipment)
        {
            bool result = shipmentManagement.UpdateshipmentStatusManually(addShipment);

            if (result == false)
                return 0;

            // Send email
            if (addShipment.GeneralInformation.Status == Utility.GetEnumDescription(ShipmentStatus.Exception))
            {
                NotificationCriteria notifications = new NotificationCriteria();
                var profile = profileManagement.getProfileByUserName(addShipment.GeneralInformation.CreatedBy);

                notifications = profileManagement.GetNotificationCriteriaByCustomerId(profile.CustomerDetails.Id);

                if (notifications != null && notifications.ShipmentException == true)
                {
                    string htmlTemplate = "";
                    TemplateLoader templateLoader = new TemplateLoader();

                    //get the email template for invoice
                    HtmlDocument template = templateLoader.getHtmlTemplatebyName("exceptionEmail");
                    htmlTemplate = template.DocumentNode.InnerHtml;

                    //replace strings in Html                       

                    var updatedString = htmlTemplate.Replace("{firstname}", profile.CustomerDetails.FirstName).Replace("{lastname}", profile.CustomerDetails.LastName).Replace("{shipmentCode}", addShipment.GeneralInformation.ShipmentCode).Replace("{shipmentReference}", addShipment.GeneralInformation.ShipmentName).Replace("\r", "").Replace("\n", "");

                    ApplicationUser existingUser = AppUserManager.FindByName(profile.CustomerDetails.Email);

                    //sending email
                    AppUserManager.SendEmail(existingUser.Id, "Exception occured in shipment - " + addShipment.GeneralInformation.TrackingNumber, updatedString);

                }

            }
            return 1;
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
        public IHttpActionResult GetSquareApplicationId()
        {
            return Ok(shipmentManagement.GetSquareApplicationId());
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        //[Authorize]
        [HttpPost]
        [Route("GetAllShipments")]
        public IHttpActionResult GetAllShipments([FromBody] PagedList shipmentSerach)
        {

            shipmentSerach.DynamicContent = shipmentSerach.filterContent;
            return Ok(shipmentManagement.GetAllShipmentsbyUser(shipmentSerach));

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
        [HttpPost]
        [Route("GetFilteredShipmentsExcel")]
        public HttpResponseMessage GetFilteredShipmentsExcel([FromBody] PagedList shipmentSerach)
        {
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            shipmentSerach.DynamicContent = shipmentSerach.filterContent;
            shipmentSerach.PageSize = 0;    // this tells to load all records.

            result.Content = new ByteArrayContent(shipmentManagement.loadAllShipmentsForExcel(shipmentSerach));
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
        [HttpPost]
        [Route("GetAllPendingShipments")]
        public IHttpActionResult GetAllPendingShipments(PagedList pageList)
        {
            return Ok(shipmentManagement.GetAllPendingShipmentsbyUser(pageList));
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

            //#region Send Booking Confirmaion Email to customer.

            //if (operationResult.Status == Status.Success)
            //{
            //    StringBuilder emailbody = new StringBuilder(sendShipmentDetails.TemplateLink);

            //    emailbody
            //        .Replace("<OrderReference>", operationResult.ShipmentDto.GeneralInformation.ShipmentName)
            //        .Replace("<PickupDate>", operationResult.ShipmentDto.CarrierInformation.PickupDate != null ? Convert.ToDateTime(operationResult.ShipmentDto.CarrierInformation.PickupDate).ToShortDateString() : string.Empty)
            //        .Replace("<ShipmentMode>", operationResult.ShipmentDto.GeneralInformation.shipmentModeName)
            //        .Replace("<ShipmentType>", operationResult.ShipmentDto.GeneralInformation.ShipmentServices)
            //        .Replace("<Carrier>", operationResult.ShipmentDto.CarrierInformation.CarrierName)
            //        .Replace("<ShipmentPrice>", operationResult.ShipmentDto.PackageDetails.ValueCurrencyString + " " + operationResult.ShipmentDto.CarrierInformation.Price.ToString())
            //        .Replace("<PaymentType>", operationResult.ShipmentDto.GeneralInformation.ShipmentPaymentTypeName);

            //    StringBuilder productList = new StringBuilder();
            //    decimal totalVol = 0;

            //    foreach (var product in operationResult.ShipmentDto.PackageDetails.ProductIngredients)
            //    {
            //        productList.Append("<tr>");

            //        productList.Append("<td style='width:290px;text-align:center;color:#fff'>");
            //        productList.Append(product.ProductType);
            //        productList.Append("</td>");

            //        productList.Append("<td style='width:290px;text-align:center;color:#fff;'>");
            //        productList.Append(product.Quantity);
            //        productList.Append("</td>");

            //        productList.Append("<td style='width:290px;text-align:center;color:#fff;'>");
            //        productList.Append(product.Weight.ToString("n2"));
            //        productList.Append("</td>");

            //        totalVol = product.Length * product.Width * product.Height * product.Quantity;
            //        productList.Append("<td style='width:290px;text-align:center;color:#fff;'>");
            //        productList.Append(totalVol.ToString("n2"));
            //        productList.Append("</td>");

            //        productList.Append("</tr>");
            //    }

            //    emailbody
            //        .Replace("<tableRecords>", productList.ToString());

            //    AppUserManager.SendEmail(sendShipmentDetails.UserId, "Order Confirmation", emailbody.ToString());
            //}

            //#endregion

            return Ok(operationResult);
        }




        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("CheckTheBookingConfirmation")]
        public IHttpActionResult CheckTheBookingConfirmation(SendShipmentDetailsDto sendShipmentDetails)
        {
            ShipmentOperationResult operationResult = new ShipmentOperationResult();

            //Checking the shipment Status wether it is sucess or not
            operationResult = shipmentManagement.CheckTheShipmentStatusToViewLabel(sendShipmentDetails);

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
        public IHttpActionResult GetShipmentStatusCounts(string userId = null)
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
        [Route("RefundCharge")]
        public IHttpActionResult RefundCharge(PaymentDto payment)
        {
            return Ok(shipmentManagement.RefundCharge(payment.ShipmentId));
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        public async void ShipmentAddResponse(ShipmentOperationResult result)
        {
            ShipmentDto shipmentDetails = shipmentManagement.GetshipmentById("", result.ShipmentId);

            if (result.Status == Status.Success && shipmentDetails.GeneralInformation.ShipmentPaymentTypeId == 2)
            {
                // This is online payment.
                try
                {
                    // call invoice generate               
                    string baseUrl = ConfigurationManager.AppSettings["PIBlobStorage"];

                    PaymentDto paymentDetails = shipmentManagement.GetPaymentbyReference(Convert.ToInt16(shipmentDetails.GeneralInformation.ShipmentId));

                    Random generator = new Random();
                    string code = generator.Next(1000000, 9999999).ToString("D7");
                    string invoiceNumber = "PI_" + DateTime.UtcNow.Year.ToString() + "_" + code;

                    //initializing azure storage
                    AzureFileManager media = new AzureFileManager();
                    var tenantId = shipmentManagement.GetTenantIdByUserId(shipmentDetails.GeneralInformation.CreatedUser);

                    var invoicePdf = new Document(PageSize.B5);
                    //getting the server path to create temp pdf file
                    var uploadFolder = "~/App_Data/Tmp/FileUploads/invoice.pdf";
                    string wanted_path = System.Web.HttpContext.Current.Server.MapPath(uploadFolder);
                    // string wanted_path = System.Web.HttpContext.Current.Server.MapPath("\\Pdf\\invoice.pdf");

                    PdfWriter writer = PdfWriter.GetInstance(invoicePdf, new FileStream(wanted_path, FileMode.Create));


                    Uri imageUrl = new Uri("http://www.12send.com/template/logo_12send.png");
                    iTextSharp.text.Image LOGO = iTextSharp.text.Image.GetInstance(imageUrl);
                    LOGO.ScalePercent(25f);
                    invoicePdf.Open();


                    float[] Addresscolumns = { 25, 15, 45, 15 };
                    PdfPTable addressTable = new PdfPTable(4);
                    addressTable.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    addressTable.SetWidthPercentage(Addresscolumns, new iTextSharp.text.Rectangle(100, 200));

                    PdfPCell logoCell = new PdfPCell(LOGO);
                    logoCell.Border = 0;


                    PdfPCell addressCell = new PdfPCell();
                    addressCell.Border = 0;

                    Paragraph line1 = new Paragraph("Parcel Intenational");
                    Paragraph line2 = new Paragraph("1900 Addison Street");
                    Paragraph line3 = new Paragraph("STE. 200");
                    Paragraph line4 = new Paragraph("CA US");
                    Paragraph line5 = new Paragraph("(510) 281-7554");
                    Paragraph line6 = new Paragraph("support@parcelinternational.com");
                    Paragraph line7 = new Paragraph("www.parcelinternational.com");

                    addressCell.AddElement(line1);
                    addressCell.AddElement(line2);
                    addressCell.AddElement(line3);
                    addressCell.AddElement(line4);
                    addressCell.AddElement(line5);
                    addressCell.AddElement(line6);
                    addressCell.AddElement(line7);

                    PdfPCell emptyCell = new PdfPCell();
                    emptyCell.Border = 0;

                    addressTable.AddCell(logoCell);
                    addressTable.AddCell(emptyCell);
                    addressTable.AddCell(new PdfPCell(addressCell));
                    addressTable.AddCell(emptyCell);

                    invoicePdf.Add(addressTable);

                    float[] Billingcolumns = { 40, 10, 10, 40 };
                    PdfPTable billingTable = new PdfPTable(4);
                    billingTable.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    billingTable.SetWidthPercentage(Billingcolumns, new iTextSharp.text.Rectangle(100, 200));

                    iTextSharp.text.Font invoiceFont = new iTextSharp.text.Font();
                    invoiceFont.Size = 10;

                    //add billing address and invoice details
                    Paragraph billingline1 = new Paragraph("BILL TO", invoiceFont);
                    Paragraph billingline5 = new Paragraph(shipmentDetails.AddressInformation.Consigner.FirstName + " " + shipmentDetails.AddressInformation.Consigner.LastName, invoiceFont);
                    Paragraph billingline2 = new Paragraph(shipmentDetails.AddressInformation.Consigner.Address1, invoiceFont);
                    Paragraph billingline3 = new Paragraph(shipmentDetails.AddressInformation.Consigner.Address2, invoiceFont);
                    Paragraph billingline4 = new Paragraph(shipmentDetails.AddressInformation.Consigner.City + "," + shipmentDetails.AddressInformation.Consigner.State + "," + shipmentDetails.AddressInformation.Consigner.Postalcode, invoiceFont);
                    Paragraph billingline6 = new Paragraph(shipmentDetails.AddressInformation.Consigner.Country, invoiceFont);

                    DateTime localDateTimeofUser = shipmentManagement.GetLocalTimeByUser(shipmentDetails.GeneralInformation.CreatedUser, DateTime.UtcNow).Value;
                    Paragraph billingDetailsline1 = new Paragraph("INVOICE # " + invoiceNumber, invoiceFont);
                    Paragraph billingDetailsline2 = new Paragraph("DATE  " + localDateTimeofUser.ToString("dd/MM/yyyy"), invoiceFont);
                    Paragraph billingDetailsline3 = new Paragraph("DUE DATE  " + localDateTimeofUser.AddDays(10).ToString("dd/MM/yyyy"), invoiceFont);
                    Paragraph billingDetailsline4 = new Paragraph("TERMS  " + "Net 10", invoiceFont);

                    PdfPCell billingaddressCell = new PdfPCell();
                    billingaddressCell.Border = 0;
                    billingaddressCell.AddElement(billingline1);
                    billingaddressCell.AddElement(billingline2);
                    billingaddressCell.AddElement(billingline3);
                    billingaddressCell.AddElement(billingline4);
                    billingaddressCell.AddElement(billingline5);
                    billingaddressCell.AddElement(billingline6);

                    PdfPCell billingDetailsCell = new PdfPCell();
                    billingDetailsCell.Border = 0;
                    billingDetailsCell.AddElement(billingDetailsline1);
                    billingDetailsCell.AddElement(billingDetailsline2);
                    billingDetailsCell.AddElement(billingDetailsline3);
                    billingDetailsCell.AddElement(billingDetailsline4);

                    billingTable.AddCell(billingaddressCell);
                    billingTable.AddCell(emptyCell);
                    billingTable.AddCell(emptyCell);
                    billingTable.AddCell(billingDetailsCell);



                    BaseFont bfTimes = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
                    iTextSharp.text.Font times = new iTextSharp.text.Font(bfTimes, 14, iTextSharp.text.Font.BOLD, BaseColor.BLUE);

                    Paragraph title = new Paragraph("INVOICE", times);
                    title.Alignment = Element.ALIGN_LEFT;
                    invoicePdf.Add(title);
                    invoicePdf.Add(billingTable);

                    invoicePdf.Add(new Phrase());
                    invoicePdf.Add(new Paragraph("  "));
                    invoicePdf.Add(new Paragraph("  "));


                    float[] Shipmentcolumns = { 50, 10, 20, 20 };
                    PdfPTable shipmentTable = new PdfPTable(4);
                    shipmentTable.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    shipmentTable.SetWidthPercentage(Shipmentcolumns, new iTextSharp.text.Rectangle(100, 200));

                    Paragraph activity = new Paragraph("ACTIVITY");
                    Paragraph qty = new Paragraph("QTY");
                    Paragraph rate = new Paragraph("RATE");
                    Paragraph amount = new Paragraph("AMOUNT");

                    PdfPCell activityCell = new PdfPCell(activity);
                    PdfPCell qtyCell = new PdfPCell(qty);
                    PdfPCell rateCell = new PdfPCell(rate);
                    PdfPCell amountCell = new PdfPCell(amount);

                    //add table column headings
                    shipmentTable.AddCell(activityCell).BackgroundColor = BaseColor.LIGHT_GRAY;
                    shipmentTable.AddCell(qtyCell).BackgroundColor = BaseColor.LIGHT_GRAY; ;
                    shipmentTable.AddCell(rateCell).BackgroundColor = BaseColor.LIGHT_GRAY; ;
                    shipmentTable.AddCell(amountCell).BackgroundColor = BaseColor.LIGHT_GRAY; ;



                    PdfPCell shipmentDetailsCell = new PdfPCell();
                    shipmentDetailsCell.Border = 0;

                    Paragraph shipline1 = new Paragraph(shipmentDetails.CarrierInformation.CarrierName, invoiceFont);
                    Paragraph shipline2 = new Paragraph("AWB#: " + shipmentDetails.GeneralInformation.TrackingNumber, invoiceFont);
                    Paragraph shipline3 = new Paragraph("Reference: " + shipmentDetails.GeneralInformation.ShipmentName + "/" + shipmentDetails.GeneralInformation.ShipmentId, invoiceFont);
                    Paragraph shipline4 = new Paragraph("Origin: " + shipmentDetails.AddressInformation.Consigner.City + " " + shipmentDetails.AddressInformation.Consigner.Country, invoiceFont);
                    Paragraph shipline5 = new Paragraph("Destination: " + shipmentDetails.AddressInformation.Consignee.City + " " + shipmentDetails.AddressInformation.Consignee.Country, invoiceFont);
                    Paragraph shipline6 = new Paragraph("Weight: " + shipmentDetails.PackageDetails.TotalWeight, invoiceFont);
                    Paragraph shipline7 = new Paragraph("Date: " + shipmentDetails.GeneralInformation.CreatedDate, invoiceFont);

                    shipmentDetailsCell.AddElement(shipline1);
                    shipmentDetailsCell.AddElement(shipline2);
                    shipmentDetailsCell.AddElement(shipline3);
                    shipmentDetailsCell.AddElement(shipline4);
                    shipmentDetailsCell.AddElement(shipline5);
                    shipmentDetailsCell.AddElement(shipline6);
                    shipmentDetailsCell.AddElement(shipline7);
                    //htmlWorker.StartDocument();
                    //htmlWorker.Parse(txtReader);

                    //packageDetails.Append("<td>" + shipmentDetails.PackageDetails.Count + "</td>");
                    //packageDetails.Append("<td>$" + Convert.ToDecimal(paymentDetails.Amount)/100 + "</td>");

                    Paragraph countPara = new Paragraph(shipmentDetails.PackageDetails.Count.ToString(), invoiceFont);
                    Paragraph ratePara = new Paragraph((Convert.ToDecimal(shipmentDetails.PackageDetails.CarrierCost)).ToString(), invoiceFont);
                    Paragraph amountPara = new Paragraph((Convert.ToDecimal(shipmentDetails.PackageDetails.CarrierCost)).ToString(), invoiceFont);
                    Paragraph balancePara = new Paragraph(((Convert.ToDecimal(shipmentDetails.PackageDetails.CarrierCost)) - (Convert.ToDecimal(paymentDetails.Amount) / 100)).ToString(), invoiceFont);

                    Paragraph paymentLabelPara = new Paragraph("PAYMENT");
                    Paragraph balanceLabelPara = new Paragraph("BALANCE DUE");

                    PdfPCell countCell = new PdfPCell(countPara);
                    countCell.Border = 0;

                    PdfPCell ratesCell = new PdfPCell(ratePara);
                    ratesCell.Border = 0;

                    PdfPCell amountsCell = new PdfPCell(amountPara);
                    amountsCell.Border = 0;

                    shipmentTable.AddCell(shipmentDetailsCell);

                    shipmentTable.AddCell(countCell);
                    shipmentTable.AddCell(ratesCell);
                    shipmentTable.AddCell(amountsCell);

                    PdfPCell paymentLabelCell = new PdfPCell(paymentLabelPara);
                    paymentLabelCell.Border = 0;
                    PdfPCell amountLabelCell = new PdfPCell(amountPara);
                    amountLabelCell.Border = 0;

                    PdfPCell balanceLabelCell = new PdfPCell(balanceLabelPara);
                    balanceLabelCell.Border = 0;
                    PdfPCell balanceCell = new PdfPCell(balancePara);
                    balanceCell.Border = 0;


                    shipmentTable.AddCell(emptyCell);
                    shipmentTable.AddCell(emptyCell);
                    shipmentTable.AddCell(paymentLabelCell);
                    shipmentTable.AddCell(amountLabelCell);

                    shipmentTable.AddCell(emptyCell);
                    shipmentTable.AddCell(emptyCell);
                    shipmentTable.AddCell(balanceLabelCell);
                    shipmentTable.AddCell(balanceCell);


                    invoicePdf.Add(shipmentTable);
                    BaseFont bfSmall = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
                    iTextSharp.text.Font small = new iTextSharp.text.Font(bfTimes, 8, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK);

                    Paragraph accountno = new Paragraph("Bank Account Number: 1660838259", small);
                    accountno.Alignment = Element.ALIGN_LEFT;

                    Paragraph wire = new Paragraph("Wire Transfers", small);
                    wire.Alignment = Element.ALIGN_LEFT;

                    Paragraph domestic = new Paragraph("Domestic Wires: 121000248", small);
                    domestic.Alignment = Element.ALIGN_LEFT;

                    Paragraph internatwire = new Paragraph("International wires: WFBIUS6S", small);
                    internatwire.Alignment = Element.ALIGN_LEFT;

                    Paragraph topay = new Paragraph("To pay this invoice online : click the link in this email or send us a", small);
                    topay.Alignment = Element.ALIGN_LEFT;

                    Paragraph paypal = new Paragraph("request to use Paypal.", small);
                    paypal.Alignment = Element.ALIGN_LEFT;

                    Paragraph checks = new Paragraph("Checks can be mailed to our office address", small);
                    checks.Alignment = Element.ALIGN_LEFT;

                    invoicePdf.Add(accountno);
                    invoicePdf.Add(wire);
                    invoicePdf.Add(domestic);
                    invoicePdf.Add(internatwire);
                    invoicePdf.Add(topay);
                    invoicePdf.Add(paypal);
                    invoicePdf.Add(checks);

                    //htmlWorker.EndDocument();
                    //htmlWorker.Close();
                    //closing the doc
                    invoicePdf.Close();


                    var invoicename = "";
                    using (Stream savedPdf = File.OpenRead(wanted_path))
                    {
                        invoicename = string.Format("{0}_{1}", System.Guid.NewGuid().ToString(), invoiceNumber + ".pdf");

                        media.InitializeStorage(tenantId.ToString(), Utility.GetEnumDescription(DocumentType.Invoice));
                        // var opResult = media.Upload(savedPdf, invoicename);
                        await media.Upload(savedPdf, invoicename);
                    }


                    //get the saved pdf url
                    var returnData = baseUrl + "TENANT_" + tenantId + "/" + Utility.GetEnumDescription(DocumentType.Invoice)
                                            + "/" + invoicename;

                    //saving Invoice details
                    InvoiceDto invoice = new InvoiceDto()
                    {

                        URL = returnData,
                        InvoiceNumber = invoiceNumber,
                        ShipmentId = Convert.ToInt16(shipmentDetails.GeneralInformation.ShipmentId),
                        CreatedBy = shipmentDetails.GeneralInformation.CreatedUser,
                        UserId = shipmentDetails.GeneralInformation.CreatedBy,
                        DueDate = DateTime.UtcNow.AddDays(10).ToString("MM/dd/yyyy"),
                        InvoiceValue = Convert.ToDecimal(paymentDetails.Amount) / 100,
                        InvoiceStatus = InvoiceStatus.Paid.ToString(),
                        InvoiceDate = DateTime.UtcNow.ToString()
                    };

                    var saveResult = invoiceManagement.SaveInvoiceDetails(invoice);

                    // Send mail

                    #region Send Booking Confirmaion Email to customer.

                    string htmlTemplate = "";
                    TemplateLoader templateLoader = new TemplateLoader();
                    //get the email template for invoice
                    HtmlDocument template = templateLoader.getHtmlTemplatebyName("OrderConfirmEmail");
                    htmlTemplate = template.DocumentNode.InnerHtml;

                    StringBuilder emailbody = new StringBuilder(htmlTemplate);

                    emailbody
                        .Replace("<OrderReference>", shipmentDetails.GeneralInformation.ShipmentName)
                        .Replace("<PickupDate>", shipmentDetails.CarrierInformation.PickupDate != null ? Convert.ToDateTime(shipmentDetails.CarrierInformation.PickupDate).ToShortDateString() : string.Empty)
                        .Replace("<ShipmentMode>", shipmentDetails.GeneralInformation.shipmentModeName)
                        .Replace("<ShipmentType>", shipmentDetails.GeneralInformation.ShipmentServices)
                        .Replace("<Carrier>", shipmentDetails.CarrierInformation.CarrierName)
                        .Replace("<ShipmentPrice>", shipmentDetails.PackageDetails.ValueCurrencyString + " " + shipmentDetails.CarrierInformation.Price.ToString())
                        .Replace("<PaymentType>", shipmentDetails.GeneralInformation.ShipmentPaymentTypeName);

                    StringBuilder productList = new StringBuilder();
                    decimal totalVol = 0;

                    foreach (var product in shipmentDetails.PackageDetails.ProductIngredients)
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

                    AppUserManager.SendEmail(shipmentDetails.UserId, "Order Confirmation", emailbody.ToString());

                    #endregion
                }
                catch (Exception ex)
                {

                }
            }
            else if (result.Status == Status.SISError && (shipmentDetails.GeneralInformation.ShipmentPaymentTypeId == 2))
            {
                shipmentManagement.RefundCharge(result.ShipmentId);
            }
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        public IHttpActionResult UpdateTrackingNo(AirwayBillDto awbDto)
        {
            return Ok(shipmentManagement.UpdateTrackingNo(awbDto));
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        [Route("GetShipmentResult")]
        public IHttpActionResult GetShipmentResult(long shipmentId)
        {
            return Ok(shipmentManagement.GetShipmentResult(shipmentId));
        }

        #region Temp solution

        /// <summary>
        /// Handle database save and payment
        /// </summary>
        /// <param name="addShipment"></param>
        /// <returns></returns>
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        public IHttpActionResult SaveShipmentV1([FromBody]ShipmentDto addShipment)
        {
            return Ok(shipmentManagement.SaveShipmentV1(addShipment));
        }

        /// <summary>
        /// Handle shipment send to SIS, generate invoice and send mail
        /// </summary>
        /// <param name="sendShipmentDetails"></param>
        /// <returns></returns>
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        public async Task<IHttpActionResult> SendShipmentDetailsV1(SendShipmentDetailsDto sendShipmentDetails)
        {
            List<ShipmentOperationResult> results = shipmentManagement.SendShipmentDetailsV1(sendShipmentDetails);

            var result = results.FirstOrDefault();

            if (result.Status != Status.Success)
            {
                return Ok(results);
            }

            // Below code will execute only if shipment is success.

            result.InvoiceURL = "";

            #region Generate Invoice and save Invoice entity.

            if (result.ShipmentDto.GeneralInformation.ShipmentPaymentTypeId == 2)
            {
                // call invoice generate               
                string baseUrl = ConfigurationManager.AppSettings["PIBlobStorage"];

                PaymentDto paymentDetails = shipmentManagement.GetPaymentbyReference(Convert.ToInt16(result.ShipmentDto.GeneralInformation.ShipmentId));

                Random generator = new Random();
                string code = generator.Next(1000000, 9999999).ToString("D7");
                string invoiceNumber = "PI_" + DateTime.UtcNow.Year.ToString() + "_" + code;

                //initializing azure storage
                AzureFileManager media = new AzureFileManager();
                var tenantId = shipmentManagement.GetTenantIdByUserId(result.ShipmentDto.GeneralInformation.CreatedUser);

                var invoicePdf = new Document(PageSize.B5);
                //getting the server path to create temp pdf file
                var uploadFolder = "~/App_Data/Tmp/FileUploads/invoice.pdf";
                var logoImageUrl = "~/App_Data/Tmp/FileUploads/12SendLogo-lg.png";
                string wanted_path = System.Web.HttpContext.Current.Server.MapPath(uploadFolder);
                string wanted_logo_path = System.Web.HttpContext.Current.Server.MapPath(logoImageUrl);
                // string wanted_path = System.Web.HttpContext.Current.Server.MapPath("\\Pdf\\invoice.pdf");
                PdfWriter writer = PdfWriter.GetInstance(invoicePdf, new FileStream(wanted_path, FileMode.Create));
                
                //Uri imageUrl = new Uri("http://www.12send.com/template/logo_12send.png");
                iTextSharp.text.Image LOGO = iTextSharp.text.Image.GetInstance(wanted_logo_path);
                LOGO.ScalePercent(25f);
                invoicePdf.Open();


                float[] Addresscolumns = { 25, 15, 45, 15 };
                PdfPTable addressTable = new PdfPTable(4);
                addressTable.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                addressTable.SetWidthPercentage(Addresscolumns, new iTextSharp.text.Rectangle(100, 200));

                PdfPCell logoCell = new PdfPCell(LOGO);
                logoCell.Border = 0;


                PdfPCell addressCell = new PdfPCell();
                addressCell.Border = 0;

                Paragraph line1 = new Paragraph("Parcel Intenational");
                Paragraph line2 = new Paragraph("1900 Addison Street");
                Paragraph line3 = new Paragraph("STE. 200");
                Paragraph line4 = new Paragraph("CA US");
                Paragraph line5 = new Paragraph("(510) 281-7554");
                Paragraph line6 = new Paragraph("support@parcelinternational.com");
                Paragraph line7 = new Paragraph("www.parcelinternational.com");

                addressCell.AddElement(line1);
                addressCell.AddElement(line2);
                addressCell.AddElement(line3);
                addressCell.AddElement(line4);
                addressCell.AddElement(line5);
                addressCell.AddElement(line6);
                addressCell.AddElement(line7);

                PdfPCell emptyCell = new PdfPCell();
                emptyCell.Border = 0;

                addressTable.AddCell(logoCell);
                addressTable.AddCell(emptyCell);
                addressTable.AddCell(new PdfPCell(addressCell));
                addressTable.AddCell(emptyCell);

                invoicePdf.Add(addressTable);

                float[] Billingcolumns = { 40, 10, 10, 40 };
                PdfPTable billingTable = new PdfPTable(4);
                billingTable.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                billingTable.SetWidthPercentage(Billingcolumns, new iTextSharp.text.Rectangle(100, 200));

                iTextSharp.text.Font invoiceFont = new iTextSharp.text.Font();
                invoiceFont.Size = 10;

                //add billing address and invoice details
                Paragraph billingline1 = new Paragraph("BILL TO", invoiceFont);

                Contract.DTOs.Address.AddressDto addressDto = shipmentManagement.GetBillingAddressByUserId(result.ShipmentDto.GeneralInformation.CreatedUser);

                Paragraph billingline5 = new Paragraph(result.ShipmentDto.AddressInformation.Consigner.FirstName + " " + result.ShipmentDto.AddressInformation.Consigner.LastName, invoiceFont);
                //Paragraph billingline2 = new Paragraph(result.ShipmentDto.AddressInformation.Consigner.Address1, invoiceFont);
                //Paragraph billingline3 = new Paragraph(result.ShipmentDto.AddressInformation.Consigner.Address2, invoiceFont);
                //Paragraph billingline4 = new Paragraph(result.ShipmentDto.AddressInformation.Consigner.City + "," + result.ShipmentDto.AddressInformation.Consigner.State + "," + result.ShipmentDto.AddressInformation.Consigner.Postalcode, invoiceFont);
                //Paragraph billingline6 = new Paragraph(result.ShipmentDto.AddressInformation.Consigner.Country, invoiceFont);
                Paragraph billingline2 = new Paragraph(addressDto.StreetAddress1, invoiceFont);
                Paragraph billingline3 = new Paragraph(addressDto.StreetAddress2, invoiceFont);
                Paragraph billingline4 = new Paragraph(addressDto.City + "," + addressDto.State + "," + addressDto.ZipCode, invoiceFont);
                Paragraph billingline6 = new Paragraph(addressDto.Country, invoiceFont);

                DateTime localDateTimeofUser = shipmentManagement.GetLocalTimeByUser(result.ShipmentDto.GeneralInformation.CreatedUser, DateTime.UtcNow).Value;
                Paragraph billingDetailsline1 = new Paragraph("INVOICE # " + invoiceNumber, invoiceFont);
                Paragraph billingDetailsline2 = new Paragraph("DATE  " + localDateTimeofUser.ToString("dd/MM/yyyy"), invoiceFont);
                Paragraph billingDetailsline3 = new Paragraph("DUE DATE  " + localDateTimeofUser.AddDays(10).ToString("dd/MM/yyyy"), invoiceFont);
                Paragraph billingDetailsline4 = new Paragraph("TERMS  " + "Net 10", invoiceFont);

                PdfPCell billingaddressCell = new PdfPCell();
                billingaddressCell.Border = 0;
                billingaddressCell.AddElement(billingline1);
                billingaddressCell.AddElement(billingline2);
                billingaddressCell.AddElement(billingline3);
                billingaddressCell.AddElement(billingline4);
                billingaddressCell.AddElement(billingline5);
                billingaddressCell.AddElement(billingline6);

                PdfPCell billingDetailsCell = new PdfPCell();
                billingDetailsCell.Border = 0;
                billingDetailsCell.AddElement(billingDetailsline1);
                billingDetailsCell.AddElement(billingDetailsline2);
                billingDetailsCell.AddElement(billingDetailsline3);
                billingDetailsCell.AddElement(billingDetailsline4);

                billingTable.AddCell(billingaddressCell);
                billingTable.AddCell(emptyCell);
                billingTable.AddCell(emptyCell);
                billingTable.AddCell(billingDetailsCell);



                BaseFont bfTimes = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
                iTextSharp.text.Font times = new iTextSharp.text.Font(bfTimes, 14, iTextSharp.text.Font.BOLD, BaseColor.BLUE);

                Paragraph title = new Paragraph("INVOICE", times);
                title.Alignment = Element.ALIGN_LEFT;
                invoicePdf.Add(title);
                invoicePdf.Add(billingTable);

                invoicePdf.Add(new Phrase());
                invoicePdf.Add(new Paragraph("  "));
                invoicePdf.Add(new Paragraph("  "));


                float[] Shipmentcolumns = { 50, 10, 20, 20 };
                PdfPTable shipmentTable = new PdfPTable(4);
                shipmentTable.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                shipmentTable.SetWidthPercentage(Shipmentcolumns, new iTextSharp.text.Rectangle(100, 200));

                Paragraph activity = new Paragraph("ACTIVITY");
                Paragraph qty = new Paragraph("QTY");
                Paragraph rate = new Paragraph("RATE");
                Paragraph amount = new Paragraph("AMOUNT");

                PdfPCell activityCell = new PdfPCell(activity);
                PdfPCell qtyCell = new PdfPCell(qty);
                PdfPCell rateCell = new PdfPCell(rate);
                PdfPCell amountCell = new PdfPCell(amount);

                //add table column headings
                shipmentTable.AddCell(activityCell).BackgroundColor = BaseColor.LIGHT_GRAY;
                shipmentTable.AddCell(qtyCell).BackgroundColor = BaseColor.LIGHT_GRAY; ;
                shipmentTable.AddCell(rateCell).BackgroundColor = BaseColor.LIGHT_GRAY; ;
                shipmentTable.AddCell(amountCell).BackgroundColor = BaseColor.LIGHT_GRAY; ;



                PdfPCell shipmentDetailsCell = new PdfPCell();
                shipmentDetailsCell.Border = 0;

                Paragraph shipline1 = new Paragraph(result.ShipmentDto.CarrierInformation.CarrierName, invoiceFont);

                StringBuilder TrackingNumbers = new StringBuilder();
                foreach (var item in results)
                {
                    TrackingNumbers.Append(item.ShipmentDto.GeneralInformation.TrackingNumber + ",");
                }

                Paragraph shipline2 = new Paragraph("AWB#: " + TrackingNumbers.ToString(), invoiceFont);
                Paragraph shipline3 = new Paragraph("Reference: " + result.ShipmentDto.GeneralInformation.ShipmentName + "/" + result.ShipmentDto.GeneralInformation.ShipmentId, invoiceFont);
                Paragraph shipline4 = new Paragraph("Origin: " + result.ShipmentDto.AddressInformation.Consigner.City + " " + result.ShipmentDto.AddressInformation.Consigner.Country, invoiceFont);
                Paragraph shipline5 = new Paragraph("Destination: " + result.ShipmentDto.AddressInformation.Consignee.City + " " + result.ShipmentDto.AddressInformation.Consignee.Country, invoiceFont);
                Paragraph shipline6 = new Paragraph("Weight: " + result.ShipmentDto.PackageDetails.TotalWeight, invoiceFont);
                Paragraph shipline7 = new Paragraph("Date: " + result.ShipmentDto.GeneralInformation.CreatedDate, invoiceFont);

                shipmentDetailsCell.AddElement(shipline1);
                shipmentDetailsCell.AddElement(shipline2);
                shipmentDetailsCell.AddElement(shipline3);
                shipmentDetailsCell.AddElement(shipline4);
                shipmentDetailsCell.AddElement(shipline5);
                shipmentDetailsCell.AddElement(shipline6);
                shipmentDetailsCell.AddElement(shipline7);
                //htmlWorker.StartDocument();
                //htmlWorker.Parse(txtReader);

                //packageDetails.Append("<td>" + shipmentDetails.PackageDetails.Count + "</td>");
                //packageDetails.Append("<td>$" + Convert.ToDecimal(paymentDetails.Amount)/100 + "</td>");

                Paragraph countPara = new Paragraph(result.ShipmentDto.PackageDetails.Count.ToString(), invoiceFont);
                Paragraph ratePara = new Paragraph((Convert.ToDecimal(result.ShipmentDto.PackageDetails.CarrierCost)).ToString(), invoiceFont);
                Paragraph amountPara = new Paragraph((Convert.ToDecimal(result.ShipmentDto.PackageDetails.CarrierCost)).ToString(), invoiceFont);
                Paragraph balancePara = new Paragraph(((Convert.ToDecimal(result.ShipmentDto.PackageDetails.CarrierCost)) - (Convert.ToDecimal(paymentDetails.Amount))).ToString(), invoiceFont);

                Paragraph paymentLabelPara = new Paragraph("PAYMENT");
                Paragraph balanceLabelPara = new Paragraph("BALANCE DUE");

                PdfPCell countCell = new PdfPCell(countPara);
                countCell.Border = 0;

                PdfPCell ratesCell = new PdfPCell(ratePara);
                ratesCell.Border = 0;

                PdfPCell amountsCell = new PdfPCell(amountPara);
                amountsCell.Border = 0;

                shipmentTable.AddCell(shipmentDetailsCell);

                shipmentTable.AddCell(countCell);
                shipmentTable.AddCell(ratesCell);
                shipmentTable.AddCell(amountsCell);

                PdfPCell paymentLabelCell = new PdfPCell(paymentLabelPara);
                paymentLabelCell.Border = 0;
                PdfPCell amountLabelCell = new PdfPCell(amountPara);
                amountLabelCell.Border = 0;

                PdfPCell balanceLabelCell = new PdfPCell(balanceLabelPara);
                balanceLabelCell.Border = 0;
                PdfPCell balanceCell = new PdfPCell(balancePara);
                balanceCell.Border = 0;


                shipmentTable.AddCell(emptyCell);
                shipmentTable.AddCell(emptyCell);
                shipmentTable.AddCell(paymentLabelCell);
                shipmentTable.AddCell(amountLabelCell);

                shipmentTable.AddCell(emptyCell);
                shipmentTable.AddCell(emptyCell);
                shipmentTable.AddCell(balanceLabelCell);
                shipmentTable.AddCell(balanceCell);


                invoicePdf.Add(shipmentTable);
                BaseFont bfSmall = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
                iTextSharp.text.Font small = new iTextSharp.text.Font(bfTimes, 8, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK);

                Paragraph accountno = new Paragraph("Bank Account Number: 1660838259", small);
                accountno.Alignment = Element.ALIGN_LEFT;

                Paragraph wire = new Paragraph("Wire Transfers", small);
                wire.Alignment = Element.ALIGN_LEFT;

                Paragraph domestic = new Paragraph("Domestic Wires: 121000248", small);
                domestic.Alignment = Element.ALIGN_LEFT;

                Paragraph internatwire = new Paragraph("International wires: WFBIUS6S", small);
                internatwire.Alignment = Element.ALIGN_LEFT;

                Paragraph topay = new Paragraph("To pay this invoice online : click the link in this email or send us a", small);
                topay.Alignment = Element.ALIGN_LEFT;

                Paragraph paypal = new Paragraph("request to use Paypal.", small);
                paypal.Alignment = Element.ALIGN_LEFT;

                Paragraph checks = new Paragraph("Checks can be mailed to our office address", small);
                checks.Alignment = Element.ALIGN_LEFT;

                invoicePdf.Add(accountno);
                invoicePdf.Add(wire);
                invoicePdf.Add(domestic);
                invoicePdf.Add(internatwire);
                invoicePdf.Add(topay);
                invoicePdf.Add(paypal);
                invoicePdf.Add(checks);

                //htmlWorker.EndDocument();
                //htmlWorker.Close();
                //closing the doc
                invoicePdf.Close();


                var invoicename = "";
                using (Stream savedPdf = File.OpenRead(wanted_path))
                {
                    invoicename = string.Format("{0}_{1}", System.Guid.NewGuid().ToString(), invoiceNumber + ".pdf");

                    media.InitializeStorage(tenantId.ToString(), Utility.GetEnumDescription(DocumentType.Invoice));
                    // var opResult = media.Upload(savedPdf, invoicename);
                    await media.Upload(savedPdf, invoicename);
                }


                //get the saved pdf url
                result.InvoiceURL = baseUrl + "TENANT_" + tenantId + "/" + Utility.GetEnumDescription(DocumentType.Invoice)
                                        + "/" + invoicename;

                //saving Invoice details
                InvoiceDto invoice = new InvoiceDto()
                {

                    URL = result.InvoiceURL,
                    InvoiceNumber = invoiceNumber,
                    ShipmentId = Convert.ToInt16(result.ShipmentDto.GeneralInformation.ShipmentId),
                    CreatedBy = result.ShipmentDto.GeneralInformation.CreatedUser,
                    UserId = result.ShipmentDto.GeneralInformation.CreatedBy,
                    DueDate = DateTime.UtcNow.AddDays(10).ToString("MM/dd/yyyy"),
                    InvoiceValue = Convert.ToDecimal(paymentDetails.Amount) / 100,
                    InvoiceStatus = InvoiceStatus.Paid.ToString(),
                    InvoiceDate = DateTime.UtcNow.ToString()
                };

                var saveResult = invoiceManagement.SaveInvoiceDetails(invoice);
            }

            #endregion

            #region Send Booking Confirmaion Email to customer.

            string htmlTemplate = "";
            TemplateLoader templateLoader = new TemplateLoader();
            //get the email template for invoice
            HtmlDocument template = templateLoader.getHtmlTemplatebyName("OrderConfirmEmail");
            htmlTemplate = template.DocumentNode.InnerHtml;

            StringBuilder emailbody = new StringBuilder(htmlTemplate);

            emailbody
                .Replace("{OrderReference}", result.ShipmentDto.GeneralInformation.ShipmentName)
                .Replace("{PickupDate}", result.ShipmentDto.CarrierInformation.PickupDate != null ? Convert.ToDateTime(result.ShipmentDto.CarrierInformation.PickupDate).ToShortDateString() : string.Empty)
                .Replace("{ShipmentMode}", result.ShipmentDto.GeneralInformation.shipmentModeName)
                .Replace("{ShipmentType}", result.ShipmentDto.GeneralInformation.ShipmentServices)
                .Replace("{Carrier}", result.ShipmentDto.CarrierInformation.CarrierName)
                .Replace("{ShipmentPrice}", result.ShipmentDto.PackageDetails.ValueCurrencyString + " " + result.ShipmentDto.CarrierInformation.Price.ToString())
                .Replace("{PaymentType}", result.ShipmentDto.GeneralInformation.ShipmentPaymentTypeName);

            StringBuilder productList = new StringBuilder();
            decimal totalVol = 0;

            foreach (var product in result.ShipmentDto.PackageDetails.ProductIngredients)
            {
                productList.Append("<tr>");

                productList.Append("<td style='width:290px;text-align:center;color:#11110d'>");
                productList.Append(product.ProductType);
                productList.Append("</td>");

                productList.Append("<td style='width:290px;text-align:center;color:#11110d;'>");
                productList.Append(product.Quantity);
                productList.Append("</td>");

                productList.Append("<td style='width:290px;text-align:center;color:#11110d;'>");
                productList.Append(product.Weight.ToString("n2"));
                productList.Append("</td>");

                totalVol = product.Length * product.Width * product.Height * product.Quantity;
                productList.Append("<td style='width:290px;text-align:center;color:#11110d;'>");
                productList.Append(totalVol.ToString("n2"));
                productList.Append("</td>");

                productList.Append("</tr>");
            }

            emailbody
                .Replace("{tableRecords}", productList.ToString());

            AppUserManager.SendEmail(result.ShipmentDto.GeneralInformation.CreatedBy, "Order Confirmation", emailbody.ToString());

            #endregion

            return Ok(results);
        }

        #endregion


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



