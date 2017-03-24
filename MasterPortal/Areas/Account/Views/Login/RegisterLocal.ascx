<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Site.Areas.Account.ViewModels.RegisterViewModel>" %>

<% using (Html.BeginForm("Register", "Login", new { area = "Account", ReturnUrl = ViewBag.ReturnUrl, InvitationCode = ViewBag.InvitationCode })) { %>
	<%: Html.AntiForgeryToken() %>
	<div class="form-horizontal">
		<fieldset>
			<legend><%: Html.TextSnippet("Account/Register/RegisterLocalFormHeading", defaultValue: "Register for a new local account", tagName: "span") %></legend>
			<%: Html.ValidationSummary(string.Empty, new {@class = "alert alert-block alert-danger"}) %>
			<% if (ViewBag.Settings.LocalLoginByEmail || ViewBag.Settings.RequireUniqueEmail) { %>
				<div class="form-group">
					<label class="col-sm-4 control-label" for="Email"><%: Html.TextSnippet("Account/Register/EmailLabel", defaultValue: "Email", tagName: "span") %></label>
					<div class="col-sm-8">
						<%: Html.TextBoxFor(model => model.Email, new { @class = "form-control" }) %>
					</div>
				</div>
			<% } %>
			<% if (!ViewBag.Settings.LocalLoginByEmail) { %>
				<div class="form-group">
					<label class="col-sm-4 control-label" for="Username"><%: Html.TextSnippet("Account/Register/UsernameLabel", defaultValue: "Username", tagName: "span") %></label>
					<div class="col-sm-8">
						<%: Html.TextBoxFor(model => model.Username, new { @class = "form-control" }) %>
					</div>
				</div>
			<% } %>
			<div class="form-group">
				<label class="col-sm-4 control-label" for="Password"><%: Html.TextSnippet("Account/Register/PasswordLabel", defaultValue: "Password", tagName: "span") %></label>
				<div class="col-sm-8">
					<%: Html.PasswordFor(model => model.Password, new { @class = "form-control" }) %>
				</div>
			</div>
			<div class="form-group">
				<label class="col-sm-4 control-label" for="ConfirmPassword"><%: Html.TextSnippet("Account/Register/ConfirmPasswordLabel", defaultValue: "Confirm Password", tagName: "span") %></label>
				<div class="col-sm-8">
					<%: Html.PasswordFor(model => model.ConfirmPassword, new { @class = "form-control" }) %>
				</div>
			</div>
			<div class="form-group">
				<div class="col-sm-offset-4 col-sm-8">
					<button id="submit-signup-local" class="btn btn-primary"><%: Html.SnippetLiteral("Account/Register/RegisterButtonText", "Register") %></button>
				</div>
			</div>
		</fieldset>
	</div>
<% } %>
<script type="text/javascript">
	$(function() {
		$("#submit-signup-local").click(function () {
			$.blockUI({ message: null, overlayCSS: { opacity: .3 } });
		});
	});
</script>
