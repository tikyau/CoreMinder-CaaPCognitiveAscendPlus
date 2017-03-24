using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Site.Areas.AccountManagement
{
	public class AccountManagementAreaRegistration : AreaRegistration
	{
		public override string AreaName
		{
			get { return "AccountManagement"; }
		}

		public override void RegisterArea(AreaRegistrationContext context)
		{
		}
	}
}