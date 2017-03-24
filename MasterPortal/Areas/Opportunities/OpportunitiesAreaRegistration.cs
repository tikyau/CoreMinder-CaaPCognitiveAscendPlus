using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Site.Areas.Opportunities
{
	public class OpportunitiesAreaRegistration: AreaRegistration
	{
		public override string AreaName
		{
			get { return "Opportunities"; }
		}

		public override void RegisterArea(AreaRegistrationContext context)
		{
		}
	}
}