using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Site.Areas.ContactUs
{
	public class ContactUsAreaRegistration : AreaRegistration
	{
		public override string AreaName
		{
			get { return "ContactUs"; }
		}

		public override void RegisterArea(AreaRegistrationContext context)
		{
		}
	}
}