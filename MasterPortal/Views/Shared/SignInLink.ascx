<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<% var signInUrl = Url.SignInUrl((string) ViewBag.ReturnUrl); %>
<% if (!string.IsNullOrWhiteSpace(signInUrl)) { %>
	<a class="<%: (string) ViewBag.Class %>" href="<%: signInUrl %>">
		<span class="fa fa-sign-in" aria-hidden="true"></span>
		<%: Html.SnippetLiteral("links/login", "Sign In") %>
	</a>
<% } %>