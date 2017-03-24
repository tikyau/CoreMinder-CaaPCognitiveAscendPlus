<%@ Page Title="" Language="C#" MasterPageFile="Account.Master" Inherits="System.Web.Mvc.ViewPage<Site.Areas.Account.ViewModels.ForgotPasswordViewModel>" %>

<asp:Content ContentPlaceHolderID="PageCopy" runat="server">
	<%: Html.HtmlSnippet("Account/ForgotPassword/PageCopy", "page-copy") %>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
	<% using (Html.BeginForm("ForgotPassword", "Login")) { %>
		<%: Html.AntiForgeryToken() %>
		<div class="form-horizontal">
			<fieldset>
				<legend><%: Html.TextSnippet("Account/PasswordReset/ForgotPasswordFormHeading", defaultValue: "Forgot your password?", tagName: "span") %></legend>
				<%: Html.ValidationSummary(string.Empty, new {@class = "alert alert-block alert-danger"}) %>
				<div class="form-group">
					<label class="col-sm-2 control-label" for="Email"><%: Html.TextSnippet("Account/PasswordReset/EmailLabel", defaultValue: "Email", tagName: "span") %></label>
					<div class="col-sm-10">
						<%: Html.TextBoxFor(model => model.Email, new { @class = "form-control" }) %>
						<p class="help-block"><%: Html.TextSnippet("Account/PasswordReset/EmailInstructionsText", defaultValue: "Enter your email address to request a password reset.", tagName: "span") %></p>
					</div>
				</div>
				<div class="form-group">
					<div class="col-sm-offset-2 col-sm-10">
						<button id="submit-forgot-password" class="btn btn-primary"><%: Html.SnippetLiteral("Account/PasswordReset/ForgotPasswordButtonText", "Send") %></button>
					</div>
				</div>
			</fieldset>
		</div>
	<% } %>
	<script type="text/javascript">
		$(function() {
			$("#submit-forgot-password").click(function () {
				$.blockUI({ message: null, overlayCSS: { opacity: .3 } });
			});
		});
	</script>
</asp:Content>
