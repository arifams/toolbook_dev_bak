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
    }
}