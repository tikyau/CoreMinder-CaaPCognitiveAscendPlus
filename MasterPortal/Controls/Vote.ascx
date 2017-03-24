<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Vote.ascx.cs" Inherits="Site.Controls.Vote" EnableViewState="true" %>
<%@ Import Namespace="Microsoft.Xrm.Portal.Web" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<asp:UpdatePanel ID="UpdatePanelVotes" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
	<ContentTemplate>
		<div id="vote">
			<div class="vote-totals">
				<h6>
					<asp:Literal Text="<%$ Snippet: Vote Heading, Votes %>" runat="server"/>
					<span class="vote-counts">
						<asp:Literal Text="<%$ Snippet: Vote Yes Count Prefix, ( %>" runat="server"/><asp:Label ID="VoteYesCount" Text="0" CssClass="count" runat="server" /><asp:Literal ID="Literal3"  Text="<%$ Snippet: Vote Yes Count Suffix, ) %>" runat="server"/>
						<asp:ImageButton ID="VoteYesButton" runat="server" ImageUrl="~/xrm-adx/samples/images/thumb_up.png" OnClick="VoteYes" CausesValidation="False" />
						<asp:Literal Text="<%$ Snippet: Vote No Count Prefix, ( %>" runat="server"/><asp:Label ID="VoteNoCount" Text="0" CssClass="count" runat="server" /><asp:Literal ID="Literal5"  Text="<%$ Snippet: Vote No Count Suffix, ) %>" runat="server"/>
						<asp:ImageButton ID="VoteNoButton" runat="server" ImageUrl="~/xrm-adx/samples/images/thumb_down.png" OnClick="VoteNo" CausesValidation="False" />
					</span>
				</h6>				
			</div>
			<div class="user-vote">
				<asp:Panel ID="User" runat="server">
					<h6><asp:Literal Text="<%$ Snippet: User Vote Heading, Your Vote %>" runat="server"/> <asp:ImageButton ID="VoteButton" Visible="false" runat="server" OnClick="DeleteVote" ToolTip="Click here to delete your vote" CausesValidation="False" ImageUrl="~/xrm-adx/samples/images/spacer.gif" Width="16px" /></h6>
					<ajax:ConfirmButtonExtender ID="ConfirmButtonExtender1" runat="server"
						TargetControlID="VoteButton"
						ConfirmText="Are you sure you want to remove your vote?" />
				</asp:Panel>
			</div>
			<div class="clearfix"></div>
		</div>
	</ContentTemplate>
</asp:UpdatePanel>




