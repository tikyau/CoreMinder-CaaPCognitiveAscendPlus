<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Comments.ascx.cs" Inherits="Site.Controls.Comments" %>
<%@ Import Namespace="Adxstudio.Xrm.Cms" %>
<%@ Import Namespace="Site.Helpers" %>
<%@ Register src="~/Controls/CommentCreator.ascx" tagname="CommentCreator" tagprefix="crm" %>
<%@ Register src="~/Controls/MultiRatingControl.ascx" tagname="MultiRatingControl" tagprefix="adx" %>

<asp:ObjectDataSource ID="CommentDataSource" TypeName="Adxstudio.Xrm.Cms.ICommentDataAdapter" OnObjectCreating="CreateCommentDataAdapter" SelectMethod="SelectComments" SelectCountMethod="SelectCommentCount" runat="server" />
<asp:ListView ID="CommentsView" DataSourceID="CommentDataSource" runat="server">
	<LayoutTemplate>
		<div class="comments">
			<legend>
				<asp:Literal Text='<%$ Snippet: Comments Heading, Comments %>' runat="server"></asp:Literal>
			</legend>
			<ul class="list-unstyled">
				<li id="itemPlaceholder" runat="server" />
			</ul>
		</div>
	</LayoutTemplate>
	<ItemTemplate>
		<li runat="server">
			<div class="row comment <%# ((bool)Eval("IsApproved")) ? "approved" : "unapproved" %>">
				<div class="col-sm-3 comment-metadata">
					<div class="comment-author">
						<asp:HyperLink rel="nofollow" NavigateUrl="<%# Url.AuthorUrl(Container.DataItem as IComment) %>" Text='<%# HttpUtility.HtmlEncode(Eval("Author.DisplayName") ?? "") %>' runat="server"></asp:HyperLink>
					</div>
					<abbr class="timeago"><%# Eval("Date", "{0:r}") %></abbr>
					<asp:Label Visible='<%# !(bool)Eval("IsApproved") %>' CssClass="label label-info" Text='<%$ Snippet: Unapproved Comment Label, Unapproved %>' runat="server"></asp:Label>
				</div>
				<div class="col-sm-9">
					<div class="comment-controls">
						<asp:Panel Visible='<%# ((bool?)Eval("Editable")).GetValueOrDefault() %>' CssClass='<%# Eval("Entity.LogicalName", "xrm-entity xrm-editable-{0}")%>' runat="server">
							<div class="btn-group">
								<a class="btn btn-default xrm-edit"><span class="fa fa-cog" aria-hidden="true"></span></a>
								<a class="btn btn-default dropdown-toggle" data-toggle="dropdown">
									<span class="caret"></span>
								</a>
								<ul class="dropdown-menu">
									<li>
										<a href="#" class="xrm-edit"><span class="fa fa-edit" aria-hidden="true"></span> Edit</a>
									</li>
									<li>
										<a href="#" class="xrm-delete"><span class="fa fa-trash-o" aria-hidden="true"></span> Delete</a>
									</li>
								</ul>
							</div>
							<asp:HyperLink NavigateUrl='<%# Eval("EditPath.AbsolutePath") %>' CssClass="xrm-entity-ref" style="display:none;" runat="server"/>
							<asp:HyperLink NavigateUrl='<%# Eval("DeletePath.AbsolutePath") %>' CssClass="xrm-entity-delete-ref" style="display:none;" runat="server"/>
						</asp:Panel>
					</div>
					<adx:MultiRatingControl ID="Rating"
						EnableRatings='<%# Eval("RatingEnabled") %>'
						SourceID='<%# Eval("Entity.Id") %>' 
						LogicalName='<%# Eval("Entity.LogicalName") %>' 
						RatingInfo='<%# Bind("RatingInfo") %>' 
						RatingType="rating" 
						InitEventName="OnDataBinding"
						runat="server" />
					<div><%# Eval("Content") %></div>
				</div>
			</div>
		</li>
	</ItemTemplate>
</asp:ListView>

<asp:Panel ID="NewCommentPanel" runat="server">
	<crm:CommentCreator ID="NewCommentCreator" runat="server" />
</asp:Panel>
