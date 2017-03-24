using System.Web.Mvc;

namespace Site.Areas.Community
{
	public class CommunityAreaRegistration : AreaRegistration
	{
		public override string AreaName
		{
			get { return "Community"; }
		}

		public override void RegisterArea(AreaRegistrationContext context)
		{
		}
	}
}