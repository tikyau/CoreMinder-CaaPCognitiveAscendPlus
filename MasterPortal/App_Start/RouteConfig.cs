using System.Web.Mvc;
using System.Web.Routing;
using Adxstudio.Xrm.Web.Handlers;

namespace Site
{
	public static class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
			routes.IgnoreRoute("css/{resource}.bundle.css");
			routes.IgnoreRoute("js/{resource}.bundle.js");

			routes.Add(new Route("{area}/about", null, new RouteValueDictionary(new { area = "_services" }), new AboutProductHandler()));
		}
	}
}
