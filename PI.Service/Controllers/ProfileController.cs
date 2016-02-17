using Microsoft.AspNet.Identity;
using PI.Business;
using PI.Contract.DTOs.AccountSettings;
using PI.Contract.DTOs.Profile;
using PI.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
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
        public ProfileDto GetProfile([FromUri]string userId)
        {
            ProfileManagement userprofile = new ProfileManagement();
            return userprofile.getProfileByUserName(userId);
        }

        [HttpGet]
        [Route("GetAllLanguages")]
        public IQueryable<LanguageDto> GetAllLanguages()
        {
            ProfileManagement userprofile = new ProfileManagement();
            IQueryable<LanguageDto> languaes = userprofile.GetAllLanguages();
            return languaes;
        }

        [HttpGet]
        [Route("GetAllCurrencies")]
        public IQueryable<CurrencyDto> GetAllCurrencies()
        {
            ProfileManagement userprofile = new ProfileManagement();
            IQueryable<CurrencyDto> currencies = userprofile.GetAllCurrencies();
            return currencies;
        }

        [HttpGet]
        [Route("GetAllTimezones")]
        public IQueryable<TimeZoneDto> GetAllTimezones()
        {
            ProfileManagement userprofile = new ProfileManagement();
            IQueryable<TimeZoneDto> timeZones = userprofile.GetAllTimeZones();
            return timeZones;
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("UpdateProfile")]
        public int UpdateProfile([FromBody] ProfileDto profile)
        {
            ProfileManagement userprofile = new ProfileManagement();

            if (!string.IsNullOrWhiteSpace(profile.NewPassword))
            {
                IdentityResult result = this.AppUserManager.ChangePassword(User.Identity.GetUserId(),
                                                            profile.OldPassword,
                                                            profile.NewPassword);
            }

            var updatedStatus = userprofile.updateProfileData(profile);

            if (updatedStatus == 1)
            {
                return 1;
            }

            return -1;
        }


    }
}
