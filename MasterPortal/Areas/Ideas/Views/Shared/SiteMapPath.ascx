﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<% foreach (var node in Html.SiteMapPath()) { %>
	<li>
		<a href="<%: node.Item1.Url %>"><%: node.Item1.Title %></a>
	</li>
<% } %>
