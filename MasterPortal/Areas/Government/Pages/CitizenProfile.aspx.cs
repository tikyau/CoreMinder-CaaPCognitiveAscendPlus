﻿using System;
using System.Linq;
using System.Web.UI.WebControls;
using Adxstudio.Xrm.Account;
using Adxstudio.Xrm.Cms;
using Adxstudio.Xrm.IdentityModel;
using Adxstudio.Xrm.Web;
using Adxstudio.Xrm.Web.UI.WebControls;
using Microsoft.IdentityModel.Claims;
using Microsoft.Xrm.Client;
using Microsoft.Xrm.Client.Messages;
using Microsoft.Xrm.Portal.IdentityModel.Configuration;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk;
using Site.Pages;

namespace Site.Areas.Government.Pages
{
	public partial class CitizenProfile : PortalPage
	{
		private const string _userFetchXmlFormat = @"
			<fetch mapping=""logical"">
				<entity name=""contact"">
					<all-attributes />
					<filter type=""and"">
						<condition attribute=""contactid"" operator=""eq"" value=""{0}""/>
					</filter>
				</entity>
			</fetch>";

		public bool ShowMarketingOptionsPanel
		{
			get
			{
				var showMarketingSetting = ServiceContext.GetSiteSettingValueByName(Website, "Profile/ShowMarketingOptionsPanel") ?? "false";

				return showMarketingSetting.ToLower() == "true";
			}
		}

		public bool ForceRegistration
		{
			get
			{
				var siteSetting = ServiceContext.GetSiteSettingValueByName(Website, "Profile/ForceSignUp") ?? "false";

				return siteSetting.ToLower() == "true";
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			RedirectToLoginIfAnonymous();

			if (ForceRegistration && !ServiceContext.ValidateProfileSuccessfullySaved(Contact))
			{
				MissingFieldsMessage.Visible = true;
			}

			if (ShowMarketingOptionsPanel)
			{
				MarketingOptionsPanel.Visible = true;
			}

			ProfileDataSource.FetchXml = _userFetchXmlFormat.FormatWith(Contact.Id);

			ProfileAlertInstructions.Visible = Contact.GetAttributeValue<Boolean?>("adx_profilealert") ?? false;

			if (IsPostBack)
			{
				return;
			}

			var contact = XrmContext.CreateQuery("contact").FirstOrDefault(c => c.GetAttributeValue<Guid>("contactid") == Contact.Id);

			if (contact == null)
			{
				throw new ApplicationException(string.Format("Could not retrieve contact record with contactid equal to '{0}'", Contact.Id));
			}

			if (ShowMarketingOptionsPanel)
			{
				marketEmail.Checked = !contact.GetAttributeValue<bool>("donotemail");
				marketFax.Checked = !contact.GetAttributeValue<bool>("donotfax");
				marketPhone.Checked = !contact.GetAttributeValue<bool>("donotphone");
				marketMail.Checked = !contact.GetAttributeValue<bool>("donotpostalmail");
			}

			PopulateMarketingLists();
		}

		protected void SubmitButton_Click(object sender, EventArgs e)
		{
			if (!Page.IsValid)
			{
				return;
			}

			MissingFieldsMessage.Visible = false;

			var contact = XrmContext.MergeClone(Contact);

			ManageLists(XrmContext, contact);

			ProfileFormView.UpdateItem();

			var returnUrlKey = FederationCrmConfigurationManager.GetUserRegistrationSettings().ReturnUrlKey ?? "returnurl";
			var returnUrl = Request[returnUrlKey];

			if (!string.IsNullOrWhiteSpace(returnUrl))
			{
				Context.RedirectAndEndResponse(returnUrl);
			}
		}

		protected void OnItemUpdating(object sender, CrmEntityFormViewUpdatingEventArgs e)
		{
			e.Values["adx_profilemodifiedon"] = DateTime.UtcNow;
			e.Values["adx_profilealert"] = ProfileAlertInstructions.Visible = false;

			if (ShowMarketingOptionsPanel)
			{
				e.Values["donotemail"] = !marketEmail.Checked;
				e.Values["donotbulkemail"] = !marketEmail.Checked;
				e.Values["donotfax"] = !marketFax.Checked;
				e.Values["donotphone"] = !marketPhone.Checked;
				e.Values["donotpostalmail"] = !marketMail.Checked;
			}
		}

		protected void OnItemUpdated(object sender, EventArgs args)
		{
			ConfirmationMessage.Visible = true;
		}

		public bool IsListChecked(object listoption)
		{
			var list = (Entity)listoption;

			if (Request.IsAuthenticated)
			{
				var contact = XrmContext.CreateQuery("contact").FirstOrDefault(c => c.GetAttributeValue<Guid>("contactid") == Contact.Id);

				return contact != null
					&& contact.GetRelatedEntities(XrmContext, new Relationship("listcontact_association"))
					.Any(l => l.GetAttributeValue<Guid>("listid") == list.Id);
			}

			return false;
		}

		public void ManageLists(OrganizationServiceContext context, Entity contact)
		{
			foreach (var item in MarketingListsListView.Items)
			{
				if (item == null)
				{
					continue;
				}

				var listViewItem = item;

				var hiddenListId = (HiddenField)listViewItem.FindControl("ListID");

				if (hiddenListId == null)
				{
					continue;
				}

				var listId = new Guid(hiddenListId.Value);

				var ml = context.CreateQuery("list").First(m => m.GetAttributeValue<Guid>("listid") == listId);

				var listCheckBox = (CheckBox)item.FindControl("ListCheckbox");

				if (listCheckBox == null)
				{
					continue;
				}

				var contactLists = contact.GetRelatedEntities(XrmContext, new Relationship("listcontact_association")).ToList();

				var inList = contactLists.Any(list => list.GetAttributeValue<Guid>("listid") == ml.Id);

				if (listCheckBox.Checked && !inList)
				{
					context.AddMemberList(ml.GetAttributeValue<Guid>("listid"), contact.GetAttributeValue<Guid>("contactid"));
				}
				else if (!listCheckBox.Checked && inList)
				{
					context.RemoveMemberList(ml.GetAttributeValue<Guid>("listid"), contact.GetAttributeValue<Guid>("contactid"));
				}
			}
		}

		protected void PopulateMarketingLists()
		{
			if (Website == null)
			{
				MarketingLists.Visible = false;
				return;
			}

			var website = XrmContext.CreateQuery("adx_website").FirstOrDefault(w => w.GetAttributeValue<Guid>("adx_websiteid") == Website.Id);

			if (website == null)
			{
				MarketingLists.Visible = false;
				return;
			}

			// Note: Marketing Lists with 'Dynamic' Type (i.e. value of 1 or true) do not support manually adding members

			if (website.GetRelatedEntities(XrmContext, new Relationship("adx_website_list")).All(l => l.GetAttributeValue<bool>("type")))
			{
				MarketingLists.Visible = false;
				return;
			}

			MarketingListsListView.DataSource = website.GetRelatedEntities(XrmContext, new Relationship("adx_website_list")).Where(l => l.GetAttributeValue<bool>("type") == false);

			MarketingListsListView.DataBind();
		}
	}
}