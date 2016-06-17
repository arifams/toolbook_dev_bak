using Microsoft.AspNet.Identity;
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

namespace PI.Service.Controllers
{
    [RoutePrefix("api/accounts")]
    public class AccountsController : BaseApiController
    {
        ICompanyManagement companyManagement;
        ICustomerManagement customerManagement;

        public AccountsController(ICompanyManagement companymanagement, ICustomerManagement customermanagement)
        {
            this.companyManagement = companymanagement;
            this.customerManagement = customermanagement;

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
                Salutation = createUserModel.Salutation,
                FirstName = createUserModel.FirstName,
                LastName = createUserModel.LastName,
                Level = 3,
                JoinDate = DateTime.Now.Date,
                IsActive = true
            };

            ApplicationUser existingUser = AppUserManager.FindByName(createUserModel.Email);
            if (existingUser == null)
            {

                //Create Tenant, Default Company, Division & CostCenter 
                //CompanyController companyManagement = new CompanyController();
                long tenantId = companyManagement.CreateCompanyDetails(createUserModel);

                // Add tenant Id to user
                user.TenantId = tenantId;
                //user.Customer = new Data.Entity.Customer();

                IdentityResult addUserResult = AppUserManager.Create(user, createUserModel.Password);

                createUserModel.UserId = user.Id;

                // Save in customer table.
                CustomerManagement customerManagement = new CustomerManagement();
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



            #region For Email Confirmaion

            string code = AppUserManager.GenerateEmailConfirmationToken(user.Id);
            //string baseUri = new Uri(Request.RequestUri.AbsoluteUri.Replace(Request.RequestUri.PathAndQuery, String.Empty));
            var callbackUrl = new Uri(Url.Content(ConfigurationManager.AppSettings["BaseWebURL"] + @"app/userLogin/userlogin.html?userId=" + user.Id + "&code=" + code));

            StringBuilder emailbody = new StringBuilder(createUserModel.TemplateLink);
            emailbody.Replace("FirstName", user.FirstName).Replace("LastName", user.LastName).Replace("Salutation", user.Salutation + ".")
                                        .Replace("ActivationURL", "<a href=\"" + callbackUrl + "\">here</a>");

            AppUserManager.SendEmail(user.Id, "Parcel International – Activate your account", emailbody.ToString());

            #endregion

            Uri locationHeader = new Uri(Url.Link("GetUserById", new { id = user.Id }));

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
            var user = AppUserManager.Find(customer.UserName, customer.Password);

            Trace.WriteLine("MessageJUST"); // Write a verbose message
            Trace.TraceInformation("MessageINFO"); // Write an information message
            Trace.TraceWarning("MessageWARNING");
            Trace.TraceError("MessageERROR");

            if (user == null)
                return Ok(new
                {
                    Id = "",
                    Message = "Username or Password is incorrect",
                    Result = -1
                });

            //var currentRoles = await this.AppUserManager.GetRolesAsync(user.Id);
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

                   // CustomerManagement customerManagement = new CustomerManagement();
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
                        Message = "Email hasn't confirmed. Please confirm the email before log to the system",
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
                   // CustomerManagement customerManagement = new CustomerManagement();
                    ProfileManagement profileManagement = new ProfileManagement();

                    string userId = user.Id;
                    long tenantId = 0;
                    long companyId = 0;
                    var userName = string.Empty;

                    var profile = profileManagement.GetUserById(userId);
                    if (profile!=null)
                    {
                        tenantId = profile.TenantId;
                        userName = profile.UserName;
                        var company = profileManagement.GetCompanyByTenantId(tenantId);
                        if (company!=null)
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
                        token= _token
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

            //var currentRoles = await this.AppUserManager.GetRolesAsync(user.Id);
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

               // CustomerManagement customerManagement = new CustomerManagement();
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
            else
            {
                if (!AppUserManager.IsEmailConfirmed(existingUser.Id))
                {
                    // user hasn't confirm his email yet. So user can't reset password.
                    return -11;
                }
            }

            var passwordResetToken = AppUserManager.GeneratePasswordResetToken(existingUser.Id);

            var callbackUrl = new Uri(Url.Content(ConfigurationManager.AppSettings["BaseWebURL"] + @"app/resetPassword/resetPassword.html?userId=" + existingUser.Id + "&code=" + passwordResetToken));

            StringBuilder emailbody = new StringBuilder(userModel.TemplateLink);
            emailbody.Replace("Salutation", existingUser.Salutation).Replace("FirstName", existingUser.FirstName).Replace("LastName", existingUser.LastName)
                                        .Replace("ActivationURL", "<a href=\"" + callbackUrl + "\">here</a>");

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

            IdentityResult result = this.AppUserManager.ResetPassword(customer.UserId, customer.Code, customer.Password);
            
            if (result.Succeeded)
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
        public List<RolesDto> GetAllRolesByUser(string userId)    // TODO : Change the string to RoleDto
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

            string[] rolList = AppUserManager.GetRoles(result.UserId).ToArray();
            //AppUserManager.RemoveFromRoles(userId, rolList);
            AssignRolesToUser(result.UserId, new string[1] { user.AssignedRoleName });
            
            if (result.IsAddUser) {

                AppUserManager.AddPassword(result.UserId, user.Password);

                #region For Email Confirmaion

                string code = AppUserManager.GenerateEmailConfirmationToken(result.UserId);
                //string baseUri = new Uri(Request.RequestUri.AbsoluteUri.Replace(Request.RequestUri.PathAndQuery, String.Empty));
                var callbackUrl = new Uri(Url.Content(ConfigurationManager.AppSettings["BaseWebURL"] + @"app/userLogin/userlogin.html?userId=" + result.UserId + "&code=" + code));

                StringBuilder emailbody = new StringBuilder(user.TemplateLink);
                emailbody.Replace("FirstName", user.FirstName).Replace("LastName", user.LastName).Replace("Salutation", user.Salutation + ".")
                                            .Replace("ActivationURL", "<a href=\"" + callbackUrl + "\">here</a>");

                AppUserManager.SendEmail(result.UserId, "Parcel International – Activate your account", emailbody.ToString());

                #endregion
            }

            return 1;
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpGet]
        [Route("GetUserByUserId")]
        public UserDto GetUserByUserId(string userId,string loggedInUser)
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
    }
}
