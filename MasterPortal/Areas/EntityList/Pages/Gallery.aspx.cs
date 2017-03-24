using System;
using Site.Pages;

namespace Site.Areas.EntityList.Pages
{
	public partial class Gallery : PortalPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			AddCrossOriginAccessHeaders();
		}

		private void AddCrossOriginAccessHeaders()
		{
			Response.Headers["Access-Control-Allow-Headers"] = "*";
			Response.Headers["Access-Control-Allow-Origin"] = "*";
		}
	}
}
