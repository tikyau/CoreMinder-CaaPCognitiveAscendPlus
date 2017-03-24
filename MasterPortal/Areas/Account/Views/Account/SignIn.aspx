<%@ Page Language="C#" MasterPageFile="../Shared/Account.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
	<% if (ViewBag.RegistrationEnabled) { %>
		<div class="well">
			Have access to an invitation code? Redeem it <a href="<%= Url.RedeemUrl(string.Empty) %>">here</a>.
			<% if (ViewBag.OpenRegistrationEnabled) { %>
				Otherwise, <a href="<%= Url.RegisterUrl(string.Empty) %>">sign up</a> for a new account.
			<% } %>
		</div>
	<% } %>
	<div class="row">
		<% if (ViewBag.MembershipProviderEnabled) { %>
			<div class="col-md-6">
				<% Html.RenderPartial("SignInLocal", ViewData["local"]); %>
			</div>
		<% } %>
		<% if (ViewBag.FederatedAuthEnabled) { %>
			<div class="col-md-6">
				<% Html.RenderPartial("SignInFederated", ViewData["federated"]); %>
			</div>
		<% } %>
	</div>
</asp:Content>
