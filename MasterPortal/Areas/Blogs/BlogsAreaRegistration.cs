using System.Web.Mvc;

namespace Site.Areas.Blogs
{
	public class BlogsAreaRegistration : AreaRegistration
	{
		public override string AreaName
		{
			get { return "Blogs"; }
		}

		public override void RegisterArea(AreaRegistrationContext context)
		{
		}
	}
}