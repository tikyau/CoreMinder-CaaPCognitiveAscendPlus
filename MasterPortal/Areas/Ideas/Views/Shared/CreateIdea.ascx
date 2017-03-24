<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Adxstudio.Xrm.Ideas.IIdeaForum>" %>

<% using (Ajax.BeginForm("Create", "Idea", new { id = Model.Id }, new AjaxOptions { UpdateTargetId = "create-idea", OnComplete = "ideaCreated"}, new{@class = "form-horizontal html-editors"})) { %>
	<fieldset>
		<legend><%: Html.SnippetLiteral("Idea New Suggestion Label", "Suggest a New Idea")%></legend>
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
			<label class="col-sm-3 control-label required" for="title">Idea</label>
			<div class="col-sm-9">
				<%= Html.TextBox("title", string.Empty, new { @class = "form-control" })%>
			</div>
		</div>
		<div class="form-group">
			<label class="col-sm-3 control-label" for="title">Description</label>
			<div class="col-sm-9">
				<%= Html.TextArea("copy", string.Empty, new { @class = "form-control" })%>
			</div>
		</div>
		<div class="form-group">
			<div class="col-sm-offset-3 col-sm-9">
				<input id="submit-idea" class="btn btn-primary" type="submit" value="<%: Html.SnippetLiteral("Idea Submit Label", "Submit Idea")%>" />
			</div>
		</div>
	</fieldset>
<% } %>
<script type="text/javascript">
	$(function() {
		$("#submit-idea").click(function() {
			$.blockUI({ message: null, overlayCSS: { opacity: .3 } });
		});
	});

	function ideaCreated() {
		if ($("#create-idea .validation-summary-errors").length) {
			portal.initializeHtmlEditors();
			prettyPrint();
			$.unblockUI();
			return;
		}
		window.location.href = '<%= Url.RouteUrl("IdeasFilter", new { ideaForumPartialUrl = Model.PartialUrl, filter = "new" }) %>';
	}
</script>
