<%@ Page Language="C#" MasterPageFile="../MasterPages/Blogs.master" AutoEventWireup="true" CodeBehind="BlogPost.aspx.cs" ValidateRequest="false" Inherits="Site.Areas.Blogs.Pages.BlogPost" %>
<%@ OutputCache CacheProfile="User" %>
<%@ Import Namespace="Adxstudio.Xrm.Blogs" %>
<%@ Import Namespace="Site.Helpers" %>
<%@ Register src="~/Controls/Comments.ascx" tagname="Comments" tagprefix="adx" %>
<%@ Register src="~/Controls/MultiRatingControl.ascx" tagname="MultiRatingControl" tagprefix="adx" %>

<asp:Content ContentPlaceHolderID="PageHeader" runat="server">
	<asp:ObjectDataSource ID="PostHeaderDataSource" TypeName="Adxstudio.Xrm.Blogs.IBlogPostDataAdapter" OnObjectCreating="CreateBlogPostDataAdapter" SelectMethod="Select" runat="server" />
	<asp:ListView ID="PostHeader" DataSourceID="PostHeaderDataSource" runat="server">
		<LayoutTemplate>
			<asp:PlaceHolder ID="itemPlaceholder" runat="server" />
		</LayoutTemplate>
		<ItemTemplate>
			<crm:CrmEntityDataSource ID="Post" DataItem='<%# Eval("Entity") %>' runat="server" />
			<div class="page-header blog-post-heading">
				<asp:HyperLink CssClass="user-avatar" NavigateUrl='<%# Url.AuthorUrl(Eval("Author") as IBlogAuthor) %>' ImageUrl='<%# Url.UserImageUrl(Eval("Author.EmailAddress")) %>' ToolTip='<%# HttpUtility.HtmlEncode(Eval("Author.Name") ?? "") %>' runat="server"/>
				<h1><adx:Property DataSourceID="Post" PropertyName="adx_name" EditType="text" runat="server" /></h1>
			</div>
		</ItemTemplate>
	</asp:ListView>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
	<asp:ObjectDataSource ID="PostDataSource" TypeName="Adxstudio.Xrm.Blogs.IBlogPostDataAdapter" OnObjectCreating="CreateBlogPostDataAdapter" SelectMethod="Select" runat="server" />
	<asp:ListView ID="Post" DataSourceID="PostDataSource" runat="server">
		<LayoutTemplate>
			<asp:PlaceHolder ID="itemPlaceholder" runat="server" />
		</LayoutTemplate>
		<ItemTemplate>
			<div class='<%# "blog-post" + ((bool)Eval("IsPublished") ? "" : " unpublished") %>' runat="server">
				<crm:CrmEntityDataSource ID="Post" DataItem='<%# Eval("Entity") %>' runat="server" />
				<div class="metadata">
					<asp:Label Visible='<%# !(bool)Eval("IsPublished") %>' CssClass="label label-info" Text='<%$ Snippet: Unpublished Post Label, Unpublished %>' runat="server"></asp:Label>
					<asp:HyperLink NavigateUrl='<%# Url.AuthorUrl(Eval("Author") as IBlogAuthor) %>' Text='<%# Eval("Author.Name") %>' runat="server" />
					&ndash;
					<abbr class="timeago"><%# Eval("PublishDate", "{0:r}") %></abbr>
					<asp:Label Visible='<%# ((BlogCommentPolicy)Eval("CommentPolicy")) != BlogCommentPolicy.None %>' runat="server">
						&ndash;
						<a href="#comments">
							<span class="fa fa-comment" aria-hidden="true"></span> <%# Eval("CommentCount") %>
						</a>
					</asp:Label>
				</div>
				<div>
					<asp:Panel Visible='<%# Eval("HasExcerpt") %>' runat="server">
						<adx:Property DataSourceID="Post" PropertyName="adx_summary" EditType="html" runat="server" />
					</asp:Panel>
					<a class="anchor" name="extended"></a>
					<adx:Property DataSourceID="Post" PropertyName="adx_copy" EditType="html" CssClass="page-copy" runat="server" />
				</div>
				<div>
					<asp:ListView runat="server" DataSource='<%# Eval("Tags") %>'>
						<LayoutTemplate>
							<ul class="tags">
								<li id="itemPlaceholder" runat="server" />
							</ul>
						</LayoutTemplate>
						<ItemTemplate>
							<li runat="server">
								<asp:HyperLink CssClass="btn btn-default btn-xs" NavigateUrl='<%# Eval("ApplicationPath.AppRelativePath") %>' runat="server">
									<span class="fa fa-tag" aria-hidden="true"></span>
									<%# Eval("Name") %>
								</asp:HyperLink>
							</li>
						</ItemTemplate>
					</asp:ListView>
				</div>
			</div>
		</ItemTemplate>
	</asp:ListView>
	<adx:Comments RatingType="vote" EnableRatings="False" runat="server" />
</asp:Content>

<asp:Content ContentPlaceHolderID="SidebarTop" runat="server">
	<adx:MultiRatingControl ID="Rating" InitEventName="OnLoad" RatingType="rating" IsPanel="True" runat="server" />
</asp:Content>
