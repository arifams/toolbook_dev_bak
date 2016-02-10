using Microsoft.AspNet.Identity;
using Newtonsoft.Json.Linq;
using PI.Business;
using PI.Contract.DTOs.Customer;
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
        public async Task<IHttpActionResult> CreateUser(CustomerDto createUserModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new ApplicationUser()
            {
                UserName = createUserModel.Email,
                Email = createUserModel.Email,
                FirstName = createUserModel.FirstName,
                LastName = createUserModel.LastName,
                Level = 3,
                JoinDate = DateTime.Now.Date,
            };

            IdentityResult addUserResult = await this.AppUserManager.CreateAsync(user, createUserModel.Password);

            if (!addUserResult.Succeeded)
            {
                return GetErrorResult(addUserResult);
            } 

            // Save in customer table.
            CustomerManagement customerManagement = new CustomerManagement();
            customerManagement.SaveCustomer(createUserModel);

            #region For Email Confirmaion

            string code = await this.AppUserManager.GenerateEmailConfirmationTokenAsync(user.Id);
            //string baseUri = new Uri(Request.RequestUri.AbsoluteUri.Replace(Request.RequestUri.PathAndQuery, String.Empty));
            var callbackUrl = new Uri(Url.Content(ConfigurationManager.AppSettings["BaseWebURL"] + @"app/userLogin/userlogin.html?userId=" + user.Id + "&code=" + code));
            
            StringBuilder emailbody = new StringBuilder(createUserModel.TemplateLink);
            emailbody.Replace("FirstName", user.FirstName).Replace("LastName", user.LastName)
                                        .Replace("ActivationURL", "<a href=\"" + callbackUrl + "\">here</a>");

            await this.AppUserManager.SendEmailAsync(user.Id, "Your account has been provisioned!", emailbody.ToString());

            #endregion
            
            Uri locationHeader = new Uri(Url.Link("GetUserById", new { id = user.Id }));

            return Created(locationHeader, TheModelFactory.Create(user));
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
        public int LoginUser(JObject customerJObject)
        {
            string username, password, userId, code, isConfirmEmail;


            string o = customerJObject.ToString();

            string p = o.Replace("{", "");
            p = p.Replace("\"", "");

            p = p.Replace("\r\n", string.Empty);
            p = p.Replace("\\", string.Empty);
            p = p.Replace("}:", string.Empty);
            p = p.Replace("}", string.Empty);

            string[] splitAsObject = p.Split(',');

            // username 
            string[] splituserName = splitAsObject[0].Split(':');
            username = splituserName[1];

            // password 
            string[] splitPassword = splitAsObject[1].Split(':');
            password = splitPassword[1];

            // userId 
            string[] splitUserId = splitAsObject[2].Split(':');
            userId = splitUserId[1];

            //code
            string[] splitCode = splitAsObject[3].Split(':');
            code = splitCode[1];
            code = code.Replace(" ", "+");

            //isConfirmEmail
            string[] splitIsConfirmEmail = splitAsObject[4].Split(':');
            isConfirmEmail = splitIsConfirmEmail[1];
            
            var user = AppUserManager.Find(username, password);

            if (user == null)
                return -1;
            else if (isConfirmEmail == "False")
                return 1;
            else
            {
                IdentityResult result = this.AppUserManager.ConfirmEmail(userId, code);
                if (result.Succeeded)
                    return 2;
                else
                    return -2;
            }

            //dynamic json = customerJObject;
            //JObject jalbum = json.Test1;

            //var album = jalbum.ToObject<CustomerDto>();

            //string a = album.Code;

            //string ss = customerJObject.ChildrenTokens[0].ToString();

            //string ss = customerJObject.selec

            //JObject jalbum1 = json[0] as JObject;

            //JToken token = customerJObject;

            //string page = token.SelectToken("username").ToString();
            //string totalPages = token.SelectToken("code").ToString();

            //string g = (string)customerJObject["First"][0]["username"];





        }

        //[EnableCors(origins: "*", headers: "*", methods: "*")]
        //[HttpPost]
        //[AllowAnonymous]
        //[Route("LoginUser")]
        //public int LoginUser(JObject customer)
        //{
        //    //var user = AppUserManager.Find(customer.UserName, customer.Password);

        //    //CustomerManagement customerManagement = new CustomerManagement();
        //    //return customerManagement.VerifyUserLogin(customer);
        //    return 1;
        //}

    }
}
