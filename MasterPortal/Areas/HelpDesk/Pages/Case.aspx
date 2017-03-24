<%@ Page Language="C#" MasterPageFile="~/MasterPages/WebForms.master" ValidateRequest="false" AutoEventWireup="true" CodeBehind="Case.aspx.cs" Inherits="Site.Areas.HelpDesk.Pages.Case" %>
<%@ Import Namespace="Adxstudio.Xrm.Notes" %>
<%@ Import Namespace="Adxstudio.Xrm.Web.Mvc.Html" %>
<%@ Import Namespace="Site.Helpers" %>

<asp:Content ContentPlaceHolderID="Head" runat="server">
	<link rel="stylesheet" href="<%: Url.Content("~/Areas/HelpDesk/css/helpdesk.css") %>">
</asp:Content>

<asp:Content ContentPlaceHolderID="Breadcrumbs" runat="server">
	<asp:PlaceHolder ID="CaseBreadcrumbs" runat="server">
		<ul class="breadcrumb">
			<% foreach (var node in Html.SiteMapPath()) { %>
				<% if (node.Item2 == SiteMapNodeType.Current) { %>
					<li class="active"><%: CurrentCase.Title %></li>
				<% } else { %>
					<li>
						<a href="<%: node.Item1.Url %>"><%: node.Item1.Title %></a>
					</li>
				<% } %>
			<% } %>
		</ul>
	</asp:PlaceHolder>
</asp:Content>

<asp:Content ContentPlaceHolderID="PageHeader" runat="server">
	<asp:Panel ID="CaseHeader" CssClass="page-header" runat="server">
		<asp:Panel ID="CaseControls" CssClass="pull-right btn-toolbar case-controls" runat="server">
			<asp:Panel ID="ResolveCase" CssClass="btn-group" runat="server">
				<a href="#resolve-case" class="btn btn-success" data-toggle="modal">
					<adx:Snippet runat="server" SnippetName="cases/editcase/resolvebuttontext" DefaultText="Resolve Case" Literal="true" EditType="text"/>
				</a>
			</asp:Panel>
			<asp:Panel ID="CancelCase" CssClass="btn-group" runat="server">
				<a href="#cancel-case" class="btn btn-danger" data-toggle="modal">
					<adx:Snippet runat="server" SnippetName="cases/editcase/cancelbuttontext" DefaultText="Cancel Case" Literal="true" EditType="text"/>
				</a>
			</asp:Panel>
			<asp:Panel ID="AddNote" CssClass="btn-group" runat="server">
				<a href="#add-note" class="btn btn-default" data-toggle="modal">
					<span class="fa fa-plus-circle" aria-hidden="true"></span>
					<adx:Snippet runat="server" SnippetName="cases/editcase/addnote/buttontext" DefaultText="Add Note" Literal="true" EditType="text"/>
				</a>
			</asp:Panel>
			<asp:Panel ID="ReopenCase" CssClass="btn-group" runat="server">
				<a href="#reopen-case" class="btn btn-default" data-toggle="modal">
					<span class="fa fa-undo" aria-hidden="true"></span>
					<adx:Snippet runat="server" SnippetName="cases/editcase/reopenbuttontext" DefaultText="Reopen Case" Literal="true" EditType="text"/>
				</a>
			</asp:Panel>
		</asp:Panel>
		<h1><%: CurrentCase.Title %> <asp:Label ID="TicketNumber" runat="server"><small class="ticket-number"><%: CurrentCase.TicketNumber %></small></asp:Label></h1>
	</asp:Panel>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server" ViewStateMode="Enabled">
	
	<%: Html.Attribute("adx_copy") %>
		
	<asp:ScriptManagerProxy runat="server">
		<Scripts>
			<asp:ScriptReference Path="~/js/jquery.validate.min.js" />
		</Scripts>
	</asp:ScriptManagerProxy>
	
	<script type="text/javascript">
		$(document).ready(function () {
			$("#content_form").validate({
				errorClass: "help-block error",
				highlight: function (label) {
					$(label).closest('.form-group').removeClass('has-success').addClass('has-error');
				},
				success: function (label) {
					$(label).closest('.form-group').removeClass('has-error').addClass('has-success');
				}
			});
			$(document).on("click", "#ResolveCaseButton", function (e) {
				var isValid = $("#content_form").valid();
				if (!isValid) {
					e.preventDefault();
				}
			});
			$(".crmEntityFormView select[disabled]").replaceWith(function () {
				return $("<span />").append($(this).find("option:selected").text());
			});
			$(".crmEntityFormView textarea[readonly]").replaceWith(function () {
				var content = replaceUrlsWithHtmlLinks($(this).val()).replace( /\n/g , "<br />");
				return $("<p />").html(content);
			});
			function replaceUrlsWithHtmlLinks(text) {
				var exp = /(\b(https?|ftp):\/\/[-A-Z0-9+&@#\/%?=~_|!:,.;]*[-A-Z0-9+&@#\/%=~_|])/ig;
				return text.replace(exp, "<a href='$1'>$1</a>");
			}
		});
	</script>

	<asp:Panel ID="NoCaseAccess" Visible="false" CssClass="alert alert-block alert-danger" runat="server">
		<adx:Snippet runat="server" SnippetName="cases/editcase/nopermissions" DefaultText="You do not have permission to view this case." Editable="true" EditType="html"/>
	</asp:Panel>

	<asp:Panel ID="CaseNotFound" Visible="false" CssClass="alert alert-block alert-danger" runat="server">
		<adx:Snippet runat="server" SnippetName="cases/editcase/casenotfound" DefaultText="The case could not be found." Editable="true" EditType="html"/>
	</asp:Panel>

	<asp:Panel ID="CaseData" CssClass="case" runat="server">
		<section class="modal" id="resolve-case" tabindex="-1" role="dialog" aria-labelledby="resolve-case-modal-label" aria-hidden="true">
			<div class="modal-dialog modal-lg">
				<div class="modal-content">
					<div class="modal-header">
						<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
						<h1 class="modal-title h4" id="resolve-case-modal-label">
							<adx:Snippet  runat="server" SnippetName="cases/editcase/resolvebuttontext" DefaultText="Resolve Case" Editable="true" EditType="text"/>
						</h1>
					</div>
					<div class="modal-body form-horizontal">
						<div class="form-group">
							<crm:CrmMetadataDataSource ID="SatisfactionSource" runat="server" EntityName="incident" AttributeName="customersatisfactioncode" CrmDataContextName="<%$ SiteSetting: Language Code %>" />
							<asp:Label AssociatedControlID="Satisfaction" CssClass="col-sm-3 control-label required" runat="server">
								<adx:Snippet runat="server" SnippetName="cases/editcase/satisfaction" DefaultText="Satisfaction" />
							</asp:Label>
							<div class="col-sm-9">
								<asp:DropDownList ID="Satisfaction" runat="server"
									CssClass="form-control required"
									DataSourceID="SatisfactionSource"
									DataTextField="OptionLabel"
									DataValueField="OptionValue" />
							</div>
						</div>
						<div class="form-group">
							<asp:Label AssociatedControlID="Resolution" CssClass="col-sm-3 control-label required" runat="server">
								<adx:Snippet runat="server" SnippetName="cases/editcase/resolution" DefaultText="Resolution" />
							</asp:Label>
							<div class="col-sm-9">
								<asp:TextBox runat="server" ID="Resolution" CssClass="form-control required" />
							</div>
						</div>
						<div class="form-group">
							<asp:Label AssociatedControlID="ResolutionDescription" CssClass="col-sm-3 control-label required" runat="server">
								<adx:Snippet runat="server" SnippetName="cases/editcase/resolution/description" DefaultText="Description" />
							</asp:Label>
							<div class="col-sm-9">
								<asp:TextBox runat="server" ID="ResolutionDescription" TextMode="MultiLine" Rows="6" CssClass="form-control required" Text='<%# CurrentCase.Resolution %>'/>
							</div>
						</div>
					</div>
					<div class="modal-footer">
						<asp:Button ID="ResolveCaseButton" CssClass="btn btn-primary" OnClick="ResolveCase_Click" Text='<%$ Snippet: cases/editcase/resolvebuttontext, Resolve Case %>' ClientIDMode="Static" runat="server" />
						<button class="btn btn-default" data-dismiss="modal" aria-hidden="true">
							<adx:Snippet  runat="server" SnippetName="cases/editcase/resolvecancelbuttontext" DefaultText="Cancel" Literal="True" EditType="text"/>
						</button>
					</div>
				</div>
			</div>
		</section>
		
		<section class="modal" id="cancel-case" tabindex="-1" role="dialog" aria-labelledby="cancel-case-modal-label" aria-hidden="true">
			<div class="modal-dialog">
				<div class="modal-content">
					<div class="modal-header">
						<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
						<h1 class="modal-title h4" id="cancel-case-modal-label">
							<adx:Snippet runat="server" SnippetName="cases/editcase/cancelbuttontext" DefaultText="Cancel Case" Editable="true" EditType="text"/>
						</h1>
					</div>
					<div class="modal-body">
						<adx:Snippet runat="server" SnippetName="cases/editcase/cancelconfirmation" DefaultText="Are you sure you want to cancel this case?" Editable="true" EditType="html"/>
					</div>
					<div class="modal-footer">
						<asp:Button CssClass="btn btn-primary" OnClick="CancelCase_Click" Text='<%$ Snippet: cases/editcase/cancelconfirmationyes, Cancel this case %>' runat="server" />
						<button class="btn btn-default" data-dismiss="modal" aria-hidden="true">
							<adx:Snippet runat="server" SnippetName="cases/editcase/cancelconfirmationno" DefaultText="No, don't cancel this case" Literal="true" EditType="text"/>
						</button>
					</div>
				</div>
			</div>
		</section>
		
		<section class="modal" id="reopen-case" tabindex="-1" role="dialog" aria-labelledby="reopen-case-modal-label" aria-hidden="true">
			<div class="modal-dialog">
				<div class="modal-content">
					<div class="modal-header">
						<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
						<h1 class="modal-title h4" id="reopen-case-modal-label">
							<adx:Snippet  runat="server" SnippetName="cases/editcase/reopenbuttontext" DefaultText="Reopen Case" Editable="true" EditType="text"/>
						</h1>
					</div>
					<div class="modal-body">
						<adx:Snippet  runat="server" SnippetName="cases/editcase/reopenconfirmation" DefaultText="Are you sure you want to reopen this case?" Editable="true" EditType="html"/>
					</div>
					<div class="modal-footer">
						<asp:Button CssClass="btn btn-primary" OnClick="ReopenCase_Click" Text='<%$ Snippet: cases/editcase/reopenconfirmationyes, Reopen this case %>' runat="server" />
						<button class="btn btn-default" data-dismiss="modal" aria-hidden="true">
							<adx:Snippet  runat="server" SnippetName="cases/editcase/reopenconfirmationno" DefaultText="No, don't reopen this case" Literal="true" EditType="text"/>
						</button>
					</div>
				</div>
			</div>
		</section>
		
		<section class="modal" id="add-note" tabindex="-1" role="dialog" aria-labelledby="add-note-modal-label" aria-hidden="true">
			<div class="modal-dialog modal-lg">
				<div class="modal-content">
					<div class="modal-header">
						<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
						<h1 class="modal-title h4" id="add-note-modal-label">
							<adx:Snippet  runat="server" SnippetName="cases/editcase/addnote/buttontext" DefaultText="Add Note" Editable="true" EditType="text"/>
						</h1>
					</div>
					<div class="modal-body form-horizontal">
						<div class="form-group">
							<asp:Label AssociatedControlID="NewNoteText" CssClass="col-sm-3 control-label" runat="server">
								<adx:Snippet runat="server" SnippetName="cases/editcase/addnote/text" DefaultText="Note" />
							</asp:Label>
							<div class="col-sm-9">
								<asp:TextBox runat="server" ID="NewNoteText" TextMode="MultiLine" Rows="6" CssClass="form-control"/>
							</div>
						</div>
						<div class="form-group">
							<asp:Label AssociatedControlID="NewNoteText" CssClass="col-sm-3 control-label" runat="server">
								<adx:Snippet runat="server" SnippetName="cases/editcase/addnote/file" DefaultText="Attach a file" />
							</asp:Label>
							<div class="col-sm-9">
								<div class="form-control-static">
									<asp:FileUpload ID="NewNoteAttachment" runat="server"/>
								</div>
							</div>
						</div>
					</div>
					<div class="modal-footer">
						<asp:Button CssClass="btn btn-primary" OnClick="AddNote_Click" Text='<%$ Snippet: cases/editcase/addnote/buttontext, Add Note %>' runat="server" />
						<button class="btn btn-default" data-dismiss="modal" aria-hidden="true">
							<adx:Snippet  runat="server" SnippetName="cases/editcase/addnote/cancelbuttontext" DefaultText="Cancel" Literal="True" EditType="text"/>
						</button>
					</div>
				</div>
			</div>
		</section>
		
		<asp:Panel ID="CaseInfo" CssClass="case-info" runat="server">
			<div class="status pull-right">
				<span class="label label-default <%: string.IsNullOrEmpty(CurrentCase.CaseTypeLabel) ? "hide" : string.Empty %>"><%: CurrentCase.CaseTypeLabel %></span>
				<span class="label <%= CurrentCase.IsActive ? "label-info" : string.Empty %> <%= CurrentCase.IsResolved ? "label-success" : string.Empty %> <%= CurrentCase.IsCanceled ? "label-important" : string.Empty %>"><%: CurrentCase.StateLabel %> &ndash; <%: CurrentCase.StatusLabel %></span>
			</div>
			<div class="opened-by">
				<asp:HyperLink ID="UserAvatar" CssClass="user-avatar" NavigateUrl='<%# Url.AuthorUrl(CurrentCase) %>' ImageUrl='<%# Url.UserImageUrl(CurrentCase) %>' ToolTip='<%# HttpUtility.HtmlEncode(CurrentCase.ResponsibleContactName ?? "") %>' runat="server"/>
				Opened <abbr class="timeago"><%: CurrentCase.CreatedOn.ToString("r") %></abbr>
				<asp:Label ID="UserName" runat="server">
					by <asp:HyperLink NavigateUrl='<%# Url.AuthorUrl(CurrentCase) %>' Text='<%# HttpUtility.HtmlEncode(CurrentCase.ResponsibleContactName ?? "") %>' runat="server"/>
				</asp:Label>
			</div>
		</asp:Panel>
		
		<asp:Panel ID="UpdateSuccessMessage" runat="server" CssClass="alert alert-success alert-block" Visible="False">
			<a class="close" data-dismiss="alert" href="#">&times;</a>
			<adx:Snippet runat="server" SnippetName="Case Update Success Text" DefaultText="Your case has been updated successfully." Editable="true" EditType="html" />
		</asp:Panel>

		<asp:Panel ID="CaseExtendedInfo" Visible="True" runat="server">
			<asp:Panel ID="PublicForm" CssClass="crmEntityFormView readonly" runat="server" Visible="false">
				<adx:CrmEntityFormView runat="server" ID="PublicFormView"
					EntityName="incident"
					FormName="Public Case Web Form"
					ShowUnsupportedFields="False"
					AutoGenerateSteps="False"
					Mode="ReadOnly"
					DataBindOnPostBack="True"
					LanguageCode="<%$ SiteSetting: Language Code, 0 %>"
					ContextName="<%$ SiteSetting: Language Code %>">
				</adx:CrmEntityFormView>
			</asp:Panel>
			
			<asp:Panel ID="PrivateOpenCaseForm" runat="server" CssClass="crmEntityFormView well" Visible="false">
				<adx:CrmEntityFormView runat="server" ID="PrivateOpenCaseFormView"
					EntityName="incident" FormName="Private Open Case Web Form"
					RecommendedFieldsRequired="True"
					ShowUnsupportedFields="False"
					ToolTipEnabled="False" Mode="Edit"
					AutoGenerateSteps="False"
					ValidationGroup="PrivateOpenCase"
					ValidationSummaryCssClass="alert alert-danger alert-block"
					SubmitButtonCssClass="btn btn-primary button submit"
					SubmitButtonText="Update"
					OnItemUpdated="OnItemUpdated"
					DataBindOnPostBack="True"
					LanguageCode="<%$ SiteSetting: Language Code, 0 %>"
					ContextName="<%$ SiteSetting: Language Code %>">
				</adx:CrmEntityFormView>
			</asp:Panel>
			
			<asp:Panel ID="PrivateClosedCaseForm" CssClass="crmEntityFormView readonly" runat="server" Visible="false">
				<adx:CrmEntityFormView runat="server" ID="PrivateClosedCaseFormView"
					EntityName="incident" FormName="Private Closed Case Web Form"
					ShowUnsupportedFields="False"
					AutoGenerateSteps="False"
					Mode="ReadOnly"
					DataBindOnPostBack="True"
					LanguageCode="<%$ SiteSetting: Language Code, 0 %>"
					ContextName="<%$ SiteSetting: Language Code %>">
				</adx:CrmEntityFormView>
			</asp:Panel>
		</asp:Panel>
		
		<asp:Panel ID="Notes" CssClass="notes" runat="server">
			<div class="page-header">
				<h3>
					<adx:Snippet runat="server" SnippetName="Case Notes Header" DefaultText="Notes" Editable="true" EditType="html"/>
				</h3>
			</div>
			<asp:ObjectDataSource ID="NoteDataSource" TypeName="Adxstudio.Xrm.Cases.ICaseDataAdapter" OnObjectCreating="GetCurrentCaseDataAdapter" SelectMethod="SelectNotes" runat="server" />
			<asp:ListView DataSourceID="NoteDataSource" runat="server">
				<LayoutTemplate>
					<asp:PlaceHolder ID="itemPlaceholder" runat="server"/>
				</LayoutTemplate>
				<ItemTemplate>
					<div class="note" runat="server">
						<div class="row">
							<div class="col-sm-3 metadata">
								<p><abbr class="timeago"><%# Eval("CreatedOn", "{0:r}") %></abbr></p>
							</div>
							<div class="col-sm-9">
								<div class="text">
									<%# AnnotationHelper.FormatNoteText(Eval("NoteText") as string) %>
								</div>
								<asp:Panel Visible='<%# Eval("FileAttachment") != null %>' CssClass="attachment alert alert-block alert-info" runat="server">
									<span class="fa fa-file" aria-hidden="true"></span>
									<asp:HyperLink NavigateUrl='<%# Eval("FileAttachment.Url") %>' Text='<%# HttpUtility.HtmlEncode(string.Format("{0} ({1:1})", Eval("FileAttachment.FileName"), Eval("FileAttachment.FileSize"))) %>' runat="server"/>
								</asp:Panel>
							</div>
						</div>
					</div>
				</ItemTemplate>
				<EmptyDataTemplate>
					<div class="alert alert-block alert-info">
						<adx:Snippet runat="server" SnippetName="No Case Notes Message" DefaultText="<p>There are no notes associated with this case. Notes are not public, and are a good place to post private information related to a case.</p>" Editable="true" EditType="html"/>
					</div>
				</EmptyDataTemplate>
			</asp:ListView>
			<asp:Panel ID="AddNoteInline" CssClass="row" runat="server">
				<div class="col-sm-offset-3 col-sm-9">
					<a href="#add-note" class="btn btn-default" data-toggle="modal">
						<span class="fa fa-plus-circle" aria-hidden="true"></span>
						<adx:Snippet runat="server" SnippetName="cases/editcase/addnote/buttontext" DefaultText="Add Note" Literal="true" EditType="text"/>
					</a>
				</div>
			</asp:Panel>
		</asp:Panel>
	</asp:Panel>
</asp:Content>