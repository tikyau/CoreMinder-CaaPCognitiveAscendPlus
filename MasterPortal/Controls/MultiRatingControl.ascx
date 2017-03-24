<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MultiRatingControl.ascx.cs" Inherits="Site.Controls.MultiRatingControl" %>
<%@ Import Namespace="Adxstudio.Xrm.Web.Mvc.Html" %>
<%@ Register TagPrefix="adx" TagName="Rating" Src="~/Controls/Rating.ascx" %>
<%@ Register TagPrefix="adx" TagName="Vote" Src="~/Controls/Vote.ascx" %>

<% if (IsPanel && EnableRatings) { %>
<div class="content-panel panel panel-default">
	<div class="panel-heading">
		<h4>
			<span class="fa fa-star-o" aria-hidden="true"></span>
			<%: Html.TextSnippet("Rating Heading", defaultValue: "Rate This", tagName: "span") %>
		</h4>
	</div>
	<div class="panel-body">
<% } %>
		<adx:Rating ID="RatingControl" runat="server" Visible="false" OnRatingChanged="Rating_Changed" EnableViewState="true" />
		<adx:Vote ID="VoteControl" runat="server" Visible="false" OnVoteChanged="Vote_Changed" EnableViewState="true" />
<% if (IsPanel && EnableRatings) { %>
	</div>
</div>
<% } %>
