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

namespace PI.Service.Controllers
{

    [RoutePrefix("api/Company")]
    public class CompanyController : BaseApiController
    {

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpPost]
        [Route("CreateCompanyDetails")]
        public long CreateCompanyDetails([FromBody] CustomerDto customerCompany)
        {
            CompanyManagement companyManagement = new CompanyManagement();
            return companyManagement.CreateCompanyDetails(customerCompany);
        }


        [EnableCors(origins:"*",headers:"*",methods:"*")]
       // [Authorize]
        [HttpPost]
        [Route("SaveDivision")]
        public int SaveDivision([FromBody] DivisionDto division)
        {
            CompanyManagement company = new CompanyManagement();
            return company.SaveDivision(division);
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpPost]
        [Route("SaveCostCenter")]
        public int SaveCostCenter([FromBody] CostCenterDto costCenter)
        {
            CompanyManagement company = new CompanyManagement();
            return company.SaveCostCenter(costCenter);
        }

        
    }
}