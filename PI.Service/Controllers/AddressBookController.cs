using PI.Business;
using PI.Contract.DTOs.AddressBook;
using PI.Contract.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace PI.Service.Controllers
{
    [RoutePrefix("api/AddressBook")]
    public class AddressBookController : BaseApiController
    {
        AddressBookManagement addressBookManagement = new AddressBookManagement();
        
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpGet]
        [Route("GetAllAddressBookDetailsByFilter")]
        public PagedList GetAllAddressBookDetailsByFilter(string type, string userId, string searchtext = "",
                                                   int page = 1, int pageSize = 10)
        {
            var pagedRecord = new PagedList();
            return pagedRecord = addressBookManagement.GetAllAddresses(type, userId, searchtext, page, pageSize);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpPost]
        [Route("DeleteAddress")]
        public int DeleteAddress([FromBody] AddressBookDto address)
        {
            return addressBookManagement.DeleteAddress(address.Id);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpPost]
        [Route("SaveAddress")]
        public int SaveAddress([FromBody] AddressBookDto address)
        {
            return addressBookManagement.SaveAddressDetail(address);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpGet]
        [Route("LoadAddress")]
        public AddressBookDto LoadAddress([FromUri]long Id)
        {
            return addressBookManagement.GetAddressBookDtoById(Id);
        }
    }
}
