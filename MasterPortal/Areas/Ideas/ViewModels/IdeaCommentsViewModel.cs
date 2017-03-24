using Adxstudio.Xrm.Cms;
using Adxstudio.Xrm.Data;
using Adxstudio.Xrm.Ideas;

namespace Site.Areas.Ideas.ViewModels
{
	public class IdeaCommentsViewModel
	{
		public PaginatedList<IComment> Comments { get; set; }

		public IIdea Idea { get; set; }
	}
}