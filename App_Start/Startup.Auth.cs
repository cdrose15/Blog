using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using Blog.Models;
using Owin.Security.Providers.LinkedIn;
using Owin.Security.Providers.GitHub;
namespace Blog
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });            
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            app.UseMicrosoftAccountAuthentication(
                clientId: "0000000040182851",
                clientSecret: "mRBKrSyOKF-6dqtDP83aw60hDJchenBb");

            app.UseTwitterAuthentication(
               consumerKey:    "YfHQthY0wUYwKwsHCn9pwH8x6",
               consumerSecret: "wcWS9VFDUNtYaDbN8jWx7FKlZn5EAQCnH0DpcrtUdWzE3QoW0y");

            app.UseFacebookAuthentication(
               appId: "483091661898024",
               appSecret: "252d9084e83370366773dfdcef9e2e40");

            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = "481490653419-q3levlq2dm35let7ucaq33rtvu1djkhr.apps.googleusercontent.com",
                ClientSecret = "FkB0b0oSj-xQCDeOceQH1yiC"
            });

            //app.UseYahooAuthentication(
                //"dj0yJmk9RGQ2V2JZdWpsUEhWJmQ9WVdrOVlqRXpaamx4TjJzbWNHbzlNQS0tJnM9Y29uc3VtZXJzZWNyZXQmeD1iNw--",
                //"b106b05ca3e7a96f4cd04925db737248d29e7920");

            app.UseLinkedInAuthentication
                ("77zywo4mfvooco",
                "xdaQVfUbC4vTEP8p");

            app.UseGitHubAuthentication
                ("29009dfbe91bf57ec4a4",
                "92e174b852e003bba995cb0e67ddd8aca2a01344");

        }
    }
}