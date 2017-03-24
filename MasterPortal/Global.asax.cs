using System;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Profile;
using System.Web.Routing;
using System.Web.Security;
using System.Web.WebPages;
using Adxstudio.Xrm.Web;
using Adxstudio.Xrm.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Xrm.Portal.Configuration;
using Site.Areas.Account.Models;

namespace Site
{
	public class Global : HttpApplication
	{
		void Application_Start(object sender, EventArgs e)
		{
			if (SetupConfig.ApplicationStart()) return;

			this.ConfigurePortal();

			//Uncomment if deploying to Azure and using Cloud Drive
			//Note: add the following using statements
			//using Microsoft.WindowsAzure.ServiceRuntime; 
			//using Microsoft.WindowsAzure.StorageClient;
			//if (RoleEnvironment.IsAvailable)
			//{
			//	var driveCache = RoleEnvironment.GetLocalResource("CloudDriveCache");
			//	CloudDrive.InitializeCache(driveCache.RootPath, driveCache.MaximumSizeInMegabytes);
			//}

			var areaRegistrationState = new PortalAreaRegistrationState();
			Application[typeof (IPortalAreaRegistrationState).FullName] = areaRegistrationState;

			AreaRegistration.RegisterAllAreas(areaRegistrationState);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
		}

		protected void Application_PostAuthenticateRequest()
		{
			this.OnPostAuthenticateRequest();
		}

		public override string GetVaryByCustomString(HttpContext context, string arg)
		{
			switch (arg)
			{
				case "roles":
					return GetVaryByRolesString(context);
				case "roles;website":
					return GetVaryByRolesAndWebsiteString(context);
				case "user":
					return GetVaryByUserString(context);
				case "user;website":
					return GetVaryByUserAndWebsiteString(context);
				case "website":
					return GetVaryByWebsiteString(context);
			}

			return base.GetVaryByCustomString(context, arg);
		}

		public void Profile_MigrateAnonymous(object sender, ProfileMigrateEventArgs e)
		{
			var portalAreaRegistrationState = Application[typeof (IPortalAreaRegistrationState).FullName] as IPortalAreaRegistrationState;

			if (portalAreaRegistrationState != null)
			{
				portalAreaRegistrationState.OnProfile_MigrateAnonymous(sender, e);
			}
		}

		void Application_EndRequest(object sender, EventArgs e)
		{
			this.RedirectOnStatusCode();
		}

		private static string GetVaryByDisplayModesString(HttpContext context)
		{
			var availableDisplayModeIds = DisplayModeProvider.Instance
				.GetAvailableDisplayModesForContext(context.Request.RequestContext.HttpContext, null)
				.Select(mode => mode.DisplayModeId);

			return string.Join(",", availableDisplayModeIds);
		}

		private static string GetVaryByRolesString(HttpContext context)
		{
			// If the role system is not enabled, fall back to varying cache by user.
			if (!Roles.Enabled)
			{
				return GetVaryByUserString(context);
			}

			var roles = context.User != null && context.User.Identity != null && context.User.Identity.IsAuthenticated
				? Roles.GetRolesForUser()
					.OrderBy(role => role)
					.Aggregate(new StringBuilder(), (sb, role) => sb.AppendFormat("[{0}]", role))
					.ToString()
				: string.Empty;

			return string.Format("IsAuthenticated={0},Roles={1},DisplayModes={2}",
				context.Request.IsAuthenticated,
				roles.GetHashCode(),
				GetVaryByDisplayModesString(context).GetHashCode());
		}

		private static string GetVaryByRolesAndWebsiteString(HttpContext context)
		{
			return string.Format("{0},{1}", GetVaryByRolesString(context), GetVaryByWebsiteString(context));
		}

		private static string GetVaryByUserString(HttpContext context)
		{
			var identity = context.User != null ? context.User.Identity.Name : string.Empty;

			return string.Format("IsAuthenticated={0},Identity={1},DisplayModes={2}",
				context.Request.IsAuthenticated,
				identity,
				GetVaryByDisplayModesString(context).GetHashCode());
		}

		private static string GetVaryByUserAndWebsiteString(HttpContext context)
		{
			return string.Format("{0},{1}", GetVaryByUserString(context), GetVaryByWebsiteString(context));
		}

		private static string GetVaryByWebsiteString(HttpContext context)
		{
			var websiteId = GetWebsiteIdFromOwinContext(context) ?? GetWebsiteIdFromPortalContext(context);

			return websiteId == null ? "Website=" : string.Format("Website={0}", websiteId.GetHashCode());
		}

		private static string GetWebsiteIdFromOwinContext(HttpContext context)
		{
			var owinContext = context.GetOwinContext();

			if (owinContext == null)
			{
				return null;
			}

			var website = owinContext.Get<ApplicationWebsite>();

			return website == null ? null : website.Id;
		}

		private static string GetWebsiteIdFromPortalContext(HttpContext context)
		{
			var portalContext = PortalCrmConfigurationManager.CreatePortalContext(request: context.Request.RequestContext);

			if (portalContext == null)
			{
				return null;
			}

			return portalContext.Website == null ? null : portalContext.Website.Id.ToString();
		}
	}
}
