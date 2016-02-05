using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PI.Contract.DTOs.Customer;
using PI.Business;
using System.Web.Http.Cors;

namespace PI.Service.Controllers.User
{
    public class UserController : ApiController
    {
        [EnableCors(origins: "http://localhost:63874", headers: "*", methods: "*")]
        [HttpPost]
        //[InitializeSimpleMembershipAttribute]
        public IHttpActionResult CreateUser([FromBody]CustomerDto customer)
        {
            CustomerManagement customerManagement = new CustomerManagement();
            customerManagement.SaveCustomer(customer);
            return null;
        }
        
    }
}
