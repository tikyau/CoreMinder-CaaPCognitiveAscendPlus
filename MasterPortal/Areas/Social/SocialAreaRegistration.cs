using System.Web.Mvc;

namespace Site.Areas.Social
{
	public class SocialAreaRegistration : AreaRegistration
	{
		public override string AreaName
		{
			get
			{
				return "Social";
			}
		}

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.MapRoute(
				"Social_default",
				"Social/{controller}/{action}/{query}",
				new { action = "Search", query = UrlParameter.Optional }
			);
		}
	}
}
