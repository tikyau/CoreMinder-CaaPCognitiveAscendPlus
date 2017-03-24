using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Site.Areas.CustomerManagement
{
	public class CustomerManagementAreaRegistration: AreaRegistration
	{
		public override string AreaName
		{
			get { return "CustomerManagement"; }
		}

		public override void RegisterArea(AreaRegistrationContext context)
		{
		}
	}
}