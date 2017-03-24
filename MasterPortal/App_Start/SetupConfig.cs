using System;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Configuration;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Adxstudio.Xrm.Web.Modules;
using Microsoft.IdentityModel.Web;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using Microsoft.Xrm.Client;
using Microsoft.Xrm.Client.Configuration;
using Microsoft.Xrm.Portal.Configuration;
using Microsoft.Xrm.Portal.Web.Routing;
using Site.Areas.Setup;

[assembly: PreApplicationStartMethod(typeof(Site.SetupConfig), "PreApplicationStart")]

namespace Site
{
	public class SetupRoutingModule : PortalRoutingModule
	{
		protected override RouteCollection Register(RouteCollection routes, IRouteHandler portalRouteHandler, IEmbeddedResourceRouteHandler embeddedResourceRouteHandler, IEmbeddedResourceRouteHandler scriptHandler)
		{
			RegisterEmbeddedResourceRoutes(routes, portalRouteHandler, embeddedResourceRouteHandler, scriptHandler);
			return routes;
		}
	}

	public static class SetupConfig
	{
		private static readonly Lazy<SetupManager> _setupManager = new Lazy<SetupManager>(GetSetupManager);

		public static SetupManager SetupManager
		{
			get { return _setupManager.Value; }
		}

		private static readonly Lazy<CrmSection> _crmSection = new Lazy<CrmSection>(CrmConfigurationManager.GetCrmSection);

		public static CrmSection Section
		{
			get { return _crmSection.Value; }
		}

		public static bool OwinEnabled()
		{
			return !InitialSetupIsRunning() && !FormsEnabled();
		}

		public static void RegisterBundles(BundleCollection bundles)
		{
			bundles.Add(new ScriptBundle("~/js/jqueryval.bundle.js").Include("~/js/jquery.unobtrusive-ajax.min.js", "~/js/jquery.validate.min.js"));
		}

		public static void PreApplicationStart()
		{
			if (InitialSetupIsRunning())
			{
				DynamicModuleUtility.RegisterModule(typeof(SetupRoutingModule));
				RegisterBundles(BundleTable.Bundles);
			}
			else
			{
				DynamicModuleUtility.RegisterModule(typeof(PortalRoutingModule));

				if (!FormsEnabled())
				{
					var portalConfiguration = PortalCrmConfigurationManager.GetPortalCrmSection();
					portalConfiguration.ConfigurationProviderType = "Site.Areas.Account.Models.ApplicationPortalCrmConfigurationProvider, Site";

					var roleManager = ConfigurationManager.GetSection("system.web/roleManager") as RoleManagerSection;

					if (roleManager != null)
					{
						var roleProvider = roleManager.Providers[roleManager.DefaultProvider];
						roleProvider.Parameters["attributeMapUsername"] = "adx_identity_username";
					}

					AntiForgeryConfig.SuppressXFrameOptionsHeader = true;
				}
				else
				{
					DynamicModuleUtility.RegisterModule(typeof(SessionAuthenticationModule));
				}

				// read the connection settings from the local file

				CrmConfigurationProvider.ConfigurationCreated += OnConfigurationCreated;
				PortalCrmConfigurationProvider.ConfigurationCreated += OnConfigurationCreated;
			}
		}

		public static bool ApplicationStart()
		{
			if (InitialSetupIsRunning())
			{
				// the setup process is in progress

				RegisterInitialSetupRoutes(RouteTable.Routes);

				return true;
			}

			return false;
		}

		private static bool FormsEnabled()
		{
			var section = ConfigurationManager.GetSection("system.web/authentication") as AuthenticationSection;
			return section != null && section.Mode == AuthenticationMode.Forms;
		}

		private static bool InitialSetupIsRunning()
		{
			// the conditions where the in-app connection setup feature is allowed to run
			// - connection strings collection is empty
			// - the local connection settings (~/App_Data/settings.xml) is not yet activated

			if (ConfigurationManager.ConnectionStrings.Count != 0) return false;
			if (Section.ConnectionStrings.Count != 0) return false;
			if (SetupManager.Exists()) return false;

			return true;
		}
		
		private static void RegisterInitialSetupRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.Add(new SetupRoute(
				"{area}/{controller}/{action}",
				new { area = "Setup", controller = "Setup", action = "Index" },
				new { area = "Setup", controller = "Setup" }));
		}
		
		private static void OnConfigurationCreated(object sender, CrmSectionCreatedEventArgs args)
		{
			if (ConfigurationManager.ConnectionStrings.Count != 0 || args.Configuration.ConnectionStrings.Count != 0) return;

			args.Configuration.ConnectionStrings.Add(GetConnectionString());
		}

		private static void OnConfigurationCreated(object sender, PortalCrmSectionCreatedEventArgs args)
		{
			var portals = args.Configuration.Portals
				.Cast<PortalContextElement>()
				.Where(portal => portal.Parameters["websiteName"] == null && portal.WebsiteSelector.Parameters["websiteName"] == null)
				.ToList();

			if (!portals.Any()) return;

			var websiteName = GetWebsiteName();

			if (!string.IsNullOrWhiteSpace(websiteName))
			{
				foreach (var portal in portals)
				{
					portal.Parameters["websiteName"] = websiteName;
					portal.WebsiteSelector.Parameters["websiteName"] = websiteName;
				}
			}
		}

		private static ConnectionStringSettings GetConnectionString()
		{
			var connectionString = SetupManager.GetConnectionString();

			if (connectionString == null)
			{
				throw new ConfigurationErrorsException("The connection is undefined.");
			}

			return connectionString;
		}

		private static string GetWebsiteName()
		{
			return SetupManager.GetWebsiteName();
		}

		private static SetupManager GetSetupManager()
		{
			var type = TypeExtensions.GetType("Site.Global");

			if (type != null)
			{
				var method = type.GetMethod("GetSetupManager", BindingFlags.Public | BindingFlags.Static);

				if (method != null)
				{
					return (SetupManager) method.Invoke(null, null);
				}
			}

			return DefaultSetupManager.Create();
		}
	}
}