using Facebook;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Linq;
using PI.Business;
using PI.Contract.Business;
using PI.Contract.DTOs.Common;
using PI.Contract.DTOs.Company;
using PI.Contract.DTOs.CostCenter;
using PI.Contract.DTOs.Customer;
using PI.Contract.DTOs.Division;
using PI.Contract.DTOs.Role;
using PI.Contract.DTOs.User;
using PI.Data.Entity.Identity;
using PI.Service.Models;
using PI.Service.Providers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Script.Serialization;
using WebApplication3.Results;

namespace PI.Service.Controllers
{
    [RoutePrefix("api/accounts")]
    public class AccountsController : BaseApiController
    {
        readonly ICompanyManagement companyManagement;
        readonly ICustomerManagement customerManagement;
        private AuthRepository authRepo = null;

        public AccountsController(ICompanyManagement companymanagement, ICustomerManagement customermanagement)
        {
            this.companyManagement = companymanagement;
            this.customerManagement = customermanagement;
            authRepo = new AuthRepository();

        }


        [CustomAuthorize]
        [Route("users")]
        public IHttpActionResult GetUsers()
        {
            return Ok(this.AppUserManager.Users.ToList().Select(u => this.TheModelFactory.Create(u)));
        }

        [CustomAuthorize]
        [Route("user/{id:guid}", Name = "GetUserById")]
        public async Task<IHttpActionResult> GetUser(string Id)
        {
            var user = await this.AppUserManager.FindByIdAsync(Id);

            if (user != null)
            {
                return Ok(this.TheModelFactory.Create(user));
            }

            return NotFound();

        }

        [CustomAuthorize]
        [Route("user/{username}")]
        public async Task<IHttpActionResult> GetUserByName(string username)
        {
            var user = await this.AppUserManager.FindByNameAsync(username);

            if (user != null)
            {
                return Ok(this.TheModelFactory.Create(user));
            }

            return NotFound();

        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [AllowAnonymous]
        [HttpPost]
        [Route("create")]
        public int CreateUser(CustomerDto createUserModel)
        {
            if (!ModelState.IsValid)
            {
                // return BadRequest(ModelState);
                return -1;
            }

            var user = new ApplicationUser()
            {
                UserName = createUserModel.Email,
                Email = createUserModel.Email,
                Salutation = "-", //createUserModel.Salutation,
                FirstName = "-", //createUserModel.FirstName,
                LastName = "-", //createUserModel.LastName,
                Level = 3,
                JoinDate = DateTime.Now.Date,
                //BirthDate = createUserModel.BirthDate,
                //HomeTown = createUserModel.HomeTown,
                IsActive = true
            };

            ApplicationUser existingUser = AppUserManager.FindByName(createUserModel.Email);
            if (existingUser == null)
            {

                //Create Tenant, Default Company, Division & CostCenter 
                createUserModel.CustomerAddress = new Contract.DTOs.Address.AddressDto();
                long tenantId = companyManagement.CreateCompanyDetails(createUserModel);

                // Add tenant Id to user
                user.TenantId = tenantId;
                user.EmailConfirmed = createUserModel.viaExternalLogin ? true : false;

                IdentityResult addUserResult = createUserModel.viaExternalLogin ? AppUserManager.Create(user) :
                                                                                  AppUserManager.Create(user, createUserModel.Password);

                createUserModel.UserId = user.Id;
              
                // Save in customer table.
                customerManagement.SaveCustomer(createUserModel);
            }
            else
            {
                return -2;
                //return GetErrorResult(IdentityResult.Failed("Email already exists!"));
            }

            // Add Business Owner Role to user
            AppUserManager.AddToRole(user.Id, "BusinessOwner");

            AppUserManager.Update(user);


            if (!createUserModel.viaExternalLogin)
            {
                #region For Email Confirmaion

                string code = AppUserManager.GenerateEmailConfirmationToken(user.Id);
                var callbackUrl = new Uri(Url.Content(ConfigurationManager.AppSettings["BaseWebURL"] + @"app/userLogin/userlogin.html?userId=" + user.Id + "&code=" + code));

                StringBuilder emailbody = new StringBuilder(createUserModel.TemplateLink);
                emailbody.Replace("ActivationURL", "<a style=\"color:#80d4ff\" href=\"" + callbackUrl + "\">here</a>");

                AppUserManager.SendEmail(user.Id, "Parcel International – Activate your account", emailbody.ToString());

                #endregion
            }

            //Uri locationHeader = new Uri(Url.Link("GetUserById", new { id = user.Id }));

            //return Created(locationHeader, TheModelFactory.Create(user));
            return 1;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("ConfirmEmail", Name = "ConfirmEmailRoute")]
        public async Task<IHttpActionResult> ConfirmEmail(string userId = "", string code = "")
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
            {
                ModelState.AddModelError("", "User Id and Code are required");
                return BadRequest(ModelState);
            }

            IdentityResult result = await this.AppUserManager.ConfirmEmailAsync(userId, code);

            if (result.Succeeded)
            {
                return Ok();
            }
            else
            {
                return GetErrorResult(result);
            }
        }

        [CustomAuthorize]
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await this.AppUserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        [CustomAuthorize]
        [Route("user/{id:guid}")]
        public async Task<IHttpActionResult> DeleteUser(string id)
        {

            //Only SuperAdmin or Admin can delete users (Later when implement roles)

            var appUser = await this.AppUserManager.FindByIdAsync(id);

            if (appUser != null)
            {
                IdentityResult result = await this.AppUserManager.DeleteAsync(appUser);

                if (!result.Succeeded)
                {
                    return GetErrorResult(result);
                }

                return Ok();

            }

            return NotFound();

        }


        [CustomAuthorize]
        [Route("user/{id:guid}/roles")]
        [HttpPut]
        public IHttpActionResult AssignRolesToUser([FromUri] string id, [FromBody] string[] rolesToAssign)
        {

            var appUser = this.AppUserManager.FindById(id);

            if (appUser == null)
            {
                return NotFound();
            }

            var currentRoles = this.AppUserManager.GetRoles(appUser.Id);

            var rolesNotExists = rolesToAssign.Except(this.AppRoleManager.Roles.Select(x => x.Name)).ToArray();

            if (rolesNotExists.Count() > 0)
            {

                ModelState.AddModelError("", string.Format("Roles '{0}' does not exixts in the system", string.Join(",", rolesNotExists)));
                return BadRequest(ModelState);
            }

            IdentityResult removeResult = this.AppUserManager.RemoveFromRoles(appUser.Id, currentRoles.ToArray());

            if (!removeResult.Succeeded)
            {
                ModelState.AddModelError("", "Failed to remove user roles");
                return BadRequest(ModelState);
            }

            IdentityResult addResult = this.AppUserManager.AddToRoles(appUser.Id, rolesToAssign);

            if (!addResult.Succeeded)
            {
                ModelState.AddModelError("", "Failed to add user roles");
                return BadRequest(ModelState);
            }

            return Ok();
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [AllowAnonymous]
        [HttpPost]
        [Route("LoginUser")]
        public IHttpActionResult LoginUser(CustomerDto customer)
        {

            var user = (!customer.viaExternalLogin) ? AppUserManager.Find(customer.UserName, customer.Password) :
                                                      AppUserManager.FindByName(customer.UserName); 

            if (user == null)
                return Ok(new
                {
                    Id = "",
                    Message = "Username or Password is incorrect",
                    Result = -1
                });

            string roleName = companyManagement.GetRoleName(user.Roles.FirstOrDefault().RoleId);

            bool isCorporateAccount = companyManagement.GetAccountType(user.Id);

            if (!customer.IsConfirmEmail)
            {
                if (AppUserManager.IsEmailConfirmed(user.Id))
                {
                    //set last logon time as current datetime
                    companyManagement.UpdateLastLoginTimeAndAduitTrail(user.Id);

                    string userId = user.Id;
                    long tenantId = 0;
                    long companyId = 0;
                    var userName = string.Empty;

                    ProfileManagement profileManagement = new ProfileManagement();

                    var profile = profileManagement.GetUserById(userId);
                    if (profile != null)
                    {
                        tenantId = profile.TenantId;
                        userName = profile.UserName;
                        var company = profileManagement.GetCompanyByTenantId(tenantId);
                        if (company != null)
                        {
                            companyId = company.Id;
                        }
                    }

                    string _token = customerManagement.GetJwtToken(userId, roleName, tenantId.ToString(), userName, companyId.ToString());

                    if (profile.IsActive)
                    {
                        return Ok(new
                        {
                            Id = user.Id,
                            Role = roleName,
                            Result = 1,
                            IsCorporateAccount = isCorporateAccount,
                            token = _token
                        });
                    }
                    else
                    {
                        return Ok(new
                        {
                            Id = "",
                            Message = "Unfortunately your account is inactive, please contact Parcel International",
                            Result = -1
                        });

                    }


                }

                else
                    return Ok(new
                    {
                        Id = "",
                        Message = "This email address is not yet confirmed by you. Please check your inbox and confirm the email address before logging in!",
                        Result = -1
                    });
                //return Ok(new
                //{
                //    Id = user.Id,
                //    Role = roleName,
                //    Result = -11 //You must have a confirmed email to log in
                //});
            }
            else
            {
                IdentityResult result = this.AppUserManager.ConfirmEmail(customer.UserId, customer.Code);
                if (result.Succeeded)
                {
                    //set last logon time as current datetime
                    companyManagement.UpdateLastLoginTimeAndAduitTrail(user.Id);
                    ProfileManagement profileManagement = new ProfileManagement();

                    string userId = user.Id;
                    long tenantId = 0;
                    long companyId = 0;
                    var userName = string.Empty;

                    var profile = profileManagement.GetUserById(userId);
                    if (profile != null)
                    {
                        tenantId = profile.TenantId;
                        userName = profile.UserName;
                        var company = profileManagement.GetCompanyByTenantId(tenantId);
                        if (company != null)
                        {
                            companyId = company.Id;
                        }
                    }

                    string _token = customerManagement.GetJwtToken(userId, roleName, tenantId.ToString(), userName, companyId.ToString());

                    return Ok(new
                    {
                        Id = user.Id,
                        Role = roleName,
                        Result = 2,
                        IsCorporateAccount = isCorporateAccount,
                        token = _token
                    });
                }
                else
                {
                    return Ok(new
                    {
                        Id = user.Id,
                        Role = roleName,
                        Result = -2
                    });
                }
            }
        }





        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [AllowAnonymous]
        [HttpPost]
        [Route("LoginAdmin")]
        public IHttpActionResult LoginAdmin(CustomerDto customer)
        {
            var user = AppUserManager.Find(customer.UserName, customer.Password);

            string roleName = companyManagement.GetRoleName(user.Roles.FirstOrDefault().RoleId);

            if (user == null)
                return Ok(new
                {
                    Id = "",
                    Role = roleName,
                    Result = -1


                });
            else
            {   //set last logon time as current datetime

                ProfileManagement profileManagement = new ProfileManagement();

                string userId = user.Id;
                long tenantId = 0;
                long companyId = 0;
                var userName = string.Empty;

                var profile = profileManagement.GetUserById(userId);
                if (profile != null)
                {
                    tenantId = profile.TenantId;
                    userName = profile.UserName;
                    var company = profileManagement.GetCompanyByTenantId(tenantId);
                    if (company != null)
                    {
                        companyId = company.Id;
                    }
                }

                string _token = customerManagement.GetJwtToken(userId, roleName, tenantId.ToString(), userName, companyId.ToString());



                companyManagement.UpdateLastLoginTimeAndAduitTrail(user.Id);
                return Ok(new
                {
                    Id = user.Id,
                    Role = roleName,
                    Result = 1,
                    token = _token

                });
            }
        }



        // GET api/Account/ExternalLogin
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        [Route("ExternalLogin", Name = "ExternalLogin")]
        public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
        {
            string redirectUri = string.Empty;

            if (error != null)
            {
                return BadRequest(Uri.EscapeDataString(error));
            }

            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(provider, this);
            }

            var redirectUriValidationResult = ValidateClientAndRedirectUri(this.Request, ref redirectUri);

            if (!string.IsNullOrWhiteSpace(redirectUriValidationResult))
            {
                return BadRequest(redirectUriValidationResult);
            }

            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                return InternalServerError();
            }

            if (externalLogin.LoginProvider != provider)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                return new ChallengeResult(provider, this);
            }

            IdentityUser user = await authRepo.FindAsync(new UserLoginInfo(externalLogin.LoginProvider, externalLogin.ProviderKey));

            bool hasRegistered = user != null;

            redirectUri = string.Format("{0}#external_access_token={1}&provider={2}&haslocalaccount={3}&external_user_name={4}",
                                            redirectUri,
                                            externalLogin.ExternalAccessToken,
                                            externalLogin.LoginProvider,
                                            hasRegistered.ToString(),
                                            externalLogin.UserName);

            return Redirect(redirectUri);

        }

        ////// GET api/Account/ExternalLogin
        ////[OverrideAuthentication]
        ////[HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        ////[AllowAnonymous]
        ////[Route("ExternalLogin", Name = "ExternalLogin")]
        ////public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
        ////{
        ////    if (error != null)
        ////    {
        ////        return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
        ////    }

        ////    if (!User.Identity.IsAuthenticated)
        ////    {
        ////        return new ChallengeResult(provider, this);
        ////    }

        ////    ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

        ////    if (externalLogin == null)
        ////    {
        ////        return InternalServerError();
        ////    }

        ////    if (externalLogin.LoginProvider != provider)
        ////    {
        ////        Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
        ////        return new ChallengeResult(provider, this);
        ////    }

        ////    ApplicationUser user = await AppUserManager.FindAsync(new UserLoginInfo(externalLogin.LoginProvider,
        ////        externalLogin.ProviderKey));

        ////    bool hasRegistered = user != null;

        ////    if (hasRegistered)
        ////    {
        ////        Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

        ////        ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(AppUserManager,
        ////           OAuthDefaults.AuthenticationType);
        ////        ClaimsIdentity cookieIdentity = await user.GenerateUserIdentityAsync(AppUserManager,
        ////            CookieAuthenticationDefaults.AuthenticationType);

        ////        AuthenticationProperties properties = CustomOAuthProvider.CreateProperties(user.UserName);
        ////        Authentication.SignIn(properties, oAuthIdentity, cookieIdentity);
        ////    }
        ////    else
        ////    {
        ////        IEnumerable<Claim> claims = externalLogin.GetClaims();
        ////        ClaimsIdentity identity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
        ////        Authentication.SignIn(identity);
        ////    }

        ////    return Ok();
        ////}


        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }
            public string ExternalAccessToken { get; set; }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                string email = "";

                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer) || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                // added the following lines
                if (providerKeyClaim.Issuer == "Facebook")
                {
                    var access_token = identity.FindFirstValue("ExternalAccessToken");
                    var fb = new FacebookClient(access_token);
                    dynamic myInfo = fb.Get("/me?fields=email"); // specify the email field
                    email = myInfo.email;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = (providerKeyClaim.Issuer == "Facebook") ? email : identity.FindFirstValue(ClaimTypes.Email),
                    // UserName = identity.FindFirstValue(ClaimTypes.Name),
                    ExternalAccessToken = identity.FindFirstValue("ExternalAccessToken"),
                };
            }
        }


        private string ValidateClientAndRedirectUri(HttpRequestMessage request, ref string redirectUriOutput)
        {

            Uri redirectUri;

            var redirectUriString = GetQueryString(Request, "redirect_uri");

            if (string.IsNullOrWhiteSpace(redirectUriString))
            {
                return "redirect_uri is required";
            }

            bool validUri = Uri.TryCreate(redirectUriString, UriKind.Absolute, out redirectUri);

            if (!validUri)
            {
                return "redirect_uri is invalid";
            }

            var clientId = GetQueryString(Request, "client_id");

            if (string.IsNullOrWhiteSpace(clientId))
            {
                return "client_Id is required";
            }

            var client = authRepo.FindClient(clientId);

            if (client == null)
            {
                return string.Format("Client_id '{0}' is not registered in the system.", clientId);
            }

            if (!string.Equals(client.AllowedOrigin, redirectUri.GetLeftPart(UriPartial.Authority), StringComparison.OrdinalIgnoreCase))
            {
                return string.Format("The given URL is not allowed by Client_id '{0}' configuration.", clientId);
            }

            redirectUriOutput = redirectUri.AbsoluteUri;

            return string.Empty;

        }


        private string GetQueryString(HttpRequestMessage request, string key)
        {
            var queryStrings = request.GetQueryNameValuePairs();

            if (queryStrings == null) return null;

            var match = queryStrings.FirstOrDefault(keyValue => string.Compare(keyValue.Key, key, true) == 0);

            if (string.IsNullOrEmpty(match.Value)) return null;

            return match.Value;
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [AllowAnonymous]
        [Route("resetForgetPassword")]
        public int ResetForgetPassword(CustomerDto userModel)
        {
            ApplicationUser existingUser = AppUserManager.FindByName(userModel.Email);
            if (existingUser == null)
            {
                return -1; // No account find by this email.
            }
            //else
            //{
            //    if (!AppUserManager.IsEmailConfirmed(existingUser.Id))
            //    {
            //        // user hasn't confirm his email yet. So user can't reset password.
            //        return -11;
            //    }
            //}

            var passwordResetToken = AppUserManager.GeneratePasswordResetToken(existingUser.Id);

            var callbackUrl = new Uri(Url.Content(ConfigurationManager.AppSettings["BaseWebURL"] + @"app/resetPassword/resetPassword.html?userId=" + existingUser.Id + "&code=" + passwordResetToken));

            StringBuilder emailbody = new StringBuilder(userModel.TemplateLink);

            emailbody.Replace("ActivationURL", "<a style=\"color:#80d4ff\" href=\"" + callbackUrl + "\">here</a>");

            AppUserManager.SendEmail(existingUser.Id, "Reset your account password", emailbody.ToString());

            return 1;
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [AllowAnonymous]
        [HttpPost]
        [Route("resetForgetPasswordConfirm")]
        public int ResetForgetPasswordConfirm(CustomerDto customer)
        {
            if (string.IsNullOrWhiteSpace(customer.UserId) || string.IsNullOrWhiteSpace(customer.Code) || string.IsNullOrWhiteSpace(customer.Password))
            {
                ModelState.AddModelError("", "User Id, Code and Password are required");
                return -1;
            }
            string code = AppUserManager.GenerateEmailConfirmationToken(customer.UserId);
            IdentityResult resultEmail = this.AppUserManager.ConfirmEmail(customer.UserId, code);
            IdentityResult result = this.AppUserManager.ResetPassword(customer.UserId, customer.Code, customer.Password);


            if (result.Succeeded && resultEmail.Succeeded)
            {
                return 1;
            }
            else
            {
                return -2;
            }
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [AllowAnonymous]
        [HttpPost]
        [Route("ValidateUserToken")]
        public bool ValidateUserToken(CustomerDto customer)
        {
            bool isValid = this.AppUserManager.VerifyUserToken(customer.UserId, "ValidateUserToken", customer.Code);
            return isValid;
        }

        // User Management

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpPost]
        [Route("DeleteUser")]
        public int DeleteUser([FromBody] CustomerDto customerDto)
        {
            return 1;
            //return companyManagement.DeleteDivision(division.Id);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpGet]
        [Route("GetAllRolesByUser")]
        public List<RolesDto> GetAllRolesByUser(string userId)
        {
            return companyManagement.GetAllActiveChildRoles(userId);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpGet]
        [Route("GetUsersByFilter")]
        public PagedList GetUsersByFilter(long division, string role, string userId, string status, string searchtext = "")
        {
            return companyManagement.GetAllUsers(division, role, userId, status, searchtext);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpPost]
        [Route("SaveUser")]
        public int SaveUser([FromBody] UserDto user)
        {
            UserResultDto result = companyManagement.SaveUser(user);

            // Existing email address
            if (!result.IsSucess)
            {
                return -1;
            }

            AssignRolesToUser(result.UserId, new string[1] { user.AssignedRoleName });

            if (result.IsAddUser)
            {

                AppUserManager.AddPassword(result.UserId, user.Password);

                #region For Email Confirmaion

                string code = AppUserManager.GenerateEmailConfirmationToken(result.UserId);
                var callbackUrl = new Uri(Url.Content(ConfigurationManager.AppSettings["BaseWebURL"] + @"app/userLogin/userlogin.html?userId=" + result.UserId + "&code=" + code));

                StringBuilder emailbody = new StringBuilder(user.TemplateLink);
                emailbody.Replace("FirstName", user.FirstName).Replace("LastName", user.LastName).Replace("Salutation", user.Salutation + ".")
                                            .Replace("ActivationURL", "<a style=\"color:#80d4ff\" href=\"" + callbackUrl + "\">here</a>");

                AppUserManager.SendEmail(result.UserId, "Parcel International – Activate your account", emailbody.ToString());

                #endregion
            }

            return 1;
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpGet]
        [Route("GetUserByUserId")]
        public UserDto GetUserByUserId(string userId, string loggedInUser)
        {
            return companyManagement.GetUserById(userId, loggedInUser);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpGet]
        [Route("LoadUserManagement")]
        public UserDto LoadUserManagement(string loggedInUser)
        {
            return companyManagement.LoadUserManagement(loggedInUser);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpGet]
        [Route("GetLoggedInUserName")]
        public string GetLoggedInUserName(string loggedInUserId)
        {
            return companyManagement.GetLoggedInUserName(loggedInUserId);
        }


        #region Helpers
        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        #endregion
    }
}
