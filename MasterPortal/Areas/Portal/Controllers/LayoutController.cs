using System.Web.Mvc;
using Adxstudio.Xrm.Web.Mvc;
using DevTrends.MvcDonutCaching;
using Site.Helpers;

namespace Site.Areas.Portal.Controllers
{
	public class LayoutController : Controller
	{
		[ChildActionOnly, PortalView, DonutOutputCache(CacheProfile = "UserShared", Options = OutputCacheOptions.ReplaceDonutsInChildActions)]
		public ActionResult Header()
		{
			ViewBag.ViewSupportsDonuts = true;

			return PartialView("Header");
		}

		[ChildActionOnly, PortalView, DonutOutputCache(CacheProfile = "Roles")]
		public ActionResult HeaderChildNavbar()
		{
			return PartialView("HeaderChildNavbar");
		}

		[ChildActionOnly, PortalView, DonutOutputCache(CacheProfile = "RolesShared")]
		public ActionResult HeaderPrimaryNavigation()
		{
			return PartialView("HeaderPrimaryNavigation");
		}

		[ChildActionOnly, PortalView, DonutOutputCache(CacheProfile = "RolesShared")]
		public ActionResult HeaderPrimaryNavigationTabs()
		{
			return PartialView("HeaderPrimaryNavigationTabs");
		}

		[ChildActionOnly, PortalView, DonutOutputCache(CacheProfile = "RolesShared")]
		public ActionResult HeaderPrimaryNavigationXs()
		{
			return PartialView("HeaderPrimaryNavigationXs");
		}

		[ChildActionOnly, PortalView, DonutOutputCache(CacheProfile = "UserShared", Options = OutputCacheOptions.ReplaceDonutsInChildActions)]
		public ActionResult Footer()
		{
			ViewBag.ViewSupportsDonuts = true;

			return PartialView("Footer");
		}

		[ChildActionOnly, PortalView]
		public ActionResult RegisterUrl()
		{
			return Content(Url.RegisterUrl());
		}

		[ChildActionOnly, PortalView]
		public ActionResult SignInLink()
		{
			return PartialView("SignInLink");
		}

		[ChildActionOnly, PortalView]
		public ActionResult SignInUrl()
		{
			return Content(Url.SignInUrl());
		}

		[ChildActionOnly, PortalView]
		public ActionResult SignOutUrl()
		{
			return Content(Url.SignOutUrl());
		}
	}
}