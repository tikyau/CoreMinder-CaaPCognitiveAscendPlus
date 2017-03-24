using System;
using System.Web.Mvc;
using Adxstudio.Xrm.Web.Mvc;

namespace Site.Areas.Ideas
{
	public class IdeasAreaRegistration : AreaRegistration
	{
		public override string AreaName
		{
			get { return "Ideas"; }
		}

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.MapSiteMarkerRoute(
				"IdeaActions",
				"Ideas",
				"idea/{action}/{id}",
				new { controller = "Idea", action = "Index", id = Guid.Empty });

			context.MapSiteMarkerRoute(
				"IdeasFilter",
				"Ideas",
				"{ideaForumPartialUrl}/filter/{filter}/{timeSpan}/{status}",
				new { controller = "Ideas", action = "Filter", filter = "top", timeSpan = "all-time", status = "new" });

			context.MapSiteMarkerRoute(
				"Ideas",
				"Ideas",
				"{ideaForumPartialUrl}/{ideaPartialUrl}",
				new { controller = "Ideas", action = "Ideas", ideaForumPartialUrl = UrlParameter.Optional, ideaPartialUrl = UrlParameter.Optional });
		}
	}
}
