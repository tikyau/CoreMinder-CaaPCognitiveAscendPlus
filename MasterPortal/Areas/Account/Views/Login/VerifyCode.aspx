﻿<%@ Page Language="C#" MasterPageFile="Account.Master" Inherits="System.Web.Mvc.ViewPage<Site.Areas.Account.ViewModels.VerifyCodeViewModel>" %>

<asp:Content ContentPlaceHolderID="PageCopy" runat="server">
	<%: Html.HtmlSnippet("Account/VerifyCode/PageCopy", "page-copy") %>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
	<% using (Html.BeginForm("VerifyCode", "Login", new { ReturnUrl = Model.ReturnUrl, InvitationCode = Model.InvitationCode })) { %>
		<%: Html.AntiForgeryToken() %>
		<%: Html.Hidden("provider", Model.Provider) %>
		<%: Html.Hidden("rememberMe", Model.RememberMe) %>
		<div class="form-horizontal">
			<fieldset>
				<legend><%: Html.TextSnippet("Account/SignIn/VerifyCodeFormHeading", defaultValue: "Enter Security Code", tagName: "span") %></legend>
				<%: Html.ValidationSummary(string.Empty, new {@class = "alert alert-block alert-danger"}) %>
				<div class="form-group">
					<label class="col-sm-4 control-label" for="Code"><%: Html.TextSnippet("Account/SignIn/VerifyCodeLabel", defaultValue: "Code", tagName: "span") %></label>
					<div class="col-sm-8">
						<%: Html.TextBoxFor(model => model.Code, new { @class = "form-control" }) %>
						<% if (Model.Provider == "EmailCode") { %>
							<p class="help-block"><%: Html.TextSnippet("Account/SignIn/VerifyCodeFromEmailText", defaultValue: "Please check your email for the security code.", tagName: "span") %></p>
						<% } else if (Model.Provider == "PhoneCode") { %>
							<p class="help-block"><%: Html.TextSnippet("Account/SignIn/VerifyCodeFromPhoneText", defaultValue: "Please check your phone for the security code.", tagName: "span") %></p>
						<% } %>
					</div>
				</div>
				<% if (ViewBag.Settings.TwoFactorEnabled && ViewBag.Settings.RememberBrowserEnabled) { %>
					<div class="form-group">
						<div class="col-sm-offset-4 col-sm-8">
							<div class="checkbox">
								<label>
									<%: Html.CheckBoxFor(model => model.RememberBrowser) %>
									<%: Html.TextSnippet("Account/SignIn/RememberBrowserLabel", defaultValue: "Remember this browser?", tagName: "span") %>
								</label>
							</div>
						</div>
					</div>
				<% } %>
				<div class="form-group">
					<div class="col-sm-offset-4 col-sm-8">
						<button id="submit-verify-code" class="btn btn-primary"><%: Html.SnippetLiteral("Account/SignIn/VerifyCodeButtonText", "Verify") %></button>
					</div>
				</div>
			</fieldset>
		</div>
	<% } %>
	<script type="text/javascript">
		$(function () {
			$("#submit-verify-code").click(function () {
				$.blockUI({ message: null, overlayCSS: { opacity: .3 } });
			});
		});
	</script>
	<% if (ViewBag.Settings.IsDemoMode && ViewBag.DemoModeCode != null) { %>
		<div class="panel panel-warning">
			<div class="panel-heading">
				<h3 class="panel-title"><span class="fa fa-wrench" aria-hidden="true"></span> DEMO MODE <small>LOCAL ONLY</small></h3>
			</div>
			<div class="panel-body">
				<%: ViewBag.DemoModeCode %>
			</div>
		</div>
	<% } %>
</asp:Content>
