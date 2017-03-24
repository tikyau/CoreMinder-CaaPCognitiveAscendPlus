<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CommentCreator.ascx.cs" Inherits="Site.Controls.CommentCreator" %>

<div class="post-comment-new-form">
	<fieldset>
		<legend>
			<asp:Literal Text='<%$ Snippet: New Comment Heading, Post a comment %>' runat="server"></asp:Literal>
		</legend>
		<asp:ValidationSummary CssClass="alert alert-danger alert-block" ValidationGroup="NewComment" runat="server" />
		<asp:Panel ID="NewCommentAuthorInfoPanel" CssClass="author" runat="server" >
			<div class="form-group">
				<asp:Label CssClass="control-label required" Text='<%$ Snippet: New Comment Author Name Label, Name %>' AssociatedControlID="CommentAuthorName" runat="server" />
				<div>
					<asp:TextBox ID="CommentAuthorName" ValidationGroup="NewComment" CssClass="form-control" runat="server"></asp:TextBox>
				</div>
				<asp:RequiredFieldValidator Visible="False" ControlToValidate="CommentAuthorName" ValidationGroup="NewComment" ErrorMessage="Name is a required field." Text="*" runat="server"></asp:RequiredFieldValidator>
			</div>
			<div class="form-group">
				<asp:Label CssClass="control-label" Text='<%$ Snippet: New Comment Author Email Label, Email %>' AssociatedControlID="CommentAuthorEmail" runat="server" />
				<div>
					<asp:TextBox ID="CommentAuthorEmail" ValidationGroup="NewComment" CssClass="form-control" runat="server"></asp:TextBox>
				</div>
			</div>
			<div class="form-group">
				<asp:Label CssClass="control-label" Text='<%$ Snippet: New Comment Author URL Label, URL %>' AssociatedControlID="CommentAuthorURL" runat="server" />
				<div>
					<asp:TextBox ID="CommentAuthorUrl" ValidationGroup="NewComment" CssClass="form-control" runat="server"></asp:TextBox>
				</div>
			</div>
		</asp:Panel>
		<adx:CrmDataSource ID="NewCommentDataSource" runat="server" CrmDataContextName="<%$ SiteSetting: Language Code %>" />
		<adx:CommentCreatorFormView ID="NewCommentFormView" OnPreRender="NewCommentFormView_OnPreRender" DataSourceID="NewCommentDataSource"
			CssClass="crmEntityFormView html-editors" Mode="Insert" AutoGenerateSteps="False" OnItemInserting="NewComment_OnItemInserting"
			OnItemInserted="NewComment_OnItemInserted" FormName="New Comment Form" ValidationGroup="NewComment" runat="server">
			<InsertItemTemplate>
				<div class="form-actions">
					<asp:Button CommandName="Insert" ValidationGroup="NewComment" Text='<%$ Snippet: New Comment Submit Button Text, Post this comment %>' CssClass="btn btn-primary" runat="server"/>
				</div>
			</InsertItemTemplate>
		</adx:CommentCreatorFormView>
	</fieldset>
</div>
