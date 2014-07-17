using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using OptimalEducation.Models;
using Owin;
using System;
using System.Configuration;

namespace OptimalEducation
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //Смотри http://azure.microsoft.com/en-us/documentation/articles/mobile-services-how-to-register-microsoft-authentication/
            app.UseMicrosoftAccountAuthentication(
                clientId: ConfigurationManager.AppSettings["Microsoft_ClientId"],
                clientSecret: ConfigurationManager.AppSettings["Microsoft_ClientSecret"]);

            //https://dev.twitter.com/
            app.UseTwitterAuthentication(
               consumerKey: ConfigurationManager.AppSettings["Twitter_ClientId"],
               consumerSecret: ConfigurationManager.AppSettings["Twitter_ClientSecret"]);

            //https://developers.facebook.com
            app.UseFacebookAuthentication(
               appId: ConfigurationManager.AppSettings["Facebook_ClientId"],
               appSecret: ConfigurationManager.AppSettings["Facebook_ClientSecret"]);

            //https://console.developers.google.com
            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = ConfigurationManager.AppSettings["Google_ClientId"],
                ClientSecret = ConfigurationManager.AppSettings["Google_ClientSecret"]
            });
        }
    }
}