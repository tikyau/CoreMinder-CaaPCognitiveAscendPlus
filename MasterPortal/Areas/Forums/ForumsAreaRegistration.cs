using System.Web.Mvc;

namespace Site.Areas.Forums
{
	public class ForumsAreaRegistration : AreaRegistration
	{
		public override string AreaName
		{
			get { return "Forums"; }
		}

		public override void RegisterArea(AreaRegistrationContext context)
		{
		}
	}
}