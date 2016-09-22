
using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.MicrosoftAccount;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using Owin;
using PI.Business;
using PI.Common;
using PI.Contract;
using PI.Contract.Business;
using PI.Data;
using PI.Service.Providers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Reflection;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;

namespace PI.Service
{
    public class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }
        public static string PublicClientId { get; private set; }
        public static OAuthBearerAuthenticationOptions OAuthBearerOptions { get; private set; }
        public static GoogleOAuth2AuthenticationOptions googleAuthOptions { get; private set; }
        public static FacebookAuthenticationOptions facebookAuthOptions { get; private set; }


        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration httpConfig = new HttpConfiguration();

            ConfigureOAuth(app);
            

            // autofac
            var builder = new ContainerBuilder();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly()).InstancePerLifetimeScope();

            builder.RegisterType<PIContext>().As<PIContext>().InstancePerLifetimeScope();
            builder.RegisterType<ConfigAutoMapper>();   // Initialize auto mapper.
            builder.RegisterType<Log4NetLogger>().As<ILogger>().InstancePerLifetimeScope();
            builder.RegisterType<CompanyManagement>().As<ICompanyManagement>().InstancePerLifetimeScope();
            //builder.RegisterType<CompanyManagement>().As<ICompanyManagement>().WithParameter("log", new Log4NetLogger()).InstancePerLifetimeScope();
            builder.RegisterType<CustomerManagement>().As<ICustomerManagement>().InstancePerLifetimeScope();
            builder.RegisterType<ShipmentsManagement>().As<IShipmentManagement>().InstancePerLifetimeScope();
            builder.RegisterType<AddressBookManagement>().As<IAddressBookManagement>().InstancePerLifetimeScope();
            builder.RegisterType<AdministrationManagment>().As<IAdministrationManagment>().InstancePerLifetimeScope();
            builder.RegisterType<SISIntegrationManager>().As<ICarrierIntegrationManager>().InstancePerLifetimeScope();
            builder.RegisterType<ProfileManagement>().As<IProfileManagement>().InstancePerLifetimeScope();
            builder.RegisterType<ProfileManagement>().As<ProfileManagement>().InstancePerLifetimeScope();   // TODO H : Remove this register, after convert Prof to IProf
            builder.RegisterType<InvoiceMangement>().As<IInvoiceMangement>().InstancePerLifetimeScope();

            var container = builder.Build();
            httpConfig.DependencyResolver = new AutofacWebApiDependencyResolver(container); // Set the dependency resolver

            ConfigureOAuthTokenGeneration(app);
            //ConfigureOAuthTokenConsumption(app);

            ConfigureWebApi(httpConfig);

            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            // autofac
            app.UseAutofacMiddleware(container);
            app.UseAutofacWebApi(httpConfig);

            app.UseWebApi(httpConfig);

            
        }


        private void ConfigureWebApi(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }

        private void ConfigureOAuthTokenGeneration(IAppBuilder app)
        {
            // Configure the application for OAuth based flow
            PublicClientId = "self";

            // Configure the db context and user manager to use a single instance per request
            app.CreatePerOwinContext(PIContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            // Configure the role manager to use a single instance per request
            app.CreatePerOwinContext<ApplicationRoleManager>(ApplicationRoleManager.Create);

            // Plugin the OAuth bearer JSON Web Token tokens generation and Consumption will be here
            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                //For Dev enviroment only (on production should be AllowInsecureHttp = false)
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/oauth/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                Provider = new CustomOAuthProvider(PublicClientId),
                AccessTokenFormat = new CustomJwtFormat("http://localhost:59822")
            };

            // OAuth 2.0 Bearer Access Token Generation
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
        }

        private void ConfigureOAuthTokenConsumption(IAppBuilder app)
        {

            var issuer = "http://localhost:59822";
            string audienceId = ConfigurationManager.AppSettings["as:AudienceId"];
            byte[] audienceSecret = TextEncodings.Base64Url.Decode(ConfigurationManager.AppSettings["as:AudienceSecret"]);

            // Api controllers with an [Authorize] attribute will be validated with JWT
            app.UseJwtBearerAuthentication(
                new JwtBearerAuthenticationOptions
                {
                    AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Active,
                    AllowedAudiences = new[] { audienceId },
                    IssuerSecurityTokenProviders = new IIssuerSecurityTokenProvider[]
                    {
                        new SymmetricKeyIssuerSecurityTokenProvider(issuer, audienceSecret)
                    }
                });
        }

        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context and user manager to use a single instance per request
            app.CreatePerOwinContext(PIContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseCookieAuthentication(new CookieAuthenticationOptions());
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Configure the application for OAuth based flow
            PublicClientId = "self";
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/Token"),
                Provider = new CustomOAuthProvider(PublicClientId),
                AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),
                // In production mode set AllowInsecureHttp = false
                AllowInsecureHttp = true
            };

            // Enable the application to use bearer tokens to authenticate users
            app.UseOAuthBearerTokens(OAuthOptions);

            // Uncomment the following lines to enable logging in with third party login providers
            app.UseMicrosoftAccountAuthentication(
                clientId: "70a1a68c-0c5e-445f-8724-5e433fe463e1", //70a1a68c-0c5e-445f-8724-5e433fe463e1
                clientSecret: "dmtmssM17b8BNxnSHkOKU3E"); //dmtmssM17b8BNxnSHkOKU3E

            //app.UseTwitterAuthentication(
            //    consumerKey: "",
            //    consumerSecret: "");

            app.UseFacebookAuthentication(
                appId: "1753464874877402",
                appSecret: "4cbc794bf7555a0dfda6585ef2b6418d");

            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = "657439870432-g98gvt35aceavp0ou6vsr3b6372m3cmr.apps.googleusercontent.com",
                ClientSecret = "WsjF353NEonbaFZMgTyMJl4h"
            });
        }


        public void ConfigureOAuth(IAppBuilder app)
        {
            //use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseExternalSignInCookie(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ExternalCookie);
            OAuthBearerOptions = new OAuthBearerAuthenticationOptions();

            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {

                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(30),
                Provider = new SimpleAuthorizationServerProvider(),
                RefreshTokenProvider = new SimpleRefreshTokenProvider()
            };

            // Token Generation
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            app.UseOAuthBearerAuthentication(OAuthBearerOptions);

            //Configure Google External Login
            googleAuthOptions = new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = "657439870432-g98gvt35aceavp0ou6vsr3b6372m3cmr.apps.googleusercontent.com",
                ClientSecret = "WsjF353NEonbaFZMgTyMJl4h",
                Provider = new GoogleAuthProvider()
            };
            app.UseGoogleAuthentication(googleAuthOptions);

            //Configure Facebook External Login
            facebookAuthOptions = new FacebookAuthenticationOptions()
            {
                AppId = "1753464874877402",
                AppSecret = "4cbc794bf7555a0dfda6585ef2b6418d",
                Provider = new FacebookAuthProvider()
            };
            facebookAuthOptions.Scope.Add("email");
            app.UseFacebookAuthentication(facebookAuthOptions);

            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "70a1a68c-0c5e-445f-8724-5e433fe463e1", //70a1a68c-0c5e-445f-8724-5e433fe463e1
            //    clientSecret: "dmtmssM17b8BNxnSHkOKU3E"); //dmtmssM17b8BNxnSHkOKU3E

            var mo = new MicrosoftAccountAuthenticationOptions
            {
                Caption = "Live",
                ClientId = "70a1a68c-0c5e-445f-8724-5e433fe463e1",
                ClientSecret = "dmtmssM17b8BNxnSHkOKU3E"
            };

            mo.Scope.Add("wl.basic");
            mo.Scope.Add("wl.emails");

            app.UseMicrosoftAccountAuthentication(mo);

        }

    }
}