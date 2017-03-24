<%@ Page Language="C#" MasterPageFile="~/MasterPages/WebForms.master" AutoEventWireup="true" CodeBehind="ReadOnlyContactView.aspx.cs" Inherits="Site.Areas.CustomerManagement.Pages.ReadOnlyContactView" %>

<asp:Content  ContentPlaceHolderID="ContentBottom" runat="server">
	<adx:Snippet ID="RecordNotFoundError" SnippetName="customermanagement/contact/read/RecordNotFoundError" DefaultText="Customer Contact could not be found. Ensure a valid customer contact ID has been specified." Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-danger" runat="server"/>
	<adx:Snippet ID="NoChannelPermissionsRecordError" SnippetName="customermanagement/contact/read/NoChannelPermissionsRecordError" DefaultText="You do not have channel permissions. Permission to read customer contacts is denied." Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-danger" runat="server"/>
	<adx:Snippet ID="ChannelPermissionsError" SnippetName="customermanagement/contact/read/ChannelPermissionsError" DefaultText="Your channel permissions deny read rights. Permission to view customer contacts is denied." Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-danger" runat="server"/>
	<adx:Snippet ID="NoParentAccountError" SnippetName="customermanagement/contact/read/NoParentAccountError" DefaultText="A parent customer account has not been assigned to you. Permission to read customer contacts is denied." Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-danger" runat="server"/>
	<adx:Snippet ID="NoChannelPermissionsForParentAccountError" SnippetName="customermanagement/contact/read/NoChannelPermissionsForParentAccountError" DefaultText="You do not have channel permissions for your parent customer account. Permission to read customer contacts is denied." Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-danger" runat="server"/>
	<adx:Snippet ID="ParentAccountClassificationCodeError" SnippetName="customermanagement/contact/read/ParentAccountClassificationCodeError" DefaultText="<p>The parent customer account assigned to you has an invalid Classification Code. Permission to read customer contacts is denied.</p><p>Account Classification Code must be set to 'Partner'.</p>" Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-danger" runat="server"/>
	<asp:Panel ID="ContactForm" CssClass="crmEntityFormView" runat="server">
		<adx:CrmEntityFormView runat="server" 
			ID="FormView" EntityName="contact" 
			FormName="Opportunity Contact Details Form" 
			RecommendedFieldsRequired="True" 
			ShowUnsupportedFields="False" 
			ToolTipEnabled="False" 
			Mode="ReadOnly"
			LanguageCode="<%$ SiteSetting: Language Code, 0 %>"
			ContextName="<%$ SiteSetting: Language Code %>">
		</adx:CrmEntityFormView>
	</asp:Panel>
</asp:Content>
