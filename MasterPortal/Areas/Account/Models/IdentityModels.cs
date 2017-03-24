using System.Security.Claims;
using System.Threading.Tasks;
using Adxstudio.Xrm.AspNet;
using Adxstudio.Xrm.AspNet.Identity;
using Adxstudio.Xrm.AspNet.Organization;
using Microsoft.AspNet.Identity;
using Microsoft.Xrm.Client;
using Microsoft.Xrm.Client.Configuration;
using Microsoft.Xrm.Sdk;

namespace Site.Areas.Account.Models
{
	// You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
	public class ApplicationUser : CrmUser<string>
	{
		public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser, string> manager)
		{
			// Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
			var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
			// Add custom user claims here
			return userIdentity;
		}
	}

	public class ApplicationOrganizationServiceManager : OrganizationServiceManager
	{
		public override IOrganizationService Create()
		{
			return CrmConfigurationManager.CreateService(new CrmConnection("Xrm"));
		}
	}

	public class ApplicationDbContext : CrmDbContext
	{
		public ApplicationDbContext(OrganizationServiceManager serviceManager)
			: base(serviceManager)
		{
		}

		public static ApplicationDbContext Create()
		{
			return new ApplicationDbContext(new ApplicationOrganizationServiceManager());
		}
	}

	public class UserStore<TUser> : CrmUserStore<TUser, string>, IUserStore<TUser>
		where TUser : ApplicationUser, new()
	{
		public UserStore(ApplicationDbContext context, CrmEntityStoreSettings settings)
			: base(context, settings)
		{
		}
	}

	public class ApplicationInvitation : CrmInvitation<string>
	{
	}

	public class InvitationStore : CrmInvitationStore<ApplicationInvitation, string>
	{
		public InvitationStore(ApplicationDbContext context)
			: base(context)
		{
		}
	}

	public class OrganizationStore : CrmOrganizationStore
	{
		public OrganizationStore(ApplicationDbContext context)
			: base(context)
		{
		}
	}
}