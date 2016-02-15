using PI.Business;
using PI.Contract.DTOs.Division;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace PI.Service.Controllers
{

    [RoutePrefix("api/Company")]
    public class CompanyController : BaseApiController
    {
        
        [EnableCors(origins:"*",headers:"*",methods:"*")]
       // [Authorize]
        [HttpPost]
        [Route("SaveDivision")]
        public int SaveDivision([FromBody] DivisionDto division)
        {
            CompanyManagement company = new CompanyManagement();
            return company.SaveDivision(division);
        }

        
    }
}