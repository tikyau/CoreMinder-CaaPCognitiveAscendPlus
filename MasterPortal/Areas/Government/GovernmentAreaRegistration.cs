using System.Web.Mvc;

namespace Site.Areas.Government
{
	public class GovernmentAreaRegistration : AreaRegistration
	{
		public override string AreaName
		{
			get
			{
				return "Government";
			}
		}

		public override void RegisterArea(AreaRegistrationContext context)
		{
		}
	}
}
