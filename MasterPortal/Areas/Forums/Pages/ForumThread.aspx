<%@ Page Language="C#" MasterPageFile="../MasterPages/Forums.master" AutoEventWireup="true" ValidateRequest="false" CodeBehind="ForumThread.aspx.cs" Inherits="Site.Areas.Forums.Pages.ForumThread" %>
<%@ OutputCache CacheProfile="User" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="Adxstudio.Xrm.Forums" %>
<%@ Import Namespace="Adxstudio.Xrm.Web.Mvc.Html" %>
<%@ Import Namespace="Site.Areas.Forums" %>
<%@ Import Namespace="Site.Helpers" %>

<asp:Content ContentPlaceHolderID="Title" runat="server"><%: Html.AttributeLiteral("adx_name", false) %></asp:Content>

<asp:Content ContentPlaceHolderID="Breadcrumbs" runat="server">
	<% Html.RenderPartial("ForumThreadBreadcrumbs"); %><%# PortalName %>
</asp:Content>

<asp:Content ContentPlaceHolderID="PageHeader" runat="server">
    
	<crm:CrmEntityDataSource ID="CurrentEntity" DataItem="<%$ CrmSiteMap: Current %>" runat="server" />
	<div class="page-header">
		<asp:Panel ID="ForumControls" CssClass="forum-controls pull-right" runat="server">
			<a class="btn btn-primary" href="#new">
				<span class="fa fa-plus-circle" aria-hidden="true"></span>
				<adx:Snippet SnippetName="Forum Post Create Heading" DefaultText="Post a reply" Literal="True" runat="server"/>
			</a>
			<asp:LinkButton ID="AddAlert" OnClick="AddAlert_Click" CssClass="btn btn-default" Visible="False" ToolTip='<%$ Snippet: Forum Thread Alert Create Tooltip, Get notified of any new posts in this thread %>' runat="server">
				<span class="fa fa-eye" aria-hidden="true"></span>
				<adx:Snippet SnippetName="Forum Thread Alert Create Heading" DefaultText="Subscribe" Literal="True" runat="server"/>
			</asp:LinkButton>
			<asp:LinkButton ID="RemoveAlert" OnClick="RemoveAlert_Click" CssClass="btn btn-danger" Visible="False" ToolTip='<%$ Snippet: Forum Thread Alert Delete Tooltip, Do not receive notifications of new posts in this thread %>' runat="server">
				<span class="fa fa-eye-slash" aria-hidden="true"></span>
				<adx:Snippet SnippetName="Forum Thread Alert Delete Heading" DefaultText="Unsubscribe" Literal="True" runat="server"/>
			</asp:LinkButton>
		</asp:Panel>
		<asp:Panel ID="ForumLockedPanel" CssClass="forum-controls pull-right" runat="server">
			<a class="btn btn-primary disabled">
				<span class="fa fa-lock" aria-hidden="true"></span>
				<adx:Snippet SnippetName="forums/threads/postreply/locked" DefaultText="Locked" Literal="True" runat="server"/>
			</a>
		</asp:Panel>
		<h1>
			<adx:Property DataSourceID="CurrentEntity" PropertyName="adx_name" EditType="text" HtmlEncode="True" LiquidEnabled="False" runat="server" />
		</h1>
	</div>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
	<asp:ObjectDataSource ID="ForumPostDataSource" TypeName="Adxstudio.Xrm.Forums.IForumPostAggregationDataAdapter" OnObjectCreating="CreateForumThreadDataAdapter" SelectMethod="SelectPosts" SelectCountMethod="SelectPostCount" EnablePaging="True" runat="server" />
	<asp:ListView ID="ForumPosts" DataSourceID="ForumPostDataSource" OnDataBound="ForumPosts_DataBound" runat="server">
		<LayoutTemplate>
			<asp:PlaceHolder ID="itemPlaceholder" runat="server"/>
			<adx:UnorderedListDataPager ID="ForumPostsPager" CssClass="pagination" PagedControlID="ForumPosts" QueryStringField="page" PageSize='<%$ SiteSetting: Forums/PostsPerPage, 20 %>' runat="server">
				<Fields>
					<adx:ListItemNextPreviousPagerField ShowNextPageButton="false" ShowFirstPageButton="True" FirstPageText="&laquo;" PreviousPageText="&lsaquo;" />
					<adx:ListItemNumericPagerField ButtonCount="10" PreviousPageText="&hellip;" NextPageText="&hellip;" />
					<adx:ListItemNextPreviousPagerField ShowPreviousPageButton="false" ShowLastPageButton="True" LastPageText="&raquo;" NextPageText="&rsaquo;" />
				</Fields>
			</adx:UnorderedListDataPager>
		</LayoutTemplate>
		<ItemTemplate>
			<div class="forum-post">
				<a id="<%# Eval("EntityReference.Id", "post-{0}") %>" name="<%# Eval("EntityReference.Id", "post-{0}") %>"></a>
				<asp:PlaceHolder Visible='<%# (bool)Eval("CanEdit") %>' ViewStateMode="Enabled" runat="server">
					<section class="modal" id="<%# Eval("EntityReference.Id", "edit-{0}") %>" tabindex="-1" role="dialog" aria-labelledby="<%# Eval("EntityReference.Id", "edit-label-{0}") %>" aria-hidden="true">
						<div class="modal-dialog modal-lg">
							<div class="modal-content">
								<div class="modal-header">
									<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
									<h1 class="modal-title h4" id="<%# Eval("EntityReference.Id", "edit-label-{0}") %>">
										<adx:Snippet runat="server" SnippetName="Forums/EditPost/ButtonText" DefaultText="Update Post" Literal="True"/>
									</h1>
								</div>
								<div class="modal-body html-editors">
									<asp:TextBox ID="ForumPostContentUpdate" TextMode="MultiLine" Text='<%# Eval("Content") %>' runat="server"/>
								</div>
								<div class="modal-footer">
									<asp:LinkButton CssClass="btn btn-primary" Text='<%$ Snippet: Forums/EditPost/ButtonText, Update Post %>' CommandArgument='<%# Eval("EntityReference.Id") %>' OnCommand="UpdatePost_OnCommand" runat="server"/>
									<button type="button" class="btn btn-default" data-dismiss="modal" aria-hidden="true">
										<adx:Snippet runat="server" SnippetName="Forums/EditPost/CancelButtonText" DefaultText="Cancel" Literal="true" EditType="text"/>
									</button>
								</div>
							</div>
						</div>
					</section>
				</asp:PlaceHolder>
				<div class="row">
					<div class="col-sm-2 metadata">
						<asp:Panel Visible='<%# Eval("IsAnswer") %>' CssClass="alert alert-success" runat="server"><span class="fa fa-check" aria-hidden="true"></span> Answer</asp:Panel>
						<asp:HyperLink CssClass="author-link" NavigateUrl='<%# Url.AuthorUrl(Eval("Author") as IForumAuthor) %>' ToolTip='<%# Eval("Author.DisplayName") %>' runat="server">
							<asp:Image CssClass="author-img" ImageUrl='<%# Url.UserImageUrl(Eval("Author") as IForumAuthor, 40) %>' AlternateText='<%# Eval("Author.DisplayName") %>' runat="server"/>
						</asp:HyperLink>
					</div>
					<div class="col-sm-10">
						<asp:Panel CssClass="btn-toolbar" Visible='<%# (bool)Eval("CanMarkAsAnswer") || (bool)Eval("CanEdit") || (bool)Eval("Editable") %>' runat="server">
							<asp:Panel CssClass="btn-group" Visible='<%# (bool)Eval("CanEdit") %>'  runat="server" >
								<a class="btn btn-xs btn-default" href="<%# Eval("EntityReference.Id", "#edit-{0}") %>" title="<%: Html.SnippetLiteral("Forums/EditPost/ButtonText", "Update Post") %>" data-toggle="modal"><span class="fa fa-edit" aria-hidden="true"></span><span class="sr-only"><%: Html.SnippetLiteral("Forums/EditPost/ButtonText", "Update Post") %></span></a>
							</asp:Panel>
							<asp:Panel CssClass="xrm-entity xrm-editable-adx_communityforumpost btn-group" Visible='<%# Eval("Editable") %>' runat="server">
								<a class="btn btn-xs btn-default xrm-edit"><span class="fa fa-cog" aria-hidden="true"></span></a>
								<a class="btn btn-xs btn-default dropdown-toggle" data-toggle="dropdown">
									<span class="caret"></span>
								</a>
								<ul class="dropdown-menu pull-right">
									<li>
										<a href="#" class="xrm-edit"><span class="fa fa-edit" aria-hidden="true"></span> Edit</a>
									</li>
									<li>
										<a href="#" class="xrm-delete"><span class="fa fa-trash-o" aria-hidden="true"></span> Delete</a>
									</li>
								</ul>
								<asp:HyperLink NavigateUrl='<%# Eval("EditPath.AbsolutePath") %>' CssClass="xrm-entity-ref" style="display:none;" runat="server"/>
								<asp:HyperLink NavigateUrl='<%# Eval("DeletePath.AbsolutePath") %>' CssClass="xrm-entity-delete-ref" style="display:none;" runat="server"/>
							</asp:Panel>
							<asp:Panel CssClass="btn-group" Visible='<%# Eval("CanMarkAsAnswer") %>' runat="server">
								<asp:LinkButton CssClass="btn btn-xs btn-success" Visible='<%# !(bool)Eval("IsAnswer") %>' CommandArgument='<%# Eval("EntityReference.Id") %>' OnCommand="MarkAsAnswer_OnCommand" runat="server">
									<span class="fa fa-check" aria-hidden="true"></span>
									<adx:Snippet SnippetName="Forum Post Mark Answer Button Text" Literal="True" DefaultText="Mark as answer" runat="server"/>
								</asp:LinkButton>
								<asp:LinkButton CssClass="btn btn-xs btn-danger" Visible='<%# (bool)Eval("IsAnswer") %>' CommandArgument='<%# Eval("EntityReference.Id") %>' OnCommand="UnmarkAsAnswer_OnCommand" runat="server">
									<span class="fa fa-minus" aria-hidden="true"></span>
									<adx:Snippet SnippetName="Forum Post Unmark Answer Button Text" Literal="True" DefaultText="Unmark as answer" runat="server"/>
								</asp:LinkButton>
							</asp:Panel>
						</asp:Panel>
						<div class="post-header small">
							<adx:Snippet SnippetName="Forum Posted Label Before" Literal="True" DefaultText="Posted" runat="server"/>
							<abbr class="timeago"><%# ForumHelpers.PostedOn(Container.DataItem as IForumPostInfo, "r") %></abbr>
							<adx:Snippet SnippetName="Forum Posted Label After" Literal="True" DefaultText="by" runat="server"/>
							<a class="author-link" href='<%# Url.AuthorUrl(Eval("Author") as IForumAuthor) %>'><%# HttpUtility.HtmlEncode(Eval("Author.DisplayName") ?? "") %></a>
							<div class="badges">
									<div data-badge="true" data-uri="<%# Url.RouteUrl("PortalBadges", new { __portalScopeId__ = Website.Id, userId = Eval("Author.EntityReference.Id"), type = "basic-badges" }) %>"></div>
							</div>
						</div>
						<div>
							<%# Eval("Content") %>
						</div>
						<asp:ListView DataSource='<%# Eval("AttachmentInfo") %>' runat="server">
							<LayoutTemplate>
								<div class="attachments list-group">
									<asp:PlaceHolder ID="itemPlaceholder" runat="server"/>
								</div>
							</LayoutTemplate>
							<ItemTemplate>
								<asp:HyperLink CssClass="list-group-item" NavigateUrl='<%# Eval("Path.AbsolutePath") %>' Visible='<%# Eval("Path") != null %>' ToolTip='<%# Eval("Name") %>' runat="server">
									<span class="fa fa-fw fa-file" aria-hidden="true"></span>
									<%# HttpUtility.HtmlEncode(string.Format("{0} ({1:1})", Eval("Name"), Eval("Size"))) %>
								</asp:HyperLink>
							</ItemTemplate>
						</asp:ListView>
					</div>
				</div>
			</div>
		</ItemTemplate>
	</asp:ListView>
	 
	<asp:Panel ID="ForumPostCreateForm" CssClass="form-horizontal form-forum-thread html-editors" ViewStateMode="Enabled" runat="server">
		<fieldset id="new">
			<legend>
				<adx:Snippet SnippetName="Forum Post Create Heading" DefaultText="Post a reply" EditType="text" runat="server"/>
			</legend>
			<adx:Snippet SnippetName="Forum Post Create Instructions" EditType="html" runat="server"/>
			<asp:ValidationSummary CssClass="alert alert-danger alert-block" ValidationGroup="NewPost" runat="server" />
			<div class="form-group">
				<asp:Label AssociatedControlID="NewForumPostContent" CssClass="col-sm-2 control-label required" runat="server">
					<%: Html.SnippetLiteral("Forum Post Create Content Label", "Content") %>
				</asp:Label>
				<div class="col-sm-10">
					<asp:TextBox ID="NewForumPostContent" TextMode="MultiLine" MaxLength="16384" runat="server"/>
					<asp:RequiredFieldValidator CssClass="validator" ControlToValidate="NewForumPostContent" EnableClientScript="False" ValidationGroup="NewPost" ErrorMessage="Content is a required field." Text="*" runat="server"/>
					<asp:CustomValidator CssClass="validator" ControlToValidate="NewForumPostContent" OnServerValidate="ValidatePostContentLength" ValidationGroup="NewPost" ErrorMessage='<%$ Snippet: forums/threads/maxlengthvalidation, The content of this post exceeds the maximum length. %>' Text="*" runat="server"/>
				</div>
			</div>
			<div class="form-group">
				<asp:Label AssociatedControlID="NewForumPostAttachment" CssClass="col-sm-2 control-label" runat="server">
					<%: Html.SnippetLiteral("Forum Post Create File Attachment Label", "Attach Files") %>
				</asp:Label>
				<div class="col-sm-10">
					<div class="form-control-static">
						<asp:FileUpload ID="NewForumPostAttachment" ValidationGroup="NewPost" AllowMultiple="True" runat="server"/>
					</div>
				</div>
			</div>
			<div class="form-group">
				<div class="col-sm-offset-2 col-sm-10">
					<asp:Button ValidationGroup="NewPost" OnClick="CreatePost_Click" CssClass="btn btn-primary" Text="Post this reply" runat="server"/>
				</div>
			</div>
		</fieldset>
	</asp:Panel>
	<adx:Snippet ID="LockedSnippet" Visible="false" SnippetName="forums/threads/islocked" DefaultText='<div class="alert alert-block alert-info"><p><span class="fa fa-lock" aria-hidden="true"></span> This Thread is locked.</p></div>' EditType="html" runat="server"/>
	<adx:Snippet ID="AnonymousMessage" SnippetName="Forum Post Anonymous Message" EditType="html" runat="server"/>
	<script type="text/javascript">
		$(function() {
			$('input[type="submit"]').click(function() {
				$.blockUI({ message: null, overlayCSS: { opacity: .3 } });
			});
		});
	</script>
</asp:Content>
