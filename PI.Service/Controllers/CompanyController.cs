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

namespace PI.Service.Controllers
{
   // [CustomAuthorize]
    [RoutePrefix("api/Company")]
    public class CompanyController : BaseApiController
    {
        ICompanyManagement companyManagement;

        public CompanyController(ICompanyManagement companymanagement)
        {
            this.companyManagement = companymanagement;
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpGet]
        [Route("GetAllDivisionsByFliter")]
        public PagedList GetAllDivisionsByFliter(long costCenter, string type,string userId, string searchtext = "", 
                                                int page = 1, int pageSize = 10, string sortBy = "Id", string sortDirection = "asc")
        {

            var pagedRecord = new PagedList();
            return pagedRecord = companyManagement.GetAllDivisions(costCenter, type, userId, searchtext, page, pageSize, sortBy, sortDirection);
        }



        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpPost]
        [Route("CreateCompanyDetails")]
        public long CreateCompanyDetails([FromBody] CustomerDto customerCompany)
        {
            return companyManagement.CreateCompanyDetails(customerCompany);
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpGet]
        [Route("GetDivisionById")]
        public DivisionDto GetDivisionById([FromUri] long id, string userId)
        {
            return companyManagement.GetDivisionById(id,userId);
        }



        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpPost]
        [Route("SaveDivision")]
        public int SaveDivision([FromBody] DivisionDto division)
        {
            return companyManagement.SaveDivision(division);
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpPost]
        [Route("DeleteDivision")]
        public int DeleteDivision([FromBody] DivisionDto division)
        {
            return companyManagement.DeleteDivision(division.Id);           
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpPost]
        [Route("SaveCostCenter")]
        public int SaveCostCenter([FromBody] CostCenterDto costCenter)
        {
            return companyManagement.SaveCostCenter(costCenter);
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpGet]
        [Route("GetAllCostCenters")]
        public IList<CostCenterDto> GetAllCostCenters([FromUri]string userId)
        {
            IList <CostCenterDto> costCenterList = companyManagement.GetAllCostCentersForCompany(userId);
            return costCenterList;
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpGet]
        [Route("GetCostCentersbyDivision")]
        public IList<CostCenterDto> GetCostCentersbyDivision([FromUri]string divisionId)
        {
            IList<CostCenterDto> costCenterList = companyManagement.GetCostCentersbyDivision(divisionId);
            return costCenterList;
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpGet]
        [Route("GetAllDivisions")]
        public IList<DivisionDto> GetAllDivisions([FromUri]string userId)
        {
            IList<DivisionDto> divisionList = companyManagement.GetAllDivisionsForCompany(userId);
            return divisionList;
        }

        [EnableCors(origins:"*",headers:"*",methods:"*")]
        [HttpGet]
        [Route("GetAssignedDivisions")]
        public IList<DivisionDto> GetAssignedDivisions([FromUri] string userId)
        {
            IList<DivisionDto> divisionList = companyManagement.GetAssignedDivisions(userId);
            return divisionList;
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpGet]
        [Route("GetAllCostCentersByFliter")]
        public PagedList GetAllCostCentersByFliter(long division, string type, string userId, string searchtext="", 
                                                   int page = 1, int pageSize = 10, string sortBy = "Id", 
                                                   string sortDirection = "asc")
        {

            var pagedRecord = new PagedList();
            return pagedRecord = companyManagement.GetAllCostCenters(division, type, userId, searchtext, page, pageSize, sortBy, 
                                                                     sortDirection);
        }
        

        
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpGet]
        [Route("GetCostCentersById")]
        public CostCenterDto GetCostCentersById([FromUri] long id, [FromUri] string userId)
        {
            return companyManagement.GetCostCentersById(id, userId);
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpPost]
        [Route("DeleteCostCenter")]
        public int DeleteCostCenter([FromBody] CostCenterDto costCenter)
        {
            return companyManagement.DeleteCostCenter(costCenter.Id);
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpGet]
        [Route("GetAllComapnies")]
        public PagedList GetAllComapnies(string status = null, string searchText = null)
        {
            return companyManagement.GetAllComapnies(status, searchText);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpGet]
        [Route("GetAllComapniesForAdminSearch")]
        public PagedList GetAllComapniesForAdmin(string searchText = null)
        {
            return companyManagement.GetAllComapniesForAdminSearch( searchText);
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpPost]
        [Route("ChangeCompanyStatus")]
        public bool ChangeCompanyStatus([FromBody] CompanyDto copmany)
        {
            return companyManagement.ChangeCompanyStatus(copmany.Id);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpGet]
        [Route("GetCompanyByUserId")]
        public CompanyDto GetCompanyByUserId(string loggedInUserId)
        {
            return companyManagement.GetCompanyByUserID(loggedInUserId);
        }


    }
}