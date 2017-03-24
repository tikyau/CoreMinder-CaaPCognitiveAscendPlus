using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;
using Adxstudio.Xrm.AspNet;
using Adxstudio.Xrm.AspNet.Cms;
using Adxstudio.Xrm.AspNet.Identity;
using Adxstudio.Xrm.AspNet.Organization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Xrm.Portal.Configuration;

namespace Site.Areas.Account.Models
{
	// Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.

	public class ApplicationUserManager : CrmUserManager<ApplicationUser, string>
	{
		public ApplicationUserManager(IUserStore<ApplicationUser> store, MembershipProviderMigrationStore<string> migrationStore, CrmIdentityErrorDescriber identityErrors)
			: base(store, migrationStore, identityErrors)
		{
		}

		public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
		{
			var dbContext = context.Get<ApplicationDbContext>();
			var website = context.Get<ApplicationWebsite>();

			var manager = new ApplicationUserManager(
				new UserStore<ApplicationUser>(dbContext, website.GetCrmUserStoreSettings()),
				website.GetMembershipProviderMigrationStore(dbContext),
				website.GetCrmIdentityErrorDescriber());

			// Configure default validation logic for usernames
			var userValidator = manager.UserValidator as UserValidator<ApplicationUser, string>;

			if (userValidator != null)
			{
				userValidator.AllowOnlyAlphanumericUserNames = false;
				userValidator.RequireUniqueEmail = false;
			}

			// Configure default validation logic for passwords
			var passwordValidator = manager.PasswordValidator as PasswordValidator;

			if (passwordValidator != null)
			{
				passwordValidator.RequiredLength = 6;
				passwordValidator.RequireNonLetterOrDigit = false;
				passwordValidator.RequireDigit = false;
				passwordValidator.RequireLowercase = false;
				passwordValidator.RequireUppercase = false;
			}

			// Configure user lockout defaults
			manager.UserLockoutEnabledByDefault = true;
			manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
			manager.MaxFailedAccessAttemptsBeforeLockout = 5;

			// Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
			// You can write your own provider and plug in here.
			manager.RegisterTwoFactorProvider("PhoneCode", new CrmPhoneNumberTokenProvider<ApplicationUser, string>(context.Get<ApplicationOrganizationManager>()));
			manager.RegisterTwoFactorProvider("EmailCode", new CrmEmailTokenProvider<ApplicationUser, string>(context.Get<ApplicationOrganizationManager>()));
			manager.EmailService = new EmailService();
			manager.SmsService = new SmsService();

			var dataProtectionProvider = options.DataProtectionProvider;

			if (dataProtectionProvider != null)
			{
				manager.UserTokenProvider =
					new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
			}

			var claimsIdentityFactory = new CrmClaimsIdentityFactory<ApplicationUser, string>(context.Authentication, dbContext)
			{
				KeepExternalLoginClaims = true,
				ClaimAttributes = new[] { "firstname", "lastname" },
			};

			manager.ClaimsIdentityFactory = claimsIdentityFactory;

			manager.Configure(website);

			return manager;
		}
	}

	public class EmailService : IIdentityMessageService
	{
		public Task SendAsync(IdentityMessage message)
		{
			// Plug in your email service here to send an email.
			return Task.FromResult(0);
		}
	}

	public class SmsService : IIdentityMessageService
	{
		public Task SendAsync(IdentityMessage message)
		{
			// Plug in your sms service here to send a text message.
			return Task.FromResult(0);
		}
	}

	public class ApplicationSignInManager : CrmSignInManager<ApplicationUser, string>
	{
		public ApplicationSignInManager(
			ApplicationUserManager userManager,
			IAuthenticationManager authenticationManager,
			MembershipProviderMigrationStore<string> migrationStore)
			: base(userManager, authenticationManager, migrationStore)
		{
		}

		public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
		{
			var manager = new ApplicationSignInManager(
				context.GetUserManager<ApplicationUserManager>(),
				context.Authentication,
				context.Get<ApplicationWebsite>().GetMembershipProviderMigrationStore(context.Get<ApplicationDbContext>()));

			return manager;
		}

		public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
		{
			return user.GenerateUserIdentityAsync(UserManager);
		}
	}

	public class ApplicationInvitationManager : InvitationManager<ApplicationInvitation, string>
	{
		public ApplicationInvitationManager(IInvitationStore<ApplicationInvitation, string> store, CrmIdentityErrorDescriber identityErrors)
			: base(store, identityErrors)
		{
		}

		public static ApplicationInvitationManager Create(IdentityFactoryOptions<ApplicationInvitationManager> options, IOwinContext context)
		{
			return new ApplicationInvitationManager(
				new InvitationStore(context.Get<ApplicationDbContext>()),
				context.Get<ApplicationWebsite>().GetCrmIdentityErrorDescriber());
		}
	}

	public class ApplicationOrganizationManager : OrganizationManager
	{
		public ApplicationOrganizationManager(IOrganizationStore store)
			: base(store)
		{
		}

		public static ApplicationOrganizationManager Create(IdentityFactoryOptions<ApplicationOrganizationManager> options, IOwinContext context)
		{
			return new ApplicationOrganizationManager(new OrganizationStore(context.Get<ApplicationDbContext>()));
		}
	}

	public class ApplicationWebsite : CrmWebsite<string>, IDisposable
	{
		public static ApplicationWebsite Create(IdentityFactoryOptions<ApplicationWebsite> options, IOwinContext context)
		{
			var websiteManager = context.Get<ApplicationWebsiteManager>();
			return websiteManager.Find(context);
		}

		void IDisposable.Dispose() { }
	}

	public class ApplicationWebsiteManager : WebsiteManager<ApplicationWebsite, string>
	{
		public ApplicationWebsiteManager(IWebsiteStore<ApplicationWebsite, string> store)
			: base(store)
		{
		}

		public static ApplicationWebsiteManager Create(ApplicationDbContext dbContext)
		{
			return CreateWebsiteManager(dbContext);
		}

		public static ApplicationWebsiteManager Create(IdentityFactoryOptions<ApplicationWebsiteManager> options, IOwinContext context)
		{
			return CreateWebsiteManager(context.Get<ApplicationDbContext>());
		}

		private static ApplicationWebsiteManager CreateWebsiteManager(ApplicationDbContext dbContext)
		{
			var manager = new ApplicationWebsiteManager(new CrmWebsiteStore<ApplicationWebsite, string>(dbContext))
			{
				WebsiteName = GetWebsiteName()
			};

			return manager;
		}

		private static string GetWebsiteName()
		{
			var portal = PortalCrmConfigurationManager.GetPortalContextElement("Xrm");
			return portal == null ? null : portal.Parameters["websiteName"];
		}
	}

	public class ApplicationPortalContextManager : PortalContextManager<ApplicationWebsite, ApplicationUser, string>
	{
		public static ApplicationPortalContextManager Create(IdentityFactoryOptions<ApplicationPortalContextManager> options, IOwinContext context)
		{
			return new ApplicationPortalContextManager();
		}
	}

	public class ApplicationPortalCrmConfigurationProvider : PortalCrmConfigurationProvider<ApplicationWebsite, ApplicationUser, string>
	{
		protected override PortalContextManager<ApplicationWebsite, ApplicationUser, string> GetPortalContextManager(RequestContext request)
		{
			return request.HttpContext.GetOwinContext().Get<ApplicationPortalContextManager>();
		}

		protected override WebsiteManager<ApplicationWebsite, string> GetWebsiteManager(RequestContext request)
		{
			return request.HttpContext.GetOwinContext().Get<ApplicationWebsiteManager>();
		}

		protected override UserManager<ApplicationUser, string> GetUserManager(RequestContext request)
		{
			return request.HttpContext.GetOwinContext().Get<ApplicationUserManager>();
		}
	}

	public class ApplicationStartupSettingsManager : StartupSettingsManager<ApplicationUserManager, ApplicationWebsite, ApplicationUser, string>
	{
		public ApplicationStartupSettingsManager(
			ApplicationWebsite website,
			Func<ApplicationUserManager, ApplicationUser, Task<ClaimsIdentity>> regenerateIdentityCallback,
			PathString loginPath,
			PathString externalLoginCallbackPath)
			: base(website, regenerateIdentityCallback, loginPath, externalLoginCallbackPath)
		{
		}

		protected override CrmDbContext GetDbContext(IOwinContext context)
		{
			return context.Get<ApplicationDbContext>();
		}
	}
}