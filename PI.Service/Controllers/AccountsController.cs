using Microsoft.AspNet.Identity;
using Newtonsoft.Json.Linq;
using PI.Business;
using PI.Contract.DTOs.Common;
using PI.Contract.DTOs.Company;
using PI.Contract.DTOs.CostCenter;
using PI.Contract.DTOs.Customer;
using PI.Contract.DTOs.Division;
using PI.Contract.DTOs.User;
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
    [RoutePrefix("api/accounts")]
    public class AccountsController : BaseApiController
    {
        [Authorize(Roles = "Admin")]
        [Route("users")]
        public IHttpActionResult GetUsers()
        {
            return Ok(this.AppUserManager.Users.ToList().Select(u => this.TheModelFactory.Create(u)));
        }

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
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
            };

            ApplicationUser existingUser = AppUserManager.FindByName(createUserModel.Email);
            if (existingUser == null)
            {
                IdentityResult addUserResult = AppUserManager.Create(user, createUserModel.Password);
                createUserModel.UserId = user.Id;
            }
            else
            {
                  return -2;
                //return GetErrorResult(IdentityResult.Failed("Email already exists!"));
            }

            // Save in customer table.
            CustomerManagement customerManagement = new CustomerManagement();
            customerManagement.SaveCustomer(createUserModel);

            //Create Tenant, Default Company, Division & CostCenter 
            CompanyController companyManagement = new CompanyController();
            long tenantId = companyManagement.CreateCompanyDetails(createUserModel);

            // Add tenant Id to user
            user.TenantId = tenantId;

            // Add Business Owner Role to user
            AppUserManager.AddToRole(user.Id, "SuperAdmin");

            AppUserManager.Update(user);


            
            #region For Email Confirmaion

            string code =  AppUserManager.GenerateEmailConfirmationToken(user.Id);
            //string baseUri = new Uri(Request.RequestUri.AbsoluteUri.Replace(Request.RequestUri.PathAndQuery, String.Empty));
            var callbackUrl = new Uri(Url.Content(ConfigurationManager.AppSettings["BaseWebURL"] + @"app/userLogin/userlogin.html?userId=" + user.Id + "&code=" + code));

            StringBuilder emailbody = new StringBuilder(createUserModel.TemplateLink);
            emailbody.Replace("FirstName", user.FirstName).Replace("LastName", user.LastName).Replace("Salutation", user.Salutation +".")
                                        .Replace("ActivationURL", "<a href=\"" + callbackUrl + "\">here</a>");

            AppUserManager.SendEmail(user.Id, "Your account has been provisioned!", emailbody.ToString());

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

        [Authorize]
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

        [Authorize(Roles = "Admin")]
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


        [Authorize(Roles = "Admin")]
        [Route("user/{id:guid}/roles")]
        [HttpPut]
        public async Task<IHttpActionResult> AssignRolesToUser([FromUri] string id, [FromBody] string[] rolesToAssign)
        {

            var appUser = await this.AppUserManager.FindByIdAsync(id);

            if (appUser == null)
            {
                return NotFound();
            }

            var currentRoles = await this.AppUserManager.GetRolesAsync(appUser.Id);

            var rolesNotExists = rolesToAssign.Except(this.AppRoleManager.Roles.Select(x => x.Name)).ToArray();

            if (rolesNotExists.Count() > 0)
            {

                ModelState.AddModelError("", string.Format("Roles '{0}' does not exixts in the system", string.Join(",", rolesNotExists)));
                return BadRequest(ModelState);
            }

            IdentityResult removeResult = await this.AppUserManager.RemoveFromRolesAsync(appUser.Id, currentRoles.ToArray());

            if (!removeResult.Succeeded)
            {
                ModelState.AddModelError("", "Failed to remove user roles");
                return BadRequest(ModelState);
            }

            IdentityResult addResult = await this.AppUserManager.AddToRolesAsync(appUser.Id, rolesToAssign);

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

            if (user == null)
                return Ok(new
                {
                    User = user,
                    Result = -1
                });
            else if (!customer.IsConfirmEmail)
            {
                if(AppUserManager.IsEmailConfirmed(user.Id))
                    return Ok(new
                     {
                         User = user,
                         Result = 1
                     });
                else
                    return Ok(new
                    {
                        User = user,
                        Result = -11 //You must have a confirmed email to log in
                    });
            }
            else
            {
                IdentityResult result = this.AppUserManager.ConfirmEmail(customer.UserId, customer.Code);
                if (result.Succeeded)
                {
                    return Ok(new
                    {
                        User = user,
                        Result = 2
                    });
                }
                else
                {
                    return Ok(new
                    {
                        User = user,
                        Result = 2
                    });
                }
            }

            //if (user == null)
            //    return -1;
            //else if (!customer.IsConfirmEmail)
            //    return 1;
            //else
            //{
            //    IdentityResult result = this.AppUserManager.ConfirmEmail(customer.UserId, customer.Code);
            //    if (result.Succeeded)
            //        return 2;
            //    else
            //        return -2;
            //}
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
            emailbody.Replace("FirstName", existingUser.FirstName).Replace("LastName", existingUser.LastName)
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
        public string GetAllRolesByUser(string userId)    // TODO : Change the string to RoleDto
        {
            //IList<DivisionDto> divisionList = companyManagement.GetAllDivisionsForCompany(userId);
            //return divisionList;
            return null;
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpGet]
        [Route("GetUsersByFilter")]
        public PagedList GetUsersByFilter(long division, string type, string userId, string status, string searchtext = "")
        {
            //var pagedRecord = new PagedList();
            //return companyManagement.GetAllDivisions(costCenter, type, userId, searchtext, page, pageSize, sortBy, sortDirection);
            return null;
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpPost]
        [Route("SaveUser")]
        public int SaveUser([FromBody] UserDto user)
        {
            return 1;
            //return companyManagement.SaveDivision(division);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        // [Authorize]
        [HttpGet]
        [Route("GetUserByUserId")]
        public UserDto GetUserByUserId(string userId)
        {
            return null;
        }
    }
}
