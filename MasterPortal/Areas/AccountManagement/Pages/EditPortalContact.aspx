<%@ Page Language="C#" MasterPageFile="~/MasterPages/WebForms.master" AutoEventWireup="true" CodeBehind="EditPortalContact.aspx.cs" Inherits="Site.Areas.AccountManagement.Pages.EditPortalContact" %>
<%@ OutputCache CacheProfile="User" %>

<asp:Content  ContentPlaceHolderID="ContentBottom" ViewStateMode="Enabled" runat="server">
	<adx:Snippet ID="RecordNotFoundError" SnippetName="customermanagement/contact/edit/RecordNotFoundError" DefaultText="Contact could not be found. Ensure a valid contact ID has been specified." Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-danger" runat="server"/>
	<adx:Snippet ID="NoParentAccountError" SnippetName="accountmanagement/contact/edit/NoParentAccountError" DefaultText="A parent account has not been assigned to you. Permission to edit contact is denied." Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-danger" runat="server"/>
	<adx:Snippet ID="NoContactAccessPermissionsRecordError" SnippetName="accountmanagement/contact/edit/NoContactAccessPermissionsRecordError" DefaultText="You do not have contact access permissions. Permission to edit contact is denied." Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-warning" runat="server"/>
	<adx:Snippet ID="ContactAccessPermissionsError" SnippetName="accountmanagement/contact/edit/ContactAccessPermissionsError" DefaultText="Your contact access permissions deny write rights. Permission to edit contact is denied." Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-warning" runat="server"/>
	<adx:Snippet ID="NoContactAccessPermissionsForParentAccountError" SnippetName="accountmanagement/contact/edit/NoContactAccessPermissionsForParentAccountError" DefaultText="You do not have contact access permissions for your parent account with scope set to account. Permission to edit contact is denied." Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-warning" runat="server"/>
	<adx:Snippet ID="ContactAccessWritePermissionDeniedMessage" SnippetName="accountmanagement/edit/ContactAccessWritePermissionDeniedMessage" DefaultText="Your contact access permissions grant read rights but deny write rights. Permission to edit contact is denied." Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-warning" runat="server"/>
	<adx:Snippet ID="UpdateSuccessMessage" SnippetName="accountmanagement/contact/edit/UpdateSuccessMessage" DefaultText="Account information has been updated successfully." Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-success" runat="server" />
	<adx:Snippet ID="InvitationConfirmationMessage" SnippetName="accountmanagement/contact/edit/InvitationConfirmationMessage" DefaultText="Contact has been invited successfully." Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-success" runat="server" />		
	<asp:Panel ID="ContactInformation" runat="server">
		<asp:Panel ID="ContactEditForm" CssClass="crmEntityFormView" runat="server">
			<adx:CrmEntityFormView runat="server" 
				ID="ContactEditFormView" 
				EntityName="contact" 
				FormName="Contact Web Form" 
				OnItemUpdating="OnItemUpdating" 
				OnItemUpdated="OnItemUpdated" 
				ValidationGroup="UpdateContact" 
				RecommendedFieldsRequired="True" 
				ShowUnsupportedFields="False" 
				ToolTipEnabled="False" Mode="Edit"
				LanguageCode="<%$ SiteSetting: Language Code, 0 %>"
				ContextName="<%$ SiteSetting: Language Code %>">
				<UpdateItemTemplate>
				</UpdateItemTemplate>
			</adx:CrmEntityFormView>
			<div class="actions">
				<asp:Button ID="UpdateButton" Text='<%$ Snippet: accountmanagement/contact/edit/UpdateButtonLabel, Update %>' CssClass="btn btn-primary" OnClick="UpdateButton_Click" CausesValidation="true" ValidationGroup="UpdateContact" runat="server" />
				<asp:Button ID="InviteButton" Text='<%$ Snippet: accountmanagement/contact/edit/InviteButtonLabel, Send Portal Invitation Email %>' CssClass="btn btn-default" OnClick="InviteButton_Click" CausesValidation="true" ValidationGroup="UpdateContact" runat="server" />
				<asp:Button ID="ManagePermissionsButton" Text='<%$ Snippet: accountmanagement/contact/edit/ManagePermissionsButtonLabel, Manage Permissions %>' CssClass="btn btn-default" OnClick="ManagePermissionsButton_Click" CausesValidation="false" runat="server" />
			</div>
		</asp:Panel>
		<asp:Panel ID="ContactReadOnlyForm" CssClass="crmEntityFormView" Visible="false" runat="server">
			<adx:CrmEntityFormView runat="server" 
				ID="ContactReadOnlyFormView" 
				EntityName="contact" 
				FormName="Contact Web Form" 
				Mode="ReadOnly"
				LanguageCode="<%$ SiteSetting: Language Code, 0 %>"
				ContextName="<%$ SiteSetting: Language Code %>">
			</adx:CrmEntityFormView>
		</asp:Panel>
	</asp:Panel>
</asp:Content>

<asp:Content ContentPlaceHolderID="Scripts" runat="server">
	<script type="text/javascript">
		$(function () {
			$("form").submit(function () {
				if (Page_IsValid) {
					$.blockUI({ message: null, overlayCSS: { opacity: .3 } });
				}
			});
		});
	</script>
</asp:Content>
