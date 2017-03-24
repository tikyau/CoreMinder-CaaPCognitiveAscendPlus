<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Site.Areas.Account.ViewModels.SignUpLocalViewModel>" %>

<% using (Html.BeginForm("SignUpLocal", "Account", ViewBag.ReturnUrlRouteValues as RouteValueDictionary)) { %>
	<%= Html.AntiForgeryToken() %>
	<div class="form-horizontal">
		<fieldset>
			<legend><%: Html.SnippetLiteral("Account/SignUp/LocalSignUpFormHeading", "Sign up for a new local account") %></legend>
			<%= Html.ValidationSummary(string.Empty, new {@class = "alert alert-block alert-danger"}) %>
			<div class="form-group">
				<label class="col-sm-4 control-label required" for="UserName"><%: Html.SnippetLiteral("Account/SignUp/UserNameLabel", "Username") %></label>
				<div class="col-sm-8">
					<%= Html.TextBoxFor(model => model.Username, new { @class = "form-control" }) %>
				</div>
			</div>
			<div class="form-group">
				<label class="col-sm-4 control-label required" for="Password"><%: Html.SnippetLiteral("Account/SignUp/PasswordLabel", "Password") %></label>
				<div class="col-sm-8">
					<%= Html.PasswordFor(model => model.Password, new { @class = "form-control" }) %>
				</div>
			</div>
			<div class="form-group">
				<label class="col-sm-4 control-label required" for="ConfirmPassword"><%: Html.SnippetLiteral("Account/SignUp/ConfirmPasswordLabel", "Confirm Password") %></label>
				<div class="col-sm-8">
					<%= Html.PasswordFor(model => model.ConfirmPassword, new { @class = "form-control" }) %>
				</div>
			</div>
			<% if (ViewBag.RequiresUniqueEmail) { %>
				<div class="form-group">
					<label class="col-sm-4 control-label required" for="Email"><%: Html.SnippetLiteral("Account/SignUp/EmailLabel", "Email") %></label>
					<div class="col-sm-8">
						<%= Html.TextBoxFor(model => model.Email, new { @class = "form-control" }) %>
					</div>
				</div>
			<% } %>
			<% if (ViewBag.RequiresQuestionAndAnswer) { %>
				<div class="form-group">
					<label class="col-sm-4 control-label required" for="Question"><%: Html.SnippetLiteral("Account/SignUp/QuestionLabel", "Question") %></label>
					<div class="col-sm-8">
						<%= Html.TextBoxFor(model => model.Question, new { @class = "form-control" }) %>
					</div>
				</div>
				<div class="form-group">
					<label class="col-sm-4 control-label required" for="Answer"><%: Html.SnippetLiteral("Account/SignUp/AnswerLabel", "Answer") %></label>
					<div class="col-sm-8">
						<%= Html.TextBoxFor(model => model.Answer, new { @class = "form-control" }) %>
					</div>
				</div>
			<% } %>
			<div class="form-group">
				<div class="col-sm-offset-4 col-sm-8">
				<input id="submit-signup-local" type="submit" class="btn btn-primary" value="<%: Html.SnippetLiteral("Account/SignUp/LocalSignUpFormButtonText", "Sign Up") %>"/>
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
