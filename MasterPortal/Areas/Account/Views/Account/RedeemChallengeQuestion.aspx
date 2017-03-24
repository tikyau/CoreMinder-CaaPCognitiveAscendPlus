<%@ Page Language="C#" MasterPageFile="../Shared/Account.Master" Inherits="System.Web.Mvc.ViewPage<Site.Areas.Account.ViewModels.RedeemViewModel>" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="Adxstudio.Xrm.Web.Mvc.Html" %>

<asp:Content runat="server" ContentPlaceHolderID="MainContent">
	<% using (Html.BeginForm("RedeemChallengeAnswer", "Account", ViewBag.ReturnUrlRouteValues as RouteValueDictionary)) { %>
		<%= Html.AntiForgeryToken() %>
		<%= Html.HiddenFor(model => model.wa) %>
		<%= Html.HiddenFor(model => model.wresult) %>
		<%= Html.HiddenFor(model => model.openauthresult) %>
		<%= Html.HiddenFor(model => model.InvitationCode) %>
		<%= Html.HiddenFor(model => model.ChallengeQuestion) %>
		<div class="form-horizontal">
			<fieldset>
				<legend><%: Html.SnippetLiteral("Account/Redeem/ChallengeQuestionFormHeading", "Answer the challenge question") %></legend>
				<%= Html.ValidationSummary(string.Empty, new {@class = "alert alert-block alert-danger"}) %>
				<div class="form-group">
					<label class="col-sm-2 control-label" for="ChallengeQuestion"><%: Html.SnippetLiteral("Account/Redeem/ChallengeQuestionLabel", "Question") %></label>
					<div class="col-sm-10">
						<p class="help-block"><%: Model.ChallengeQuestion %></p>
					</div>
				</div>
				<div class="form-group">
					<label class="col-sm-2 control-label required" for="ChallengeAnswer"><%: Html.SnippetLiteral("Account/Redeem/ChallengeAnswerLabel", "Answer") %></label>
					<div class="col-sm-10">
						<%= Html.TextBoxFor(model => model.ChallengeAnswer) %>
					</div>
				</div>
				<div class="form-group">
					<div class="col-sm-offset-2 col-sm-10">
						<input id="submit-challenge-answer" type="submit" class="btn btn-primary" value="<%: Html.SnippetLiteral("Account/Redeem/ChallengeQuestionFormButtonText", "Submit") %>"/>
					</div>
				</div>
			</fieldset>
		</div>
	<% } %>
	<script type="text/javascript">
		$(function() {
			$("#submit-challenge-answer").click(function () {
				$.blockUI({ message: null, overlayCSS: { opacity: .3 } });
			});
		});
	</script>
</asp:Content>
