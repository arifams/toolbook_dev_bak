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

namespace PI.Service.Controllers
{

    [RoutePrefix("api/Company")]
    public class CompanyController : BaseApiController
    {
        CompanyManagement companyManagement = new CompanyManagement();


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpGet]
        [Route("GetAllDivisionsByFliter")]
        public PagedList GetAllDivisionsByFliter(string searchtext, int page = 1, int pageSize = 10,
                                      string sortBy = "Id", string sortDirection = "asc")
        {

            var pagedRecord = new PagedList();
            return pagedRecord = companyManagement.GetAllDivisions(searchtext, page, pageSize, sortBy, sortDirection);
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
        public DivisionDto GetDivisionById([FromUri] long id)
        {
            return companyManagement.GetDivisionById(id);
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


    }
}