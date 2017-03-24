using Adxstudio.Xrm.Data;
using Adxstudio.Xrm.Ideas;

namespace Site.Areas.Ideas.ViewModels
{
	public class IdeaForumViewModel
	{
		public IIdeaForum IdeaForum { get; set; }

		public PaginatedList<IIdea> Ideas { get; set; }
	}
}