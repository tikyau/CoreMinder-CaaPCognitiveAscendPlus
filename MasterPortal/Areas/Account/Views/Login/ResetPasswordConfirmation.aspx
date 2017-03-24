<%@ Page Title="" Language="C#" MasterPageFile="Account.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ContentPlaceHolderID="PageCopy" runat="server">
	<%: Html.HtmlSnippet("Account/ResetPasswordConfirmation/PageCopy", "page-copy") %>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
	<div class="form-horizontal">
		<fieldset>
			<legend><%: Html.TextSnippet("Account/PasswordReset/ResetPasswordFormHeading", defaultValue: "Reset Password", tagName: "span") %></legend>
			<div class="alert alert-success">
				<%: Html.HtmlSnippet("Account/PasswordReset/ResetPasswordSuccessText", defaultValue: "Your password has been reset.") %>
			</div>
			<% Html.RenderPartial("SignInLink", new ViewDataDictionary(ViewData) { { "class", "btn btn-primary" }, { "ReturnUrl", string.Empty } }); %>
		</fieldset>
	</div>
</asp:Content>
