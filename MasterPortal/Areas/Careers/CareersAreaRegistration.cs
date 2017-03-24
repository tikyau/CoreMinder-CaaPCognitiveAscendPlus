using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Adxstudio.Xrm.Web;

namespace Site.Areas.Careers
{
	public class CareersAreaRegistration : AreaRegistration
	{
		public override string AreaName
		{
			get { return "Careers"; }
		}

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.Routes.Add("JobPostingsFeed", new Route("{area}/jobpostings.xml", null, new RouteValueDictionary(new { area = "_services" }), new JobPostingsFeedRouteHandler()));
		}

		private class JobPostingsFeedRouteHandler : IRouteHandler
		{
			public IHttpHandler GetHttpHandler(RequestContext requestContext)
			{
				return new JobPostingsFeedHandler();
			}
		}
	}
}