using AzureMediaManager;
using Newtonsoft.Json;
using PI.Business;
using PI.Common;
using PI.Contract.Business;
using PI.Contract.DTOs;
using PI.Contract.DTOs.AddressBook;
using PI.Contract.DTOs.FileUpload;
using PI.Contract.Enums;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace PI.Service.Controllers
{
    [CustomAuthorize]
    [RoutePrefix("api/Admin")]
    public class AdminController : BaseApiController
    {
        IAdministrationManagment adminManagement = new AdministrationManagment();

        //public AdminController(IAdministrationManagment adminmanagementa)
        //{
        //    this.adminManagement = adminmanagementa;
        //}

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpPost]
      // This is from System.Web.Http, and not from System.Web.Mvc
        [Route("UploadRateSheet")]
        public async Task<HttpResponseMessage> UploadRateSheet(string userId)
        {
             OperationResult opResult = new OperationResult();
             try
             {
                 var responce = await Upload();

                 Result result = null;
                 var urlJson = await responce.Content.ReadAsStringAsync();

                 result = JsonConvert.DeserializeObject<Result>(urlJson);

                 opResult = adminManagement.ImportRateSheetExcel(result.returnData);
             }
             catch (Exception ex)
             {
                 opResult.Status = Status.Error;
                 opResult.Message = "Controller  :" + ex.ToString();
             }

            return this.Request.CreateResponse(opResult.Status == Status.Success ? HttpStatusCode.OK : HttpStatusCode.InternalServerError, opResult.Message);
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
            CompanyManagement companyManagement = new CompanyManagement();
            string imageFileNameInFull = null;
            // Make absolute link
            string baseUrl = ConfigurationManager.AppSettings["PIBlobStorage"];

            var tenantId = companyManagement.GettenantIdByUserId(fileDetails.UserId);
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


            if (fileDetails.DocumentType != DocumentType.AddressBook && fileDetails.DocumentType != DocumentType.RateSheet)
            {
                // Insert document record to DB.
                ShipmentsManagement shipmentManagement = new ShipmentsManagement();
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

                if (fileUploadDto.DocumentType != DocumentType.AddressBook && fileUploadDto.DocumentType != DocumentType.RateSheet)
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

    }
}
