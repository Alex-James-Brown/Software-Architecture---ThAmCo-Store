using Owin;
using System.Linq;
using System.Web.Http;
using System.Net.Http.Formatting;
using Newtonsoft.Json.Serialization;
using AuthService.Infrastructure;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin;
using System;
using AuthService.Providers;
using System.Configuration;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security;

namespace AuthService
{
    /// <summary>
    /// Class Startup.
    /// </summary>
    public class Startup
    {
        public static HttpConfiguration HttpConfiguration { get; private set; }

        /// <summary>
        /// Configures the application.
        /// </summary>
        /// <param name="app">App Builder.</param>
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration = new HttpConfiguration();
            ConfigureOAuthTokenGeneration(app);
            ConfigureOAuthTokenConsumption(app);
            ConfigureWebApi(HttpConfiguration);
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            System.Web.Mvc.AreaRegistration.RegisterAllAreas();
            app.UseWebApi(HttpConfiguration);
        }
        /// <summary>
        /// Configures the o authentication token generation.
        /// </summary>
        /// <param name="app">App builder.</param>
        private void ConfigureOAuthTokenGeneration(IAppBuilder app)
        {
            // Configure the db context and user manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationRoleManager>(ApplicationRoleManager.Create);

            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                //For Dev enviroment only (on production should be AllowInsecureHttp = false)
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/oauth/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                Provider = new OAuthProvider(),
                AccessTokenFormat = new CustomJwtFormat("http://localhost:4020")
            };
            // OAuth 2.0 Bearer Access Token Generation
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
        }

        /// <summary>
        /// Configures the o authentication token consumption.
        /// </summary>
        /// <param name="app">App builder.</param>
        private void ConfigureOAuthTokenConsumption(IAppBuilder app)
        {
            var issuer = "http://localhost:4020";
            string audienceId = ConfigurationManager.AppSettings["as:AudienceId"];
            byte[] audienceSecret = TextEncodings.Base64Url.Decode(ConfigurationManager.AppSettings["as:AudienceSecret"]);

            // Api controllers with an [Authorize] attribute will be validated with JWT
            app.UseJwtBearerAuthentication(
                new JwtBearerAuthenticationOptions
                {
                    AuthenticationMode = AuthenticationMode.Active,
                    AllowedAudiences = new[] { audienceId },
                    IssuerSecurityTokenProviders = new IIssuerSecurityTokenProvider[]
                    {
                        new SymmetricKeyIssuerSecurityTokenProvider(issuer, audienceSecret)
                    }
                });
        }

        /// <summary>
        /// Configures web API.
        /// </summary>
        /// <param name="config">Http Configuration.</param>
        private void ConfigureWebApi(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }
    }
}