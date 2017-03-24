<%@ Page Language="C#" MasterPageFile="~/MasterPages/WebFormsContent.master" ValidateRequest="false" AutoEventWireup="true" CodeBehind="CreateCase.aspx.cs" Inherits="Site.Areas.HelpDesk.Pages.CreateCase" %>

<asp:Content ContentPlaceHolderID="Head" runat="server">
	<link rel="stylesheet" href="<%: Url.Content("~/Areas/HelpDesk/css/helpdesk.css") %>">
</asp:Content>

<asp:Content ContentPlaceHolderID="ContentBottom" runat="server" ViewStateMode="Enabled">
	<fieldset>
		<asp:Panel ID="CreateCaseForm" runat="server" Visible="true">
			<adx:CrmDataSource ID="WebFormDataSource" runat="server" CrmDataContextName="<%$ SiteSetting: Language Code %>" />
			<adx:CrmEntityFormView runat="server" ID="FormView" DataSourceID="WebFormDataSource"
				ValidationText=""
				FormName="Create Case Web Form"
				EntityName="incident"
				ValidationGroup="CreateCase"
				OnItemInserted="OnItemInserted"
				Mode="Insert"
				OnItemInserting="OnItemInserting"
				CssClass="crmEntityFormView"
				LanguageCode="<%$ SiteSetting: Language Code, 0 %>"
				ContextName="<%$ SiteSetting: Language Code %>"
				ClientIDMode="Static">
			 <InsertItemTemplate>
				<div>
					<div class="cell">
						<div class="info">
							<asp:Label AssociatedControlID="CustomerDropdown" runat="server"><adx:Snippet runat="server" SnippetName="cases/createCase/customer" DefaultText="Customer" /></asp:Label>
						</div>
						<div class="control">
							<asp:DropDownList ID="CustomerDropdown" CssClass="form-control" runat="server" />
						</div>
					</div>
				</div>
				<div>
					<div class="cell">
						<div class="info">
							<asp:Label AssociatedControlID="Attachment" runat="server"><adx:Snippet  runat="server" SnippetName="cases/createCase/attachement" DefaultText="Attach a file" /></asp:Label>
						</div>
						<div class="control">
							<asp:FileUpload ID="Attachment" size="42" runat="server" />
						</div>
					</div>
				</div>
				<div class="actions">
					<asp:Button ID="CreateButton" runat="server" CssClass="btn btn-primary" Text="<%$ Snippet: Cases/CreateCase, Create Case %>" CommandName="Insert" ValidationGroup="CreateCase" />
				</div>
			 </InsertItemTemplate>
			</adx:CrmEntityFormView>
		</asp:Panel>

		<asp:Panel ID="NoCaseAccessMessage" CssClass="alert alert-block alert-success" runat="server" Visible="false">
			<adx:Snippet runat="server" SnippetName="cases/access/nopermissions" DefaultText="You do not have case access permissions to create cases" Editable="true" EditType="html"/>
		</asp:Panel>
	</fieldset>
</asp:Content>
