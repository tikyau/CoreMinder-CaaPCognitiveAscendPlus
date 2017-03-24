using System;
using Adxstudio.Xrm.Search;
using Adxstudio.Xrm.Web.UI.WebControls;
using Microsoft.Xrm.Portal.Web;

namespace Site.Pages
{
	public partial class Search : PortalPage
	{
		protected void Page_Load(object sender, EventArgs e) {}

		protected string GetDisplayUrl(object urlData)
		{
			if (urlData == null)
			{
				return string.Empty;
			}

			try
			{
				return new UrlBuilder(urlData.ToString()).ToString();
			}
			catch (FormatException)
			{
				return string.Empty;
			}
		}

		protected void SearchData_OnSelected(object sender, SearchDataSourceStatusEventArgs args)
		{
			// If the SearchDataSource reports that the index was not found, try to build the index.
			if (args.Exception is IndexNotFoundException)
			{
				using (var builder = args.Provider.GetIndexBuilder())
				{
					builder.BuildIndex();
				}

				args.ExceptionHandled = true;

				// Redirect/refresh to let the user continue their search.
				Response.Redirect(Request.Url.PathAndQuery);
			}
		}
	}
}