﻿using System.Web.Mvc;
using System.Web.Routing;

namespace Site.Areas.Company
{
	public class CoompanyAreaRegistration : AreaRegistration
	{
		public override string AreaName
		{
			get { return "Company"; }
		}

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.Routes.Add("NewsFeed", new Route("{area}/newsfeed.xml", null, new RouteValueDictionary(new { area = "_services" }), new NewsFeedHandler.RouteHandler()));
		}
	}
}