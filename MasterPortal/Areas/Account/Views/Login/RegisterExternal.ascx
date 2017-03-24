<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<Microsoft.Owin.Security.AuthenticationDescription>>" %>

<% using (Html.BeginForm("ExternalLogin", "Login", new { area = "Account", ReturnUrl = ViewBag.ReturnUrl, InvitationCode = ViewBag.InvitationCode })) { %>
	<%: Html.AntiForgeryToken() %>
	<div class="form-horizontal">
		<fieldset>
			<legend><%: Html.TextSnippet("Account/Register/RegisterExternalFormHeading", defaultValue: "Register using an external account", tagName: "span") %></legend>
			<% Html.RenderPartial("LoginProvider", Model); %>
		</fieldset>
	</div>
<% } %>
