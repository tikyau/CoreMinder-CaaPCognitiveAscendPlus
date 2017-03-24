<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Adxstudio.Xrm.Issues.IIssueForum>" %>

<% using (Ajax.BeginForm("Create", "Issue", new { id = Model.Id }, new AjaxOptions { UpdateTargetId = "create-issue", OnComplete = "issueCreated"}, new{@class = "form-horizontal html-editors"})) { %>
	<fieldset>
		<legend><%: Html.SnippetLiteral("Issue Add New Label", "Add a New Issue")%></legend>
		<%= Html.ValidationSummary(string.Empty, new {@class = "alert alert-block alert-danger"})%>
		<% if (!Request.IsAuthenticated) { %>
			<div class="form-group">
				<label class="col-sm-3 control-label required" for="authorName">Your Name</label>
				<div class="col-sm-9">
					<%= Html.TextBox("authorName", string.Empty, new { @class = "form-control" })%>
				</div>
			</div>
			<div class="form-group">
				<label class="col-sm-3 control-label required" for="authorEmail">E-mail</label>
				<div class="col-sm-9">
					<%= Html.TextBox("authorEmail", string.Empty, new { @class = "form-control" })%>
				</div>
			</div>
		<% } %>
		<div class="form-group">
			<label class="col-sm-3 control-label required" for="title">Issue</label>
			<div class="col-sm-9">
				<%= Html.TextBox("title", string.Empty, new { @class = "form-control" })%>
			</div>
		</div>
		<div class="form-group">
			<label class="col-sm-3 control-label" for="copy">Description</label>
			<div class="col-sm-9">
				<%= Html.TextArea("copy", string.Empty, new { @class = "form-control" })%>
			</div>
		</div>
		<div class="form-group">
			<div class="col-sm-offset-3 col-sm-9">
				<div class="checkbox">
					<label><%= Html.CheckBox("track", true)%> Be notified when someone updates or comments on this issue.</label>
				</div>
			</div>
		</div>
		<div class="form-group">
			<div class="col-sm-offset-3 col-sm-9">
				<input id="submit-issue" class="btn btn-primary" type="submit" value="<%: Html.SnippetLiteral("Issue Submit Label", "Submit Issue")%>" />
			</div>
		</div>
	</fieldset>
<% } %>
<script type="text/javascript">
	$(function() {
		$("#submit-issue").click(function() {
			$.blockUI({ message: null, overlayCSS: { opacity: .3 } });
		});
	});

	function issueCreated() {
		if ($("#create-issue .validation-summary-errors").length) {
			portal.initializeHtmlEditors();
			prettyPrint();
			$.unblockUI();
			return;
		}
		window.location.href = '<%= Url.RouteUrl("IssuesFilter", new { issueForumPartialUrl = Model.PartialUrl, filter = "open", status = "new-or-unconfirmed" }) %>';
	}
</script>