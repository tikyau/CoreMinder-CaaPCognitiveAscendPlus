using System;
using System.Linq;
using System.Web.Mvc;
using Adxstudio.Xrm.Blogs;
using Adxstudio.Xrm.Data;
using Adxstudio.Xrm.Forums;
using Adxstudio.Xrm.Ideas;
using Adxstudio.Xrm.Web.Mvc;
using Microsoft.Xrm.Portal.Configuration;
using Site.Areas.PublicProfile.ViewModels;

namespace Site.Areas.PublicProfile.Controllers
{
	[PortalView, PortalSecurity]
	public class PublicProfileController : Controller
	{
		public ActionResult ProfileBlogPosts(string contactId, int? page)
		{
			Guid guid;

			if (!Guid.TryParse(contactId, out guid))
			{
				return HttpNotFound();
			}

			var context = PortalCrmConfigurationManager.CreateServiceContext();

			var contact = context.CreateQuery("contact").FirstOrDefault(c => c.GetAttributeValue<Guid>("contactid") == guid);

			if (contact == null)
			{
				return HttpNotFound();
			}

			var portal = PortalCrmConfigurationManager.CreatePortalContext();
            
			var blogDataAdapter = new AuthorWebsiteBlogAggregationDataAdapter(guid,
				new Adxstudio.Xrm.Blogs.PortalContextDataAdapterDependencies(portal, requestContext: Request.RequestContext));

			var profileViewModel = new ProfileViewModel
			{
				BlogCount = blogDataAdapter.SelectBlogCount(),
				User = contact,
                Website = portal.Website
			};

			profileViewModel.BlogPosts = new PaginatedList<IBlogPost>(page, profileViewModel.BlogCount, blogDataAdapter.SelectPosts);

			return View(profileViewModel);
		}

		public ActionResult ProfileIdeas(string contactId, int? page)
		{
			Guid guid;

			if (!Guid.TryParse(contactId, out guid))
			{
				return HttpNotFound();
			}

			var context = PortalCrmConfigurationManager.CreateServiceContext();

			var contact = context.CreateQuery("contact").FirstOrDefault(c => c.GetAttributeValue<Guid>("contactid") == guid);
            var portal = PortalCrmConfigurationManager.CreatePortalContext();

			if (contact == null)
			{
				return HttpNotFound();
			}

			var ideaDataAdapter = new WebsiteIdeaUserAggregationDataAdapter(guid);

			var profileViewModel = new ProfileViewModel
			{
				IdeaCount = ideaDataAdapter.SelectIdeaCount(),
				User = contact,
                Website = portal.Website
			};

			profileViewModel.Ideas = new PaginatedList<IIdea>(page, profileViewModel.IdeaCount, ideaDataAdapter.SelectIdeas);

			return View(profileViewModel);
		}

		public ActionResult ProfileForumPosts(string contactId, int? page)
		{
			Guid guid;

			if (!Guid.TryParse(contactId, out guid))
			{
				return HttpNotFound();
			}

			var context = PortalCrmConfigurationManager.CreateServiceContext();

			var contact = context.CreateQuery("contact").FirstOrDefault(c => c.GetAttributeValue<Guid>("contactid") == guid);

			if (contact == null)
			{
				return HttpNotFound();
			}

			var portal = PortalCrmConfigurationManager.CreatePortalContext();

			var forumDataAdapter =
				new WebsiteForumPostUserAggregationDataAdapter(guid, new Adxstudio.Xrm.Forums.PortalContextDataAdapterDependencies(portal));

			var profileViewModel = new ProfileViewModel
			{
				ForumPostCount = forumDataAdapter.SelectPostCount(),
                User = contact,
                Website = portal.Website
			};

			profileViewModel.ForumPosts = new PaginatedList<IForumPost>(page, profileViewModel.ForumPostCount, forumDataAdapter.SelectPostsDescending);

			return View(profileViewModel);
		}
    }
}
