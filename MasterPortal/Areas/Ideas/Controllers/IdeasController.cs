using System;
using System.Linq;
using System.Web.Mvc;
using Adxstudio.Xrm.Cms;
using Adxstudio.Xrm.Data;
using Adxstudio.Xrm.Ideas;
using Adxstudio.Xrm.Web.Mvc;
using Microsoft.Xrm.Client;
using Microsoft.Xrm.Client.Security;
using Microsoft.Xrm.Portal.Configuration;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Site.Areas.Ideas.ViewModels;
using OrganizationServiceContextExtensions = Adxstudio.Xrm.Cms.OrganizationServiceContextExtensions;

namespace Site.Areas.Ideas.Controllers
{
	[PortalView, PortalSecurity]
	public class IdeasController : Controller
	{
		private const string PageNotFoundSiteMarker = "Page Not Found";

		// GET: /ideas/{ideaForumPartialUrl}/{ideaPartialUrl}
		public ActionResult Ideas(string ideaForumPartialUrl, string ideaPartialUrl, int? page)
		{
			var context = PortalCrmConfigurationManager.CreateServiceContext();
			
			if (string.IsNullOrWhiteSpace(ideaForumPartialUrl))
			{
				return GetIdeasViewOrRedirectToOnlyIdeaForum();
			}

			var ideaForum = GetIdeaForum(ideaForumPartialUrl, context);

			if (ideaForum == null || !Authorized(context, ideaForum))
			{
				return RedirectToPageNotFound();
			}
			
			if (string.IsNullOrWhiteSpace(ideaPartialUrl))
			{
				return GetIdeaForumView(ideaForum, page);
			}

			var idea = GetIdea(ideaPartialUrl, ideaForum, context);

			if (idea == null)
			{
				return RedirectToPageNotFound();
			}

			return GetIdeaView(ideaForum, idea, page);
		}

		// GET: /ideas/{ideaForumPartialUrl}/filter/{filter}/{timeSpan}/{status}
		public ActionResult Filter(string ideaForumPartialUrl, string filter, string timeSpan, string status, int? page)
		{
			var context = PortalCrmConfigurationManager.CreateServiceContext();

			if (string.IsNullOrWhiteSpace(ideaForumPartialUrl))
			{
				return RedirectToAction("Ideas");
			}

			var ideaForum = GetIdeaForum(ideaForumPartialUrl, context);

			if (ideaForum == null || !Authorized(context, ideaForum))
			{
				return RedirectToAction("Ideas");
			}

			return GetIdeaForumView(ideaForum, page, filter, timeSpan, status);
		}

		private static bool Authorized(OrganizationServiceContext context, Entity entity)
		{
			var securityProvider = PortalCrmConfigurationManager.CreateCrmEntitySecurityProvider();

			return securityProvider.TryAssert(context, entity, CrmEntityRight.Read);
		}

		private static Entity GetIdea(string ideaPartialUrl, Entity ideaForum, OrganizationServiceContext context)
		{
			var idea = context.CreateQuery("adx_idea")
				.FirstOrDefault(adxIdea => adxIdea.GetAttributeValue<EntityReference>("adx_ideaforumid") == ideaForum.ToEntityReference()
					&& adxIdea.GetAttributeValue<string>("adx_partialurl") == ideaPartialUrl
					&& adxIdea.GetAttributeValue<bool?>("adx_approved").GetValueOrDefault(false)
					&& adxIdea.GetAttributeValue<OptionSetValue>("statecode") != null && adxIdea.GetAttributeValue<OptionSetValue>("statecode").Value == 0);
			
			return idea;
		}

		private static Entity GetIdeaForum(string ideaForumPartialUrl, OrganizationServiceContext context)
		{
			var website = PortalCrmConfigurationManager.CreatePortalContext().Website;

			var ideaForum = context.CreateQuery("adx_ideaforum")
				.FirstOrDefault(forum => forum.GetAttributeValue<EntityReference>("adx_websiteid") == website.ToEntityReference()
					&& forum.GetAttributeValue<string>("adx_partialurl") == ideaForumPartialUrl
					&& forum.GetAttributeValue<OptionSetValue>("statecode") != null && forum.GetAttributeValue<OptionSetValue>("statecode").Value == 0);
			
			return ideaForum;
		}

		private ActionResult GetIdeaForumView(Entity ideaForum, int? page, string filter = null, string timeSpan = null, string status = "new")
		{
			IdeaStatus ideaStatus;
			IdeaForumDataAdapter ideaForumDataAdapter;
			
			if (string.Equals(filter, "new", StringComparison.InvariantCultureIgnoreCase))
			{
				ideaForumDataAdapter = new IdeaForumByNewDataAdapter(ideaForum);
			}
			else if (string.Equals(filter, "hot", StringComparison.InvariantCultureIgnoreCase))
			{
				ideaForumDataAdapter = new IdeaForumByHotDataAdapter(ideaForum);
			}
			else
			{
				ideaForumDataAdapter = new IdeaForumDataAdapter(ideaForum);
			}
			
			ideaForumDataAdapter.MinDate = timeSpan == "this-year" ? DateTime.UtcNow.AddYears(-1).Date
				: timeSpan == "this-month" ? DateTime.UtcNow.AddMonths(-1).Date
				: timeSpan == "this-week" ? DateTime.UtcNow.AddDays(-7).Date
				: timeSpan == "today" ? DateTime.UtcNow.AddHours(-24)
				: (DateTime?)null;
			
			ideaForumDataAdapter.Status = Enum.TryParse(status, true, out ideaStatus) ? (int)ideaStatus : (int?)null;

			var ideaForumViewModel = new IdeaForumViewModel
			{
				IdeaForum = ideaForumDataAdapter.Select(),
				Ideas = new PaginatedList<IIdea>(page, ideaForumDataAdapter.SelectIdeaCount(), ideaForumDataAdapter.SelectIdeas)
			};

			return View("IdeaForum", ideaForumViewModel);
		}

		private ActionResult GetIdeasViewOrRedirectToOnlyIdeaForum()
		{
			var websiteDataAdapter = new WebsiteDataAdapter();

			var ideaForums = websiteDataAdapter.SelectIdeaForums().ToArray();

			var ideaForumCount = websiteDataAdapter.SelectIdeaForumCount();

			if (ideaForums.Count() == 1)
			{
				return RedirectToAction("Ideas", new { ideaForumPartialUrl = ideaForums.First().PartialUrl });
			}

			var ideasViewModel = new IdeasViewModel
			{
				IdeaForums = ideaForums,
				IdeaForumCount = ideaForumCount
			};

			return View("Ideas", ideasViewModel);
		}

		private ActionResult GetIdeaView(Entity adxIdeaForum, Entity adxIdea, int? page)
		{
			var ideaDataAdapter = new IdeaDataAdapter(adxIdea) { ChronologicalComments = true };

			var idea = ideaDataAdapter.Select();
			var comments = new PaginatedList<IComment>(page, ideaDataAdapter.SelectCommentCount(), ideaDataAdapter.SelectComments);

			var ideaViewModel = new IdeaViewModel
			{
				IdeaForum = new IdeaForumDataAdapter(adxIdeaForum).Select(),
				Idea = idea,
				Comments = new IdeaCommentsViewModel { Comments = comments, Idea = idea }
			};

			return View("Idea", ideaViewModel);
		}

		private ActionResult RedirectToPageNotFound()
		{
			var context = PortalCrmConfigurationManager.CreatePortalContext();

			var page = context.ServiceContext.GetPageBySiteMarkerName(context.Website, PageNotFoundSiteMarker);

			if (page == null)
			{
				throw new Exception("Please contact your System Administrator. Required Site Marker '{0}' is missing.".FormatWith(PageNotFoundSiteMarker));
			}

			var path = OrganizationServiceContextExtensions.GetUrl(context.ServiceContext, page);

			if (path == null)
			{
				throw new Exception("Please contact your System Administrator. Unable to build URL for Site Marker '{0}'.".FormatWith(PageNotFoundSiteMarker));
			}

			return Redirect(path);
		}
	}
}
