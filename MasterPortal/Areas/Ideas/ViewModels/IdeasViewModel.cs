using System.Collections.Generic;
using Adxstudio.Xrm.Ideas;

namespace Site.Areas.Ideas.ViewModels
{
	public class IdeasViewModel
	{
		public int IdeaForumCount { get; set; }

		public IEnumerable<IIdeaForum> IdeaForums { get; set; }
	}
}