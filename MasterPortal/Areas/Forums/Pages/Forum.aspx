<%@ Page Language="C#" MasterPageFile="../MasterPages/Forums.master" AutoEventWireup="true" ValidateRequest="false" CodeBehind="Forum.aspx.cs" Inherits="Site.Areas.Forums.Pages.Forum" %>
<%@ OutputCache CacheProfile="User" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="Adxstudio.Xrm.Forums" %>
<%@ Import Namespace="Adxstudio.Xrm.Web.Mvc.Html" %>
<%@ Import Namespace="Site.Areas.Forums" %>
<%@ Import Namespace="Site.Helpers" %>

<asp:Content ContentPlaceHolderID="Breadcrumbs" runat="server">
	<% Html.RenderPartial("ForumBreadcrumbs"); %>
</asp:Content>

<asp:Content ContentPlaceHolderID="PageHeader" runat="server">
	<crm:CrmEntityDataSource ID="CurrentEntity" DataItem="<%$ CrmSiteMap: Current %>" runat="server" />
	<div class="page-header">
		<asp:Panel ID="ForumControls" CssClass="forum-controls pull-right" runat="server">
			<a class="btn btn-primary" href="#new">
				<span class="fa fa-plus-circle" aria-hidden="true"></span>
				<adx:Snippet SnippetName="Forum Thread Create Heading" DefaultText="Create a new thread" Literal="True" runat="server"/>
			</a>
		</asp:Panel>
		<h1>
			<adx:Property DataSourceID="CurrentEntity" PropertyName="adx_name" EditType="text" runat="server" />
			<small>
				<adx:Property DataSourceID="CurrentEntity" PropertyName="adx_description" EditType="text" runat="server" />
			</small>
		</h1>
	</div>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
	<asp:ObjectDataSource ID="ForumAccouncementDataSource" TypeName="Adxstudio.Xrm.Forums.IForumDataAdapter" OnObjectCreating="CreateForumDataAdapter" SelectMethod="SelectAnnouncements" runat="server" />
	<asp:ListView DataSourceID="ForumAccouncementDataSource" runat="server">
		<LayoutTemplate>
			<asp:PlaceHolder ID="itemPlaceholder" runat="server"/>
		</LayoutTemplate>
		<ItemTemplate>
			<crm:CrmEntityDataSource ID="Announcement" DataItem='<%# Eval("Entity") %>' runat="server"/>
			<div class="alert alert-block alert-info">
				<h4>
					<adx:Property DataSourceID="Announcement" PropertyName="adx_name" EditType="text" runat="server"/>
				</h4>
				<adx:Property DataSourceID="Announcement" PropertyName="adx_content" EditType="html" runat="server"/>
			</div>
		</ItemTemplate>
	</asp:ListView>

	<asp:ObjectDataSource ID="ForumThreadDataSource" TypeName="Adxstudio.Xrm.Forums.IForumThreadAggregationDataAdapter" OnObjectCreating="CreateForumDataAdapter" SelectMethod="SelectThreads" SelectCountMethod="SelectThreadCount" EnablePaging="True" runat="server" />
	<asp:ListView ID="ForumThreads" DataSourceID="ForumThreadDataSource" OnDataBound="ForumThreads_DataBound" runat="server">
		<LayoutTemplate>
			<table class="table forums forum-threads">
				<thead>
					<tr>
						<th class="labels"></th>
						<th class="name">
							<adx:Snippet SnippetName="Forum Thread Name Heading" DefaultText="Thread" EditType="text" runat="server"/>
						</th>
						<th class="author">
							<adx:Snippet SnippetName="Forum Thread Author Heading" DefaultText="Author" EditType="text" runat="server"/>
						</th>
						<th class="last-post">
							<adx:Snippet SnippetName="Forum Thread Last Post Heading" DefaultText="Last Post" EditType="text" runat="server"/>
						</th>
						<th class="count">
							<adx:Snippet SnippetName="Forum Thread Reply Count Heading" DefaultText="Replies" EditType="text" runat="server"/>
						</th>
					</tr>
				</thead>
				<tbody>
					<asp:PlaceHolder ID="itemPlaceholder" runat="server" />
				</tbody>
			</table>
			
			<adx:UnorderedListDataPager ID="ForumThreadsPager" CssClass="pagination" PagedControlID="ForumThreads" QueryStringField="page" PageSize='<%$ SiteSetting: Forums/ThreadsPerPage, 20 %>' runat="server">
				<Fields>
					<adx:ListItemNextPreviousPagerField ShowNextPageButton="false" ShowFirstPageButton="True" FirstPageText="&laquo;" PreviousPageText="&lsaquo;" />
					<adx:ListItemNumericPagerField ButtonCount="10" PreviousPageText="&hellip;" NextPageText="&hellip;" />
					<adx:ListItemNextPreviousPagerField ShowPreviousPageButton="false" ShowLastPageButton="True" LastPageText="&raquo;" NextPageText="&rsaquo;" />
				</Fields>
			</adx:UnorderedListDataPager>
		</LayoutTemplate>
		<ItemTemplate>
			<tr>
				<td class="labels">
					<asp:Label CssClass="label label-default" Visible='<%# (bool)Eval("IsSticky") &&  (bool)Eval("Locked")%>' runat="server"><span class="fa fa-lock" aria-hidden="true"></span> Sticky</asp:Label>
					<asp:Label CssClass="label label-default" Visible='<%# (bool)Eval("IsSticky") && (!(bool)Eval("Locked")) %>' Text="Sticky" runat="server"/>
					<asp:Label CssClass="fa fa-lock" Visible='<%# (bool)Eval("Locked") &&  (!(bool)Eval("IsSticky"))%>' ToolTip='<%$ Snippet: forums/thread/locked, This thread has been marked as locked %>' runat="server" />
					<asp:Label CssClass="fa fa-check" Visible='<%# Eval("IsAnswered") %>' ToolTip='<%$ Snippet: Forum Thread Is Answered ToolTip, This thread has been marked as answered %>' runat="server"></asp:Label>
					<asp:Label CssClass="fa fa-question-circle" Visible='<%# (bool)Eval("ThreadType.RequiresAnswer") && (!(bool)Eval("IsAnswered")) && (!(bool)Eval("Locked"))%>' ToolTip='<%$ Snippet: Forum Thread Requires Answer ToolTip, This thread requires an answer %>' runat="server"/>	
				</td>
				<td class="name">
					<h4><asp:HyperLink NavigateUrl='<%# Eval("Url") %>' Text='<%# HttpUtility.HtmlEncode(Eval("Name") ?? "") %>' runat="server"/></h4>
				</td>
				<td class="author small">
					<a class="author-link" href='<%# Url.AuthorUrl(Eval("Author") as IForumAuthor) %>'>
						<%# HttpUtility.HtmlEncode(Eval("Author.DisplayName") ?? "") %>
					</a>
					<div class="badges" style="display:block;">
						<div data-badge="true" data-uri="<%# Url.RouteUrl("PortalBadges", new { __portalScopeId__ = Website.Id, userId = Eval("Author.EntityReference.Id"), type = "basic-badges" }) %>"></div>
					</div>
				</td>
				<td class="last-post">
					<div class="media">
						<div class="media-left">
							<asp:HyperLink CssClass="author-link" NavigateUrl='<%# Url.AuthorUrl(Eval("LatestPost.Author") as IForumAuthor) %>' ToolTip='<%# Eval("LatestPost.Author.DisplayName") %>' runat="server">
								<asp:Image CssClass="author-img" ImageUrl='<%# Url.UserImageUrl(Eval("LatestPost.Author") as IForumAuthor, 40) %>' AlternateText='<%# Eval("LatestPost.Author.DisplayName") %>' runat="server"/>
							</asp:HyperLink>
						</div>
						<div class="media-body">
							<div class="last-post-info small">
								<asp:HyperLink CssClass="author-link" NavigateUrl='<%# Url.AuthorUrl(Eval("LatestPost.Author") as IForumAuthor) %>' Text='<%# HttpUtility.HtmlEncode(Eval("LatestPost.Author.DisplayName") ?? "") %>' ToolTip='<%# Eval("LatestPost.Author.DisplayName") %>' runat="server" />
								<div class="postedon">
									<abbr class="timeago">
										<%# ForumHelpers.PostedOn(Eval("LatestPost") as IForumPostInfo, "r") %>
									</abbr>
								</div>
								<div class="badges" style="display:block;">
									<div data-badge="true" data-uri="<%# Url.RouteUrl("PortalBadges", new { __portalScopeId__ = Website.Id, userId = Eval("LatestPost.Author.EntityReference.Id"), type = "basic-badges" }) %>"></div>
								</div>
							</div>
							<asp:HyperLink CssClass="last-post-link" NavigateUrl='<%# Eval("LatestPostUrl") %>' Visible='<%# Eval("LatestPost") != null && Eval("LatestPostUrl") != null %>' ToolTip="Last post in thread" runat="server">
								<span class="fa fa-arrow-circle-o-right" aria-hidden="true"></span>
							</asp:HyperLink>
						</div>
					</div>
				</td>
				<td class="count"><%# Eval("ReplyCount") %></td>
			</tr>
		</ItemTemplate>
	</asp:ListView>
	
	<asp:Panel ID="ForumThreadCreateForm" CssClass="form-horizontal form-forum-thread html-editors" ViewStateMode="Enabled" runat="server">
		<fieldset id="new">
			<legend>
				<adx:Snippet SnippetName="Forum Thread Create Heading" DefaultText="Create a new thread" EditType="text" runat="server"/>
			</legend>
			<adx:Snippet SnippetName="Forum Thread Create Instructions" EditType="html" runat="server"/>
			<asp:ValidationSummary CssClass="alert alert-danger alert-block" ValidationGroup="NewThread" runat="server" />
			<div class="form-group">
				<asp:Label AssociatedControlID="NewForumThreadName" CssClass="col-sm-2 control-label required" runat="server">
					<%: Html.SnippetLiteral("Forum Thread Create Name Label", "Thread Title") %>
				</asp:Label>
				<div class="col-sm-10">
					<asp:TextBox ID="NewForumThreadName" ValidationGroup="NewThread" MaxLength="95" CssClass="form-control" runat="server"/>
					<asp:RequiredFieldValidator CssClass="validator" ValidationGroup="NewThread" ControlToValidate="NewForumThreadName" ErrorMessage="Thread Title is a required field." Text="*" EnableClientScript="False" runat="server"/>
				</div>
			</div>
			<div class="form-group">
				<asp:Label AssociatedControlID="NewForumThreadType" CssClass="col-sm-2 control-label required" runat="server">
					<%: Html.SnippetLiteral("Forum Thread Create Type Label", "Thread Type") %>
				</asp:Label>
				<div class="col-sm-10">
					<asp:ObjectDataSource ID="ForumThreadTypeDataSource" TypeName="Adxstudio.Xrm.Forums.IForumDataAdapter" OnObjectCreating="CreateForumDataAdapter" SelectMethod="SelectThreadTypeListItems" runat="server" />
					<asp:DropDownList DataSourceID="ForumThreadTypeDataSource" ValidationGroup="NewThread" ID="NewForumThreadType" DataTextField="Text" DataValueField="Value" CssClass="form-control" runat="server"/>
					<asp:RequiredFieldValidator CssClass="validator" ValidationGroup="NewThread" ControlToValidate="NewForumThreadType" ErrorMessage="Thread Type is a required field." Text="*" EnableClientScript="False" runat="server"/>
				</div>
			</div>
			<div class="form-group">
				<asp:Label AssociatedControlID="NewForumThreadContent" CssClass="col-sm-2 control-label required" runat="server">
					<%: Html.SnippetLiteral("Forum Thread Create Content Label", "Content") %>
				</asp:Label>
				<div class="col-sm-10">
					<asp:TextBox ID="NewForumThreadContent" ValidationGroup="NewThread" TextMode="MultiLine" CssClass="form-control" runat="server"/>
					<asp:RequiredFieldValidator CssClass="validator" ValidationGroup="NewThread" ControlToValidate="NewForumThreadContent" ErrorMessage="Content is a required field." Text="*" EnableClientScript="False" runat="server"/>
					<asp:CustomValidator CssClass="validator" ValidationGroup="NewThread" ControlToValidate="NewForumThreadContent" OnServerValidate="ValidatePostContentLength" ErrorMessage='<%$ Snippet: forums/threads/maxlengthvalidation, The content of this post exceeds the maximum length. %>' Text="*" runat="server"/>
				</div>
			</div>
			<div class="form-group">
				<asp:Label AssociatedControlID="NewForumThreadAttachment" CssClass="col-sm-2 control-label" runat="server">
					<%: Html.SnippetLiteral("Forum Thread Create File Attachment Label", "Attach Files") %>
				</asp:Label>
				<div class="col-sm-10">
					<div class="form-control-static">
						<asp:FileUpload ID="NewForumThreadAttachment" ValidationGroup="NewThread" AllowMultiple="True" runat="server"/>
					</div>
				</div>
			</div>
			<div class="form-group">
				<div class="col-sm-offset-2 col-sm-10">
					<div class="checkbox">
						<label>
							<asp:CheckBox ID="NewForumThreadSubscribe" Checked="True" runat="server"/>
							<adx:Snippet SnippetName="Forum Thread Create Subscribe Label" DefaultText="Subscribe to this thread" EditType="text" runat="server"/>
						</label>
					</div>
				</div>
			</div>
			<div class="form-group">
				<div class="col-sm-offset-2 col-sm-10">
					<asp:Button CssClass="btn btn-primary" OnClick="CreateThread_Click" Text='<%$ Snippet: Forum Thread Create Button Text, Create this thread %>' ValidationGroup="NewThread" runat="server"/>
				</div>
			</div>
		</fieldset>
	</asp:Panel>
	
	<adx:Snippet ID="AnonymousMessage" SnippetName="Forum Post Anonymous Message" EditType="html" runat="server"/>
	
	<script type="text/javascript">
		$(function () {
			$('input[type="submit"]').click(function () {
				$.blockUI({ message: null, overlayCSS: { opacity: .3 } });
			});
		});
	</script>

</asp:Content>
