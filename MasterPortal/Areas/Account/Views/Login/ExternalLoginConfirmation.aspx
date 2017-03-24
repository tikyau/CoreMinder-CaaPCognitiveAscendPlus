<%@ Page Title="" Language="C#" MasterPageFile="Account.Master" Inherits="System.Web.Mvc.ViewPage<Site.Areas.Account.ViewModels.ExternalLoginConfirmationViewModel>" %>

<asp:Content ContentPlaceHolderID="PageCopy" runat="server">
	<%: Html.HtmlSnippet("Account/ExternalLoginConfirmation/PageCopy", "page-copy") %>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
	<% using (Html.BeginForm("ExternalLoginConfirmation", "Login", new { ReturnUrl = ViewBag.ReturnUrl, InvitationCode = ViewBag.InvitationCode })) { %>
		<%: Html.AntiForgeryToken() %>
		<div class="form-horizontal">
			<fieldset>
				<legend><%: Html.TextSnippet("Account/Register/AssociateFormHeading", defaultValue: "Register your external account", tagName: "span") %></legend>
				<%: Html.ValidationSummary(string.Empty, new {@class = "alert alert-block alert-danger"}) %>
				<% if (!string.IsNullOrWhiteSpace(ViewBag.InvitationCode)) { %>
					<div class="form-group">
						<label class="col-sm-2 control-label" for="InvitationCode"><%: Html.TextSnippet("Account/Redeem/InvitationCodeLabel", defaultValue: "Invitation Code", tagName: "span") %></label>
						<div class="col-sm-10">
							<%: Html.TextBox("InvitationCode", (string) ViewBag.InvitationCode, new { @class = "form-control", @readonly = "readonly" }) %>
						</div>
					</div>
				<% } %>
				<div class="form-group">
					<label class="col-sm-2 control-label" for="Email"><%: Html.TextSnippet("Account/Register/EmailLabel", defaultValue: "Email") %></label>
					<div class="col-sm-10">
						<%: Html.TextBoxFor(model => model.Email, new { @class = "form-control" }) %>
						<p class="help-block"><%: Html.TextSnippet("Account/Register/EmailInstructionsText", defaultValue: "Provide an email address to complete the external account registration.", tagName: "span") %></p>
					</div>
				</div>
				<div class="form-group">
					<div class="col-sm-offset-2 col-sm-10">
						<button class="btn btn-primary"><%: Html.SnippetLiteral("Account/Register/RegisterButtonText", "Register") %></button>
					</div>
				</div>
			</fieldset>
		</div>
	<% } %>
</asp:Content>
