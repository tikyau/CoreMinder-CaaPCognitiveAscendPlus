<%@ Page Title="" Language="C#" MasterPageFile="Manage.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ContentPlaceHolderID="PageCopy" runat="server">
	<%: Html.HtmlSnippet("Account/ChangeTwoFactor/PageCopy", "page-copy") %>
</asp:Content>

<asp:Content ContentPlaceHolderID="ProfileNavbar" runat="server">
	<% Html.RenderPartial("ProfileNavbar"); %>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
	<div class="form-horizontal">
		<fieldset>
			<legend><%: Html.TextSnippet("Account/ChangeTwoFactor/ChangeTwoFactorFormHeading", defaultValue: "Change Two-Factor Authentication", tagName: "span") %></legend>
			<%  if (ViewBag.HasEmail && !ViewBag.IsEmailConfirmed) { %>
				<div class="alert alert-warning clearfix">
					<a class="btn btn-warning btn-xs pull-right" href="<%: Url.Action("ConfirmEmailRequest", "Manage") %>">
						<span class="fa fa-envelope-o" aria-hidden="true"></span> <%: Html.SnippetLiteral("Account/ChangeTwoFactor/ConfirmEmailButtonText", "Confirm Email") %>
					</a>
					<span class="fa fa-exclamation-circle" aria-hidden="true"></span> <%: Html.TextSnippet("Account/ChangeTwoFactor/ConfirmEmailInstructionsText", defaultValue: "A confirmed email is required for two-factor authentication.", tagName: "span") %>
				</div>
			<% } else if (!ViewBag.HasEmail) { %>
				<div class="alert alert-warning clearfix">
					<a class="btn btn-warning btn-xs pull-right" href="<%: Url.Action("ChangeEmail", "Manage") %>">
						<%: Html.TextSnippet("Account/ChangeTwoFactor/ChangeEmailButtonText", defaultValue: "Change Email") %>
					</a>
					<span class="fa fa-exclamation-circle" aria-hidden="true"></span> <%: Html.HtmlSnippet("Account/ChangeTwoFactor/ChangeEmailInstructionsText", defaultValue: "A confirmed email is required for two-factor authentication.") %>
				</div>
			<% } %>
			<% if (ViewBag.IsTwoFactorEnabled) { %>
				<div class="alert alert-info clearfix">
					<% using (Html.BeginForm("DisableTFA", "Manage")) { %>
						<%: Html.AntiForgeryToken() %>
						<button class="btn btn-primary btn-xs pull-right"><span class="fa fa-times-circle" aria-hidden="true"></span> <%: Html.SnippetLiteral("Account/ChangeTwoFactor/DisableTwoFactorButtonText", "Disable") %></button>
					<% } %>
					<%  if (ViewBag.HasEmail && ViewBag.IsEmailConfirmed) { %>
						<%: Html.TextSnippet("Account/ChangeTwoFactor/TwoFactorEnabledText", defaultValue: "Two-factor authentication is currently enabled.", tagName: "span") %>
					<% } else { %>
						<%: Html.TextSnippet("Account/ChangeTwoFactor/TwoFactorWaitingText", defaultValue: "Two-factor authentication is waiting for email confirmation.", tagName: "span") %>
					<% } %>
				</div>
				<% if (ViewBag.TwoFactorBrowserRemembered) { %>
					<div class="alert alert-warning clearfix">
						<% using (Html.BeginForm("ForgetBrowser", "Manage")) { %>
							<%: Html.AntiForgeryToken() %>
							<button class="btn btn-warning btn-xs pull-right"><span class="fa fa-trash-o" aria-hidden="true"></span> <%: Html.SnippetLiteral("Account/ChangeTwoFactor/ForgetBrowserButtonText", "Forget Browser") %></button>
						<% } %>
						<%: Html.TextSnippet("Account/ChangeTwoFactor/TwoFactorBrowserRememberedText", defaultValue: "The two-factor authentication session is remembered for this browser.", tagName: "span") %>
					</div>
				<% } %>
			<% } else if (ViewBag.HasEmail && ViewBag.IsEmailConfirmed) { %>
				<div class="alert alert-warning clearfix">
					<% using (Html.BeginForm("EnableTFA", "Manage")) { %>
						<%: Html.AntiForgeryToken() %>
						<button class="btn btn-warning btn-xs pull-right"><span class="fa fa-check-circle" aria-hidden="true"></span> <%: Html.SnippetLiteral("Account/ChangeTwoFactor/EnableTwoFactorButtonText", "Enable") %></button>
					<% } %>
					<span class="fa fa-exclamation-circle" aria-hidden="true"></span> <%: Html.TextSnippet("Account/ChangeTwoFactor/TwoFactorDisabledText", defaultValue: "Two-factor authentication is currently disabled.", tagName: "span") %>
				</div>
			<% } %>
		</fieldset>
	</div>
</asp:Content>
