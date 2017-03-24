<%@ Page Language="C#" MasterPageFile="~/MasterPages/WebForms.master" AutoEventWireup="true" CodeBehind="ReadOnlyAccountView.aspx.cs" Inherits="Site.Areas.CustomerManagement.Pages.ReadOnlyAccountView" %>

<asp:Content  ContentPlaceHolderID="ContentBottom" runat="server">
	<adx:Snippet ID="RecordNotFoundError" SnippetName="customermanagement/account/read/RecordNotFoundError" DefaultText="Customer Account could not be found. Ensure a valid customer account ID has been specified." Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-danger" runat="server"/>
	<adx:Snippet ID="NoChannelPermissionsRecordError" SnippetName="customermanagement/account/read/NoChannelPermissionsRecordError" DefaultText="You do not have channel permissions. Permission to read customer accounts is denied." Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-danger" runat="server"/>
	<adx:Snippet ID="ChannelPermissionsError" SnippetName="customermanagement/account/read/ChannelPermissionsError" DefaultText="Your channel permissions deny read rights. Permission to view customer accounts is denied." Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-danger" runat="server"/>
	<adx:Snippet ID="NoParentAccountError" SnippetName="customermanagement/account/read/NoParentAccountError" DefaultText="A parent customer account has not been assigned to you. Permission to read customer accounts is denied." Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-danger" runat="server"/>
	<adx:Snippet ID="NoChannelPermissionsForParentAccountError" SnippetName="customermanagement/account/read/NoChannelPermissionsForParentAccountError" DefaultText="You do not have channel permissions for your parent customer account. Permission to read customer accounts is denied." Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-danger" runat="server"/>
	<adx:Snippet ID="ParentAccountClassificationCodeError" SnippetName="customermanagement/account/read/ParentAccountClassificationCodeError" DefaultText="<p>The parent customer account assigned to you has an invalid Classification Code. Permission to read customer accounts is denied.</p><p>Account Classification Code must be set to 'Partner'.</p>" Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-danger" runat="server"/>
	<asp:Panel ID="AccountForm" CssClass="crmEntityFormView" runat="server">
		<adx:CrmEntityFormView runat="server" 
			ID="FormView" EntityName="account" 
			FormName="Account Read Only Web Form" 
			RecommendedFieldsRequired="True" 
			ShowUnsupportedFields="False" 
			ToolTipEnabled="False" 
			Mode="ReadOnly"
			LanguageCode="<%$ SiteSetting: Language Code, 0 %>"
			ContextName="<%$ SiteSetting: Language Code %>">
		</adx:CrmEntityFormView>
	</asp:Panel>
</asp:Content>