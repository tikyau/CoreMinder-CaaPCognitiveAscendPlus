using System.Web.Mvc;
using Microsoft.Xrm.Portal.IdentityModel.Configuration;

namespace Site.Areas.Account
{
	public class RegistrationActionFilterAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			var registrationSettings = FederationCrmConfigurationManager.GetUserRegistrationSettings();

			if (!registrationSettings.Enabled)
			{
				filterContext.Result = new HttpNotFoundResult();

				return;
			}

			base.OnActionExecuting(filterContext);
		}
	}
}