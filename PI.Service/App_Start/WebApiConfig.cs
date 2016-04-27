using JwtAuthForWebAPI;
using PI.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace PI.Service
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            config.EnableCors();
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );


            //CustomerManagement cust = new CustomerManagement();
            //var securityKey = cust.GetBytes("anyoldrandomtext");
            //var builder = new SecurityTokenBuilder();
            //var jwtHandler = new JwtAuthenticationMessageHandler
            //{
            //    AllowedAudience = "http://localhost:5555/",
            //    Issuer = "self",
            //    SigningToken = builder.CreateFromKey(securityKey)
            //};

            //config.MessageHandlers.Add(jwtHandler);

            config.Filters.Add(new CustomAuthorizeAttribute());

        }
    }
}
