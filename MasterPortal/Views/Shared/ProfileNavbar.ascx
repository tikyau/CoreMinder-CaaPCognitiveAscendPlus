<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<% Func<string, bool> isCurrentAction = action => string.Equals(action, ViewContext.RouteData.Values["action"] as string, StringComparison.Ordinal); %>

<div class="panel panel-default nav-profile">
	<div class="panel-heading">
		<h3 class="panel-title"><span class="fa fa-lock" aria-hidden="true"></span> <%: Html.TextSnippet("Profile/SecurityNav/Title", defaultValue: "Security", tagName: "span") %></h3>
	</div>
	<div class="list-group nav-profile">
		<% if (ViewBag.Settings.LocalLoginEnabled) { %>
			<% if (ViewBag.Nav.HasPassword) { %>
				<a class="list-group-item <%: isCurrentAction("ChangePassword") ? "active" : string.Empty %>" href="<%: Url.Action("ChangePassword", "Manage", new { area = "Account" })%>">
					<%: Html.TextSnippet("Profile/SecurityNav/ChangePassword", defaultValue: "Change Password", tagName: "span") %>
				</a>
			<% } else { %>
				<a class="list-group-item <%: isCurrentAction("SetPassword") ? "active" : string.Empty %>" href="<%: Url.Action("SetPassword", "Manage", new { area = "Account" })%>">
					<%: Html.TextSnippet("Profile/SecurityNav/SetPassword", defaultValue: "Set Password", tagName: "span") %>
				</a>
			<% } %>
		<% } %>
		<a class="list-group-item <%: isCurrentAction("ChangeEmail") ? "active" : string.Empty %>" href="<%: Url.Action("ChangeEmail", "Manage", new { area = "Account" })%>">
			<% if (ViewBag.Settings.EmailConfirmationEnabled && !ViewBag.Nav.IsEmailConfirmed) { %>
				<span class="fa fa-exclamation-circle pull-right profile-alert" title="<%: Html.SnippetLiteral("Profile/SecurityNav/Unconfirmed", "Unconfirmed") %>"></span>
			<% } %>
			<%: Html.TextSnippet("Profile/SecurityNav/ChangeEmail", defaultValue: "Change Email", tagName: "span") %>
		</a>
		<% if (ViewBag.Settings.MobilePhoneEnabled) { %>
			<a class="list-group-item <%: isCurrentAction("ChangePhoneNumber") ? "active" : string.Empty %>" href="<%: Url.Action("ChangePhoneNumber", "Manage", new { area = "Account" })%>">
				<% if (!ViewBag.Nav.IsMobilePhoneConfirmed) { %>
					<span class="fa fa-exclamation-circle pull-right profile-alert" title="<%: Html.SnippetLiteral("Profile/SecurityNav/Unconfirmed", "Unconfirmed") %>"></span>
				<% } %>
				<%: Html.TextSnippet("Profile/SecurityNav/ChangeMobilePhone", defaultValue: "Change Mobile Phone", tagName: "span") %>
			</a>
		<% } %>
		<% if (ViewBag.Settings.TwoFactorEnabled) { %>
			<a class="list-group-item <%: isCurrentAction("ChangeTwoFactor") ? "active" : string.Empty %>" href="<%: Url.Action("ChangeTwoFactor", "Manage", new { area = "Account" })%>">
				<% if (ViewBag.Nav.IsTwoFactorEnabled && !ViewBag.Nav.IsEmailConfirmed) { %>
					<span class="fa fa-exclamation-circle pull-right profile-alert" title="<%: Html.SnippetLiteral("Profile/SecurityNav/Pending", "Pending") %>"></span>
				<% } %>
				<%: Html.TextSnippet("Profile/SecurityNav/ChangeTwoFactor", defaultValue: "Change Two-Factor Authentication", tagName: "span") %>
			</a>
		<% } %>
		<% if (ViewBag.Settings.ExternalLoginEnabled) { %>
			<a class="list-group-item <%: isCurrentAction("ChangeLogin") ? "active" : string.Empty %>" href="<%: Url.Action("ChangeLogin", "Manage", new { area = "Account" })%>">
				<%: Html.TextSnippet("Profile/SecurityNav/ChangeLogin", defaultValue: "Manage External Authentication", tagName: "span") %>
			</a>
		<% } %>
	</div>
</div>