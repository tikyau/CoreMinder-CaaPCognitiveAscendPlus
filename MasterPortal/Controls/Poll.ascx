<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Poll.ascx.cs" Inherits="Site.Controls.Poll" %>
<div class="poll content-panel panel panel-default" data-url="/Polls/Placements/<%= PlacementName %>/Random" data-submit-url="/Polls/Submit">
			<div class="panel-heading">
				<asp:HyperLink CssClass="pull-right" NavigateUrl='<%$ CrmSiteMap: SiteMarker=Poll Archives, Return=Url %>' Text='<%$ Snippet: polls/archiveslabel, Poll Archives %>' runat="server" />
				<h4>
					<i class="fa fa-question-circle"></i>
					<adx:Snippet SnippetName="polls/title" EditType="text" DefaultText="Poll" runat="server"/>
				</h4>
			</div>
	<div class="panel-body poll-content"></div>
		</div>
