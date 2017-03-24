<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/WebFormsContent.master" CodeBehind="ContactUs.aspx.cs" Inherits="Site.Areas.ContactUs.Pages.ContactUs" %>
<%@ Register TagPrefix="recaptcha" Namespace="Recaptcha" Assembly="Recaptcha, Version=1.0.5.0, Culture=neutral, PublicKeyToken=9afc4d65b28c38c2" %>
<%@ Register TagPrefix="site" Namespace="Site.Controls" Assembly="Site" %>
<%@ Import Namespace="Adxstudio.Xrm.Web.Mvc.Html" %>

<%--
To enable the Recaptcha validation control:
1. Sign up for an API key at http://www.google.com/recaptcha/whyrecaptcha
2. Add two new site settings to the website with the names "/Recaptcha/PublicKey" and "/Recaptcha/PrivateKey" to hold the public and private keys
--%>

<asp:Content  ContentPlaceHolderID="MainContent" runat="server">
</asp:Content>

<asp:Content ContentPlaceHolderID="ContentBottom" runat="server">
		<crm:CrmDataSource ID="WebFormDataSource" runat="server" />
		<adx:CrmEntityFormView runat="server" ID="FormView" DataSourceID="WebFormDataSource" 
			CssClass="crmEntityFormView" 
			FormName="Web Form" 
			EntityName="lead" 
			OnItemInserted="OnItemInserted" 
			ValidationGroup="ContactUs"
			ValidationSummaryCssClass="alert alert-danger alert-block"
			LanguageCode="<%$ SiteSetting: Language Code, 0 %>"
			ContextName="<%$ SiteSetting: Language Code %>">
			<InsertItemTemplate>
				<div class="row">
					<div class="cell">
						<div class="info">
							<asp:CustomValidator
								ID="RecaptchaValidator"
								runat="server"
								ValidationGroup="ContactUs"
								ErrorMessage="reCAPTCHA is invalid. Please try again."
								OnServerValidate="RecaptchaValidator_ServerValidate" Display="None" />
						</div>
						<div class="control">
							<recaptcha:RecaptchaControl
								ID="RecaptchaImage"
								runat="server"
								PublicKey='<%$ SiteSetting: Recaptcha/PublicKey, null %>'
								PrivateKey='<%$ SiteSetting: Recaptcha/PrivateKey, null %>' />
						</div>
					</div>
				</div>
				<div class="actions">
					<asp:Button ID="SubmitButton" Text='<%$ Snippet: ContactUs/Submit, Submit %>' CssClass="btn btn-primary" 
						CommandName="Insert" ValidationGroup="ContactUs" runat="server" />
				</div>
			</InsertItemTemplate>
		</adx:CrmEntityFormView>

	<asp:Panel ID="ConfirmationMessage" runat="server" Visible="false">
		<adx:Snippet runat="server" SnippetName="ContactUs/ConfirmationMsg" DefaultText="Thank you." EditType="html" />
	</asp:Panel>
</asp:Content>

<asp:Content ContentPlaceHolderID="SidebarTop" runat="server">
	<div class="content-panel panel panel-default">
		<div class="panel-heading">
			<h4>
				<%: Html.TextSnippet("contact-us/instructions", defaultValue: "Instructions", tagName: "span") %>
			</h4>
		</div>
		<div class="panel-body">
			<%: Html.HtmlAttribute("adx_copy") %>
		</div>
	</div>
	<site:ChildNavigation runat="server"/>
</asp:Content>

