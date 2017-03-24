using System;
using System.Linq;
using System.Web.Mvc;
using Adxstudio.Xrm.Cms;
using Adxstudio.Xrm.Data;
using Adxstudio.Xrm.Ideas;
using Adxstudio.Xrm.Search;
using Adxstudio.Xrm.Web.Mvc;
using Microsoft.Xrm.Client.Security;
using Microsoft.Xrm.Portal.Configuration;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Site.Areas.Ideas.ViewModels;

namespace Site.Areas.Ideas.Controllers
{
	[PortalView, PortalSecurity]
	public class IdeaController : Controller
	{
		public ActionResult Index()
		{
			return RedirectToAction("Ideas", "Ideas");
		}

		// AJAX: /idea/Create/{id}
		[HttpPost, ValidateInput(false)]
		public ActionResult Create(Guid id, string title, string authorName, string authorEmail, string copy)
		{
			var context = PortalCrmConfigurationManager.CreateServiceContext();

			var ideaForum = context.CreateQuery("adx_ideaforum").FirstOrDefault(ideaforum => ideaforum.GetAttributeValue<Guid>("adx_ideaforumid") == id);

			if (ideaForum == null || !Authorized(context, ideaForum))
			{
				return new EmptyResult();
			}

			var ideaForumDataAdapter = new IdeaForumDataAdapter(ideaForum);

			TryAddIdea(ideaForumDataAdapter, title, authorName, authorEmail, copy);

			return PartialView("CreateIdea", ideaForumDataAdapter.Select());
		}

		// AJAX: /idea/CommentCreate/{id}
		[HttpPost, ValidateInput(false)]
		public ActionResult CommentCreate(Guid id, string authorName, string authorEmail, string copy)
		{
			var context = PortalCrmConfigurationManager.CreateServiceContext();

			var idea = context.CreateQuery("adx_idea").FirstOrDefault(adxIdea => adxIdea.GetAttributeValue<Guid>("adx_ideaid") == id);

			if (idea == null || !Authorized(context, idea))
			{
				return new EmptyResult();
			}

			var ideaDataAdapter = new IdeaDataAdapter(idea) { ChronologicalComments = true };

			TryAddComment(ideaDataAdapter, authorName, authorEmail, copy);

			var commentsViewModel = new IdeaCommentsViewModel
			{
				Idea = ideaDataAdapter.Select(),
				Comments = new PaginatedList<IComment>(PaginatedList.Page.Last, ideaDataAdapter.SelectCommentCount(), ideaDataAdapter.SelectComments)
			};

			return PartialView("Comments", commentsViewModel);
		}

		// GET: /ideas/search
		public ActionResult Search(string q, int? page)
		{
			if (string.IsNullOrWhiteSpace(q))
			{
				return new EmptyResult();
			}

			var pageNumber = page.GetValueOrDefault(1);

			var searchProvider = SearchManager.Provider;

			var query = new CrmEntityQuery(q, pageNumber, PaginatedList.PageSize, new[] { "adx_idea" });

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

		// AJAX: /idea/Vote/{id}
		[HttpPost]
		public ActionResult Vote(int voteValue, Guid id)
		{
			IdeaDataAdapter ideaDataAdapter;

			if (!TryAddVote(voteValue, id, out ideaDataAdapter))
			{
				return new EmptyResult();
			}

			return PartialView("Votes", ideaDataAdapter.Select());
		}

		private static bool Authorized(OrganizationServiceContext context, Entity entity)
		{
			var securityProvider = PortalCrmConfigurationManager.CreateCrmEntitySecurityProvider();

			return securityProvider.TryAssert(context, entity, CrmEntityRight.Read);
		}

		private bool TryAddComment(IdeaDataAdapter ideaDataAdapter, string authorName, string authorEmail, string content)
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

			ideaDataAdapter.CreateComment(content, authorName, authorEmail);

			return true;
		}

		private bool TryAddIdea(IdeaForumDataAdapter ideaForumDataAdapter, string title, string authorName, string authorEmail, string copy)
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
				ModelState.AddModelError("title", "Idea is required.");
			}

			if (!ModelState.IsValid)
			{
				return false;
			}

			ideaForumDataAdapter.CreateIdea(title, copy, authorName, authorEmail);

			return true;
		}

		private bool TryAddVote(int voteValue, Guid ideaId, out IdeaDataAdapter ideaDataAdapter)
		{
			ideaDataAdapter = null;

			var context = PortalCrmConfigurationManager.CreateServiceContext();

			var idea = context.CreateQuery("adx_idea").FirstOrDefault(adxIdea => adxIdea.GetAttributeValue<Guid>("adx_ideaid") == ideaId);

			if (idea == null || !Authorized(context, idea))
			{
				return false;
			}

			ideaDataAdapter = new IdeaDataAdapter(idea);

			if (!ideaDataAdapter.Select().CurrentUserCanVote(voteValue))
			{
				return false;
			}

			ideaDataAdapter.CreateVote(voteValue, "Anonymous", "anonymous@sample.com");

			return true;
		}
	}
}
