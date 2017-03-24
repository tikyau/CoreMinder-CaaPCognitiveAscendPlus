<%@ Page Language="C#" MasterPageFile="../Shared/Account.master" Inherits="System.Web.Mvc.ViewPage<Site.Areas.Account.ViewModels.RedeemViewModel>" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="Adxstudio.Xrm.Web.Mvc.Html" %>

<asp:Content runat="server" ContentPlaceHolderID="MainContent">
	<div class="validation-summary">
		<% if (!string.IsNullOrWhiteSpace(ViewBag.ResultCode as string)) { %>
			<% if (string.Equals(ViewBag.ResultCode, "confirm")) { %>
				<div class="alert alert-success"><%: Html.SnippetLiteral("Account/Redeem/ConfirmMessage", "Please check your email for a confirmation link or an invitation code.") %></div>
			<% } else if (string.Equals(ViewBag.ResultCode, "unregistered")) { %>
				<div class="alert alert-info"><%: Html.SnippetLiteral("Account/Redeem/UnregisteredMessage", "This account is not eligible for transfer. Please sign in with a registered Windows Live ID account.") %></div>
			<% } else if (string.Equals(ViewBag.ResultCode, "inactive")) { %>
				<div class="alert alert-danger"><%: Html.SnippetLiteral("Account/Redeem/InactiveMessage", "This account is pending registration.") %></div>
			<% } else if (string.Equals(ViewBag.ResultCode, "invalid-invitation-code")) { %>
				<div class="alert alert-danger"><%: Html.SnippetLiteral("Account/Redeem/InvalidMessage", "The invitation code is invalid. Please enter the code and try again.") %></div>
			<% } %>
		<% } %>
		<% if (Model.Redeemed) { %>
			<span class="alert alert-success"><%: Html.SnippetLiteral("Account/Redeem/RedeemedMessage", "The invitation has been redeemed successfully.") %></span>
		<% } %> 
	</div>
	<% if (!Model.Redeemed) { %>
		<% using (Html.BeginForm("RedeemInvitationCode", "Account", ViewBag.ReturnUrlRouteValues as RouteValueDictionary)) { %>
			<%= Html.AntiForgeryToken() %>
			<%= Html.HiddenFor(model => model.wa) %>
			<%= Html.HiddenFor(model => model.wresult) %>
			<%= Html.HiddenFor(model => model.openauthresult) %>
			<div class="form-horizontal">
				<fieldset>
					<legend><%: Html.SnippetLiteral("Account/Redeem/InvitationCodeFormHeading", "Sign up with an invitation code") %></legend>
					<%= Html.ValidationSummary(string.Empty, new { @class = "alert alert-block alert-danger" }) %>
					<div class="form-group">
						<label class="col-sm-2 control-label required" for="InvitationCode"><%: Html.SnippetLiteral("Account/Redeem/InvitationCodeLabel", "Invitation Code") %></label>
						<div class="col-sm-10">
							<%= Html.TextBoxFor(model => model.InvitationCode, new { @class = "form-control" }) %>
						</div>
					</div>
					<div class="form-group">
						<div class="col-sm-offset-2 col-sm-10">
							<input id="submit-invitation-code" type="submit" class="btn btn-primary" value="<%: Html.SnippetLiteral("Account/Redeem/InvitationCodeFormButtonText", "Submit") %>"/>
						</div>
					</div>
				</fieldset>
			</div>
		<% } %>
		<script type="text/javascript">
			$(function() {
				$("#submit-invitation-code").click(function () {
					$.blockUI({ message: null, overlayCSS: { opacity: .3 } });
				});
			});
		</script>
	<% } %>
</asp:Content>
