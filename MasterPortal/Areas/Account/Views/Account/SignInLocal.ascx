<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Site.Areas.Account.ViewModels.SignInLocalViewModel>" %>
<%@ Import namespace="Adxstudio.Xrm.Collections.Generic" %>

<div class="form-horizontal">
	<% using (Html.BeginForm("SignInLocal", "Account", ViewBag.ReturnUrlRouteValues as RouteValueDictionary)) { %>
		<%= Html.AntiForgeryToken() %>
			<fieldset>
				<legend><%: Html.SnippetLiteral("Account/SignIn/LocalSignInFormHeading", "Sign in with a local account") %></legend>
				<%= Html.ValidationSummary(string.Empty, new {@class = "alert alert-block alert-danger"}) %>
				<div class="form-group">
					<label class="col-sm-4 control-label required" for="UserName"><%: Html.SnippetLiteral("Account/SignIn/UserNameLabel", "Username") %></label>
					<div class="col-sm-8">
						<%= Html.TextBoxFor(model => model.UserName, new { @class = "form-control" }) %>
					</div>
				</div>
				<div class="form-group">
					<label class="col-sm-4 control-label required" for="Password"><%: Html.SnippetLiteral("Account/SignIn/PasswordLabel", "Password") %></label>
					<div class="col-sm-8">
						<%= Html.PasswordFor(model => model.Password, new { @class = "form-control" }) %>
					</div>
				</div>
				<% if (ViewBag.DisplayRememberMe) { %>
					<div class="form-group">
						<div class="col-sm-offset-4 col-sm-8">
							<div class="checkbox">
								<label>
									<%= Html.CheckBoxFor(model => model.RememberMe) %>
									<%: Html.SnippetLiteral("Account/SignIn/RememberMeLabel", "Remember me?") %>
								</label>
							</div>
						</div>
					</div>
				<% } %>
				<div class="form-group">
					<div class="col-sm-offset-4 col-sm-8">
						<% if (Membership.EnablePasswordReset) { %>
							<a class="btn btn-default pull-right" href="<%= Html.SiteMarkerUrl("Login") %>PasswordRecovery?<%: (ViewBag.PasswordRecoveryQueryString as NameValueCollection).ToQueryString() %>"><%: Html.SnippetLiteral("Account/SignIn/PasswordRecovery", "Forgot Your Password?") %></a>
						<% } %>
						<input id="submit-signin-local" type="submit" class="btn btn-primary" value="<%: Html.SnippetLiteral("Account/SignIn/LocalSignInFormButtonText", "Sign in") %>"/>
					</div>
				</div>
			</fieldset>
	<% } %>
</div>
<script type="text/javascript">
	$(function() {
		$("#submit-signin-local").click(function () {
			$.blockUI({ message: null, overlayCSS: { opacity: .3 } });
		});
	});
</script>
