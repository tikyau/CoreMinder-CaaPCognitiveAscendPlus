using Adxstudio.Xrm.Data;
using Adxstudio.Xrm.Forums;
using Adxstudio.Xrm.Ideas;
using Adxstudio.Xrm.Blogs;
using Microsoft.Xrm.Sdk;

namespace Site.Areas.PublicProfile.ViewModels
{
	public class ProfileViewModel
	{
		public Entity User { get; set; }

        public Entity Website { get; set; }

		public int IdeaCount { get; set; }

		public int BlogCount { get; set; }

		public int ForumPostCount { get; set; }

		public PaginatedList<IIdea> Ideas { get; set; }

		public PaginatedList<IBlogPost> BlogPosts { get; set; }

		public PaginatedList<IForumPost> ForumPosts { get; set; }
	}
}