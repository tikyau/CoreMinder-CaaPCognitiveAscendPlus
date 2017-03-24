<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Site.Areas.Account.ViewModels.RedeemViewModel>" %>
<%@ Import Namespace="Adxstudio.Xrm.Account" %>
<%@ Import namespace="Adxstudio.Xrm.Collections.Generic" %>

<% using (Html.BeginForm("RedeemLocal", "Account", ViewBag.ReturnUrlRouteValues as RouteValueDictionary)) { %>
	<%= Html.AntiForgeryToken() %>
	<%= Html.HiddenFor(model => model.InvitationCode) %>
	<%= Html.HiddenFor(model => model.InvitationType) %>
	<%= Html.HiddenFor(model => model.UserExists) %>
	<%= Html.HiddenFor(model => model.ChallengeAnswer) %>
	
	<% if (Model.UserExists || Model.InvitationType == InvitationType.Group) { %>
		<div class="form-horizontal">		
			<fieldset>
				<legend><%: Html.SnippetLiteral("Account/SignIn/LocalSignInFormHeading", "Sign in with a local account") %></legend>
				<div class="form-group">
					<label class="col-sm-4 control-label required" for="SignInUsername"><%: Html.SnippetLiteral("Account/SignIn/UserNameLabel", "Username") %></label>
					<div class="col-sm-8">
						<%= Html.TextBoxFor(model => model.SignInUsername, new { @class = "form-control" }) %>
					</div>
				</div>
				<div class="form-group">
					<label class="col-sm-4 control-label required" for="SignInPassword"><%: Html.SnippetLiteral("Account/SignIn/PasswordLabel", "Password") %></label>
					<div class="col-sm-8">
						<%= Html.PasswordFor(model => model.SignInPassword, new { @class = "form-control" }) %>
					</div>
				</div>
				<div class="form-group">
					<div class="col-sm-offset-4 col-sm-8">
						<% if (Membership.EnablePasswordReset) { %>
							<a class="btn btn-default pull-right" href="<%= Html.SiteMarkerUrl("Login") %>PasswordRecovery?<%: (ViewBag.PasswordRecoveryQueryString as NameValueCollection).ToQueryString() %>"><%: Html.SnippetLiteral("Account/SignIn/PasswordRecovery", "Forgot Your Password?") %></a>
						<% } %>
						<input id="submit-signin-local" type="submit" class="btn btn-primary" value="<%: Html.SnippetLiteral("Account/SignIn/LocalSignInFormButtonText", "Sign in") %>"/>
					</div>
				</div>
			</fieldset>
		</div>
	<% } if (!Model.UserExists || Model.InvitationType == InvitationType.Group) { %>
		<div class="form-horizontal">
		<fieldset>
			<legend><%: Html.SnippetLiteral("Account/Redeem/LocalSignUpFormHeading", "Sign up for a new local account") %></legend>
			<%= Html.ValidationSummary(string.Empty, new {@class = "alert alert-block alert-danger"}) %>
			<div class="form-group">
				<label class="col-sm-4 control-label required" for="UserName"><%: Html.SnippetLiteral("Account/Redeem/UserNameLabel", "Username") %></label>
				<div class="col-sm-8">
					<%= Html.TextBoxFor(model => model.Username, new {@class = "form-control"}) %>
				</div>
			</div>
			<div class="form-group">
				<label class="col-sm-4 control-label required" for="Password"><%: Html.SnippetLiteral("Account/Redeem/PasswordLabel", "Password") %></label>
				<div class="col-sm-8">
					<%= Html.PasswordFor(model => model.Password, new {@class = "form-control"}) %>
				</div>
			</div>
			<div class="form-group">
				<label class="col-sm-4 control-label required" for="ConfirmPassword"><%: Html.SnippetLiteral("Account/Redeem/ConfirmPasswordLabel", "Confirm Password") %></label>
				<div class="col-sm-8">
					<%= Html.PasswordFor(model => model.ConfirmPassword, new {@class = "form-control"}) %>
				</div>
			</div>
			<% if (ViewBag.RequiresUniqueEmail && !ViewBag.RequiresConfirmation) { %>
				<div class="form-group">
					<label class="col-sm-4 control-label required" for="Email"><%: Html.SnippetLiteral("Account/Redeem/EmailLabel", "Email") %></label>
					<div class="col-sm-8">
						<%= Html.TextBoxFor(model => model.Email, new {@class = "form-control"}) %>
					</div>
				</div>
			<% } %>
			<% if (ViewBag.RequiresQuestionAndAnswer) { %>
				<div class="form-group">
					<label class="col-sm-4 control-label required" for="Question"><%: Html.SnippetLiteral("Account/Redeem/QuestionLabel", "Question") %></label>
					<div class="col-sm-8">
						<%= Html.TextBoxFor(model => model.Question, new {@class = "form-control"}) %>
					</div>
				</div>
				<div class="form-group">
					<label class="col-sm-4 control-label required" for="Answer"><%: Html.SnippetLiteral("Account/Redeem/AnswerLabel", "Answer") %></label>
					<div class="col-sm-8">
						<%= Html.TextBoxFor(model => model.Answer, new {@class = "form-control"}) %>
					</div>
				</div>
			<% } %>
			<div class="form-group">
				<div class="col-sm-offset-4 col-sm-8">
				<input id="submit-redeem-local" type="submit" class="btn btn-primary" value="<%: Html.SnippetLiteral("Account/Redeem/LocalSignUpFormButtonText", "Create Local Account") %>"/>
			</div>
			</div>
		</fieldset>
	</div>
	<% } %>
<% } %>
<script type="text/javascript">
	$(function() {
		$("#submit-signin-local, #submit-redeem-local").click(function () {
			$.blockUI({ message: null, overlayCSS: { opacity: .3 } });
		});
	});
</script>
