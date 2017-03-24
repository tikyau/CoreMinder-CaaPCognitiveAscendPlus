using System;

namespace Site.MasterPages
{
	public partial class WebForms : PortalMasterPage
	{
		protected void Page_PreRender(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(Page.Form.Action))
			{
				Page.Form.Action = Request.Url.PathAndQuery;
			}
		}
	}
}