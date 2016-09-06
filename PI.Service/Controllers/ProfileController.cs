using Microsoft.AspNet.Identity;
using PI.Business;
using PI.Contract.Business;
using PI.Contract.DTOs.AccountSettings;
using PI.Contract.DTOs.Profile;
using PI.Data.Entity.Identity;
using PI.Service.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace PI.Service.Controllers
{
    [CustomAuthorize]
    [RoutePrefix("api/profile")]
    public class ProfileController : BaseApiController
    {
        readonly IProfileManagement userprofile;

        public ProfileController(IProfileManagement userprofile)
        {
            this.userprofile = userprofile;
        }

        //get profile details on profile page on load
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        [Route("GetProfile")]
        public IHttpActionResult GetProfile([FromUri]string userId)
        {
            return Ok(userprofile.getProfileByUserName(userId));
        }


        //get the profile language when loading the app
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        [Route("GetProfileLanguageByUserId")]
        public IHttpActionResult GetProfileLanguageByUserId(string userId)
        {
            return Ok(userprofile.GetLanguageCodeByUserId(userId));
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        [Route("GetProfileForShipment")]
        public IHttpActionResult GetProfileForShipment([FromUri]string userId)
        {
            return Ok(userprofile.getProfileByUserNameForShipment(userId));
        }


        [HttpGet]
        [Route("GetAllAccountSettings")]
        public IHttpActionResult GetAllAccountSettings(long customerId)
        {
            return Ok(userprofile.GetAccountSettings(customerId));
        }


        [HttpGet]
        [Route("GetCustomerAddressDetails")]
        public IHttpActionResult GetCustomerAddressDetails(long cusomerAddressId, long companyId)
        {
            return Ok(userprofile.GetCustomerAddressDetails(cusomerAddressId, companyId));
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("UpdateProfileGeneral")]
        public IHttpActionResult UpdateProfileGeneral(ProfileDto profile)
        {
            var updatedStatus = userprofile.UpdateProfileGeneral(profile);

            if (updatedStatus == 2)
            {
                ApplicationUser existingUser = AppUserManager.FindByName(profile.CustomerDetails.Email);

                #region For Email Confirmaion

                string code = AppUserManager.GenerateEmailConfirmationToken(existingUser.Id);
                var callbackUrl = new Uri(Url.Content(ConfigurationManager.AppSettings["BaseWebURL"] + @"app/userLogin/userlogin.html?userId=" + existingUser.Id + "&code=" + code));

                StringBuilder emailbody = new StringBuilder(profile.CustomerDetails.TemplateLink);
                emailbody.Replace("FirstName", existingUser.FirstName).Replace("LastName", existingUser.LastName).Replace("Salutation", profile.CustomerDetails.Salutation + ".")
                                             .Replace("ActivationURL", "<a style=\"color:#80d4ff\" href=\"" + callbackUrl + "\">here</a>");
                AppUserManager.SendEmail(existingUser.Id, "Your account has been provisioned!", emailbody.ToString());

                #endregion
            }

            if (updatedStatus == 0)
            {
                return BadRequest();
            }
            else if (updatedStatus == -1)
            {
                return BadRequest("This email address is already in use!");
            }
            else if (updatedStatus == 2)
            {
                return Ok("We have inbox you the username change confirmation email. Please confirm before login.");
            }

            return Ok("Profile Updated Successfully!");
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("UpdateProfileAddress")]
        public IHttpActionResult UpdateProfileAddress(ProfileDto profile)
        {
            var updatedStatus = userprofile.UpdateProfileAddress(profile);

            if (updatedStatus == 0)
            {
                return BadRequest();
            }

            return Ok();
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("UpdateProfileBillingAddress")]
        public IHttpActionResult UpdateProfileBillingAddress(ProfileDto profile)
        {
            var updatedStatus = userprofile.UpdateProfileBillingAddress(profile);

            if (updatedStatus == 0)
            {
                return BadRequest();
            }

            return Ok();
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("UpdateSetupWizardBillingAddress")]
        public IHttpActionResult UpdateSetupWizardBillingAddress(ProfileDto profile)
        {
            var updatedStatus = userprofile.UpdateSetupWizardBillingAddress(profile);

            if (updatedStatus == 0)
            {
                return BadRequest();
            }

            return Ok();
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("updateProfileLoginDetails")]
        public IHttpActionResult updateProfileLoginDetails(ProfileDto profile)
        {
            if (!string.IsNullOrWhiteSpace(profile.NewPassword) && (!string.IsNullOrWhiteSpace(profile.CustomerDetails.UserId)))
            {
                IdentityResult result = this.AppUserManager.ChangePassword(profile.CustomerDetails.UserId,
                                                            profile.OldPassword,
                                                           profile.NewPassword);
                if (result.Errors != null && result.Errors.Count() > 0)
                {
                    return BadRequest("Old password You Entered is Invalid");
                }
                return Ok();
            }
            return BadRequest();
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("updateProfileAccountSettings")]
        public IHttpActionResult updateProfileAccountSettings(ProfileDto profile)
        {
            var updatedStatus = userprofile.UpdateProfileAccountSettings(profile);

            if (updatedStatus == 0)
            {
                return BadRequest();
            }

            return Ok();
        }


        /// <summary>
        /// Update Theme Colour
        /// </summary>
        /// <param name="updatedProfile"></param>
        /// <returns></returns>
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("UpdateThemeColour")]
        public IHttpActionResult UpdateThemeColour(ProfileDto updatedProfile)
        {
            var updatedStatus = userprofile.UpdateThemeColour(updatedProfile);

            if (updatedStatus == 0)
            {
                return BadRequest();
            }

            return Ok();
        }

    }
}
