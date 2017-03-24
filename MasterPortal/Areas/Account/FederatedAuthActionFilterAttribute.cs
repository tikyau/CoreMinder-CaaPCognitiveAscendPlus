using System.Web.Mvc;
using Adxstudio.Xrm.Configuration;
using Adxstudio.Xrm.OpenAuth.Configuration;

namespace Site.Areas.Account
{
	public class FederatedAuthActionFilterAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			if (!OpenAuthConfigurationManager.GetSection().Enabled && !AdxstudioCrmConfigurationManager.GetCrmSection().IdentityModelEnabled)
			{
				filterContext.Result = new HttpNotFoundResult();

				return;
			}

			base.OnActionExecuting(filterContext);
		}
	}
}