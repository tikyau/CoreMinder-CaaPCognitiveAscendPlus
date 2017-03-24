using System;
using Adxstudio.Xrm.IdentityModel;
using Microsoft.IdentityModel.Claims;

namespace Site.Pages
{
	public partial class ChangePassword : PortalPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			RedirectToLoginIfAnonymous();

			IClaimsIdentity identity;

			var isFederatedIdentity = Context.User.TryGetFederatedIdentity(out identity);

			ChangePasswordPanel.Visible = !isFederatedIdentity;
			FederatedIdentityMessage.Visible = isFederatedIdentity;
		}
	}
}