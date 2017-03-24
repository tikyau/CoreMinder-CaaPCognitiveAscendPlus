<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<Microsoft.Owin.Security.AuthenticationDescription>>" %>

<% if (Model != null) { %>
	<% foreach (var provider in Model) { %>
		<button
			name="provider" type="submit" class="btn btn-primary btn-line"
			id="<%: provider.AuthenticationType %>"
			title="<%: string.Format(Html.SnippetLiteral("Account/SignIn/IdentityProviderTitle", "Sign in with your {0} account."), provider.Caption) %>"
			value="<%: provider.AuthenticationType %>"><%: provider.Caption %>
		</button>
	<% } %>
<% } %>
