<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%@ Import Namespace="Adxstudio.Xrm.Web.Mvc.Html" %>

<div class="content-panel panel panel-default">
	<div class="panel-heading">
		<h4>
			<span class="fa fa-search" aria-hidden="true"></span>
			<%: Html.TextSnippet("Issue Search Heading", defaultValue: "Search Issues", tagName: "span") %>
		</h4>
	</div>
	<div class="panel-body">
		<% using (Html.BeginForm("search", "Issue", FormMethod.Get, new { id = "issues-search-form", @class = "form-search" })) { %>
			<div class="input-group">
				<%= Html.TextBox("q", string.Empty, new { @class = "form-control", placeholder = Html.SnippetLiteral("Issue Search Heading", "Search Issues") })%>
				<div class="input-group-btn">
					<button type="submit" class="btn btn-default" aria-label="<%: Html.SnippetLiteral("Issue Search Heading", "Search Issues") %>"><span class="fa fa-search" aria-hidden="true"></span></button>
				</div>
			</div>
		<% } %>
		<script type="text/javascript">
			$(function () {
				$("#issues-search-form").submit(function () {
					if ($("#issues-search-form #q").val().trim().length) {
						return true;
					}
					return false;
				});
			});
		</script>
	</div>
</div>
