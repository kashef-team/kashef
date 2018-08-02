//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Claims;
//using System.Threading.Tasks;
//using Microsoft.AspNet.Identity;
//using Microsoft.AspNet.Identity.EntityFramework;
//using Microsoft.AspNet.Identity.Owin;
//using Microsoft.Owin.Security;
//using Microsoft.Owin.Security.Cookies;
//using Microsoft.Owin.Security.OAuth;  
//using LiveNavigation.DataAccess.Repositories.Implementation; 
//using LiveNavigation.DataAccess.Core.Context;
//using LiveNavigation.DataAccess.Models.Base;
//using LiveNavigation.Services;
//using LiveNavigation.DataAccess.Repositories.Contracts;

//namespace Kashef.API.Providers
//{
//    public class LiveNavigationOAuthProvider : OAuthAuthorizationServerProvider
//    {
//        #region Properties

//        private readonly string _publicClientId;

//        //private IAccountManagmentService _accountMangementService = null;

//        //private User currentUser = null;

//        #endregion

//        #region Constructors

//        public LiveNavigationOAuthProvider(string publicClientId)
//        {
//            if (publicClientId == null)
//            {
//                throw new ArgumentNullException("publicClientId");
//            }

//            _publicClientId = publicClientId;

//          //  _accountMangementService = Kashef.API.Models.UnityBootstrapper.ResolveInstance<IAccountManagmentService>();

//        }

//        #endregion

//        #region Methods

//        /// <summary>
//        /// This methods validate the user credential and issue token in case valid credential
//        /// </summary>
//        /// <param name="context"></param>
//        /// <returns></returns>
//        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
//        {
//            //Validate User credential..
//            currentUser = _accountMangementService.Login(context.UserName, context.Password);

//            //Precondition check...
//            if (null == currentUser)
//            {
//                context.SetError("invalid_grant", "The user name or password is incorrect.");
//                return;
//            }

//            //Create Identity Claims 
//            ClaimsIdentity claimsIdentity = new ClaimsIdentity(context.Options.AuthenticationType);
//            claimsIdentity.AddClaim(new Claim("user", context.UserName));
//            claimsIdentity.AddClaim(new Claim("UID_User", currentUser.UID_User)); 

//            //Add roles
//            List<string> roles = _accountMangementService.GetAssignedRoles(currentUser.UID_User);

//            if (null != roles || 0 != roles.Count)
//            {
//                foreach (string role in roles)
//                {
//                    claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
//                }
//            }
//            else
//            {
//                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "Customer"));
//            }
//            context.Validated(claimsIdentity);

//            //Create Properties
//            AuthenticationProperties properties = CreateProperties(currentUser);

//            //Generate ticket...
//            AuthenticationTicket ticket = new AuthenticationTicket(claimsIdentity, properties);

//            //Validate Ticket..
//            context.Validated(ticket);

//            //Sign In
//            context.Request.Context.Authentication.SignIn(claimsIdentity);
//        }

//        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
//        {
//            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
//            {
//                context.AdditionalResponseParameters.Add(property.Key, property.Value);
//            }

//            if (null != currentUser)
//            {
//                context.AdditionalResponseParameters.Add("FullName", currentUser.FullName); 
//                context.AdditionalResponseParameters.Add("ProfilePictureUrl", currentUser.ProfilePictureUrl); 
//            }

//            return Task.FromResult<object>(null);
//        }

//        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
//        {
//            // Resource owner password credentials does not provide a client ID.
//            if (context.ClientId == null)
//            {
//                context.Validated();
//            }

//            return Task.FromResult<object>(null);
//        }

//        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
//        {
//            if (context.ClientId == _publicClientId)
//            {
//                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

//                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
//                {
//                    context.Validated();
//                }
//            }

//            return Task.FromResult<object>(null);
//        }

//        //public static AuthenticationProperties CreateProperties(User user)
//        //{
//        //    IDictionary<string, string> data = new Dictionary<string, string>
//        //    {
//        //       // { "userName", userName },
//        //        { "UID_User", user.UID_User.ToString()}
//        //    };
//        //    return new AuthenticationProperties(data);
//        //}

//        public override Task MatchEndpoint(OAuthMatchEndpointContext context)
//        {
//            if (context.IsTokenEndpoint && context.Request.Method == "OPTIONS")
//            {
//                context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
//                context.OwinContext.Response.Headers.Add("Access-Control-Allow-Headers", new[] { "authorization" });
//                context.RequestCompleted();
//                return Task.FromResult(0);
//            }
//            return base.MatchEndpoint(context);
//        }

//        #endregion

//    }
//}