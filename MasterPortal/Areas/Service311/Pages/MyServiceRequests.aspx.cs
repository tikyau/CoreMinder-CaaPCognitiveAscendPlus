using System;
using System.Linq;
using System.Web.UI.WebControls;
using Adxstudio.Xrm.Web.UI;
using Microsoft.Xrm.Sdk;
using Site.Pages;

namespace Site.Areas.Service311.Pages
{
	public partial class MyServiceRequests : PortalPage
	{
		private const string SavedQueryName = "Status Web View";
		private const string EntityName = "adx_servicerequest";

		protected void Page_Load(object sender, EventArgs e)
		{
			RedirectToLoginIfAnonymous();


		}

	}
}