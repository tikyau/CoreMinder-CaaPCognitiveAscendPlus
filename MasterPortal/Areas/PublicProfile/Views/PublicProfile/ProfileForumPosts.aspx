<%@ Page Language="C#" MasterPageFile="PublicProfile.master" Inherits="System.Web.Mvc.ViewPage<Site.Areas.PublicProfile.ViewModels.ProfileViewModel>" %>
<%@ Import Namespace="Adxstudio.Xrm.Data" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
	<ul class="toolbar-nav nav nav-tabs">
		<li><%: Html.ActionLink("Blog Posts", "ProfileBlogPosts")%></li>
		<li><%: Html.ActionLink("Ideas", "ProfileIdeas")%></li>
		<li class="active"><a href="#forum-posts" data-toggle="tab">Forum Posts</a></li>
	</ul>
	<div class="tab-content">
		<div class="tab-pane fade active in" id="forum-posts">
			<ul class="activity-list">
				<% IPaginated paginatedForumPostList = Model.ForumPosts;
				foreach (var post in Model.ForumPosts) { %>
					<li>
						<h3>
							<a href="<%= post.Url %>"><%: post.Name %></a>
						</h3>
						<div class="metadata">
							<abbr class="timeago"><%: post.Entity.GetAttributeValue<DateTime>("adx_date").ToString("r") %></abbr>
							&ndash;
							<a href="<%= post.Thread.Url %>"><%: post.Thread.Name ?? post.Thread.Entity.GetAttributeValue<string>("adx_name") %></a>
						</div>
						<div>
							<%= post.Content %>
						</div>
					</li>
				<% } %>
			</ul>
			<% Html.RenderPartial("Pagination", paginatedForumPostList); %>
		</div>
	</div>
</asp:Content>
