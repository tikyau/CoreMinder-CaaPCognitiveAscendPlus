<%@ Page Language="C#" MasterPageFile="Manage.Master" Inherits="System.Web.Mvc.ViewPage<Site.Areas.Account.ViewModels.SetPasswordViewModel>" %>

<asp:Content ContentPlaceHolderID="PageCopy" runat="server">
	<%: Html.HtmlSnippet("Account/SetPassword/PageCopy", "page-copy") %>
</asp:Content>

<asp:Content ContentPlaceHolderID="ProfileNavbar" runat="server">
	<% Html.RenderPartial("ProfileNavbar"); %>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
	<div class="form-horizontal">
		<fieldset>
			<legend><%: Html.TextSnippet("Account/SetPassword/SetPasswordFormHeading", defaultValue: "Set Password", tagName: "span") %></legend>
			<% if (ViewBag.Settings.LocalLoginByEmail && !ViewBag.HasEmail) { %>
				<div class="alert alert-warning clearfix">
					<a class="btn btn-warning btn-xs pull-right" href="<%: Url.Action("ChangeEmail", "Manage") %>">
						<%: Html.TextSnippet("Account/SetPassword/ChangeEmailButtonText", defaultValue: "Set Email", tagName: "span") %>
					</a>
					<span class="fa fa-exclamation-circle" aria-hidden="true"></span> <%: Html.HtmlSnippet("Account/SetPassword/ChangeEmailInstructionsText", defaultValue: "An email is required to set a local account password.") %>
				</div>
			<% } else { %>
				<% using (Html.BeginForm("SetPassword", "Manage", new { area = "Account" })) { %>
					<%: Html.AntiForgeryToken() %>
					<%: Html.ValidationSummary(string.Empty, new {@class = "alert alert-block alert-danger"}) %>
					<% if (ViewBag.Settings.LocalLoginByEmail) { %>
						<div class="form-group">
							<label class="col-sm-4 control-label" for="Email"><%: Html.TextSnippet("Account/SetPassword/Email", defaultValue: "Email", tagName: "span") %></label>
							<div class="col-sm-8">
								<%: Html.TextBoxFor(model => model.Email, new { @class = "form-control", @readonly = "readonly" }) %>
							</div>
						</div>
					<% } else { %>
						<div class="form-group">
							<label class="col-sm-4 control-label" for="Username"><%: Html.TextSnippet("Account/SetPassword/Username", defaultValue: "Username", tagName: "span") %></label>
							<div class="col-sm-8">
								<%: Html.TextBoxFor(model => model.Username, new { @class = "form-control" }) %>
							</div>
						</div>
					<% } %>
					<div class="form-group">
						<label class="col-sm-4 control-label" for="NewPassword"><%: Html.TextSnippet("Account/SetPassword/NewPassword", defaultValue: "New Password", tagName: "span") %></label>
						<div class="col-sm-8">
							<%: Html.PasswordFor(model => model.NewPassword, new { @class = "form-control" }) %>
						</div>
					</div>
					<div class="form-group">
						<label class="col-sm-4 control-label" for="ConfirmPassword"><%: Html.TextSnippet("Account/SetPassword/ConfirmPassword", defaultValue: "Confirm Password", tagName: "span") %></label>
						<div class="col-sm-8">
							<%: Html.PasswordFor(model => model.ConfirmPassword, new { @class = "form-control" }) %>
						</div>
					</div>
					<div class="form-group">
						<div class="col-sm-offset-4 col-sm-8">
							<button class="btn btn-primary"><%: Html.SnippetLiteral("Account/SetPassword/SetPasswordButtonText", "Set Password") %></button>
						</div>
					</div>
				<% } %>
			<% } %>
		</fieldset>
	</div>
</asp:Content>
