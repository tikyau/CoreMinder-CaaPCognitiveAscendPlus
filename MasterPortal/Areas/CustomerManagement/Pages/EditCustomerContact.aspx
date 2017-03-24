<%@ Page Language="C#" MasterPageFile="~/MasterPages/WebForms.master" AutoEventWireup="true" CodeBehind="EditCustomerContact.aspx.cs" Inherits="Site.Areas.CustomerManagement.Pages.EditCustomerContact" %>

<asp:Content  ContentPlaceHolderID="ContentBottom" ViewStateMode="Enabled" runat="server">
	<adx:Snippet ID="RecordNotFoundError" SnippetName="customermanagement/contact/edit/RecordNotFoundError" DefaultText="Customer Contact could not be found. Ensure a valid customer ID has been specified." Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-danger" runat="server"/>
	<adx:Snippet ID="NoChannelPermissionsRecordError" SnippetName="customermanagement/contact/edit/NoChannelPermissionsRecordError" DefaultText="You do not have channel permissions. Permission to edit customer contacts is denied." Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-danger" runat="server"/>
	<adx:Snippet ID="ChannelPermissionsError" SnippetName="customermanagement/contact/edit/ChannelPermissionsError" DefaultText="Your channel permissions deny write rights. Permission to edit customer contacts is denied." Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-danger" runat="server"/>
	<adx:Snippet ID="NoParentAccountError" SnippetName="customermanagement/contact/edit/NoParentAccountError" DefaultText="A parent customer account has not been assigned to you. Permission to edit customer contacts is denied." Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-danger" runat="server"/>
	<adx:Snippet ID="NoChannelPermissionsForParentAccountError" SnippetName="customermanagement/contact/edit/NoChannelPermissionsForParentAccountError" DefaultText="You do not have channel permissions for your parent customer account. Permission to edit customer contacts is denied." Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-danger" runat="server"/>
	<adx:Snippet ID="ParentAccountClassificationCodeError" SnippetName="customermanagement/contact/edit/ParentAccountClassificationCodeError" DefaultText="<p>The parent customer account assigned to you has an invalid Classification Code. Permission to edit customer contacts is denied.</p><p>Account Classification Code must be set to 'Partner'.</p>" Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-danger" runat="server"/>
	<adx:Snippet ID="SuccessMessage" SnippetName="customermanagement/contact/edit/SuccessMessage" DefaultText="Contact information has been updated successfully." Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-success" runat="server"/>
	<asp:Panel ID="EditContactForm" CssClass="crmEntityFormView" runat="server">
		<adx:CrmEntityFormView runat="server" 
			ID="ContactFormView" 
			EntityName="contact" 
			FormName="Contact Web Form" 
			OnItemUpdating="OnItemUpdating" 
			OnItemUpdated="OnItemUpdated" 
			ValidationGroup="UpdateContact" 
			RecommendedFieldsRequired="True" 
			ShowUnsupportedFields="False" 
			ToolTipEnabled="False"
			Mode="Edit"
			LanguageCode="<%$ SiteSetting: Language Code, 0 %>"
			ContextName="<%$ SiteSetting: Language Code %>">
			<UpdateItemTemplate></UpdateItemTemplate>
		</adx:CrmEntityFormView>
		<div class="actions">
			<asp:Button ID="UpdateContactButton" Text='<%$ Snippet: customermanagement/contact/edit/UpdateContactButtonLabel, Update %>' CssClass="btn btn-primary" OnClick="UpdateContactButton_Click" CausesValidation="true" ValidationGroup="UpdateContact" runat="server" />
		</div>
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
