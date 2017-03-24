﻿using System;
using Adxstudio.Xrm.Web.UI.WebControls;
using Adxstudio.Xrm.Web.UI.WebForms;

namespace Site.Areas.HelpDesk.Controls
{
	public partial class WebFormSupportRequestEntitlement : WebFormUserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			CaseEntitlementControl.ValidationGroup = ValidationGroup;
		}

		protected override void OnSubmit(object sender, WebFormSubmitEventArgs e)
		{
			e = CaseEntitlementControl.Submit(this, e);
		}
	}
}