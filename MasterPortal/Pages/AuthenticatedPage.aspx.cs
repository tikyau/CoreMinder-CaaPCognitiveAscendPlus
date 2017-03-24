using System;

namespace Site.Pages
{
	public partial class AuthenticatedPage : PortalPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			RedirectToLoginIfAnonymous();
		}
	}
}
