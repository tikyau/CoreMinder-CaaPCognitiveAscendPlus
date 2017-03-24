<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Adxstudio.Xrm.Ideas.IIdea>" %>
<%@ Import Namespace="Adxstudio.Xrm.Ideas" %>
<%@ Import Namespace="Microsoft.Xrm.Client" %>
<%@ Import Namespace="Site.Helpers" %>

<div class="well voting-well">
	<% if (Model.CurrentUserCanVote()) {
		if (Model.VotesPerIdea == 1) { %>
			<%= Ajax.RawActionLink(@"<span class=""fa fa-arrow-up"" aria-hidden=""true""></span>", "Vote", "Idea", new { voteValue = 1, id = Model.Id },
				new AjaxOptions { HttpMethod = "POST", UpdateTargetId = "vote-status-" + Model.Id, OnComplete = "updateUserVoteCount(1)" },
				new { @class = "btn btn-xs btn-info" }) %>
		<% } else { %>
			<a class="btn btn-xs btn-info" id="vote-modal-<%: Model.Id %>-link" data-toggle="modal" href="#vote-modal-<%: Model.Id %>"><span class="fa fa-arrow-up" aria-hidden="true"></span></a>
		<% }
	} else { %>
		<a class="btn btn-xs btn-default disabled"><span class="fa fa-arrow-up" aria-hidden="true"></span></a>
	<% } %>
	<h4><%: Model.VoteSum %></h4>
	<% if (Model.VotingType == IdeaForumVotingType.UpOrDown) {
		if (Model.CurrentUserCanVote()) { %>
			<%= Ajax.RawActionLink(@"<span class=""fa fa-arrow-down"" aria-hidden=""true""></span>", "Vote", "Idea", new { voteValue = -1, id = Model.Id },
				new AjaxOptions { HttpMethod = "POST", UpdateTargetId = "vote-status-" + Model.Id, OnComplete = "updateUserVoteCount(-1)" },
				new { @class = "btn btn-xs btn-info" }) %>
		<% } else { %>
			<a class="btn btn-xs btn-default disabled"><span class="fa fa-arrow-down" aria-hidden="true"></span></a>
		<% }
	} %>
</div>
<% if (Model.CurrentUserCanVote() && Model.VotesPerIdea != 1) { %>
	<section class="modal modal-vote" id="vote-modal-<%: Model.Id %>" tabindex="-1" role="dialog" aria-labelledby="vote-modal-title-<%: Model.Id %>" aria-hidden="true">
		<div class="modal-dialog">
			<div class="modal-content">
				<div class="modal-header">
					<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
					<h1 class="modal-title h4" id="vote-modal-title-<%: Model.Id %>">Vote</h1>
				</div>
				<div class="modal-body">
					<div class="btn-group">
						<% for (var i = 1; i < Model.VotesPerIdea + 1; i++) {
							if (Model.CurrentUserCanVote(i)) { %>
								<%= Ajax.ActionLink("+{0}".FormatWith(i), "Vote", "Idea",
									new RouteValueDictionary(new { voteValue = i, id = Model.Id }),
									new AjaxOptions { HttpMethod = "POST", UpdateTargetId = "vote-status-" + Model.Id, OnComplete = "updateUserVoteCount({0})".FormatWith(i) },
									new Dictionary<string, object> { {"class", "btn btn-default vote"}, {"data-dismiss", "modal" } })%>
							<% } else { %>
								<a class="btn btn-default disabled">+<%= i %></a>
							<% } %>
						<% } %>
					</div>
				</div>
			</div>
		</div>
	</section>
	<script type="text/javascript">
		$(function () {
			$("#vote-modal-<%: Model.Id %>-link").click(function (e) {
				$("#vote-modal-<%: Model.Id %>").css({ top: e.pageY - window.pageYOffset + 'px', left: e.pageX - window.pageXOffset + 'px' });
			});
		});
	</script>
<% } %>	
