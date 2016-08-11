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
    //[CustomAuthorize]
    [RoutePrefix("api/profile")]
    public class ProfileController : BaseApiController
    {
        readonly IProfileManagement userprofile;
        readonly CommonLogic commonLogics;

        public ProfileController(IProfileManagement userprofile)
        {
            this.userprofile = userprofile;
            commonLogics = new CommonLogic(); // TODO: H - initialize from DI
        }

        //get profile details on profile page on load
        [EnableCors(origins: "*", headers: "*", methods: "*")]       
        [HttpGet]
        [Route("GetProfile")]
        public ProfileDto GetProfile([FromUri]string userId)
        {
            return userprofile.getProfileByUserName(userId);
        }


        //get the profile language when loading the app
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        [Route("GetProfileLanguageByUserId")]
        public string GetProfileLanguageByUserId(string userId)
        {
           return userprofile.GetLanguageCodeByUserId(userId);
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]       
        [HttpGet]
        [Route("GetProfileForShipment")]
        public ProfileDto GetProfileForShipment([FromUri]string userId)
        {
            return userprofile.getProfileByUserNameForShipment(userId);
        }

        

        [HttpGet]        
        [Route("GetAllAccountSettings")]
        public ProfileDto GetAllAccountSettings(long customerId)
        {
           return userprofile.GetAccountSettings(customerId);
        }

        
        [HttpGet]
        [Route("GetCustomerAddressDetails")]
        public ProfileDto GetCustomerAddressDetails(long cusomerAddressId, long companyId)
        {
            return userprofile.GetCustomerAddressDetails(cusomerAddressId, companyId);
        }


        
        [EnableCors(origins: "*", headers: "*", methods: "*")]        
        [HttpPost]
        [Route("UpdateProfile")]
        public int UpdateProfile([FromBody] ProfileDto profile)
        {
            if (!string.IsNullOrWhiteSpace(profile.NewPassword) && (!string.IsNullOrWhiteSpace(profile.CustomerDetails.UserId)))
            {
                IdentityResult result = this.AppUserManager.ChangePassword(profile.CustomerDetails.UserId,
                                                            profile.OldPassword,
                                                           profile.NewPassword);
                if (result.Errors!=null && result.Errors.Count()> 0)
                {
                    return -3;
                } 
            }

            var updatedStatus = userprofile.updateProfileData(profile);

            if (updatedStatus==3)
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

            if (updatedStatus == 1 || updatedStatus == -2 || updatedStatus == 3)
            {
              return  updatedStatus;
            }           

            return -1;
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]       
        [HttpPost]
        [Route("UpdateProfileGeneral")]
        public int UpdateProfileGeneral(ProfileDto profile)
        {
            var updatedStatus = userprofile.UpdateProfileGeneral(profile);

            if (updatedStatus == 3)
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

            if (updatedStatus == 1 || updatedStatus == -2 || updatedStatus == 3)
            {
                return updatedStatus;
            }

            return -1;
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]       
        [HttpPost]
        [Route("UpdateProfileAddress")]
        public int UpdateProfileAddress(ProfileDto profile)
        {
            var updatedStatus = userprofile.UpdateProfileAddress(profile);

            if (updatedStatus == 1 || updatedStatus == -2)
            {
                return updatedStatus;
            }

            return -1;
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]       
        [HttpPost]
        [Route("UpdateProfileBillingAddress")]
        public int UpdateProfileBillingAddress(ProfileDto profile)
        {
            var updatedStatus = userprofile.UpdateProfileBillingAddress(profile);

            if (updatedStatus == 1 || updatedStatus == -2)
            {
                return updatedStatus;
            }

            return -1;
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("UpdateSetupWizardBillingAddress")]
        public int UpdateSetupWizardBillingAddress(ProfileDto profile)
        {
            var updatedStatus = userprofile.UpdateSetupWizardBillingAddress(profile);

            if (updatedStatus == 1 || updatedStatus == -2)
            {
                return updatedStatus;
            }

            return -1;
            
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]        
        [HttpPost]
        [Route("updateProfileLoginDetails")]
        public int updateProfileLoginDetails(ProfileDto profile)
        {
            if (!string.IsNullOrWhiteSpace(profile.NewPassword) && (!string.IsNullOrWhiteSpace(profile.CustomerDetails.UserId)))
            {
                IdentityResult result = this.AppUserManager.ChangePassword(profile.CustomerDetails.UserId,
                                                            profile.OldPassword,
                                                           profile.NewPassword);
                if (result.Errors != null && result.Errors.Count() > 0)
                {
                    return -3;
                }
            }

            var updatedStatus = userprofile.UpdateProfileLoginDetails(profile);

            if (updatedStatus == 1 || updatedStatus == -2)
            {
                return updatedStatus;
            }

            return -1;
        }        


        [EnableCors(origins: "*", headers: "*", methods: "*")]       
        [HttpPost]
        [Route("updateProfileAccountSettings")]
        public int updateProfileAccountSettings(ProfileDto profile)
        {
            var updatedStatus = userprofile.UpdateProfileAccountSettings(profile);

            if (updatedStatus == 1 || updatedStatus == -2)
            {
                return updatedStatus;
            }

            return -1;
        }


        /// <summary>
        /// Update Theme Colour
        /// </summary>
        /// <param name="updatedProfile"></param>
        /// <returns></returns>
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("UpdateThemeColour")]
        public int UpdateThemeColour(ProfileDto updatedProfile)
        { 
            var updatedStatus = userprofile.UpdateThemeColour(updatedProfile);

            if (updatedStatus == 1 || updatedStatus == -2)
            {
                return updatedStatus;
            }

            return -1;
        }

    }
}
