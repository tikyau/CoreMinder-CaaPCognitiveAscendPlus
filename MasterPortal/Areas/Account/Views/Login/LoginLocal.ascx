<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Site.Areas.Account.ViewModels.LoginViewModel>" %>

<% using (Html.BeginForm("Login", "Login", new { area = "Account", ReturnUrl = ViewBag.ReturnUrl, InvitationCode = ViewBag.InvitationCode })) { %>
	<%: Html.AntiForgeryToken() %>
	<div class="form-horizontal">
		<fieldset>
			<legend><%: Html.TextSnippet("Account/SignIn/SignInLocalFormHeading", defaultValue: "Sign in with a local account", tagName: "span") %></legend>
			<%: Html.ValidationSummary(true, string.Empty, new {@class = "alert alert-block alert-danger"}) %>
			<% if (ViewBag.Settings.LocalLoginByEmail) { %>
				<div class="form-group">
					<label class="col-sm-4 control-label" for="Email"><%: Html.TextSnippet("Account/SignIn/EmailLabel", defaultValue: "Email", tagName: "span") %></label>
					<div class="col-sm-8">
						<%: Html.TextBoxFor(model => model.Email, new { @class = "form-control" }) %>
					</div>
				</div>
			<% } else { %>
				<div class="form-group">
					<label class="col-sm-4 control-label" for="Username"><%: Html.TextSnippet("Account/SignIn/UsernameLabel", defaultValue: "Username", tagName: "span") %></label>
					<div class="col-sm-8">
						<%: Html.TextBoxFor(model => model.Username, new { @class = "form-control" }) %>
					</div>
				</div>
			<% } %>
			<div class="form-group">
				<label class="col-sm-4 control-label" for="Password"><%: Html.TextSnippet("Account/SignIn/PasswordLabel", defaultValue: "Password", tagName: "span") %></label>
				<div class="col-sm-8">
					<%: Html.PasswordFor(model => model.Password, new { @class = "form-control" }) %>
				</div>
			</div>
			<% if (ViewBag.Settings.RememberMeEnabled) { %>
				<div class="form-group">
					<div class="col-sm-offset-4 col-sm-8">
						<div class="checkbox">
							<label>
								<%: Html.CheckBoxFor(model => model.RememberMe) %>
								<%: Html.TextSnippet("Account/SignIn/RememberMeLabel", defaultValue: "Remember me?", tagName: "span") %>
							</label>
						</div>
					</div>
				</div>
			<% } %>
			<div class="form-group">
				<div class="col-sm-offset-4 col-sm-8">
					<% if (ViewBag.Settings.ResetPasswordEnabled) { %>
						<a class="btn btn-default pull-right" href="<%: Url.Action("ForgotPassword") %>"><%: Html.SnippetLiteral("Account/SignIn/PasswordResetLabel", "Forgot Your Password?") %></a>
					<% } %>
					<button id="submit-signin-local" class="btn btn-primary"><%: Html.SnippetLiteral("Account/SignIn/SignInLocalButtonText", "Sign in") %></button>
				</div>
			</div>
		</fieldset>
	</div>
<% } %>
<script type="text/javascript">
	$(function() {
		$("#submit-signin-local").click(function () {
			$.blockUI({ message: null, overlayCSS: { opacity: .3 } });
		});
	});
</script>
