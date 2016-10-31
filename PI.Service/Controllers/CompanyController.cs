using PI.Business;
using PI.Contract.DTOs.Division;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using PI.Contract.DTOs.CostCenter;
using PI.Contract.DTOs.Company;
using PI.Contract.DTOs.Customer;
using PI.Service.Models;
using PI.Contract.DTOs.Common;
using PI.Contract.Business;
using PI.Data.Entity;
using PI.Contract.DTOs.Node;
using System.Net;
using Newtonsoft.Json;
using PI.Contract.DTOs.FileUpload;
using PI.Common;
using PI.Contract.Enums;
using System.IO;
using System.Threading.Tasks;
using System.Configuration;
using AzureMediaManager;
using PI.Contract.DTOs.AddressBook;

namespace PI.Service.Controllers
{
    [CustomAuthorize]
    [RoutePrefix("api/Company")]
    public class CompanyController : BaseApiController
    {
        readonly ICompanyManagement companyManagement;

        public CompanyController(ICompanyManagement companymanagement)
        {
            this.companyManagement = companymanagement;
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpGet]
        [Route("GetAllDivisionsByFliter")]
        public IHttpActionResult GetAllDivisionsByFliter(long costCenter, string type, string userId, string searchtext = "",
                                                int page = 1, int pageSize = 10, string sortBy = "Id", string sortDirection = "asc")
        {

            return Ok(companyManagement.GetAllDivisions(costCenter, type, userId, searchtext, page,
                      pageSize, sortBy, sortDirection));
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpGet]
        [Route("GetDivisionById")]
        public IHttpActionResult GetDivisionById([FromUri] long id, string userId)
        {
            return Ok(companyManagement.GetDivisionById(id, userId));
        }



        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpPost]
        [Route("SaveDivision")]
        public IHttpActionResult SaveDivision([FromBody] DivisionDto division)
        {
            int result = companyManagement.SaveDivision(division);

            if (result == 0)
            {
                return BadRequest();
            }
            else if (result == -1)
            {
                return BadRequest("A division with the same name/description already exists");
            }
            return Ok();
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpPost]
        [Route("DeleteDivision")]
        public IHttpActionResult DeleteDivision([FromBody] DivisionDto division)
        {
            var result = companyManagement.DeleteDivision(division.Id);

            if (result == 0)
            {
                return BadRequest();
            }
            else if (result == -1)
            {
                return BadRequest("Division does not exist!");
            }
            return Ok();
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpPost]
        [Route("SaveCostCenter")]
        public IHttpActionResult SaveCostCenter([FromBody] CostCenterDto costCenter)
        {
            var result = companyManagement.SaveCostCenter(costCenter);

            if (result == 0)
            {
                return BadRequest();
            }
            else if (result == -1)
            {
                return BadRequest("A cost center with the same name already exists");
            }
            return Ok();
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpGet]
        [Route("GetAllCostCenters")]
        public IHttpActionResult GetAllCostCenters([FromUri]string userId)
        {
            return Ok(companyManagement.GetAllCostCentersForCompany(userId));
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpGet]
        [Route("GetCostCentersbyDivision")]
        public IHttpActionResult GetCostCentersbyDivision([FromUri]string divisionId)
        {
            return Ok(companyManagement.GetCostCentersbyDivision(divisionId));
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpGet]
        [Route("GetDefaultCostCentersbyDivision")]
        public IHttpActionResult GetDefaultCostCentersbyDivision([FromUri]string divisionId)
        {
            return Ok(companyManagement.GetDefaultCostCentersbyDivision(divisionId));
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpGet]
        [Route("GetAllDivisions")]
        public IHttpActionResult GetAllDivisions([FromUri]string userId)
        {
            return Ok(companyManagement.GetAllDivisionsForCompany(userId));
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        [Route("GetAssignedDivisions")]
        public IHttpActionResult GetAssignedDivisions([FromUri] string userId)
        {
            return Ok(companyManagement.GetAssignedDivisions(userId));
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpGet]
        [Route("GetAllCostCentersByFliter")]
        public IHttpActionResult GetAllCostCentersByFliter(long division, string type, string userId, string searchtext = "",
                                                   int page = 1, int pageSize = 10, string sortBy = "Id",
                                                   string sortDirection = "asc")
        {

            return Ok(companyManagement.GetAllCostCenters(division, type, userId, searchtext, page,
                                                          pageSize, sortBy, sortDirection));
        }



        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpGet]
        [Route("GetCostCentersById")]
        public IHttpActionResult GetCostCentersById([FromUri] long id, [FromUri] string userId)
        {
            return Ok(companyManagement.GetCostCentersById(id, userId));
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpPost]
        [Route("DeleteCostCenter")]
        public IHttpActionResult DeleteCostCenter([FromBody] CostCenterDto costCenter)
        {
            var result = companyManagement.DeleteCostCenter(costCenter.Id);

            if (result == 0)
            {
                return BadRequest();
            }
            else if (result == -1)
            {
                return BadRequest("Cost center does not exist!");
            }
            return Ok();
        }


     
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpGet]
        [Route("GetCompanyByUserId")]
        public IHttpActionResult GetCompanyByUserId(string loggedInUserId)
        {
            return Ok(companyManagement.GetCompanyByUserID(loggedInUserId));
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpGet]
        [Route("GetLogoUrl")]
        public IHttpActionResult GetLogoUrl(string loggedInUserId)
        {
            var company = companyManagement.GetCompanyByUserID(loggedInUserId);

            if (company.LogoUrl != null)
            {
                return Ok(company.LogoUrl);
            }
            return BadRequest();
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpGet]
        [Route("GetOrganizationStructure")]
        public IHttpActionResult GetOrganizationStructure(string userId)
        {
            return Ok(companyManagement.GetOrganizationStructure(userId));
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpPost]
        [Route("UploadLogo")]
        public async Task<HttpResponseMessage> UploadLogo()
        {
            HttpResponseMessage uploadResult = new HttpResponseMessage();
            var logoUpdated = false;
            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                {
                    this.Request.CreateResponse(HttpStatusCode.UnsupportedMediaType);
                }

                var provider = GetMultipartProvider();
                var result = await Request.Content.ReadAsMultipartAsync(provider);
                var fileDetails = GetFormData<FileUploadDto>(result);

                uploadResult = await this.Upload(result);

                var urlJson = await uploadResult.Content.ReadAsStringAsync();

                Result deSelizalizedObject = null;
                deSelizalizedObject = JsonConvert.DeserializeObject<Result>(urlJson);

                if (uploadResult.Content != null)
                {
                    logoUpdated = companyManagement.UpdateCompanyLogo(deSelizalizedObject.returnData, fileDetails.UserId);

                }

            }
            catch (Exception ex)
            {
                throw new Exception();
            }

            return this.Request.CreateResponse(uploadResult.StatusCode == HttpStatusCode.OK && logoUpdated ? HttpStatusCode.OK : HttpStatusCode.InternalServerError);
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [AllowAnonymous]
        [HttpGet]
        [Route("TestMethodA")]
        public string TestMethodA(string param)
        {
            return "Success: " + param;
        }

        [AllowAnonymous]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        [Route("TestMethodB")]
        public string TestMethodB()
        {
            return "Success TestMethodB";
        }


        #region Private methods


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


            if (fileDetails.DocumentType == DocumentType.Logo)
            {
                var fileNameSplitByDot = originalFileName.Split(new char[1] { '.' });
                string fileExtention = fileNameSplitByDot[fileNameSplitByDot.Length - 1];

                imageFileNameInFull = System.Guid.NewGuid().ToString() + "logo." + fileExtention;
                fileDetails.ClientFileName = originalFileName;
                fileDetails.UploadedFileName = imageFileNameInFull;
            }


            media.InitializeStorage(fileDetails.TenantId.ToString(), Utility.GetEnumDescription(fileDetails.DocumentType));
            await media.Upload(stream, imageFileNameInFull);


            // Through the request response you can return an object to the Angular controller
            // You will be able to access this in the .success callback through its data attribute
            // If you want to send something to the .error callback, use the HttpStatusCode.BadRequest instead
            var returnData = baseUrl + "TENANT_" + fileDetails.TenantId + "/" + Utility.GetEnumDescription(fileDetails.DocumentType)
                             + "/" + fileDetails.UploadedFileName;


            return this.Request.CreateResponse(HttpStatusCode.OK, new { returnData });
        }

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