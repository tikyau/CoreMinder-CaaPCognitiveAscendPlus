<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WebFormPermitSuccess.ascx.cs" Inherits="Site.Areas.Permits.Controls.WebFormPermitSuccess" %>

<asp:Panel ID="PanelSuccess" runat="server" Visible="False">
	<div class="alert alert-block alert-success">
		<adx:Snippet ID="DefaultSuccessMessageSnippet" runat="server" Visible="False" SnippetName="Permit Default Success Message" EditType="html" Editable="true" DefaultText="<p><strong><em>Thank you.</em></strong></p><p>Your permit application has been successfully submitted.</p><p>Your application will be reviewed within 24 hours and you will receive confirmation and a follow-up message.</p>" ClientIDMode="Static" />
		<asp:Label ID="CustomSuccessMessage" runat="server" />
	</div>
	<asp:Panel ID="PermitNumberPanel" runat="server" Visible="False" CssClass="alert alert-block alert-info">
		<p>
			<adx:Snippet ID="PermitNumberLabel" runat="server" SnippetName="Permit Number Label Text" EditType="text" Editable="true" DefaultText="Permit Number:" ClientIDMode="Static" />&nbsp;<strong><asp:Label ID="PermitNumber" runat="server" ClientIDMode="Static"/></strong>
		</p>
	</asp:Panel>
</asp:Panel>
