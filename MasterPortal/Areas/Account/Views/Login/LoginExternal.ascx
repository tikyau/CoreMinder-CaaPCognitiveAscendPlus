<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<Microsoft.Owin.Security.AuthenticationDescription>>" %>

<% using (Html.BeginForm("ExternalLogin", "Login", new { ReturnUrl = ViewBag.ReturnUrl, InvitationCode = ViewBag.InvitationCode })) { %>
	<%: Html.AntiForgeryToken() %>
	<div class="form-horizontal">
		<fieldset>
			<legend><%: Html.TextSnippet("Account/SignIn/SignInExternalFormHeading", defaultValue: "Sign in with an external account", tagName: "span") %></legend>
			<% Html.RenderPartial("LoginProvider", Model); %>
		</fieldset>
	</div>
<% } %>
