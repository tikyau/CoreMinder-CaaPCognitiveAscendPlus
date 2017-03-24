﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%@ Import Namespace="Adxstudio.Xrm.Web.Mvc.Html" %>

<div class="content-panel panel panel-default">
	<div class="panel-heading">
		<h4>
			<span class="fa fa-search" aria-hidden="true"></span>
			<%: Html.TextSnippet("Idea Search Heading", defaultValue: "Search Ideas", tagName: "span") %>
		</h4>
	</div>
	<div class="panel-body">
		<% using (Html.BeginForm("search", "Idea", FormMethod.Get, new { id = "ideas-search-form", @class = "form-search" })) { %>
			<div class="input-group">
				<%= Html.TextBox("q", string.Empty, new { @class = "form-control", placeholder = Html.SnippetLiteral("Idea Search Heading", "Search Ideas") })%>
				<div class="input-group-btn">
					<button type="submit" class="btn btn-default" aria-label="<%: Html.SnippetLiteral("Idea Search Heading", "Search Ideas") %>"><span class="fa fa-search" aria-hidden="true"></span></button>
				</div>
			</div>
		<% } %>
		<script type="text/javascript">
			$(function () {
				$("#ideas-search-form").submit(function () {
					if ($("#ideas-search-form #q").val().trim().length) {
						return true;
					}
					return false;
				});
			});
		</script>
	</div>
</div>
