using Adxstudio.Xrm.Cms;
using Adxstudio.Xrm.Data;
using Adxstudio.Xrm.Issues;

namespace Site.Areas.Issues.ViewModels
{
	public class IssueCommentsViewModel
	{
		public PaginatedList<IComment> Comments { get; set; }

		public IIssue Issue { get; set; }
	}
}