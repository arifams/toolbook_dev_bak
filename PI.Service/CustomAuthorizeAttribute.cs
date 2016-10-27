using Microsoft.AspNet.Identity;
using PI.Business;
using PI.Contract.DTOs;
using PI.Data.Entity.Identity;
using PI.Service.Controllers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace PI.Service
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        public string WebURL
        {
            get
            {
                return ConfigurationManager.AppSettings["BaseWebURL"].ToString();
            }
        }

        public string ServiceURL
        {
            get
            {
                return ConfigurationManager.AppSettings["ServiceURL"].ToString();
            }
        }

        private static bool SkipAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            // Contract.Assert(actionContext != null);

            return actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any()
                       || actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();
        }

        //overriding on authorization method
        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            HttpResponseMessage httpMessage = new HttpResponseMessage();

            if (SkipAuthorization(actionContext))
                return;

            httpMessage.StatusCode = AuthorizeRequest(actionContext).StatusCode;

            if (httpMessage.StatusCode == HttpStatusCode.OK)
            {
                return;
            }
            else
            {
                //HandleUnauthorizedRequest(actionContext, httpMessage.StatusCode);
                actionContext.Response = new System.Net.Http.HttpResponseMessage();
                actionContext.Response.StatusCode = httpMessage.StatusCode;
            }

        }

        ////handle unauthorize requests
        //protected override void HandleUnauthorizedRequest(System.Web.Http.Controllers.HttpActionContext actionContext, HttpStatusCode statusCode)
        //{
        //    actionContext.Response = new System.Net.Http.HttpResponseMessage();

        //    //Code to handle unauthorized request
        //    actionContext.Response.StatusCode = statusCode;
        //}


        private HttpResponseMessage AuthorizeRequest(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            var roles = this.Roles;
            var authHeader = actionContext.Request.Headers.Authorization;
            string signedtoken = string.Empty;
            if (authHeader != null)
                signedtoken = actionContext.Request.Headers.Authorization.Parameter;

            var plainTextSecurityKey = "Secretkeyforparcelinternational_base64string_test1";
            var signingKey = new InMemorySymmetricSecurityKey(Encoding.UTF8.GetBytes(plainTextSecurityKey));

            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidAudiences = new string[]
             {
                 WebURL
             },
                ValidIssuers = new string[]
            {
                ServiceURL
            },
                IssuerSigningKey = signingKey
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken validatedToken;
            try
            {
                tokenHandler.ValidateToken(signedtoken,
                tokenValidationParameters, out validatedToken);
            }
            catch (Exception)
            {
                //handle if there is an error in decoding
                validatedToken = null;
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }

            if (validatedToken == null)
            {
                // return false;
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }

            var tokenstring = validatedToken.ToString();
            string output = tokenstring.Substring(tokenstring.IndexOf('.') + 1);
            JWT tokenobjects = new JavaScriptSerializer().Deserialize<JWT>(output);
            if (tokenobjects == null)
            {
                //return false;
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }

            ProfileManagement prof = new ProfileManagement();
            var user = prof.GetUserById(tokenobjects.UserId);

            if (user == null)
            {
                //return false;
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);

            }

            var roleId = user.Roles.FirstOrDefault().RoleId;
            var role = prof.GetRoleNameById(roleId);

            if (!string.IsNullOrEmpty(roles))
            {
                var roleArray = roles.Split(',');

                if (!string.IsNullOrEmpty(tokenobjects.role) && roleArray.Contains(tokenobjects.role))
                {
                    //return true;
                    return new HttpResponseMessage(HttpStatusCode.OK);
                }
                else
                {
                    //return false;
                    return new HttpResponseMessage(HttpStatusCode.Forbidden);
                }

            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.OK);

            }


        }


    }


}


