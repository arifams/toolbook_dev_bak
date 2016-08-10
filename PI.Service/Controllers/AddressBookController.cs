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
        public IHttpActionResult GetAllAddressBookDetailsByFilter(string type, string userId, string searchtext = "",
                                                   int page = 1, int pageSize = 10)
        {
            return Ok(addressBookManagement.GetAllAddresses(type, userId, searchtext, page, pageSize));
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        //[Authorize]
        [HttpGet]
        [Route("GetSerchedAddressList")]
        public IHttpActionResult GetSerchedAddressList(string userId, string searchtext = "")
        {
            return Ok(addressBookManagement.GetFilteredAddresses(userId, searchtext));
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpPost]
        [Route("DeleteAddress")]
        public IHttpActionResult DeleteAddress([FromBody] AddressBookDto address)
        {
            int result = addressBookManagement.DeleteAddress(address.Id);
            if (result == 0)
            {
                return BadRequest();
            }
            return Ok();
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [CustomAuthorize]
        [HttpPost]
        [Route("SaveAddress")]
        public IHttpActionResult SaveAddress([FromBody] AddressBookDto address)
        {
            int result = addressBookManagement.SaveAddressDetail(address);
            if (result == 0)
            {
                return BadRequest();
            }
            return Ok();
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpPost]
        [Route("ImportAddresses")]
        public IHttpActionResult ImportAddresses([FromBody]IList<ImportAddressDto> addresses, string userId)
        {
            int result = addressBookManagement.ImportAddressBook(addresses, userId);

            if (result == 0)
            {
                return BadRequest();
            }
            else if (result == -1)
            {
                return BadRequest("");
            }
            return Ok();
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpGet]
        [Route("LoadAddress")]
        public IHttpActionResult LoadAddress([FromUri]long Id)
        {
            return Ok(addressBookManagement.GetAddressBookDtoById(Id));
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpGet]
        [Route("GetAddressBookDetailsExcel")]
        public HttpResponseMessage GetAddressBookDetailsExcel(string type, string userId, string searchtext = "",
                                                   int page = 1, int pageSize = 10)
        {
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new ByteArrayContent(addressBookManagement.GetAddressBookDetailsByUserId(type, userId, searchtext, page, pageSize));
            result.Content.Headers.Add("x-filename", "AddressBook.xlsx");
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            return result;


        }


    }

}
