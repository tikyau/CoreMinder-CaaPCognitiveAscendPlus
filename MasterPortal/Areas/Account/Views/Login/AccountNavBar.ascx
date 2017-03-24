<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% if (ViewBag.Settings.RegistrationEnabled && (ViewBag.Settings.OpenRegistrationEnabled || ViewBag.Settings.InvitationEnabled)) { %>
	<ul class="nav nav-tabs nav-account" role="navigation">
		<li class="<%: ViewBag.SubArea == "SignIn" ? "active" : string.Empty %>"><a href="<%: Url.SignInUrl(string.Empty) %>"><span class="fa fa-sign-in" aria-hidden="true"></span> <%: Html.SnippetLiteral("Account/Nav/SignIn", "Sign In") %></a></li>
		<% if (ViewBag.Settings.OpenRegistrationEnabled) { %>
			<li class="<%: ViewBag.SubArea == "Register" ? "active" : string.Empty %>"><a href="<%: Url.RegisterUrl(string.Empty) %>"><%: Html.SnippetLiteral("Account/Nav/Register", "Register") %></a></li>
		<% } %>
		<% if (ViewBag.Settings.InvitationEnabled) { %>
		<li class="<%: ViewBag.SubArea == "Redeem" ? "active" : string.Empty %>"><a href="<%: Url.RedeemUrl(string.Empty) %>"><%: Html.SnippetLiteral("Account/Nav/Redeem", "Redeem Invitation") %></a></li>
		<% } %>
	</ul>
<% } %>