<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Site.Areas.Account.ViewModels.ISignInFederatedViewModel>" %>

<div id="external-provider-list">
	<% if (Model.IdentityProviders != null) { %>
		<% foreach (var provider in Model.IdentityProviders) { %>
			<a class="btn btn-primary btn-line" title="Sign in with your <%: provider.Name %> account." href="<%: provider.SignInUrl %>"><%: provider.Name %></a>
		<% } %>
	<% } %>
</div>

<% if (!string.IsNullOrWhiteSpace(Model.HomeRealmDiscoveryMetadataFeedUrl)) { %>
	<script type="text/javascript">
		function ShowSignInPage(providers) {
			for (var key in providers) {
				var provider = providers[key];
				var link = $('<a/>')
					.addClass('btn btn-primary btn-line')
					.attr('title', 'Sign in with your ' + provider.Name + ' account.')
					.attr('href', provider.LoginUrl)
					.text(provider.Name);
				$('#external-provider-list').append(link).append(' ');
			}
		}
	</script>
	<script src="<%: Model.HomeRealmDiscoveryMetadataFeedUrl %>" type="text/javascript"></script>
<% } %>