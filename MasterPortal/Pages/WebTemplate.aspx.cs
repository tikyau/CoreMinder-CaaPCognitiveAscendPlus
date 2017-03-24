using System;
using System.Linq;
using System.Web;
using Adxstudio.Xrm.Web.Mvc.Html;
using Microsoft.Xrm.Portal.Configuration;

namespace Site.Pages
{
	public partial class WebTemplate : PortalPage
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
				var source = serviceContext.CreateQuery("adx_webtemplate")
					.Where(e => e.GetAttributeValue<Guid>("adx_webtemplateid") == templateId)
					.Where(e => e.GetAttributeValue<int?>("statecode") == 0)
					.Select(e => e.GetAttributeValue<string>("adx_source"))
					.FirstOrDefault();
				Liquid.Html = Html.Liquid(source);
			}
		}
	}
}
