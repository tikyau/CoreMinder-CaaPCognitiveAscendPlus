using System.Web;
using System.Web.Mvc;
using Adxstudio.Xrm.AspNet.Cms;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Owin;
using Site.Areas.Account.Models;

namespace Site
{
	public partial class Startup
	{
		// For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
		public void ConfigureAuth(IAppBuilder app)
		{
			// Configure the db context, user manager and role manager to use a single instance per request
			app.CreatePerOwinContext(ApplicationDbContext.Create);
			app.CreatePerOwinContext<ApplicationOrganizationManager>(ApplicationOrganizationManager.Create);
			app.CreatePerOwinContext<ApplicationWebsiteManager>(ApplicationWebsiteManager.Create);
			app.CreatePerOwinContext<ApplicationWebsite>(ApplicationWebsite.Create);
			app.CreatePerOwinContext<ApplicationPortalContextManager>(ApplicationPortalContextManager.Create);
			app.CreatePerOwinContext<ApplicationInvitationManager>(ApplicationInvitationManager.Create);
			app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
			app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

			var websiteManager = ApplicationWebsiteManager.Create(ApplicationDbContext.Create());
			var request = HttpContext.Current.Request.RequestContext;
			var url = new UrlHelper(request);
			var loginPath = new PathString(url.Action("Login", "Login", new { area = "Account" }));
			var externalLoginCallbackPath = new PathString(url.Action("ExternalLoginCallback", "Login", new { area = "Account" }));
			var website = websiteManager.Find(request);
			var settingsManager = new ApplicationStartupSettingsManager(website, (manager, user) => user.GenerateUserIdentityAsync(manager), loginPath, externalLoginCallbackPath);

			app.UseSiteMapAuthentication(settingsManager.ApplicationCookie);

			// Enable the application to use a cookie to store information for the signed in user
			// and to use a cookie to temporarily store information about a user logging in with a third party login provider
			// Configure the sign in cookie
			app.UseKentorOwinCookieSaver();
			app.UseCookieAuthentication(settingsManager.ApplicationCookie);
			app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

			// Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
			app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, settingsManager.TwoFactorCookie.ExpireTimeSpan);

			// Enables the application to remember the second login verification factor such as phone or email.
			// Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
			// This is similar to the RememberMe option when you log in.
			app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

			app.UsePortalsAuthentication(settingsManager);
		}
	}
}