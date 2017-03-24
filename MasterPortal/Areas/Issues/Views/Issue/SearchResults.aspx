<%@ Page Language="C#" MasterPageFile="../Shared/Issues.master" Inherits="System.Web.Mvc.ViewPage<Adxstudio.Xrm.Search.ICrmEntitySearchResultPage>" %>
<%@ OutputCache CacheProfile="User" %>
<%@ Import Namespace="Adxstudio.Xrm.Data" %>
<%@ Import Namespace="Microsoft.Xrm.Portal.Web" %>
<%@ Import Namespace="Adxstudio.Xrm.Search" %>

<asp:Content runat="server" ContentPlaceHolderID="Title"><%: Html.SnippetLiteral("Issue Search Title", "Search Issues") %></asp:Content>
<asp:Content ContentPlaceHolderID="PageHeader" runat="server">
	<ul class="breadcrumb">
		<% Html.RenderPartial("SiteMapPath"); %>
		<li class="active"><%: Html.SnippetLiteral("Issue Search Title", "Search Issues") %></li>
	</ul>
</asp:Content>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
	<div class="search-results">
		<% if (Model.Any()) { %>
			<h2>Results <%: Model.First().ResultNumber %>&ndash;<%: Model.Last().ResultNumber %> of <%: Model.ApproximateTotalHits %> for <em class="querytext"><%: Request["q"] %></em></h2>
			<ul>
				<% foreach (var result in Model) { %>
					<li>
						<h3><a href="<%= result.Url %>"><%: result.Title %></a></h3>
						<p class="fragment"><%= result.Fragment %></p>
						<a href="<%= result.Url %>"><%: new UrlBuilder(result.Url.ToString()) %></a>
					</li>
				<% } %>
			</ul>
		<% Html.RenderPartial("Pagination", new PaginatedList<ICrmEntitySearchResult>(Model.PageNumber, Model.ApproximateTotalHits, Model.AsEnumerable())); %>
		<% } else if (Model.PageNumber == 1) { %>
			<h2>No results found for <em class="querytext"><%: Request["q"] %></em></h2>
		<% } %>
	</div>
</asp:Content>
