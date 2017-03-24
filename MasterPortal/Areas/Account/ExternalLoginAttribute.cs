using System;
using System.Web.Mvc;

namespace Site.Areas.Account
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public class ExternalLoginAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			if (!filterContext.Controller.ViewBag.Settings.ExternalLoginEnabled)
			{
				filterContext.Result = new HttpNotFoundResult();

				return;
			}

			base.OnActionExecuting(filterContext);
		}
	}
}