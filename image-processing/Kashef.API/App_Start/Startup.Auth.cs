using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.OAuth;
using Owin; 
using System.Threading.Tasks;
using System.Security.Claims; 

namespace Kashef.API
{
    public partial class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        public static OAuthBearerAuthenticationOptions OAuthBearerOptions { get; private set; }

        public static string PublicClientId { get; private set; }

        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context and user manager to use a single instance per request
            //app.CreatePerOwinContext(ApplicationDbContext.Create);
           
            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseCookieAuthentication(new CookieAuthenticationOptions());
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Configure the application for OAuth based flow
            PublicClientId = "self";
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/api/v1/account/Token"),
              //  Provider = new LiveNavigationOAuthProvider(PublicClientId),
                AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(180),
                AllowInsecureHttp = true
            };

            // Enable the application to use bearer tokens to authenticate users
            app.UseOAuthBearerTokens(OAuthOptions);
    
            OAuthBearerOptions = new OAuthBearerAuthenticationOptions();
            OAuthBearerOptions.AccessTokenFormat = OAuthOptions.AccessTokenFormat;
            OAuthBearerOptions.AccessTokenProvider = OAuthOptions.AccessTokenProvider;
            OAuthBearerOptions.AuthenticationMode = OAuthOptions.AuthenticationMode;
            OAuthBearerOptions.AuthenticationType = OAuthOptions.AuthenticationType;
            OAuthBearerOptions.Description = OAuthOptions.Description;
            // The provider is the only object we need to redefine. See below for the implementation
            OAuthBearerOptions.Provider = new CustomBearerAuthenticationProvider();
            OAuthBearerOptions.SystemClock = OAuthOptions.SystemClock;
            app.UseOAuthBearerAuthentication(OAuthBearerOptions);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //    consumerKey: "",
            //    consumerSecret: "");

            //app.UseFacebookAuthentication(
            //    appId: "",
            //    appSecret: "");

            //app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            //{
            //    ClientId = "",
            //    ClientSecret = ""
            //});
        }
    }

    public class CustomBearerAuthenticationProvider : OAuthBearerAuthenticationProvider
    {
        // This validates the identity based on the issuer of the claim.
        // The issuer is set in the API endpoint that logs the user in
        public override Task ValidateIdentity(OAuthValidateIdentityContext context)
        {
            IEnumerable<Claim> claims = context.Ticket.Identity.Claims; 

            if (null == claims || 0 == claims.Count() || claims.Any(claim => claim.Issuer.ToString() != "Facebook" && claim.Issuer.ToString() != "Google" && claim.Issuer.ToString() != "LOCAL_AUTHORITY"))
            {
                context.Rejected();
            }
            return Task.FromResult<object>(null);
        }

        public override Task RequestToken(OAuthRequestTokenContext context)
        {
            return base.RequestToken(context);
        }
    }
}
