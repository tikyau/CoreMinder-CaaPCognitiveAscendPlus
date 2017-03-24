<%@ Page Language="C#" MasterPageFile="~/MasterPages/WebForms.master" AutoEventWireup="true" CodeBehind="CreateCustomerAccount.aspx.cs" Inherits="Site.Areas.CustomerManagement.Pages.CreateCustomerAccount" %>

<asp:Content ContentPlaceHolderID="ContentBottom" ViewStateMode="Enabled" runat="server">
	<adx:Snippet ID="NoChannelPermissionsRecordError" SnippetName="customermanagement/account/create/NoChannelPermissionsRecordError" DefaultText="You do not have channel permissions. Permission to create customer accounts is denied." Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-danger" runat="server"/>
	<adx:Snippet ID="ChannelPermissionsError" SnippetName="customermanagement/account/create/ChannelPermissionsError" DefaultText="Your channel permissions deny create rights. Permission to manage customer accounts is denied." Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-danger" runat="server"/>
	<adx:Snippet ID="NoParentAccountError" SnippetName="customermanagement/account/create/NoParentAccountError" DefaultText="A parent customer account has not been assigned to you. Permission to create customer accounts is denied." Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-danger" runat="server"/>
	<adx:Snippet ID="NoChannelPermissionsForParentAccountError" SnippetName="customermanagement/account/create/NoChannelPermissionsForParentAccountError" DefaultText="You do not have channel permissions for your parent customer account. Permission to create customer accounts is denied." Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-danger" runat="server"/>
	<adx:Snippet ID="ParentAccountClassificationCodeError" SnippetName="customermanagement/account/create/ParentAccountClassificationCodeError" DefaultText="<p>The parent customer account assigned to you has an invalid Classification Code. Permission to create customer accounts is denied.</p><p>Account Classification Code must be set to 'Partner'.</p>" Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-danger" runat="server"/>
	<asp:Panel ID="CreateAccountForm" CssClass="crmEntityFormView" runat="server">
		<adx:CrmDataSource ID="WebFormDataSource" runat="server" CrmDataContextName="<%$ SiteSetting: Language Code %>" />
		<adx:CrmEntityFormView runat="server" ID="AccountFormView" EntityName="account" FormName="Account Web Form" ValidationGroup="CreateAccount" OnItemInserting="OnItemInserting" OnItemInserted="OnItemInserted" RecommendedFieldsRequired="True" ShowUnsupportedFields="False" DataSourceID="WebFormDataSource" ToolTipEnabled="False" Mode="Insert" LanguageCode="<%$ SiteSetting: Language Code, 0 %>" ContextName="<%$ SiteSetting: Language Code %>">
			<InsertItemTemplate>
			</InsertItemTemplate>
		</adx:CrmEntityFormView>
		<div class="actions">
			<asp:Button ID="CreateAccountButton" 
				Text='<%$ Snippet: customermanagement/account/create/CreateAccountButtonLabel, Create %>' 
				CssClass="btn btn-primary"
				CausesValidation="true"
				ValidationGroup="CreateAccount" 
				OnClick="CreateAccountButton_Click" 
				runat="server" />
		</div>
	</asp:Panel>
</asp:Content>
