﻿using System;
using System.Threading;
using System.Web.UI.WebControls;
using Adxstudio.Xrm.Forums;
using Adxstudio.Xrm.Web.Mvc.Html;
using Microsoft.Xrm.Portal;
using Microsoft.Xrm.Portal.Configuration;
using Site.Pages;

namespace Site.Areas.Forums.Pages
{
	public partial class Forums : PortalPage
	{
		private readonly Lazy<IPortalContext> _portal = new Lazy<IPortalContext>(() => PortalCrmConfigurationManager.CreatePortalContext(), LazyThreadSafetyMode.None);

		protected void Page_Load(object sender, EventArgs e) {}

		protected void CreateForumAggregationDataAdapter(object sender, ObjectDataSourceEventArgs args)
		{
			args.ObjectInstance = new WebsiteForumDataAdapter(new PortalContextDataAdapterDependencies(
				_portal.Value,
				new PaginatedLatestPostUrlProvider("page", Html.IntegerSetting("Forums/PostsPerPage").GetValueOrDefault(20)),
				requestContext:Request.RequestContext));
		} 
	}
}
