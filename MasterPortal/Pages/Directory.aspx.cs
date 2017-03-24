using System;
using Microsoft.Xrm.Sdk;

namespace Site.Pages
{
	public partial class Directory : PortalPage
	{
		protected void Page_Load(object sender, EventArgs e) {}

		protected bool IsWebPage(object dataItem)
		{
			var entity = dataItem as Entity;

			return entity != null && entity.LogicalName == "adx_webpage";
		}
	}
}