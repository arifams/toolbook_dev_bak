using PI.Business;
using PI.Contract.DTOs.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace PI.Service.Controllers
{
   
    [RoutePrefix("api/profile")]
    public class ProfileController : BaseApiController
    {
        //get profile details on profile page on load
        [EnableCors(origins: "*", headers: "*", methods: "*")]
       
        [HttpGet]
        [Route("GetProfile")]
        public ProfileDto GetProfile([FromUri]string username)
        {
            ProfileManagement userprofile = new ProfileManagement();            
            return userprofile.getProfileByUserName(username);
        }


        [EnableCors(origins:"*",headers:"*",methods:"*")]
        [Authorize]
        [HttpPost]
        [Route("UpdateProfile")]
        public int UpdateProfile([FromBody] ProfileDto profile)
        {
            ProfileManagement userprofile = new ProfileManagement();
            return userprofile.updateProfileData(profile);
        }




    }
}
