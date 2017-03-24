<%@ Page Language="C#" MasterPageFile="PublicProfile.master" Inherits="System.Web.Mvc.ViewPage<Site.Areas.PublicProfile.ViewModels.ProfileViewModel>" %>
<%@ Import Namespace="Adxstudio.Xrm.Data" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
	<ul class="toolbar-nav nav nav-tabs">
		<li class="active"><a href="#blog-posts" data-toggle="tab">Blog Posts</a></li>
		<li><%: Html.ActionLink("Ideas", "ProfileIdeas")%></li>
		<li><%: Html.ActionLink("Forum Posts", "ProfileForumPosts")%></li>
	</ul>
	<div class="tab-content">
		<div class="tab-pane fade active in" id="blog-posts">
			<ul class="activity-list">
				<% IPaginated paginatedBlogList = Model.BlogPosts;
				foreach (var post in Model.BlogPosts.Where(p => p.IsPublished)) { %>
					<li class="blog-post">
						<h3>
							<a href='<%= post.ApplicationPath.AppRelativePath %>'><%= post.Title ?? post.Entity.GetAttributeValue<string>("adx_name") %> </a>
						</h3>
						<div class="metadata">
							<abbr class="timeago"><%: post.Entity.GetAttributeValue<DateTime>("adx_date").ToString("r") %></abbr>
							&ndash;
							<a href="<%= post.ApplicationPath.AbsolutePath + "#comments" %>">
								<span class="fa fa-comment" aria-hidden="true"></span> <%= post.CommentCount %> 
							</a>
						</div>
						<div>
							<% if (post.HasExcerpt) { %>
								<%= post.Summary %>
							<% } %>
						</div>
					</li>
				<% } %>
			</ul>
			<% Html.RenderPartial("Pagination", paginatedBlogList); %>
		</div>
	</div>
</asp:Content>
