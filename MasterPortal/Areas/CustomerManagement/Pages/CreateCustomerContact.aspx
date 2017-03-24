<%@ Page Language="C#" MasterPageFile="~/MasterPages/WebForms.master" AutoEventWireup="true" CodeBehind="CreateCustomerContact.aspx.cs" Inherits="Site.Areas.CustomerManagement.Pages.CreateCustomerContact" %>

<asp:Content ContentPlaceHolderID="ContentBottom" ViewStateMode="Enabled" runat="server">
	<adx:Snippet ID="NoChannelPermissionsRecordError" SnippetName="customermanagement/contact/create/NoChannelPermissionsRecordError" DefaultText="You do not have channel permissions. Permission to create customer contacts is denied." Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-danger" runat="server"/>
	<adx:Snippet ID="ChannelPermissionsError" SnippetName="customermanagement/contact/create/ChannelPermissionsError" DefaultText="Your channel permissions deny create rights. Permission to manage customer contacts is denied." Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-danger" runat="server"/>
	<adx:Snippet ID="NoParentAccountError" SnippetName="customermanagement/contact/create/NoParentAccountError" DefaultText="A parent customer account has not been assigned to you. Permission to create customer contacts is denied." Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-danger" runat="server"/>
	<adx:Snippet ID="NoChannelPermissionsForParentAccountError" SnippetName="customermanagement/contact/create/NoChannelPermissionsForParentAccountError" DefaultText="You do not have channel permissions for your parent customer account. Permission to create customer contacts is denied." Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-danger" runat="server"/>
	<adx:Snippet ID="ParentAccountClassificationCodeError" SnippetName="customermanagement/contact/create/ParentAccountClassificationCodeError" DefaultText="<p>The parent customer account assigned to you has an invalid Classification Code. Permission to create customer contacts is denied.</p><p>Account Classification Code must be set to 'Partner'.</p>" Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-danger" runat="server"/>
	<asp:Panel ID="CreateContactForm" CssClass="crmEntityFormView" runat="server" Visible="true">
		<adx:CrmDataSource ID="WebFormDataSource" runat="server" CrmDataContextName="<%$ SiteSetting: Language Code %>" />
		<div class="form-group">
			<label>
				<adx:Snippet runat="server" SnippetName="customermanagement/contact/create/ParentCustomerFieldLabel" DefaultText="Parent Customer" Editable="true" EditType="text" />
			</label>
			<asp:DropDownList ID="CompanyNameList" runat="server" CssClass="form-control" Enabled="False"/>
		</div>
		<adx:CrmEntityFormView runat="server" ID="ContactFormView" EntityName="contact" FormName="Contact Web Form" OnItemInserting="OnItemInserting" OnItemInserted="OnItemInserted" RecommendedFieldsRequired="True" ShowUnsupportedFields="False" DataSourceID="WebFormDataSource" ToolTipEnabled="False" Mode="Insert" ValidationGroup="CreateContact" LanguageCode="<%$ SiteSetting: Language Code, 0 %>" ContextName="<%$ SiteSetting: Language Code %>">
			<InsertItemTemplate></InsertItemTemplate>
		</adx:CrmEntityFormView>
		<div class="actions">
			<asp:Button ID="CreateContactButton" Text='<%$ Snippet: customermanagement/contact/create/CreateContactButtonLabel, Update %>' CssClass="btn btn-primary" OnClick="CreateContactButton_Click" ValidationGroup="CreateContact" runat="server" />
		</div>
	</asp:Panel>
</asp:Content>

