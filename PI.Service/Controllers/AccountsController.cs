using Facebook;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using PI.Business;
using PI.Contract;
using PI.Contract.Business;
using PI.Contract.DTOs.Common;
using PI.Contract.DTOs.Customer;
using PI.Contract.DTOs.Role;
using PI.Contract.DTOs.User;
using PI.Data.Entity.Identity;
using PI.Service.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using PI.Service.Results;
using Twilio;
using System.Data.Entity.Validation;
using PI.Contract.TemplateLoader;
using HtmlAgilityPack;

namespace PI.Service.Controllers
{
    [RoutePrefix("api/accounts")]
    public class AccountsController : BaseApiController
    {
        readonly ICompanyManagement companyManagement;
        readonly ICustomerManagement customerManagement;
        private AuthRepository authRepo = null;
        private ProfileManagement profileManagement;    // TODO : H - Change this to interface
        TemplateLoader templateLoader = new TemplateLoader();


        public AccountsController(ICompanyManagement companymanagement, ICustomerManagement customermanagement, ILogger logger, ProfileManagement profileManagement)
        {
            this.companyManagement = companymanagement;
            this.customerManagement = customermanagement;
            authRepo = new AuthRepository();
            this.profileManagement = profileManagement;
            logger.SetType(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
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
        public IHttpActionResult CreateUser(CustomerDto createUserModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                ApplicationUser existingUser = AppUserManager.FindByName(createUserModel.Email);

                if (existingUser != null && existingUser.EmailConfirmed)
                {
                    // User existing and already confirmed the email address.
                    return BadRequest("Email address is already in use!");
                }


                bool isUserExistAndOldAccount = existingUser != null && existingUser.JoinDate.AddHours(24) < DateTime.UtcNow && !existingUser.EmailConfirmed;

                if (!createUserModel.viaExternalLogin && isUserExistAndOldAccount)
                {
                    // Account is older than 24 hour.
                    // Delete account and all associated records.
                    customerManagement.DeleteCustomer(existingUser.Id);
                    AppUserManager.Delete(existingUser);
                    companyManagement.DeleteCompanyDetails(existingUser.TenantId, existingUser.Id);
                }

                if (existingUser == null)
                {
                    var user = new ApplicationUser()
                    {
                        UserName = createUserModel.Email,
                        Email = createUserModel.Email,
                        Salutation = "Mr",
                        FirstName = createUserModel.FirstName,
                        LastName = createUserModel.LastName,
                        Level = 3,
                        JoinDate = DateTime.UtcNow,
                        IsActive = true
                    };

                    if (existingUser == null || isUserExistAndOldAccount)
                    {
                        //Create Tenant, Default Company, Division & CostCenter 
                        createUserModel.CustomerAddress = new Contract.DTOs.Address.AddressDto();

                        //Currently only corporate users will exist in the system.
                        createUserModel.IsCorporateAccount = true;
                        long tenantId = companyManagement.CreateCompanyDetails(createUserModel);

                        // Add tenant Id to user
                        user.TenantId = tenantId;
                        user.EmailConfirmed = createUserModel.viaExternalLogin ? true : false;

                        IdentityResult addUserResult = createUserModel.viaExternalLogin ? AppUserManager.Create(user) :
                                                                                          AppUserManager.Create(user, createUserModel.Password);

                        createUserModel.UserId = user.Id;

                        // Save in customer table.
                        customerManagement.SaveCustomer(createUserModel,true);
                    }
                    
                    // Add Business Owner Role to user
                    AppUserManager.AddToRole(user.Id, "BusinessOwner");

                    AppUserManager.Update(user);

                    if (!createUserModel.viaExternalLogin)
                    {
                        #region For Email Confirmaion

                        string code = AppUserManager.GenerateEmailConfirmationToken(user.Id);
                        var callbackUrl = new Uri(Url.Content(ConfigurationManager.AppSettings["BaseWebURL"] + @"app/userLogin/userlogin.html?userId=" + user.Id + "&code=" + code));

                        //get the email template for invoice
                        HtmlDocument template = templateLoader.getHtmlTemplatebyName("RegistrationEmailTemplate");
                        string emailbody =  template.DocumentNode.InnerHtml;

                        //StringBuilder emailbody = new StringBuilder(createUserModel.TemplateLink);
                        var updatedString = emailbody.Replace("FirstName", user.FirstName).Replace("LastName", user.LastName).Replace("Salutation", createUserModel.Salutation)
                                           .Replace("ActivationURL", "<a style=\"color:#80d4ff\" href=\"" + callbackUrl + "\">here</a>");

                        AppUserManager.SendEmail(user.Id, "Parcel International – Activate your account", updatedString);

                        #endregion

                    }
                }
                else
                {
                    return BadRequest("Email address is already in use!");
                }

                //Uri locationHeader = new Uri(Url.Link("GetUserById", new { id = user.Id }));

            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            return Ok();

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


        [AllowAnonymous]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        [Route("GetNewSignedToken")]
        public async Task<IHttpActionResult> GetNewSignedToken(string currentToken)
        {
            return  Ok(customerManagement.GetJwtTokenFromCurrentToken(currentToken));
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

            //SendSMS(customer.Email);

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
                {
                    if (user.JoinDate.AddHours(24) < DateTime.UtcNow)
                    {
                        // user account is expired
                        return Ok(new
                        {
                            Id = "",
                            Message = "Invalid user account",
                            Result = -1
                        });
                    }
                    else
                    {
                        return Ok(new
                        {
                            Id = "",
                            Message = "This email address is not yet confirmed by you. Please check your inbox and confirm the email address before logging in!",
                            Result = -1
                        });
                    }

                }

            }
            else
            {
                IdentityResult result = this.AppUserManager.ConfirmEmail(customer.UserId, customer.Code);
                if (result.Succeeded)
                {
                    //set last logon time as current datetime
                    companyManagement.UpdateLastLoginTimeAndAduitTrail(user.Id);

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
                    Result = -1
                });
            else
            {   //set last logon time as current datetime
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

            redirectUri = string.Format("{0}#external_access_token={1}&provider={2}&haslocalaccount={3}&external_user_name={4}&external_first_name={5}&external_last_name={6}",
                                            redirectUri,
                                            externalLogin.ExternalAccessToken,
                                            externalLogin.LoginProvider,
                                            hasRegistered.ToString(),
                                            externalLogin.UserName,
                                            externalLogin.FirstName,
                                            externalLogin.LastName);

            return Redirect(redirectUri);

        }


        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string ExternalAccessToken { get; set; }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                string email = "", firstname = "", lastname = "";

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
                    dynamic myInfo = fb.Get("/me?fields=email,first_name,last_name"); // specify the email field
                    email = myInfo.email;
                    firstname = myInfo.first_name;
                    lastname = myInfo.last_name;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = (providerKeyClaim.Issuer == "Facebook") ? email : identity.FindFirstValue(ClaimTypes.Email),
                    FirstName = (providerKeyClaim.Issuer == "Facebook") ? firstname : identity.FindFirstValue(ClaimTypes.GivenName),
                    LastName = (providerKeyClaim.Issuer == "Facebook") ? lastname : identity.FindFirstValue(ClaimTypes.Surname),
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
        public IHttpActionResult ResetForgetPassword(CustomerDto userModel)
        {
            ApplicationUser existingUser = AppUserManager.FindByName(userModel.Email);
            if (existingUser == null)
            {
                return BadRequest("No account found by this email. Please enter the registered Email"); // No account find by this email.
            }

            var passwordResetToken = AppUserManager.GeneratePasswordResetToken(existingUser.Id);

            var callbackUrl = new Uri(Url.Content(ConfigurationManager.AppSettings["BaseWebURL"] + @"app/resetPassword/resetPassword.html?userId=" + existingUser.Id + "&code=" + passwordResetToken));

            StringBuilder emailbody = new StringBuilder(userModel.TemplateLink);

            emailbody.Replace("FirstName", existingUser.FirstName).Replace("LastName", existingUser.LastName)
                                        .Replace("ActivationURL", "<a style=\"color:#80d4ff\" href=\"" + callbackUrl + "\">here</a>");

            AppUserManager.SendEmail(existingUser.Id, "Reset your account password", emailbody.ToString());

            return Ok();
        }


        [CustomAuthorize]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [AllowAnonymous]
        [HttpPost]
        [Route("ResetForgetPasswordConfirm")]
        public IHttpActionResult ResetForgetPasswordConfirm(CustomerDto customer)
        {
            if (string.IsNullOrWhiteSpace(customer.UserId) || string.IsNullOrWhiteSpace(customer.Code) || string.IsNullOrWhiteSpace(customer.Password))
            {
                return BadRequest("Valid token and password required!");
            }

            string code = AppUserManager.GenerateEmailConfirmationToken(customer.UserId);
            IdentityResult resultEmail = this.AppUserManager.ConfirmEmail(customer.UserId, code);
            IdentityResult result = this.AppUserManager.ResetPassword(customer.UserId, customer.Code, customer.Password);

            if (result.Succeeded && resultEmail.Succeeded)
            {
                return Ok();
            }
            else
            {
                return BadRequest("Invalid token. Please resend the password reset URL.");
            }
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [AllowAnonymous]
        [HttpPost]
        [Route("ValidateUserToken")]
        public IHttpActionResult ValidateUserToken(CustomerDto customer)
        {
            return Ok(this.AppUserManager.VerifyUserToken(customer.UserId, "ValidateUserToken", customer.Code));
        }


        // User Management
        [CustomAuthorize]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        [Route("GetAllRolesByUser")]
        public IHttpActionResult GetAllRolesByUser(string userId)
        {
            return Ok(companyManagement.GetAllActiveChildRoles(userId));
        }

        [CustomAuthorize]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        [Route("GetUsersByFilter")]
        public IHttpActionResult GetUsersByFilter(string role, string loggedInuserId, string status, string searchtext = "")
        {
            return Ok(companyManagement.GetAllUsers(role, loggedInuserId, status, searchtext));
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("SaveUser")]
        public IHttpActionResult SaveUser([FromBody] UserDto user)
        {
            UserResultDto result = companyManagement.SaveUser(user);

            // Existing email address
            if (!result.IsSucess)
            {
                return BadRequest("There is already an user with the same email address");
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

            return Ok();
        }


        [CustomAuthorize]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        [Route("GetUserByUserId")]
        public IHttpActionResult GetUserByUserId(string userId, string loggedInUser)
        {
            return Ok(companyManagement.GetUserById(userId, loggedInUser));
        }

        [CustomAuthorize]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        [Route("LoadUserManagement")]
        public IHttpActionResult LoadUserManagement(string loggedInUser)
        {
            return Ok(companyManagement.LoadUserManagement(loggedInUser));
        }

        [CustomAuthorize]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        [Route("GetLoggedInUserName")]
        public IHttpActionResult GetLoggedInUserName(string loggedInUserId)
        {
            return Ok(companyManagement.GetLoggedInUserName(loggedInUserId));
        }


        #region TFA
        [AllowAnonymous]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        [Route("IsPhoneNumberVerified")]
        public IHttpActionResult IsPhoneNumberVerified(string email)
        {
            var user = this.AppUserManager.FindByName(email);

            if (user == null)
            {
                return Ok(new
                {
                    Message = "Email is not confirmed!",
                    Result = -1
                });
            }
            return Ok(new
            {
                Result = user.PhoneNumberConfirmed ? 1 : 0
            });
        }


        [AllowAnonymous]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("SendOPTCodeForPhoneValidation")]
        public IHttpActionResult SendOPTCodeForPhoneValidation(UserDto userDetails)
        {
            Random generator = new Random();
            string code = generator.Next(100000, 999999).ToString("D6"); // Security code

            try
            {
                if (!userDetails.isViaProfileSettings)
                {
                    var customer = profileManagement.GetCustomerByUserEmail(userDetails.Email);
                    userDetails.MobileNumber = customer.MobileNumber;
                }

                // var accountSid = "ACe8df3ab4cb9ad89edca435a14f8bb922"; // Your Account SID from www.twilio.com/console
                // var authToken = "1bd4508f4bdb95f110a4360c4805ee65";  // Your Auth Token from www.twilio.com/console

                var accountSid = "ACbed90a44fddfdd6047d7e1aa24aafee2"; // Prod account
                var authToken = "abbe2c853c29d366a9679886de7fa2d2";  // Prod token

                var twilio = new TwilioRestClient(accountSid, authToken);

                var message = twilio.SendMessage(
                    "+3197004498550", // fromPhone
                     userDetails.MobileNumber, // To (Replace with your phone number)
                    "Your security code is: " + code
                    );

                //Store the security code and the time in DB.
                companyManagement.SaveUserPhoneCode(new UserDto
                {
                    Email = userDetails.Email,
                    MobileVerificationCode = code,
                    MobileNumber = userDetails.MobileNumber
                });

                if (message.RestException != null)
                {
                    return BadRequest(message.RestException.Message);
                }

                return Ok();
            }
            catch (Exception ex) { throw ex; }
        }

        public bool CheckResendOPTCode(string email)
        {
            var user = this.AppUserManager.FindByName(email);

            bool resendAllowed = (!string.IsNullOrWhiteSpace(user.MobileVerificationCode) &&
                                  user.MobileVerificationExpiry.GetValueOrDefault().CompareTo(DateTime.UtcNow) < 30) ?
                                  false : true;

            return resendAllowed;
        }


        [AllowAnonymous]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("VerifyPhoneCode")]
        public IHttpActionResult VerifyPhoneCode(UserDto userDetails)
        {
            var user = this.AppUserManager.FindByName(userDetails.Email);
            if (user.MobileVerificationCode == userDetails.MobileVerificationCode)
            {
                if (userDetails.isViaProfileSettings)
                {
                    //Store the security code and the time in DB.
                    companyManagement.SaveUserPhoneConfirmation(new UserDto { Email = userDetails.Email });
                }
                return Ok(true);
            }
            else
            {
                return Ok(false);
            }
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [AllowAnonymous]
        [HttpPost]
        [Route("ResetForgetPasswordViaPhone")]
        public IHttpActionResult ResetForgetPasswordViaPhone(UserDto userDtails)
        {
            var user = this.AppUserManager.FindByName(userDtails.Email);

            string code = AppUserManager.GeneratePasswordResetToken(user.Id);
            IdentityResult result = this.AppUserManager.ResetPassword(user.Id, code, userDtails.Password);

            if (result.Succeeded)
            {
                return Ok();
            }
            return BadRequest();
        }

        #endregion

        //public async Task<bool> SendTwoFactorCode(string provider)
        //{
        //    var userId = await GetVerifiedUserIdAsync();
        //    if (userId == null)
        //    {
        //        return false;
        //    }

        //    var token = await this.AppUserManager.GenerateTwoFactorTokenAsync(userId, provider);
        //    // See IdentityConfig.cs to plug in Email/SMS services to actually send the code
        //    await this.AppUserManager.NotifyTwoFactorTokenAsync(userId, provider, token);
        //    return true;
        //}


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
