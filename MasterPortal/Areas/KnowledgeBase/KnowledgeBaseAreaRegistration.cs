using System.Web.Mvc;
using Adxstudio.Xrm.Web.Mvc;

namespace Site.Areas.KnowledgeBase
{
	public class KnowledgeBaseAreaRegistration : AreaRegistration
	{
		public override string AreaName
		{
			get { return "KnowledgeBase"; }
		}

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.MapSiteMarkerRoute(
				"KnowledgeBaseArticle",
				"Knowledge Base",
				"{number}",
				new {controller = "Article", action = "Index"});
		}
	}
}