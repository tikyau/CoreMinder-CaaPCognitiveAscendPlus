<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Site.Areas.Issues.ViewModels.IssueViewModel>" %>
<%@ Import Namespace="Site.Helpers" %>

<% if (!Model.CurrentUserHasAlert) { %>
	<%= Ajax.RawActionLink(@"<span class=""fa fa-eye"" aria-hidden=""true""></span> " + Html.SnippetLiteral("Issue Alert Create Label", "Track"),
		"AlertCreate", "Issue", new { id = Model.Issue.Id },
		new AjaxOptions { HttpMethod = "POST", UpdateTargetId = "issue-tracking" },
		new { @class = "btn btn-default btn-lg btn-block" }) %>
<% } else { %>
	<%= Ajax.RawActionLink(@"<span class=""fa fa-eye-slash"" aria-hidden=""true""></span> " + Html.SnippetLiteral("Issue Alert Remove Label", "Stop Tracking"),
		"AlertRemove", "Issue", new { id = Model.Issue.Id },
		new AjaxOptions { HttpMethod = "POST", UpdateTargetId = "issue-tracking" },
		new { @class = "btn btn-danger btn-lg btn-block" }) %>
<% } %>
<script type="text/javascript">
	$(function () {
		$("div#issue-tracking > a").click(function () {
			$(this).block({ message: null, overlayCSS: { opacity: .3} });
		});
	});
</script>