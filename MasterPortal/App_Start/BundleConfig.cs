using System.Web.Optimization;

namespace Site
{
	public static class BundleConfig
	{
		public static void RegisterBundles(BundleCollection bundles)
		{
			bundles.Add(new StyleBundle("~/css/default.bundle.css").Include(
				"~/js/google-code-prettify/prettify.css",
				"~/css/bootstrap-datetimepicker.min.css",
				"~/css/portal.css",
				"~/css/comments.css",
				"~/css/ratings.css",
				"~/css/map.css",
				"~/css/webforms.css"));

			bundles.Add(new ScriptBundle("~/js/default.preform.bundle.js").Include(
				"~/js/jquery-1.11.1.min.js",
				"~/js/jquery-migrate-{version}.js",
				"~/js/respond.min.js",
				"~/js/underscore-min.js",
				"~/js/moment-with-locales.min.js",
				"~/js/DateFormat.js",
				"~/js/bootstrap-datetimepicker.min.js"));

			bundles.Add(new ScriptBundle("~/js/default.bundle.js").Include(
				"~/js/bootstrap.min.js",
				"~/js/eventListener.polyfill.js",
				"~/js/handlebars.js",
				"~/js/date.js",
				"~/js/jquery.timeago.js",
				"~/js/google-code-prettify/prettify.js",
				"~/js/jquery.cookie.js",
				"~/js/jquery.bootstrap-pagination.js",
				"~/js/jquery.blockUI.js",
				"~/js/entity-notes.js",
				"~/js/entity-form.js",
				"~/js/entity-grid.js",
				"~/js/entity-associate.js",
				"~/js/entity-lookup.js",
				"~/js/quickform.js",
				"~/js/URI.min.js",
				"~/js/serialized-query.js",
				"~/Areas/Cms/js/ads.js",
				"~/Areas/Cms/js/polls.js",
				"~/Areas/Parature/js/case-deflection.js",
				"~/Areas/Parature/js/submit-ticket.js",
				"~/Areas/Parature/js/ticket-actions.js",
				"~/Areas/Parature/js/ticket-add-attachments.js",
				"~/js/badges.js",
				"~/js/sharepoint-grid.js",
				"~/js/portal.js"));
		}
	}
}
