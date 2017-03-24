<%@ Page Title="" Language="C#" MasterPageFile="../Shared/Account.Master" Inherits="System.Web.Mvc.ViewPage<Site.Areas.Account.ViewModels.PasswordRecoveryViewModel>" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
	<div class="form-horizontal">
		<% using (Html.BeginForm("PasswordRecoveryEmail", "Account", ViewBag.ReturnUrlRouteValues as RouteValueDictionary)) { %>
			<%= Html.AntiForgeryToken() %>
			<fieldset>
				<legend><%: Html.SnippetLiteral("Account/PasswordRecovery/UserNameFormHeading", "Reset your password") %></legend>
				<%= Html.ValidationSummary(string.Empty, new {@class = "alert alert-block alert-danger"}) %>
				<div class="form-group">
					<label class="col-sm-2 control-label required" for="UserName"><%: Html.SnippetLiteral("Account/PasswordRecovery/UserNameLabel", "Username") %></label>
					<div class="col-sm-10">
						<%= Html.TextBoxFor(model => model.Username, new { @class = "form-control" }) %>
						<p class="help-block"><%: Html.SnippetLiteral("Account/PasswordRecovery/UserNameInstructionsText", "Enter your username to reset your password.") %></p>
					</div>
				</div>
				<div class="form-group">
					<div class="col-sm-offset-2 col-sm-10">
						<input id="submit-username" type="submit" class="btn btn-primary" value="<%: Html.SnippetLiteral("Account/PasswordRecovery/UserNameFormButtonText", "Submit") %>"/>
					</div>
				</div>
			</fieldset>
		<% } %>
	</div>
	<script type="text/javascript">
		$(function () {
			$("#submit-username").click(function () {
				$.blockUI({ message: null, overlayCSS: { opacity: .3 } });
			});
		});
	</script>
</asp:Content>
