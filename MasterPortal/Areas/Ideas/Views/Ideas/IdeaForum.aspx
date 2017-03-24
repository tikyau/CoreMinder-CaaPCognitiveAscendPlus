<%@ Page Language="C#" MasterPageFile="../Shared/Ideas.master" Inherits="System.Web.Mvc.ViewPage<Site.Areas.Ideas.ViewModels.IdeaForumViewModel>" %>
<%@ OutputCache CacheProfile="User" %>
<%@ Import Namespace="Adxstudio.Xrm.Ideas" %>
<%@ Import Namespace="Site.Helpers" %>

<asp:Content runat="server" ContentPlaceHolderID="Title"><%: Model.IdeaForum.Title %></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="PageHeader">
	<ul class="breadcrumb">
		<% Html.RenderPartial("SiteMapPath"); %>
		<li class="active"><%: Model.IdeaForum.Title %></li>
	</ul>
	<div class="page-header">
		<h1><%: Model.IdeaForum.Title %></h1>
	</div>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="MainContent">
	<div class="bottom-break"><%= Model.IdeaForum.Summary %></div>
	<ul class="ideas-nav nav nav-tabs">
		<li <%= (RouteData.Values["filter"] ?? "top") as string == "top" ? @"class=""active""" : string.Empty %>>
			<%: Html.ActionLink("top", "Filter", new { filter = "top", status = RouteData.Values["status"], timeSpan = RouteData.Values["timeSpan"] })%>
		</li>
		<li <%= RouteData.Values["filter"] as string == "hot" ? @"class=""active""" : string.Empty %>>
			<%: Html.ActionLink("hot", "Filter", new { filter = "hot", status = RouteData.Values["status"], timeSpan = RouteData.Values["timeSpan"] })%>
		</li>
		<li <%= RouteData.Values["filter"] as string == "new" ? @"class=""active""" : string.Empty %>>
			<%: Html.ActionLink("new", "Filter", new { filter = "new", status = RouteData.Values["status"], timeSpan = RouteData.Values["timeSpan"] })%>
		</li>
		<li id="time-span-filter" class="dropdown">
			<a class="dropdown-toggle" data-toggle="dropdown" href="#">from: <%: RouteData.Values["timeSpan"] ?? "all-time" %> <b class="caret"></b></a>
			<ul class="dropdown-menu">
				<li><%: Html.ActionLink("all-time", "Filter", new { timeSpan = "all-time", status = RouteData.Values["status"] })%></li>
				<li><%: Html.ActionLink("today", "Filter", new { timeSpan = "today", status = RouteData.Values["status"] })%></li>
				<li><%: Html.ActionLink("this-week", "Filter", new { timeSpan = "this-week", status = RouteData.Values["status"] })%></li>
				<li><%: Html.ActionLink("this-month", "Filter", new { timeSpan = "this-month", status = RouteData.Values["status"] })%></li>
				<li><%: Html.ActionLink("this-year", "Filter", new { timeSpan = "this-year", status = RouteData.Values["status"] })%></li>
			</ul>
		</li>
		<li id="status-filter" class="dropdown">
			<a class="dropdown-toggle" data-toggle="dropdown" href="#">status: <%: RouteData.Values["status"] ?? "new" %> <b class="caret"></b></a>
			<ul class="dropdown-menu">
				<li><%: Html.ActionLink("any", "Filter", new { status = "any" })%></li>
				<li><%: Html.ActionLink("new", "Filter", new { status = "new" })%></li>
				<li><%: Html.ActionLink("accepted", "Filter", new { status = "accepted" })%></li>
				<li><%: Html.ActionLink("completed", "Filter", new { status = "completed" })%></li>
				<li><%: Html.ActionLink("rejected", "Filter", new { status = "rejected" })%></li>
			</ul>
		</li>
	</ul>
	<ul id="ideas" class="list-unstyled clearfix">
		<% foreach (var idea in Model.Ideas) { %>
			<li>
				<div id="vote-status-<%: idea.Id %>">
					<% Html.RenderPartial("Votes", idea); %>
				</div>
				<div class="idea-container">
					<h3><%: Html.ActionLink(idea.Title, "Ideas", "Ideas", new { ideaForumPartialUrl = idea.IdeaForumPartialUrl, ideaPartialUrl = idea.PartialUrl }, null) %></h3>
					<p>
						<%: Html.SnippetLiteral("Idea Author Label", "Suggested by")%>
						<% if (idea.AuthorId.HasValue) { %>
							<a href="<%= Url.AuthorUrl(idea) %>"><%= idea.AuthorName %></a>
						<% } else { %>
							<%: idea.AuthorName %>
						<% } %>
						&ndash;<% Html.RenderPartial("IdeaStatus", idea); %>
						&ndash; <a href="<%: Url.Action("Ideas", "Ideas", new { ideaForumPartialUrl = idea.IdeaForumPartialUrl, ideaPartialUrl = idea.PartialUrl }) %>#comments">
							<span class="fa fa-comment" aria-hidden="true"></span> <%: idea.CommentCount %></a>
					</p>
					<%= idea.Copy %>
				</div>
			</li>
		<% } %>
	</ul>
	<% Html.RenderPartial("Pagination", Model.Ideas); %>
	<% if (Model.IdeaForum.CurrentUserCanSubmitIdeas) { %>
		<div id="create-idea">
			<% Html.RenderPartial("CreateIdea", Model.IdeaForum); %>
		</div>
	<% } %>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SideBarContent">
	<% if ((Model.IdeaForum.VotingPolicy == IdeaForumVotingPolicy.OpenToAuthenticatedUsers || Model.IdeaForum.IdeaSubmissionPolicy == IdeaForumIdeaSubmissionPolicy.OpenToAuthenticatedUsers) && !Request.IsAuthenticated) { %>
		<div class="section">
			<div class="alert alert-block alert-info"><%: Html.SnippetLiteral("idea-forum/sign-in-message", "Please sign in to provide all types of feedback in this idea forum.")%></div>
		</div>
	<% } %>
	<% if (Model.IdeaForum.VotesPerUser.HasValue) { %>
		<div class="content-panel panel panel-default">
			<div class="panel-heading">
				<h4>
					<span class="fa fa-thumbs-o-up" aria-hidden="true"></span>
					<%: Html.TextSnippet("Ideas Voting Heading", defaultValue: "Voting", tagName: "span") %>
				</h4>
			</div>
			<ul class="list-group">
				<li class="list-group-item">
					<span class="badge"><%: Model.IdeaForum.VotesPerUser - Model.IdeaForum.CurrentUserActiveVoteCount %></span>
					<%: Html.SnippetLiteral("Ideas User Votes Left", "Your number of votes left")%>
				</li>
			</ul>
		</div>
	<% } %>
</asp:Content>
