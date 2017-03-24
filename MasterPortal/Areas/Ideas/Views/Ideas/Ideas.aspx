<%@ Page Language="C#" MasterPageFile="../Shared/Ideas.master" Inherits="System.Web.Mvc.ViewPage<Site.Areas.Ideas.ViewModels.IdeasViewModel>" %>
<%@ OutputCache CacheProfile="User" %>

<asp:Content runat="server" ContentPlaceHolderID="PageHeader">
	<% Html.RenderPartial("Breadcrumbs"); %>
	<div class="page-header">
		<h1><%: Html.TextAttribute("adx_name") %></h1>
	</div>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="MainContent">
	<div class="page-copy">
		<%= Html.HtmlAttribute("adx_copy") %>
	</div>
	<ul class="ideas list-unstyled">
		<% foreach (var ideaForum in Model.IdeaForums)
		 { %>
		<li>
			<h3><%: Html.ActionLink(ideaForum.Title, "Ideas", new { ideaForumPartialUrl = ideaForum.PartialUrl } ) %></h3>
			<div class="bottom-break"><%= ideaForum.Summary %></div>
		</li>
		<% } %>
	</ul>
</asp:Content>
