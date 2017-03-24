using System;
using System.Web.Mvc;

namespace Site.Areas.Account
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public class LocalLoginAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			if (!filterContext.Controller.ViewBag.Settings.LocalLoginEnabled)
			{
				filterContext.Result = new HttpNotFoundResult();

				return;
			}

			base.OnActionExecuting(filterContext);
		}
	}
}