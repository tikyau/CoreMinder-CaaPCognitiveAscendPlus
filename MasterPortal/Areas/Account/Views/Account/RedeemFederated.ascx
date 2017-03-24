<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Site.Areas.Account.ViewModels.RedeemViewModel>" %>

<% using (Html.BeginForm("RedeemFederated", "Account", ViewBag.ReturnUrlRouteValues as RouteValueDictionary)) { %>
	<%= Html.AntiForgeryToken() %>
	<%= Html.HiddenFor(model => model.InvitationCode) %>
	<%= Html.HiddenFor(model => model.ChallengeAnswer) %>
	<div class="form-horizontal">
		<fieldset>
			<legend><%: Html.SnippetLiteral("Account/Redeem/FederatedSignUpFormHeading", "Sign up with an identity provider") %></legend>
			<% Html.RenderPartial("IdentityProviders", Model); %>
		</fieldset>
	</div>
<% } %>
