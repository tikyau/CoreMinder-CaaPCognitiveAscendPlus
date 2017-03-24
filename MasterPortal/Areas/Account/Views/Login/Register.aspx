<%@ Page Title="" Language="C#" MasterPageFile="Account.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ContentPlaceHolderID="AccountNavBar" runat="server">
	<% Html.RenderPartial("AccountNavBar", new ViewDataDictionary(ViewData) { { "SubArea", "Register" } }); %>
</asp:Content>

<asp:Content ContentPlaceHolderID="PageCopy" runat="server">
	<%: Html.HtmlSnippet("Account/Register/PageCopy", "page-copy") %>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
	<% if (ViewBag.Settings.RegistrationEnabled && (ViewBag.Settings.OpenRegistrationEnabled || ViewBag.Settings.InvitationEnabled)) { %>
		<% if (!string.IsNullOrWhiteSpace(ViewBag.InvitationCode)) { %>
			<div class="alert alert-info"><%: Html.SnippetLiteral("Account/Redeem/InvitationCodeAlert", "Redeeming code:") %> <strong><%: ViewBag.InvitationCode %></strong></div>
		<% } %>
		<div class="row">
			<% if (ViewBag.Settings.LocalLoginEnabled) { %>
				<div class="col-md-6">
					<% Html.RenderPartial("RegisterLocal", ViewData["local"]); %>
				</div>
			<% } %>
			<% if (ViewBag.Settings.ExternalLoginEnabled) { %>
				<div class="col-md-6">
					<% Html.RenderPartial("RegisterExternal", ViewData["external"]); %>
				</div>
			<% } %>
		</div>
	<% } %>
</asp:Content>