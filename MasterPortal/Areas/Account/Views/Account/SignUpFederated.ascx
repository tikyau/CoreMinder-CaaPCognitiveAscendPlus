<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Site.Areas.Account.ViewModels.SignInFederatedViewModel>" %>

<div class="form-horizontal">
	<fieldset>
		<legend><%: Html.SnippetLiteral("Account/SignUp/FederatedSignUpFormHeading", "Sign up with an identity provider") %></legend>
		<% Html.RenderPartial("IdentityProviders", Model); %>
	</fieldset>
</div>
