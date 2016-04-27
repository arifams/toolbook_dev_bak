using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http;

namespace PI.Service
{
    public class CustomAuthorizeAttribute: AuthorizeAttribute
    {
        //overriding on authorization method
        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
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
            var signedtoken = actionContext.Request.Headers.Authorization.Parameter;
            var plainTextSecurityKey = "Secretkeyforparcelinternational_base64string_test1";
            var signingKey = new InMemorySymmetricSecurityKey(Encoding.UTF8.GetBytes(plainTextSecurityKey));

            var tokenValidationParameters = new TokenValidationParameters()
             {
                ValidAudiences = new string[]
             {
                 "http://localhost:49995/"
             },
                ValidIssuers = new string[]
            {
                "http://localhost:55555/"

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
            
            if (validatedToken!=null)
            {
                return true;
            }
            else
            {
                return false;
            }
           

        }

    }
}