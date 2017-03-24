
using System.Web.Mvc;

namespace Site.Areas.Customer
{
	public class CustomerAreaRegistration : AreaRegistration
	{
		public override string AreaName
		{
			get { return "Customer"; }
		}

		public override void RegisterArea(AreaRegistrationContext context)
		{
		}
	}
}