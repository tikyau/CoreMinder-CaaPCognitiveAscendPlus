<%@ Page Title="" Language="C#" MasterPageFile="../Shared/Account.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
	<div class="well">
		Have access to an invitation code? Redeem it <a href="<%= Url.RedeemUrl(string.Empty) %>">here</a>.
	</div>
	<% if (ViewBag.RequiresConfirmation) { %>
		<% Html.RenderPartial("SignUpByEmail", ViewData["confirmation"]); %>
	<% } else if (ViewBag.OpenRegistrationEnabled) { %>
		<div class="row">
			<% if (ViewBag.MembershipProviderEnabled) { %>
				<div class="col-md-6">
					<% Html.RenderPartial("SignUpLocal", ViewData["local"]); %>
				</div>
			<% } %>
			<% if (ViewBag.FederatedAuthEnabled) { %>
				<div class="col-md-6">
					<% Html.RenderPartial("SignUpFederated", ViewData["federated"]); %>
				</div>
			<% } %>
		</div>
	<% } %>
</asp:Content>