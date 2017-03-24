using System;
using System.Linq;
using Adxstudio.Xrm.Web.Mvc.Html;
using Microsoft.Xrm.Portal.Configuration;

namespace Site.Pages
{
	public partial class WebTemplateNoMaster : PortalPage
	{
		protected void Page_Init(object sender, EventArgs args)
		{
			if (!System.Web.SiteMap.Enabled)
			{
				return;
			}

			var currentNode = System.Web.SiteMap.CurrentNode;

			if (currentNode == null)
			{
				return;
			}

			var templateIdString = currentNode["adx_webtemplateid"];

			if (string.IsNullOrEmpty(templateIdString))
			{
				return;
			}

			Guid templateId;

			if (!Guid.TryParse(templateIdString, out templateId))
			{
				return;
			}

			using (var serviceContext = PortalCrmConfigurationManager.CreateServiceContext(PortalName))
			{
				var webTemplate = serviceContext.CreateQuery("adx_webtemplate")
					.FirstOrDefault(e => e.GetAttributeValue<Guid>("adx_webtemplateid") == templateId
						&& e.GetAttributeValue<int?>("statecode") == 0);

				if (webTemplate == null)
				{
					return;
				}

				var source = webTemplate.GetAttributeValue<string>("adx_source");
				Liquid.Html = Html.Liquid(source);

				var mimetype = webTemplate.GetAttributeValue<string>("adx_mimetype");

				if (!string.IsNullOrWhiteSpace(mimetype))
				{
					ContentType = mimetype;
				}
			}
		}
	}
}
