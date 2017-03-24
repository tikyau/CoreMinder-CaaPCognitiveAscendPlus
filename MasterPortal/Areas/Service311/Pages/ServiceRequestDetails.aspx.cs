using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Adxstudio.Xrm.Cms;
using Adxstudio.Xrm.Mapping;
using Adxstudio.Xrm.Notes;
using Adxstudio.Xrm.Services.Query;
using Adxstudio.Xrm.Text;
using Adxstudio.Xrm.Web.UI.WebControls;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Site.Helpers;
using Site.Pages;
using Adxstudio.Xrm.Core;


namespace Site.Areas.Service311.Pages
{
	public partial class ServiceRequestDetails : PortalPage
	{

		public Entity ServiceRequest { get; set; }

		private Entity _serviceRequestRollupRecord;

		private Entity ServiceRequestRollupRecord
		{
			get
			{
				if (_serviceRequestRollupRecord == null)
				{
					var reference = Entity.GetAttributeValue<EntityReference>("adx_entityform");

					var entityFormRecord =
						XrmContext.CreateQuery("adx_entityform").FirstOrDefault(
							ef => ef.GetAttributeValue<Guid>("adx_entityformid") == reference.Id);


					if (entityFormRecord == null) return null;

					var recordEntityLogicalName = entityFormRecord.GetAttributeValue<string>("adx_entityname");

					Guid recordId;

					if (!Guid.TryParse(Request["id"], out recordId))
					{
						return null;
					}

					var metadataRequest = new RetrieveEntityRequest
											  {
												  LogicalName = recordEntityLogicalName,
												  EntityFilters = EntityFilters.Attributes
											  };

					var metadataResponse = (RetrieveEntityResponse) XrmContext.Execute(metadataRequest);

					var primaryFieldLogicalName = metadataResponse.EntityMetadata.PrimaryIdAttribute;

					_serviceRequestRollupRecord =
						XrmContext.CreateQuery(recordEntityLogicalName).FirstOrDefault(
							r => r.GetAttributeValue<Guid>(primaryFieldLogicalName) == recordId);
				}

				return _serviceRequestRollupRecord;
			}
		}

		public string ServiceRequestNumber
		{
			get
			{
				var number = ServiceRequestRollupRecord.GetAttributeValue<string>("adx_servicerequestnumber");

				return number;
			}
		}

		private string RegardingContactFieldName { get; set; }

		protected void Page_Load(object sender, EventArgs e)
		{
			if (ServiceRequestRollupRecord != null)
			{

				var serviceRequestTypeReference =
					ServiceRequestRollupRecord.GetAttributeValue<EntityReference>("adx_servicerequesttype");

				var serviceRequestType =
					XrmContext.CreateQuery("adx_servicerequesttype").FirstOrDefault(
						srt => srt.GetAttributeValue<Guid>("adx_servicerequesttypeid") == serviceRequestTypeReference.Id);

				var entityName = serviceRequestType.GetAttributeValue<string>("adx_entityname");

				RegardingContactFieldName = serviceRequestType.GetAttributeValue<string>("adx_regardingcontactfieldname");

				var trueMetadataRequest = new RetrieveEntityRequest
											  {
												  LogicalName = entityName,
												  EntityFilters = EntityFilters.Attributes
											  };

				var trueMetadataResponse = (RetrieveEntityResponse) XrmContext.Execute(trueMetadataRequest);

				var primaryFieldName = trueMetadataResponse.EntityMetadata.PrimaryIdAttribute;

				var entityId = ServiceRequestRollupRecord.GetAttributeValue<string>("adx_entityid");

				var trueRecordId = Guid.Parse(entityId);

				var trueRecord =
					XrmContext.CreateQuery(entityName).FirstOrDefault(r => r.GetAttributeValue<Guid>(primaryFieldName) == trueRecordId);

				ServiceRequest = trueRecord;

				

				var regardingContact = ServiceRequest.GetAttributeValue<EntityReference>(RegardingContactFieldName);

				if (regardingContact == null || Contact == null || regardingContact.Id != Contact.Id)
				{
					AddANote.Enabled = false;
					AddANote.Visible = false;
					AddNoteInline.Visible = false;
					AddNoteInline.Enabled = false;

					RenderCrmEntityFormView(entityName, primaryFieldName, trueRecordId, FormViewMode.ReadOnly);

					var dataAdapterDependencies =
						new PortalConfigurationDataAdapterDependencies(requestContext: Request.RequestContext, portalName: PortalName);
					var dataAdapter = new AnnotationDataAdapter(dataAdapterDependencies);
					var annotations = dataAdapter.GetAnnotations(ServiceRequest.ToEntityReference(),
						new List<Order> {new Order("createdon")});

					if (!annotations.Any())
					{
						NotesLabel.Visible = false;
						NotesList.Visible = false;
					}

					NotesList.DataSource = annotations;
					NotesList.DataBind();
				}
				else
				{
					RenderCrmEntityFormView(entityName, primaryFieldName, trueRecordId, FormViewMode.Edit);

					var dataAdapterDependencies =
						new PortalConfigurationDataAdapterDependencies(requestContext: Request.RequestContext, portalName: PortalName);
					var dataAdapter = new AnnotationDataAdapter(dataAdapterDependencies);
					var annotations = dataAdapter.GetAnnotations(ServiceRequest.ToEntityReference(),
						new List<Order> {new Order("createdon")},
						privacy: AnnotationPrivacy.Web | AnnotationPrivacy.Private | AnnotationPrivacy.Public);

					NotesList.DataSource = annotations;
					NotesList.DataBind();
				}

				if (Request.IsAuthenticated && Contact != null)
				{
					var dataAdapter = CreateAlertDataAdapter();

					var hasAlert = dataAdapter.HasAlert(Contact.ToEntityReference());

					AddAlert.Visible = !hasAlert;
					RemoveAlert.Visible = hasAlert;
				}
				else
				{
					AddAlertLoginLink.Visible = true;
				}

				DisplaySlaDetails(serviceRequestType);

			}
		}

		private void RenderCrmEntityFormView(string entityName, string primaryFieldName, Guid trueRecordId, FormViewMode formMode)
		{
			var serviceRequestDataSource = CreateDataSource("SeriveRequestDataSource", entityName, primaryFieldName, trueRecordId);

			var serviceRequestFormView = new CrmEntityFormView()
											 {
												 FormName = "Web Details",
												 Mode = formMode,
												 EntityName = entityName,
												 CssClass = "crmEntityFormView",
												 SubmitButtonCssClass = "btn btn-primary",
												 AutoGenerateSteps = false,
												 ClientIDMode = ClientIDMode.Static,
												 MappingFieldCollection = new MappingFieldMetadataCollection(){}
											 };

			var languageCodeSetting = ServiceContext.GetSiteSettingValueByName(Portal.Website, "Language Code");
			if (!string.IsNullOrWhiteSpace(languageCodeSetting))
			{
				int languageCode;
				if (int.TryParse(languageCodeSetting, out languageCode))
				{
					serviceRequestFormView.LanguageCode = languageCode;
					serviceRequestFormView.ContextName = languageCode.ToString(CultureInfo.InvariantCulture);
					serviceRequestDataSource.CrmDataContextName = languageCode.ToString(CultureInfo.InvariantCulture);
				}
			}

			CrmEntityFormViewPanel.Controls.Add(serviceRequestFormView);

			serviceRequestFormView.DataSourceID = serviceRequestDataSource.ID;
		}

		private CrmDataSource CreateDataSource(string dataSourceId, string entityName, string entityIdAttribute, Guid? entityId)
		{
			var formViewDataSource = new CrmDataSource
			{
				ID = dataSourceId,
				FetchXml = string.Format(@"<fetch mapping='logical'> <entity name='{0}'> <all-attributes /> <filter type='and'> <condition attribute = '{1}' operator='eq' value='{{{2}}}'/> </filter> </entity> </fetch>",
					entityName,
					entityIdAttribute,
					entityId)
			};

			CrmEntityFormViewPanel.Controls.Add(formViewDataSource);

			return formViewDataSource;
		}

		protected static IHtmlString FormatNote(object text)
		{
			return text == null ? null : new SimpleHtmlFormatter().Format(text.ToString().Replace("*WEB* ", string.Empty).Replace("*PUBLIC* ", string.Empty));
		}

		protected void AddNote_Click(object sender, EventArgs e)
		{
			var regardingContact = ServiceRequest.GetAttributeValue<EntityReference>(RegardingContactFieldName);

			if (regardingContact == null || Contact == null || regardingContact.Id != Contact.Id)
			{
				throw new InvalidOperationException("Unable to retrieve order.");
			}
			
			var dataAdapterDependencies = new PortalConfigurationDataAdapterDependencies(
				requestContext: Request.RequestContext, portalName: PortalName);
			var serviceContext = dataAdapterDependencies.GetServiceContext();

			var dataAdapter = new AnnotationDataAdapter(dataAdapterDependencies);

			if (NewNotePublic.Checked)
			{
				if (!string.IsNullOrEmpty(NewNoteText.Text) || (NewNoteAttachment.PostedFile != null && NewNoteAttachment.PostedFile.ContentLength > 0))
				{
					var annotation = new Annotation
					{
						NoteText = string.Format("{0}{1}", AnnotationHelper.PublicAnnotationPrefix, NewNoteText.Text),
						Subject = AnnotationHelper.BuildNoteSubject(dataAdapterDependencies),
						Regarding = ServiceRequest.ToEntityReference()
					};
					if (NewNoteAttachment.PostedFile != null && NewNoteAttachment.PostedFile.ContentLength > 0)
					{
						annotation.FileAttachment = AnnotationDataAdapter.CreateFileAttachment(new HttpPostedFileWrapper(NewNoteAttachment.PostedFile));
					}
					dataAdapter.CreateAnnotation(annotation);
				}
			}
			else
			{
				if (!string.IsNullOrEmpty(NewNoteText.Text) ||
					(NewNoteAttachment.PostedFile != null && NewNoteAttachment.PostedFile.ContentLength > 0))
				{
					var annotation = new Annotation
					{
						NoteText = string.Format("{0}{1}", AnnotationHelper.WebAnnotationPrefix, NewNoteText.Text),
						Subject = AnnotationHelper.BuildNoteSubject(dataAdapterDependencies),
						Regarding = ServiceRequest.ToEntityReference()
					};
					if (NewNoteAttachment.PostedFile != null && NewNoteAttachment.PostedFile.ContentLength > 0)
					{
						annotation.FileAttachment = AnnotationDataAdapter.CreateFileAttachment(new HttpPostedFileWrapper(NewNoteAttachment.PostedFile));
					}
					dataAdapter.CreateAnnotation(annotation);
				}
			}

			Response.Redirect(Request.Url.PathAndQuery);
		}

		private IAlertSubscriptionDataAdapter CreateAlertDataAdapter()
		{
			return new ActivityEnabledEntityDataAdapter(ServiceRequest.ToEntityReference(), new Adxstudio.Xrm.Cms.PortalContextDataAdapterDependencies(Portal, requestContext: Request.RequestContext));
		}

		protected void AddAlertLoginLink_Click(object sender, EventArgs e)
		{
			var url = Url.SignInUrl();

			Response.Redirect(url);
		}

		protected void AddAlert_Click(object sender, EventArgs e)
		{
			if (!Request.IsAuthenticated)
			{
				return;
			}

			var user = Portal.User;

			if (user == null)
			{
				return;
			}

			var dataAdapter = CreateAlertDataAdapter();

			var url = XrmContext.GetUrl(Entity);

			var id = ServiceRequest.GetAttributeValue<EntityReference>("adx_servicerequest").Id.ToString();

			dataAdapter.CreateAlert(user.ToEntityReference(), url, id);

			Response.Redirect(Request.Url.PathAndQuery);
		}

		protected void RemoveAlert_Click(object sender, EventArgs e)
		{
			if (!Request.IsAuthenticated)
			{
				return;
			}

			var user = Portal.User;

			if (user == null)
			{
				return;
			}

			var dataAdapter = CreateAlertDataAdapter();

			dataAdapter.DeleteAlert(user.ToEntityReference());

			Response.Redirect(Request.Url.PathAndQuery);
		}

		private void DisplaySlaDetails(Entity serviceRequestType)
		{
			SlaLabel.Text = serviceRequestType.GetAttributeValue<string>("adx_name") + " SLA Details";

			var slaResponseTime = serviceRequestType.GetAttributeValue<int?>("adx_responsesla");
			if (slaResponseTime != null)
			{
				var slaResponseTimeInHours = slaResponseTime / 60;

				SlaResponseTime.Text = "SLA Response Time: " + slaResponseTimeInHours.ToString() + " hours";
			}


			var slaResolutionTime = serviceRequestType.GetAttributeValue<int?>("adx_resolutionsla");
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
	}
}