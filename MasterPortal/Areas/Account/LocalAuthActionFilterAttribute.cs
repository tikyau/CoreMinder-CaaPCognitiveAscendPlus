using System.Web.Mvc;
using Adxstudio.Xrm.Configuration;

namespace Site.Areas.Account
{
	public class LocalAuthActionFilterAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			if (!AdxstudioCrmConfigurationManager.GetCrmSection().MembershipProviderEnabled)
			{
				filterContext.Result = new HttpNotFoundResult();

				return;
			}

			base.OnActionExecuting(filterContext);
		}
	}
}