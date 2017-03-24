<%@ Page Language="C#" MasterPageFile="../Shared/Account.Master" Inherits="System.Web.Mvc.ViewPage<Site.Areas.Account.ViewModels.RedeemViewModel>" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>

<asp:Content runat="server" ContentPlaceHolderID="MainContent">
	<% if (!string.IsNullOrEmpty(Model.InvitationCode)) { %>
		<div class="alert alert-info"><%: Html.SnippetLiteral("Account/Redeem/InvitationCodeAlert", "Redeeming code:") %> <strong><%: Model.InvitationCode %></strong></div>
	<% } %>
	<div class="row">
		<% if (ViewBag.MembershipProviderEnabled) { %>
			<div class="col-md-6">
				<% Html.RenderPartial("RedeemLocal", Model); %>
			</div>
		<% } %>
		<% if (ViewBag.FederatedAuthEnabled) { %>
			<div class="col-md-6">
				<% Html.RenderPartial("RedeemFederated", Model); %>
			</div>
		<% } %>
	</div>
</asp:Content>
