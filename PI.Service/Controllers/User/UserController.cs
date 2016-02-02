using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PI.Contract;
using PI.Contract.DTOs.Customer;
using PI.Business;

namespace PI.Service.Controllers.User
{
    public class UserController : ApiController
    {
        [HttpPost]
        public IHttpActionResult CreateUser([FromBody]CustomerDto customer)
        {
            CustomerManagement customerManagement = new CustomerManagement();
            customerManagement.SaveCustomer(customer);
            return null;
        }
        
    }
}
