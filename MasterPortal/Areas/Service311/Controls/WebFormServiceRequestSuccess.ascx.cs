using System;
using System.Linq;
using Adxstudio.Xrm.Cms;
using Adxstudio.Xrm.Web.UI.WebForms;
using Microsoft.Xrm.Portal.Configuration;
using Microsoft.Xrm.Portal.Web;
using Site.Controls;

namespace Site.Areas.Service311.Controls
{
	public partial class WebFormServiceRequestSuccess : WebFormPortalUserControl
	{
		protected void Page_PreRender(object sender, EventArgs e)
		{
			WebForm.ShowHideNextButton(false);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			if (PreviousStepEntityID == Guid.Empty)
			{
				throw new NullReferenceException("The ID of the previous web form step's created entity is null.");
			}

			CustomSuccessMessage.Text = WebForm.SuccessMessage ?? string.Empty;

			DefaultSuccessMessageSnippet.Visible = string.IsNullOrWhiteSpace(WebForm.SuccessMessage);

			var context = PortalCrmConfigurationManager.CreateServiceContext();

			var entity = context.CreateQuery(CurrentStepEntityLogicalName).FirstOrDefault(o => o.GetAttributeValue<Guid>(PreviousStepEntityPrimaryKeyLogicalName) == PreviousStepEntityID);

			if (entity == null)
			{
				throw new NullReferenceException(string.Format("The {0} record with primary key '{1}' equal to '{2}' could not be found.", PreviousStepEntityLogicalName, PreviousStepEntityPrimaryKeyLogicalName, PreviousStepEntityID));
			}

			var type = context.CreateQuery("adx_servicerequesttype").FirstOrDefault(s => s.GetAttributeValue<string>("adx_entityname") == PreviousStepEntityLogicalName);

			if (type == null)
			{
				throw new NullReferenceException(string.Format("The {0} record could not be found with Entity Name equal to '{1}'.", "adx_servicerequesttype", PreviousStepEntityLogicalName));
			}

			var field = type.GetAttributeValue<string>("adx_servicerequestnumberfieldname");

			if (!string.IsNullOrWhiteSpace(field))
			{
				try
				{
					var serviceRequestNumber = entity.GetAttributeValue<string>(field);

					if (!string.IsNullOrWhiteSpace(serviceRequestNumber))
					{
						LinkToServiceRequest.Text = serviceRequestNumber;

						if (ServiceContext.GetPageBySiteMarkerName(Website, "check-status") != null)
						{
							ServiceRequestNumberPanel.Visible = true;
						}
					}
				}
				catch (Exception)
				{
					ServiceRequestNumberPanel.Visible = false;
				}
				try
				{
					var slaResponseTime = type.GetAttributeValue<int?>("adx_responsesla");
					if (slaResponseTime != null)
					{
						var slaResponseTimeInHours = slaResponseTime / 60;

						SlaResponseTime.Text = "SLA Response Time: " + slaResponseTimeInHours.ToString() + " hours";

					}
					

					var slaResolutionTime = type.GetAttributeValue<int?>("adx_resolutionsla");
					if (slaResolutionTime != null)
					{
						var slaResolutionTimeInHours = slaResolutionTime / 60;

						SlaResolutionTime.Text = "SLA Resolution Time: " + slaResolutionTimeInHours.ToString() + " hours";
					}

					if (slaResolutionTime == null && slaResponseTime == null)
					{
						SLAPanel.Visible = false;
					}

				}
				catch (Exception)
				{
					ServiceRequestNumberPanel.Visible = false;
				}

			}

			PanelSuccess.Visible = true;
		}

		protected void LinkToServiceRequest_Click(object sender, EventArgs e)
		{
			
			var page = ServiceContext.GetPageBySiteMarkerName(Website, "check-status");

			var url = new UrlBuilder(ServiceContext.GetUrl(page));

			url.QueryString.Set("refnum", LinkToServiceRequest.Text);

			Response.Redirect(url.PathWithQueryString);
		}
	}
}