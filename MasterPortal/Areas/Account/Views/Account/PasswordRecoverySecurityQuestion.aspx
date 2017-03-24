<%@ Page Language="C#" MasterPageFile="../Shared/Account.Master" Inherits="System.Web.Mvc.ViewPage<Site.Areas.Account.ViewModels.PasswordRecoveryViewModel>" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
	<div class="form-horizontal">
		<% using (Html.BeginForm("PasswordRecoverySecurityAnswer", "Account", ViewBag.ReturnUrlRouteValues as RouteValueDictionary)) { %>
			<%= Html.AntiForgeryToken() %>
			<%= Html.HiddenFor(model => model.Username) %>
			<%= Html.HiddenFor(model => model.Question) %>
			<fieldset>
				<legend><%: Html.SnippetLiteral("Account/PasswordRecovery/SecurityQuestionFormHeading", "Reset your password") %></legend>
				<%= Html.ValidationSummary(string.Empty, new {@class = "alert alert-block alert-danger"}) %>
				<div class="form-group">
					<label class="col-sm-2 control-label" for="UserName"><%: Html.SnippetLiteral("Account/PasswordRecovery/UserNameLabel", "Username") %></label>
					<div class="col-sm-10">
						<p class="help-block"><%: Model.Username %></p>
					</div>
				</div>
				<div class="form-group">
					<label class="col-sm-2 control-label" for="Question"><%: Html.SnippetLiteral("Account/PasswordRecovery/QuestionLabel", "Question") %></label>
					<div class="col-sm-10">
						<p class="help-block"><%: Model.Question %></p>
					</div>
				</div>
				<div class="form-group">
					<label class="col-sm-2 control-label required" for="Answer"><%: Html.SnippetLiteral("Account/PasswordRecovery/AnswerLabel", "Answer") %></label>
					<div class="col-sm-10">
						<%= Html.TextBoxFor(model => model.Answer) %>
						<p class="help-block"><%: Html.SnippetLiteral("Account/PasswordRecovery/AnswerInstructionsText", "Answer the question correctly to receive your password.") %></p>
					</div>
				</div>
				<div class="form-group">
					<div class="col-sm-offset-2 col-sm-10">
						<input id="submit-security-answer" type="submit" class="btn btn-primary" value="<%: Html.SnippetLiteral("Account/PasswordRecovery/SecurityQuestionFormButtonText", "Submit") %>"/>
					</div>
				</div>
			</fieldset>
		<% } %>
	</div>
	<script type="text/javascript">
		$(function () {
			$("#submit-security-answer").click(function () {
				$.blockUI({ message: null, overlayCSS: { opacity: .3 } });
			});
		});
	</script>
</asp:Content>
