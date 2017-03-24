using System;
using System.Web;
using System.Web.Mvc;
using Adxstudio.Xrm.AspNet.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Site.Areas.Account.Models;
using Site.Areas.Account.ViewModels;

namespace Site.MasterPages
{
	public partial class Profile : PortalMasterPage
	{
		public bool OwinEnabled
		{
			get { return UserManager != null && WebsiteManager != null; }
		}

		public ApplicationUserManager UserManager
		{
			get { return HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
		}

		public ApplicationWebsiteManager WebsiteManager
		{
			get { return HttpContext.Current.GetOwinContext().GetUserManager<ApplicationWebsiteManager>(); }
		}

		private ViewDataDictionary _viewData;

		public ViewDataDictionary ViewData
		{
			get
			{
				if (_viewData == null)
				{
					if (!OwinEnabled) return null;

					var website = Request.GetOwinContext().Get<ApplicationWebsite>();
					var settings = website.GetAuthenticationSettings<ApplicationWebsite, string>();
					var user = UserManager.FindById(HttpContext.Current.User.Identity.GetUserId());
					var nav = user == null
						? new ManageNavSettings()
						: new ManageNavSettings
						{
							HasPassword = UserManager.HasPassword(HttpContext.Current.User.Identity.GetUserId()),
							IsEmailConfirmed = string.IsNullOrWhiteSpace(user.Email) || user.EmailConfirmed,
							IsMobilePhoneConfirmed = string.IsNullOrWhiteSpace(user.PhoneNumber) || user.PhoneNumberConfirmed,
							IsTwoFactorEnabled = user.TwoFactorEnabled,
						};

					_viewData = new ViewDataDictionary(Html.ViewData) { { "Settings", settings }, { "Nav", nav } };
				}

				return _viewData;
			}
		}

		public bool EmailConfirmationEnabled
		{
			get
			{
				if (ViewData == null) return true;
				var settings = ViewData["Settings"] as AuthenticationSettings;
				return settings != null && settings.EmailConfirmationEnabled;
			}
		}

		public bool IsEmailConfirmed
		{
			get
			{
				if (ViewData == null) return false;
				var nav = ViewData["Nav"] as ManageNavSettings;
				return nav != null && nav.IsEmailConfirmed;
			}
		}

		protected void Page_Load(object sender, EventArgs e) {}
	}
}