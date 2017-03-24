<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<% Func<string, bool> isCurrentAction = action => string.Equals(action, ViewContext.RouteData.Values["action"] as string, StringComparison.Ordinal); %>

<div class="panel panel-default nav-profile">
	<div class="panel-heading">
		<h3 class="panel-title"><span class="fa fa-lock" aria-hidden="true"></span> <%: Html.TextSnippet("Profile/SecurityNav/Title", defaultValue: "Security", tagName: "span") %></h3>
	</div>
	<div class="list-group nav-profile">
		<a class="list-group-item <%: isCurrentAction("ChangeEmail") ? "active" : string.Empty %>" href="<%: Url.Action("ChangeEmail", "Manage", new { area = "Account" })%>">
			<% if (!ViewBag.Nav.IsEmailConfirmed) { %>
				<span class="fa fa-exclamation-circle pull-right profile-alert" title="<%: Html.SnippetLiteral("Profile/SecurityNav/Unconfirmed", "Unconfirmed") %>"></span>
			<% } %>
			<%: Html.TextSnippet("Profile/SecurityNav/ChangeEmail", defaultValue: "Change Email", tagName: "span") %>
		</a>
		<a class="list-group-item <%: isCurrentAction("ChangePhoneNumber") ? "active" : string.Empty %>" href="<%: Url.Action("ChangePhoneNumber", "Manage", new { area = "Account" })%>">
			<% if (!ViewBag.Nav.IsMobilePhoneConfirmed) { %>
				<span class="fa fa-exclamation-circle pull-right profile-alert" title="<%: Html.SnippetLiteral("Profile/SecurityNav/Unconfirmed", "Unconfirmed") %>"></span>
			<% } %>
			<%: Html.TextSnippet("Profile/SecurityNav/ChangeMobilePhone", defaultValue: "Change Mobile Phone", tagName: "span") %>
		</a>
	</div>
</div>