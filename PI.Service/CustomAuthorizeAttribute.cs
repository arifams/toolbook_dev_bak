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
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace PI.Service
{
    public class CustomAuthorizeAttribute: AuthorizeAttribute
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
            if (SkipAuthorization(actionContext))
                return;

            if (AuthorizeRequest(actionContext))
            {
                return;
            }
            HandleUnauthorizedRequest(actionContext);
        }

        //handle unauthorize requests
        protected override void HandleUnauthorizedRequest(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            actionContext.Response = new System.Net.Http.HttpResponseMessage();
            //Code to handle unauthorized request
            actionContext.Response.StatusCode = System.Net.HttpStatusCode.Unauthorized;
        }

        private bool AuthorizeRequest(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            var roles=this.Roles;
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
            }
            
            if (validatedToken==null)
            {
                return false;
            }

                var tokenstring = validatedToken.ToString();
                string output = tokenstring.Substring(tokenstring.IndexOf('.') + 1);
                JWT tokenobjects = new JavaScriptSerializer().Deserialize<JWT>(output);
                if (tokenobjects == null)
                {
                    return false;
                }

                ProfileManagement prof = new ProfileManagement();
                var user = prof.GetUserById(tokenobjects.UserId);

                if (user == null)
                {
                    return false;
                }

                var roleId = user.Roles.FirstOrDefault().RoleId;
                var role = prof.GetRoleNameById(roleId);
                
                if (!string.IsNullOrEmpty(role)&& !string.IsNullOrEmpty(tokenobjects.role) && role.Equals(tokenobjects.role) )
                {
                   if (!string.IsNullOrEmpty(roles) )
                   {
                    if (roles.Equals(tokenobjects.role))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                  
                   }
                   
                    return true;
                }
                else
                {
                    return false;
                }

                    
                }

                
            }
           

        }

    
