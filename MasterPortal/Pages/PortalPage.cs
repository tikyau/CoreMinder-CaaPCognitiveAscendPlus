using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Adxstudio.Xrm.Cms;
using Adxstudio.Xrm.Web;
using Adxstudio.Xrm.Web.Mvc;
using Microsoft.Xrm.Client;
using Microsoft.Xrm.Portal;
using Microsoft.Xrm.Portal.Configuration;
using Microsoft.Xrm.Portal.Web;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;

namespace Site.Pages
{
	public class PortalPage : PortalViewPage
	{
		public const string WebAnnotationPrefix = "*WEB*";

		public const string PublicWebAnnotationPrefix = "*PUBLIC*";

		private readonly Lazy<OrganizationServiceContext> _xrmContext;

		public PortalPage()
		{
			_xrmContext = new Lazy<OrganizationServiceContext>(() => CreateXrmServiceContext());
		}

		/// <summary>
		/// A general use <see cref="OrganizationServiceContext"/> for managing entities on the page.
		/// </summary>
		public OrganizationServiceContext XrmContext
		{
			get { return _xrmContext.Value; }
		}

		/// <summary>
		/// The current <see cref="IPortalContext"/> instance.
		/// </summary>
		public IPortalContext Portal
		{
			get { return PortalCrmConfigurationManager.CreatePortalContext(PortalName); }
		}

		/// <summary>
		/// The <see cref="OrganizationServiceContext"/> that is associated with the current <see cref="IPortalContext"/> and used to manage its entities.
		/// </summary>
		/// <remarks>
		/// This <see cref="OrganizationServiceContext"/> instance should be used when querying against the Website, User, or Entity properties.
		/// </remarks>
		public OrganizationServiceContext ServiceContext
		{
			get { return Portal.ServiceContext; }
		}

		/// <summary>
		/// The current adx_website <see cref="Entity"/>.
		/// </summary>
		public Entity Website
		{
			get { return Portal.Website; }
		}

		/// <summary>
		/// The current contact <see cref="Entity"/>.
		/// </summary>
		public Entity Contact
		{
			get { return Portal.User; }
		}

		/// <summary>
		/// The <see cref="Entity"/> representing the current page.
		/// </summary>
		public Entity Entity
		{
			get { return Portal.Entity; }
		}

		protected void AssertContactHasParentAccount()
		{
			var parentCustomer = Contact.GetAttributeValue<EntityReference>("parentcustomerid");

			if (parentCustomer == null || parentCustomer.LogicalName != "account")
			{
				throw new Exception("The logged in contact must have an account as its parent customer.");
			}
		}
		
		protected OrganizationServiceContext CreateXrmServiceContext(MergeOption? mergeOption = null)
		{
			var context = PortalCrmConfigurationManager.CreateServiceContext(PortalName);
			if (context != null && mergeOption != null) context.MergeOption = mergeOption.Value;
			return context;
		}

		private readonly IDictionary<int, string> _campaignStatusLabelCache = new Dictionary<int, string>();

		protected string GetCampaignStatusLabel(object dataItem)
		{
			var campaign = dataItem as Entity;

			if (campaign == null || campaign.GetAttributeValue<OptionSetValue>("statuscode") == null)
			{
				return string.Empty;
			}

			string cachedLabel;

			if (_campaignStatusLabelCache.TryGetValue(campaign.GetAttributeValue<OptionSetValue>("statuscode").Value, out cachedLabel))
			{
				return cachedLabel;
			}

			var response = (RetrieveAttributeResponse)ServiceContext.Execute(new RetrieveAttributeRequest
			{
				EntityLogicalName = campaign.LogicalName,
				LogicalName = "statuscode"
			});

			var statusCodeMetadata = response.AttributeMetadata as StatusAttributeMetadata;

			if (statusCodeMetadata == null)
			{
				return string.Empty;
			}

			var option = statusCodeMetadata.OptionSet.Options.FirstOrDefault(o => o.Value == campaign.GetAttributeValue<OptionSetValue>("statuscode").Value);

			if (option == null)
			{
				return string.Empty;
			}

			var label = option.Label.UserLocalizedLabel.Label;

			if (option.Value.HasValue)
			{
				_campaignStatusLabelCache[option.Value.Value] = label;
			}

			return label;
		}

		protected UrlBuilder GetUrlForRequiredSiteMarker(string siteMarkerName)
		{
			var page = ServiceContext.GetPageBySiteMarkerName(Website, siteMarkerName);

			if (page == null)
			{
				throw new Exception("Please contact your System Administrator. Required Site Marker '{0}' is missing.".FormatWith(siteMarkerName));
			}

			var path = ServiceContext.GetUrl(page);

			if (path == null)
			{
				throw new Exception("Please contact your System Administrator. Unable to build URL for Site Marker '{0}'.".FormatWith(siteMarkerName));
			}

			return new UrlBuilder(path);
		}

		protected virtual void LinqDataSourceSelecting(object sender, LinqDataSourceSelectEventArgs e)
		{
			e.Arguments.RetrieveTotalRowCount = false;
		}

		protected void RedirectToLoginIfAnonymous()
		{
			if (!Request.IsAuthenticated)
			{
				Response.ForbiddenAndEndResponse();
			}
		}

		protected void RedirectToLoginIfNecessary()
		{
			if (Request.IsAuthenticated)
				return;

			var portal = PortalCrmConfigurationManager.CreatePortalContext();

			var portalPage = portal.Entity;

			if (portalPage == null)
			{
				return;
			}

			var webPage = ServiceContext.CreateQuery("adx_webpage").FirstOrDefault(w => w.GetAttributeValue<Guid>("adx_webpageid") == portalPage.Id);

			if (webPage == null || webPage.GetRelatedEntities(ServiceContext, new Relationship("adx_webpage_sitemarker")).Any(s => s.GetAttributeValue<string>("adx_name") == "Login" || s.GetAttributeValue<string>("adx_name") == "Page Not Found"))
			{
				return;
			}

			Response.ForbiddenAndEndResponse();
		}
	}
}