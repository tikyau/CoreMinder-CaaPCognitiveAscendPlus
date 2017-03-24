using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Site.Areas.Partner
{
	public class PartnerAreaRegistration : AreaRegistration
	{
		public override string AreaName
		{
			get { return "Partner"; }
		}

		public override void RegisterArea(AreaRegistrationContext context)
		{
		}
	}
}