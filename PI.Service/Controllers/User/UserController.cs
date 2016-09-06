using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PI.Contract.DTOs.Customer;
using PI.Business;
using System.Web.Http.Cors;
using PI.Contract.Business;

namespace PI.Service.Controllers.User
{
    public class UserController : ApiController
    {
        private ICustomerManagement customerManagement;

        public UserController(ICustomerManagement customerManagement)
        {
            this.customerManagement = customerManagement;
        }

        [CustomAuthorize]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        public int CreateUser([FromBody]CustomerDto customer)
        {
            return customerManagement.SaveCustomer(customer);
        }


        //[EnableCors(origins: "*", headers: "*", methods: "*")]
        //[HttpPost]
        ////[InitializeSimpleMembershipAttribute]
        //public int LoginUser([FromBody]CustomerDto customer)
        //{
        //    CustomerManagement customerManagement = new CustomerManagement();
        //    return customerManagement.VerifyUserLogin(customer);
        //}

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        public int TestApi()
        {
            return 1;
        }
    }
}
