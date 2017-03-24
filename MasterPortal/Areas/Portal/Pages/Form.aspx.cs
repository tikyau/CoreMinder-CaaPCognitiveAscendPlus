using System;
using System.Globalization;
using System.Web;
using Adxstudio.Xrm.Web.UI.EntityForm;
using Adxstudio.Xrm.Web.UI.WebControls;
using Microsoft.Xrm.Portal.Configuration;
using Microsoft.Xrm.Sdk;
using Site.Pages;

namespace Site.Areas.Portal.Pages
{
	public partial class Form : PortalPage
	{
		protected void Page_Init(object sender, EventArgs e)
		{
			var languageCodeSetting = HttpContext.Current.Request["languagecode"];
			
			if (string.IsNullOrWhiteSpace(languageCodeSetting))
			{
				return;
			}

			int languageCode;
			
			if (!int.TryParse(languageCodeSetting, out languageCode))
			{
				return;
			}

			EntityFormControl.LanguageCode = languageCode;

			var portalName = languageCode.ToString(CultureInfo.InvariantCulture);

			var portals = Microsoft.Xrm.Portal.Configuration.PortalCrmConfigurationManager.GetPortalCrmSection().Portals;
			
			if (portals.Count <= 0) return;

			var found = false;
			
			foreach (var portal in portals)
			{
				var portalContext = portal as PortalContextElement;
				if (portalContext != null && portalContext.Name == portalName)
				{
					found = true;
				}
			}

			if (found)
			{
				EntityFormControl.PortalName = portalName;
			}
		}

		protected void Page_PreRender(object sender, EventArgs e)
		{
			Guid entityFormId;
			var entityFormIdValue = HttpContext.Current.Request["entityformid"];

			if (!Guid.TryParse(entityFormIdValue, out entityFormId))
			{
				return;
			}

			EntityFormControl.EntityFormReference = new EntityReference("adx_entityform", entityFormId);

			if (string.IsNullOrEmpty(Page.Form.Action))
			{
				Page.Form.Action = Request.Url.PathAndQuery;
			}
		}

		protected void OnItemSaved(object sender, EntityFormSavedEventArgs e)
		{
			if (e == null)
			{
				return;
			}
			
			if (e.Exception == null)
			{
				var cs = Page.ClientScript;
				
				if (!cs.IsClientScriptBlockRegistered(GetType(), "EntityFormOnSuccessScript"))
				{
					cs.RegisterClientScriptBlock(GetType(), "EntityFormOnSuccessScript", @"window.parent.postMessage(""Success"", ""*"");", true);
				}
			}
			else
			{
				Response.Write("<div class='container' role='alert'><div class='alert alert-block alert-danger'><p class='text-danger'><span class='fa fa-exclamation-triangle' aria-hidden='true'></span> " + Server.HtmlEncode(e.Exception.InnerException == null ? e.Exception.Message : e.Exception.InnerException.Message) + "</p></div></div>");

				e.ExceptionHandled = true;
			}
		}
	}
}