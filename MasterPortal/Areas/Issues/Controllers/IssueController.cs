using System;
using System.Linq;
using System.Web.Mvc;
using Adxstudio.Xrm.Cms;
using Adxstudio.Xrm.Data;
using Adxstudio.Xrm.Issues;
using Adxstudio.Xrm.Search;
using Adxstudio.Xrm.Web.Mvc;
using Microsoft.Xrm.Client.Security;
using Microsoft.Xrm.Portal.Configuration;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Site.Areas.Issues.ViewModels;

namespace Site.Areas.Issues.Controllers
{
	[PortalView, PortalSecurity]
	public class IssueController : Controller
	{
		public ActionResult Index()
		{
			return RedirectToAction("Issues", "Issues");
		}

		// AJAX: /issue/AlertCreate/{id}
		[HttpPost, ValidateInput(false)]
		public ActionResult AlertCreate(Guid id)
		{
			var context = PortalCrmConfigurationManager.CreateServiceContext();

			var issue = context.CreateQuery("adx_issue").FirstOrDefault(adxIssue => adxIssue.GetAttributeValue<Guid>("adx_issueid") == id);

			if (issue == null || !Authorized(context, issue))
			{
				return new EmptyResult();
			}

			var issueDataAdapter = new IssueDataAdapter(issue);
			var user = PortalCrmConfigurationManager.CreatePortalContext().User;

			if (user == null)
			{
				return new EmptyResult();
			}

			issueDataAdapter.CreateAlert(user.ToEntityReference());

			var issueViewModel = new IssueViewModel
			{
				Issue = issueDataAdapter.Select(),
				CurrentUserHasAlert = issueDataAdapter.HasAlert()
			};

			return PartialView("Tracking", issueViewModel);
		}

		// AJAX: /issue/AlertRemove/{id}
		[HttpPost, ValidateInput(false)]
		public ActionResult AlertRemove(Guid id)
		{
			var context = PortalCrmConfigurationManager.CreateServiceContext();

			var issue = context.CreateQuery("adx_issue").FirstOrDefault(adxIssue => adxIssue.GetAttributeValue<Guid>("adx_issueid") == id);

			if (issue == null || !Authorized(context, issue))
			{
				return new EmptyResult();
			}

			var issueDataAdapter = new IssueDataAdapter(issue);
			var user = PortalCrmConfigurationManager.CreatePortalContext().User;

			if (user == null)
			{
				return new EmptyResult();
			}

			issueDataAdapter.DeleteAlert(user.ToEntityReference());

			var issueViewModel = new IssueViewModel
			{
				Issue = issueDataAdapter.Select(),
				CurrentUserHasAlert = issueDataAdapter.HasAlert()
			};

			return PartialView("Tracking", issueViewModel);
		}

		// AJAX: /issue/Create/{id}
		[HttpPost, ValidateInput(false)]
		public ActionResult Create(Guid id, string title, string authorName, string authorEmail, string copy, bool track)
		{
			var context = PortalCrmConfigurationManager.CreateServiceContext();

			var issueForum = context.CreateQuery("adx_issueforum").FirstOrDefault(issueforum => issueforum.GetAttributeValue<Guid>("adx_issueforumid") == id);

			if (issueForum == null || !Authorized(context, issueForum))
			{
				return new EmptyResult();
			}

			var issueForumDataAdapter = new IssueForumDataAdapter(issueForum);

			TryAddIssue(issueForumDataAdapter, title, authorName, authorEmail, copy, track);

			return PartialView("CreateIssue", issueForumDataAdapter.Select());
		}

		// AJAX: /issue/CommentCreate/{id}
		[HttpPost, ValidateInput(false)]
		public ActionResult CommentCreate(Guid id, string authorName, string authorEmail, string copy)
		{
			var context = PortalCrmConfigurationManager.CreateServiceContext();

			var issue = context.CreateQuery("adx_issue").FirstOrDefault(adxIssue => adxIssue.GetAttributeValue<Guid>("adx_issueid") == id);

			if (issue == null || !Authorized(context, issue))
			{
				return new EmptyResult();
			}

			var issueDataAdapter = new IssueDataAdapter(issue) { ChronologicalComments = true };

			TryAddComment(issueDataAdapter, authorName, authorEmail, copy);

			var commentsViewModel = new IssueCommentsViewModel
			{
				Issue = issueDataAdapter.Select(),
				Comments = new PaginatedList<IComment>(PaginatedList.Page.Last, issueDataAdapter.SelectCommentCount(), issueDataAdapter.SelectComments)
			};

			return PartialView("Comments", commentsViewModel);
		}

		// GET: /issues/search
		public ActionResult Search(string q, int? page)
		{
			if (string.IsNullOrWhiteSpace(q))
			{
				return new EmptyResult();
			}

			var pageNumber = page.GetValueOrDefault(1);

			var searchProvider = SearchManager.Provider;

			var query = new CrmEntityQuery(q, pageNumber, PaginatedList.PageSize, new[] { "adx_issue" });

			ICrmEntityIndexSearcher searcher;
			
			try
			{
				searcher = searchProvider.GetIndexSearcher();
			}
			catch(IndexNotFoundException)
			{
				searchProvider.GetIndexBuilder().BuildIndex();
				
				searcher = searchProvider.GetIndexSearcher();
			}
			
			var results = searcher.Search(query);

			searcher.Dispose();

			return View("SearchResults", results);
		}

		private static bool Authorized(OrganizationServiceContext context, Entity entity)
		{
			var securityProvider = PortalCrmConfigurationManager.CreateCrmEntitySecurityProvider();

			return securityProvider.TryAssert(context, entity, CrmEntityRight.Read);
		}

		private bool TryAddComment(IssueDataAdapter issueDataAdapter, string authorName, string authorEmail, string content)
		{
			if (!Request.IsAuthenticated)
			{
				if (string.IsNullOrWhiteSpace(authorName))
				{
					ModelState.AddModelError("authorName", "Your name is required.");
				}

				if (string.IsNullOrWhiteSpace(authorEmail))
				{
					ModelState.AddModelError("authorEmail", "Email is required; it will not be displayed.");
				}
			}

			if (string.IsNullOrWhiteSpace(content))
			{
				ModelState.AddModelError("content", "Comment is required.");
			}

			if (!ModelState.IsValid)
			{
				return false;
			}

			issueDataAdapter.CreateComment(content, authorName, authorEmail);

			return true;
		}

		private bool TryAddIssue(IssueForumDataAdapter issueForumDataAdapter, string title, string authorName, string authorEmail, string copy, bool track)
		{
			if (!Request.IsAuthenticated)
			{
				if (string.IsNullOrWhiteSpace(authorName))
				{
					ModelState.AddModelError("authorName", "Your name is required.");
				}

				if (string.IsNullOrWhiteSpace(authorEmail))
				{
					ModelState.AddModelError("authorEmail", "Email is required; it will not be displayed.");
				}
			}

			if (string.IsNullOrWhiteSpace(title))
			{
				ModelState.AddModelError("title", "Issue is required.");
			}

			if (!ModelState.IsValid)
			{
				return false;
			}

			issueForumDataAdapter.CreateIssue(title, copy, track, authorName, authorEmail);

			return true;
		}
	}
}
