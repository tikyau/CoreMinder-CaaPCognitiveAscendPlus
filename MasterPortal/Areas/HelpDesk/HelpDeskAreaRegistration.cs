using System.Web.Mvc;

namespace Site.Areas.HelpDesk
{
	public class HelpDeskAreaRegistration : AreaRegistration
	{
		public override string AreaName
		{
			get { return "HelpDesk"; }
		}

		public override void RegisterArea(AreaRegistrationContext context)
		{
		}
	}
}