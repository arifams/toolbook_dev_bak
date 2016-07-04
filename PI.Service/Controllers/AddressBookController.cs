using PI.Business;
using PI.Contract.Business;
using PI.Contract.DTOs.AddressBook;
using PI.Contract.DTOs.Common;
using PI.Contract.DTOs.ImportAddress;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Cors;

namespace PI.Service.Controllers
{
    [CustomAuthorize]
    [RoutePrefix("api/AddressBook")]
    public class AddressBookController : BaseApiController
    {
        readonly IAddressBookManagement addressBookManagement;

        public AddressBookController(IAddressBookManagement addressbookmanagement)
        {
            this.addressBookManagement = addressbookmanagement;
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        [Route("GetAllAddressBookDetailsByFilter")]
        public PagedList GetAllAddressBookDetailsByFilter(string type, string userId, string searchtext = "",
                                                   int page = 1, int pageSize = 10)
        {
            return addressBookManagement.GetAllAddresses(type, userId, searchtext, page, pageSize);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        //[Authorize]
        [HttpGet]
        [Route("GetSerchedAddressList")]
        public PagedList GetSerchedAddressList(string userId, string searchtext = "")
        {
            return addressBookManagement.GetFilteredAddresses(userId, searchtext);
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
        [CustomAuthorize]
        [HttpPost]
        [Route("SaveAddress")]
        public int SaveAddress([FromBody] AddressBookDto address)
        {
            return addressBookManagement.SaveAddressDetail(address);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpPost]
        [Route("ImportAddresses")]
        public int ImportAddresses([FromBody]IList<ImportAddressDto> addresses, string userId)
        {
            return addressBookManagement.ImportAddressBook(addresses, userId);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpGet]
        [Route("LoadAddress")]
        public AddressBookDto LoadAddress([FromUri]long Id)
        {
            return addressBookManagement.GetAddressBookDtoById(Id);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpGet]
        [Route("GetAddressBookDetailsExcel")]
        public HttpResponseMessage GetAddressBookDetailsExcel([FromUri]string userId)
        {
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new ByteArrayContent(addressBookManagement.GetAddressBookDetailsByUserId(userId));
            result.Content.Headers.Add("x-filename", "AddressBook.xlsx");
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            return result;


        }


    }

}
