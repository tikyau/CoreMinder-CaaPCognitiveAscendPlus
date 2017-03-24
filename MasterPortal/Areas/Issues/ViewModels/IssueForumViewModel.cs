using Adxstudio.Xrm.Data;
using Adxstudio.Xrm.Issues;

namespace Site.Areas.Issues.ViewModels
{
	public class IssueForumViewModel
	{
		public IIssueForum IssueForum { get; set; }

		public PaginatedList<IIssue> Issues { get; set; }
	}
}