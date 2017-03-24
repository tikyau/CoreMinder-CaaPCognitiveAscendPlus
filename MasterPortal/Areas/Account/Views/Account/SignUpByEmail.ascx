<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Site.Areas.Account.ViewModels.EmailConfirmationViewModel>" %>

<% using (Html.BeginForm("SignUpByEmail", "Account", ViewBag.ReturnUrlRouteValues as RouteValueDictionary)) { %>
	<%= Html.AntiForgeryToken() %>
	<div class="form-horizontal">
		<fieldset>
			<legend><%: Html.SnippetLiteral("Account/SignUp/EmailConfirmationFormHeading", "Sign up for a new account") %></legend>
			<%= Html.ValidationSummary(string.Empty, new {@class = "alert alert-block alert-danger"}) %>
			<div class="form-group">
				<label class="col-sm-4 control-label" for="Email"><%: Html.SnippetLiteral("Account/SignUp/EmailLabel", "Provide a valid email") %></label>
				<div class="col-sm-8">
					<%= Html.TextBoxFor(model => model.Email, new { @class = "form-control" }) %>
				</div>
			</div>
			<div class="form-group">
				<div class="col-sm-offset-4 col-sm-8">
					<input id="submit-email-confirmation" type="submit" class="btn btn-primary" value="<%: Html.SnippetLiteral("Account/SignUp/EmailConfirmationFormButtonText", "Submit") %>"/>
				</div>
			</div>
		</fieldset>
	</div>
<% } %>
<script type="text/javascript">
	$(function() {
		$("#submit-email-confirmation").click(function () {
			$.blockUI({ message: null, overlayCSS: { opacity: .3 } });
		});
	});
</script>
