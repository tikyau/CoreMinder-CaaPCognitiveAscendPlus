<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<% var signInUrl = Url.FacebookSignInUrl(); %>
<% if (!string.IsNullOrWhiteSpace(signInUrl)) { %>
	<a class="facebook-signin" href="<%: signInUrl %>">
		<span class="fa fa-sign-in" aria-hidden="true"></span>
		<%: Html.SnippetLiteral("links/login", "Sign In") %>
	</a>
<% } %>