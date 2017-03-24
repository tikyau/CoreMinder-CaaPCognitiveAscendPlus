using System.Web.Mvc;
using System.Web.Security;
using Adxstudio.Xrm.Web.Mvc;

namespace Site.Areas.Account
{
	public class AccountAreaRegistration : AreaRegistration
	{
		public override string AreaName
		{
			get { return "Account"; }
		}

		public override void RegisterArea(AreaRegistrationContext context)
		{
			if (SetupConfig.OwinEnabled())
			{
				context.MapRoute(
					"Account/SignIn",
					"SignIn",
					new { area = "Account", controller = "Login", action = "Login" }
				);

				context.MapRoute(
					"Account/Login",
					"Account/{controller}/{action}",
					new { area = "Account", action = "Login" },
					new { controller = "Login" }
				);

				context.MapRoute(
					"Account/Redeem",
					"Register",
					new { area = "Account", controller = "Login", action = "RedeemInvitation" }
				);

				context.MapRoute(
					"Account/Manage",
					"Account/{controller}/{action}",
					new { area = "Account", action = "Index" },
					new { controller = "Manage" }
				);

				context.MapRoute("Facebook/Pages", "app/facebook", new { controller = "Login", action = "FacebookExternalLoginCallback" });
			}
			else
			{
				context.MapRoute("auth/federation-axd", "Federation.axd", new { controller = "Account", action = "SignInWsFederation" });
				context.MapRoute("auth/wsfederation", "auth/wsfederation", new { controller = "Account", action = "SignInWsFederation" });
				context.MapRoute("signin/wsfederation", "signin-federation", new { controller = "Account", action = "SignInWsFederation" });
				context.MapRoute("signin/openauth", "signin-{provider}", new { controller = "Account", action = "SignInOpenAuth" });
				context.MapRoute("app/facebook", "app/facebook", new { controller = "Account", action = "FacebookApp", provider = "facebook" });

				context.MapSiteMarkerRoute(
					"SignIn",
					"Login",
					"{action}",
					new { controller = "Account", action = "SignIn" }, new { action = @"^SignIn.*" });

				if (Membership.EnablePasswordReset)
				{
					context.MapSiteMarkerRoute(
						"PasswordRecovery",
						"Login",
						"{action}",
						new { controller = "Account", action = "PasswordRecovery" }, new { action = @"^PasswordRecovery.*" });
				}

				context.MapSiteMarkerRoute(
					"Sign Up",
					"Sign Up",
					"{action}",
					new { controller = "Account", action = "SignUp" }, new { action = @"^SignUp.*" });

				context.MapRoute(
					"Account",
					"account-signout",
					new { controller = "Account", action = "SignOut" }, new { action = @"^SignOut.*" });

				context.MapSiteMarkerRoute(
					"Register",
					"Register",
					"{action}",
					new { controller = "Account", action = "Redeem" }, new { action = @"^Redeem.*" });
			}
		}
	}
}
