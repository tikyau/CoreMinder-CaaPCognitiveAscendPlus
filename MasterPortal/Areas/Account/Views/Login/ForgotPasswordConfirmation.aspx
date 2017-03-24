<%@ Page Language="C#" MasterPageFile="Account.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ContentPlaceHolderID="PageCopy" runat="server">
	<%: Html.HtmlSnippet("Account/ForgotPasswordConfirmation/PageCopy", "page-copy") %>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
	<div class="form-horizontal">
		<fieldset>
			<legend><%: Html.TextSnippet("Account/PasswordReset/ForgotPasswordFormHeading", defaultValue: "Forgot your password?") %></legend>
			<div class="alert alert-info">
				<%: Html.HtmlSnippet("Account/PasswordReset/ForgotPasswordConfirmationSuccessText", defaultValue: "Please check your email to reset your password.") %>
			</div>
		</fieldset>
	</div>
	<% if (ViewBag.Settings.IsDemoMode && ViewBag.DemoModeLink != null) { %>
		<div class="panel panel-warning">
			<div class="panel-heading">
				<h3 class="panel-title"><span class="fa fa-wrench" aria-hidden="true"></span> DEMO MODE <small>LOCAL ONLY</small></h3>
			</div>
			<div class="panel-body">
				<a class="btn btn-default" href="<%: ViewBag.DemoModeLink %>"><%: Html.SnippetLiteral("Account/SignIn/ForgotPasswordConfirmationButtonText", "Reset Password") %></a>
			</div>
		</div>
	<% } %>
</asp:Content>
