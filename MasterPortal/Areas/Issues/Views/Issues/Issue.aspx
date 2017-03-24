<%@ Page Language="C#" MasterPageFile="../Shared/Issues.master" Inherits="System.Web.Mvc.ViewPage<Site.Areas.Issues.ViewModels.IssueViewModel>" %>
<%@ OutputCache CacheProfile="User" %>
<%@ Import Namespace="Adxstudio.Xrm.Issues" %>
<%@ Import Namespace="Site.Helpers" %>

<asp:Content runat="server" ContentPlaceHolderID="Title"><%: Model.Issue.Title %></asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="PageHeader">
	<ul class="breadcrumb">
		<% Html.RenderPartial("SiteMapPath"); %>
		<li><%: Html.ActionLink(Model.IssueForum.Title, "Issues", new { issueForumPartialUrl = Model.IssueForum.PartialUrl, issuePartialUrl = string.Empty }) %></li>
		<li class="active"><%: Model.Issue.Title %></li>
	</ul>
	<div class="page-header">
		<h1><%: Model.Issue.Title %></h1>
	</div>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="MainContent">
	<div class="issue-container">
		<p>
		<% if (Model.Issue.AuthorId.HasValue) { %>
			<a href="<%= Url.AuthorUrl(Model.Issue) %>"><%= Model.Issue.AuthorName%></a>
		<% } else { %>
			<%: Model.Issue.AuthorName%>
		<% } %>
		&ndash; <abbr class="timeago"><%: Model.Issue.SubmittedOn.ToString("r") %></abbr>
		&ndash;<% Html.RenderPartial("IssueStatus", Model.Issue); %></p>
		<%= Model.Issue.Copy %>
		<% if (!string.IsNullOrWhiteSpace(Model.Issue.StatusComment)) { %>
			<h3><%: Html.SnippetLiteral("Issue Status Comment Label", "Status Details")%></h3>
			<%= Model.Issue.StatusComment %>
		<% } %>
	</div>
	<div id="comments">
		<% Html.RenderPartial("Comments", Model.Comments); %>
	</div>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SideBarContent">
	<% if ((Model.Issue.CommentPolicy == IssueForumCommentPolicy.OpenToAuthenticatedUsers) && !Request.IsAuthenticated) { %>
		<div class="section">
			<div class="alert alert-block alert-info"><%: Html.SnippetLiteral("issue/sign-in-message", "Please sign in to provide all types of feedback for this issue.")%></div>
		</div>
	<% } else { %>
		<div class="section">
			<div id="issue-tracking">
				<% Html.RenderPartial("Tracking", Model); %>
			</div>
		</div>
	<% } %>
</asp:Content>
