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
        public int CreateUser([FromBody]CustomerDto customer)
        {
            CustomerManagement customerManagement = new CustomerManagement();
            return customerManagement.SaveCustomer(customer);
        }


        [EnableCors(origins: "http://localhost:63874", headers: "*", methods: "*")]
        [HttpPost]
        //[InitializeSimpleMembershipAttribute]
        public int LoginUser([FromBody]CustomerDto customer)
        {
            CustomerManagement customerManagement = new CustomerManagement();
            return customerManagement.VerifyUserLogin(customer);
        }
        
    }
}
