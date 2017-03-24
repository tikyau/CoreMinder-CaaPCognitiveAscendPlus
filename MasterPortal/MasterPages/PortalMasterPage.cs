using System;
using System.Web.UI.WebControls;
using Adxstudio.Xrm.Account;
using Adxstudio.Xrm.Cms;
using Adxstudio.Xrm.Web;
using Adxstudio.Xrm.Web.Mvc;
using Microsoft.Security.Application;
using Microsoft.Xrm.Client;
using Microsoft.Xrm.Portal;
using Microsoft.Xrm.Portal.Configuration;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;

namespace Site.MasterPages
{
	public class PortalMasterPage : PortalViewMasterPage
	{
		private readonly Lazy<OrganizationServiceContext> _xrmContext;

		public PortalMasterPage()
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

		public bool ForceRegistration
		{
			get
			{
				var siteSetting = ServiceContext.GetSiteSettingValueByName(Website, "Profile/ForceSignUp") ?? "false";

				bool value;

				return bool.TryParse(siteSetting, out value) && value;
			}
		}

		protected override void OnInit(EventArgs args)
		{
			base.OnInit(args);

			if (!ForceRegistration)
			{
				return;
			}

			if (!Request.IsAuthenticated || Contact == null)
			{
				return;
			}

			var profilePage = ServiceContext.GetPageBySiteMarkerName(Website, "Profile");

			if (profilePage == null || Entity.ToEntityReference().Equals(profilePage.ToEntityReference()))
			{
				return;
			}

			var profilePath = ServiceContext.GetUrl(profilePage);

			var returnUrl = Encoder.UrlEncode(Request.Url.PathAndQuery);

			var profileUrl = "{0}{1}{2}={3}".FormatWith(profilePath, profilePath.Contains("?") ? "&" : "?", "ReturnURL", returnUrl);

			if (!ServiceContext.ValidateProfileSuccessfullySaved(Contact))
			{
				Context.RedirectAndEndResponse(profileUrl);
			}
		}

		protected OrganizationServiceContext CreateXrmServiceContext(MergeOption? mergeOption = null)
		{
			var context = PortalCrmConfigurationManager.CreateServiceContext(PortalName);
			if (context != null && mergeOption != null) context.MergeOption = mergeOption.Value;
			return context;
		}

		protected virtual void LinqDataSourceSelecting(object sender, LinqDataSourceSelectEventArgs e)
		{
			e.Arguments.RetrieveTotalRowCount = false;
		}
	}
}