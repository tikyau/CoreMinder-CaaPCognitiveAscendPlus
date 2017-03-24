using System;
using System.Threading;
using System.Web.UI.WebControls;
using Adxstudio.Xrm.Blogs;
using Microsoft.Xrm.Portal;
using Microsoft.Xrm.Portal.Configuration;

namespace Site.Areas.Blogs.Pages
{
	public partial class Blogs : System.Web.UI.Page
	{
		private readonly Lazy<IPortalContext> _portal = new Lazy<IPortalContext>(() => PortalCrmConfigurationManager.CreatePortalContext(), LazyThreadSafetyMode.None);

		protected void Page_Load(object sender, EventArgs e) {}

		protected void CreateBlogAggregationDataAdapter(object sender, ObjectDataSourceEventArgs e)
		{
			e.ObjectInstance = new WebsiteBlogAggregationDataAdapter(new PortalContextDataAdapterDependencies(_portal.Value, requestContext: Request.RequestContext));
		}
	}
}