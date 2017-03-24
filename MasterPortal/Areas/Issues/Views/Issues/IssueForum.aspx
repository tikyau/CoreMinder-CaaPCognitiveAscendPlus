﻿<%@ Page Language="C#" MasterPageFile="../Shared/Issues.master" Inherits="System.Web.Mvc.ViewPage<Site.Areas.Issues.ViewModels.IssueForumViewModel>" %>
<%@ OutputCache CacheProfile="User" %>
<%@ Import Namespace="Adxstudio.Xrm.Issues" %>
<%@ Import Namespace="Site.Helpers" %>

<asp:Content runat="server" ContentPlaceHolderID="Title"><%: Model.IssueForum.Title %></asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="PageHeader">
	<ul class="breadcrumb">
		<% Html.RenderPartial("SiteMapPath"); %>
		<li class="active"><%: Model.IssueForum.Title %></li>
	</ul>
	<div class="page-header">
		<h1><%: Model.IssueForum.Title %></h1>
	</div>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="MainContent">
	<div class="bottom-break"><%= Model.IssueForum.Summary %></div>
	<ul class="issues-nav nav nav-tabs">
		<li id="open-filter" class="dropdown <%= (RouteData.Values["filter"] ?? "open") as string == "open" ? "active" : string.Empty %>">
			<a class="dropdown-toggle" data-toggle="dropdown" href="#">open<%= (RouteData.Values["filter"] ?? "open") as string == "open" && (RouteData.Values["status"] ?? "all") as string != "all" ? ": " + RouteData.Values["status"] : string.Empty%> <b class="caret"></b></a>
			<ul class="dropdown-menu">
				<li><%: Html.ActionLink("all-open", "Filter", new { status = "all", filter = "open", priority = RouteData.Values["priority"] })%></li>
				<li><%: Html.ActionLink("new-or-unconfirmed", "Filter", new { status = "new-or-unconfirmed", filter = "open", priority = RouteData.Values["priority"] })%></li>
				<li><%: Html.ActionLink("confirmed", "Filter", new { status = "confirmed", filter = "open", priority = RouteData.Values["priority"] })%></li>
				<li><%: Html.ActionLink("workaround-available", "Filter", new { status = "workaround-available", filter = "open", priority = RouteData.Values["priority"] })%></li>
			</ul>
		</li>
		<li id="closed-filter" class="dropdown <%= RouteData.Values["filter"] as string == "closed" ? "active" : string.Empty %>">
			<a class="dropdown-toggle" data-toggle="dropdown" href="#">closed<%= (RouteData.Values["filter"] ?? "closed") as string == "closed" && (RouteData.Values["status"] ?? "all") as string != "all" ? ": " + RouteData.Values["status"] : string.Empty%> <b class="caret"></b></a>
			<ul class="dropdown-menu">
				<li><%: Html.ActionLink("all-closed", "Filter", new { status = "all", filter = "closed", priority = RouteData.Values["priority"] })%></li>
				<li><%: Html.ActionLink("resolved", "Filter", new { status = "resolved", filter = "closed", priority = RouteData.Values["priority"] })%></li>
				<li><%: Html.ActionLink("will-not-fix", "Filter", new { status = "will-not-fix", filter = "closed", priority = RouteData.Values["priority"] })%></li>
				<li><%: Html.ActionLink("by-design", "Filter", new { status = "by-design", filter = "closed", priority = RouteData.Values["priority"] })%></li>
				<li><%: Html.ActionLink("unable-to-reproduce", "Filter", new { status = "unable-to-reproduce", filter = "closed", priority = RouteData.Values["priority"] })%></li>
			</ul>
		</li>
		<li <%= RouteData.Values["filter"] as string == "all" ? @"class=""active""" : string.Empty %>>
			<%: Html.ActionLink("all", "Filter", new { filter = "all", status = "all", priority = RouteData.Values["priority"] })%>
		</li>
		<li id="priority-filter" class="dropdown">
			<a class="dropdown-toggle" data-toggle="dropdown" href="#">priority: <%: RouteData.Values["priority"] ?? "any" %> <b class="caret"></b></a>
			<ul class="dropdown-menu">
				<li><%: Html.ActionLink("any", "Filter", new { priority = "any" })%></li>
				<li><%: Html.ActionLink("low", "Filter", new { priority = "low" })%></li>
				<li><%: Html.ActionLink("medium", "Filter", new { priority = "medium" })%></li>
				<li><%: Html.ActionLink("high", "Filter", new { priority = "high" })%></li>
				<li><%: Html.ActionLink("critical", "Filter", new { priority = "critical" })%></li>
			</ul>
		</li>
	</ul>
	<ul id="issues" class="list-unstyled clearfix">
		<% foreach (var issue in Model.Issues) { %>
			<li>
				<div class="issue-container">
					<h3><%: Html.ActionLink(issue.Title, "Issues", "Issues", new { issueForumPartialUrl = issue.IssueForumPartialUrl, issuePartialUrl = issue.PartialUrl }, null) %></h3>
					<p>
						<% if (issue.AuthorId.HasValue) { %>
							<a href="<%= Url.AuthorUrl(issue) %>"><%= issue.AuthorName%></a>
						<% } else { %>
							<%: issue.AuthorName %>
						<% } %>
						&ndash; <abbr class="timeago"><%: issue.SubmittedOn.ToString("r") %></abbr>
						&ndash;<% Html.RenderPartial("IssueStatus", issue); %>
						&ndash; <a href="<%: Url.Action("Issues", "Issues", new { issueForumPartialUrl = issue.IssueForumPartialUrl, issuePartialUrl = issue.PartialUrl }) %>#comments">
							<span class="fa fa-comment" aria-hidden="true"></span> <%: issue.CommentCount %></a>
					</p>
					<%= issue.Copy %>
				</div>
			</li>
		<% } %>
	</ul>
	<% Html.RenderPartial("Pagination", Model.Issues); %>
	<% if (Model.IssueForum.CurrentUserCanSubmitIssues) { %>
		<div id="create-issue">
			<% Html.RenderPartial("CreateIssue", Model.IssueForum); %>
		</div>
	<% } %>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SideBarContent">
	<% if (Model.IssueForum.IssueSubmissionPolicy == IssueForumIssueSubmissionPolicy.OpenToAuthenticatedUsers && !Request.IsAuthenticated) { %>
		<div class="section">
			<div class="alert alert-block alert-info"><%: Html.SnippetLiteral("issue-forum/sign-in-message", "Please sign in to provide all types of feedback in this issue forum.")%></div>
		</div>
	<% } %>
</asp:Content>
