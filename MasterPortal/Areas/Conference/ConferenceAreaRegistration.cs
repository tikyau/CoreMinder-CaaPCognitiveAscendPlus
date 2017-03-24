using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Site.Areas.Conference
{
	public class ConferenceAreaRegistration : AreaRegistration
	{
		public override string AreaName
		{
			get { return "Conference"; }
		}

		public override void RegisterArea(AreaRegistrationContext context)
		{
		}
	}
}