using System;
using System.Linq;
using Adxstudio.Xrm.Web.UI.WebForms;
using Microsoft.Xrm.Portal.Configuration;

namespace Site.Areas.Permits.Controls
{
	public partial class WebFormPermitSuccess : WebFormUserControl
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

			var type = context.CreateQuery("adx_permittype").FirstOrDefault(s => s.GetAttributeValue<string>("adx_entityname") == PreviousStepEntityLogicalName);

			if (type == null)
			{
				throw new NullReferenceException(string.Format("The {0} record could not be found with Entity Name equal to '{1}'.", "adx_permittype", PreviousStepEntityLogicalName));
			}

			var field = type.GetAttributeValue<string>("adx_permitnumberfieldname");

			if (!string.IsNullOrWhiteSpace(field))
			{
				try
				{
					var permitNumber = entity.GetAttributeValue<string>(field);

					if (!string.IsNullOrWhiteSpace(permitNumber))
					{
						PermitNumber.Text = permitNumber;

						PermitNumberPanel.Visible = true;
					}
				}
				catch (Exception)
				{
					PermitNumberPanel.Visible = false;
				}
			}

			PanelSuccess.Visible = true;
		}
	}
}