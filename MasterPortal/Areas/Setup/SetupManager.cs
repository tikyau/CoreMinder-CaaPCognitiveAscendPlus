using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Web.Hosting;
using System.Web.Security;
using System.Xml.Linq;
using Microsoft.Xrm.Client;
using Microsoft.Xrm.Client.Services;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;

namespace Site.Areas.Setup
{
	public abstract class SetupManager
	{
		public abstract bool Exists();
		public abstract ConnectionStringSettings GetConnectionString();
		public abstract string GetWebsiteName();
		public abstract void Save(Uri organizationServiceUrl, AuthenticationProviderType authenticationType, string domain, string username, string password, Guid websiteId);
		
		protected virtual void SaveWebsiteBinding(Guid websiteId)
		{
			using (var service = new OrganizationService(new CrmConnection(GetConnectionString())))
			{
				var website = new EntityReference("adx_website", websiteId);
				var siteName = Adxstudio.Xrm.Cms.WebsiteSelectors.Extensions.GetSiteName();
				var virtualPath = HostingEnvironment.ApplicationVirtualPath ?? "/";

				var query = new QueryExpression("adx_websitebinding") {ColumnSet = new ColumnSet("adx_websiteid")};
				query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
				query.Criteria.AddCondition("adx_sitename", ConditionOperator.Equal, siteName);

				var pathFilter = query.Criteria.AddFilter(LogicalOperator.Or);
				if (!virtualPath.StartsWith("/")) virtualPath = "/" + virtualPath;
				pathFilter.AddCondition("adx_virtualpath", ConditionOperator.Equal, virtualPath);
				pathFilter.AddCondition("adx_virtualpath", ConditionOperator.Equal, virtualPath.Substring(1));
				if (virtualPath.Substring(1) == string.Empty)
				{
					pathFilter.AddCondition("adx_virtualpath", ConditionOperator.Null);
				}

				var bindings = service.RetrieveMultiple(query);

				if (bindings.Entities.Count == 0)
				{
					var websiteBinding = CreateWebsiteBinding(website, siteName, virtualPath);
					service.Create(websiteBinding);
				}
			}
		}
		
		private static Entity CreateWebsiteBinding(EntityReference websiteId, string siteName, string virtualPath)
		{
			var websiteBinding = new Entity("adx_websitebinding");
			websiteBinding.SetAttributeValue<string>("adx_name", "Binding: {0}{1}".FormatWith(siteName, virtualPath));
			websiteBinding.SetAttributeValue<EntityReference>("adx_websiteid", websiteId);
			websiteBinding.SetAttributeValue<string>("adx_sitename", siteName);
			websiteBinding.SetAttributeValue<string>("adx_virtualpath", virtualPath);

			return websiteBinding;
		}
	}

	internal class DefaultSetupManager : SetupManager
	{
		private DefaultSetupManager() {}

		public static SetupManager Create()
		{
			return new DefaultSetupManager();
		}

		public override void Save(Uri organizationServiceUrl, AuthenticationProviderType authenticationType, string domain, string username, string password, Guid websiteId)
		{
			var xml = new XElement("settings");
			xml.SetElementValue("organizationServiceUrl", organizationServiceUrl.OriginalString);
			xml.SetElementValue("authenticationType", authenticationType.ToString());
			xml.SetElementValue("domain", domain);
			xml.SetElementValue("username", username);
			xml.SetElementValue("password", Convert.ToBase64String(MachineKey.Protect(Encoding.UTF8.GetBytes(password), _machineKeyPurposes)));
			xml.Save(_settingsPath.Value);

			try
			{
				SaveWebsiteBinding(websiteId);
			}
			catch (Exception)
			{
				File.Delete(_settingsPath.Value);
			}
		}

		public override bool Exists()
		{
			return File.Exists(_settingsPath.Value);
		}

		public override ConnectionStringSettings GetConnectionString()
		{
			return Exists() ? _connectionString.Value : null;
		}

		public override string GetWebsiteName()
		{
			return Exists() ? _websiteName.Value : null;
		}

		private static readonly Lazy<string> _settingsPath = new Lazy<string>(GetSettingsPath);
		private static readonly Lazy<ConnectionStringSettings> _connectionString = new Lazy<ConnectionStringSettings>(InitConnectionString);
		private static readonly Lazy<string> _websiteName = new Lazy<string>(InitWebsiteName);
		private static readonly string[] _machineKeyPurposes = { "adxstudio", "setup" };

		private static string GetSettingsPath()
		{
			var virtualPath = ConfigurationManager.AppSettings[typeof(SetupManager).FullName + ".SettingsPath"] ?? "~/App_Data/settings.xml";
			var settingsPath = HostingEnvironment.MapPath(virtualPath);
			var dataDirectory = Path.GetDirectoryName(settingsPath);

			if (!Directory.Exists(dataDirectory))
			{
				Directory.CreateDirectory(dataDirectory);
			}

			return settingsPath;
		}

		private static ConnectionStringSettings InitConnectionString()
		{
			var xml = XElement.Load(_settingsPath.Value);
			var organizationServiceUrl = GetUrl(xml, "organizationServiceUrl");
			var authenticationType = GetEnum<AuthenticationProviderType>(xml, "authenticationType");
			var domain = GetText(xml, "domain");
			var username = GetText(xml, "username");
			var password = Encoding.UTF8.GetString(MachineKey.Unprotect(Convert.FromBase64String(GetText(xml, "password")), _machineKeyPurposes));

			var connectionString = authenticationType == AuthenticationProviderType.ActiveDirectory
				? "Url={0}; Username={1}; Password={2}; Domain={3};".FormatWith(organizationServiceUrl, username, password, domain)
				: "Url={0}; Username={1}; Password={2};".FormatWith(organizationServiceUrl, username, password);

			return new ConnectionStringSettings("Xrm", connectionString);
		}

		private static string InitWebsiteName()
		{
			var xml = XElement.Load(_settingsPath.Value);
			return GetText(xml, "websiteName");
		}

		private static string GetText(XContainer xml, string tagName)
		{
			var element = xml.Element(tagName);
			return element != null ? element.Value : null;
		}

		private static Uri GetUrl(XContainer xml, string tagName)
		{
			var text = GetText(xml, tagName);
			return !string.IsNullOrWhiteSpace(text) ? new Uri(text) : null;
		}

		private static T? GetEnum<T>(XContainer xml, string tagName) where T : struct
		{
			var text = GetText(xml, tagName);
			return !string.IsNullOrWhiteSpace(text) ? text.ToEnum<T>() as T? : null;
		}
	}
}