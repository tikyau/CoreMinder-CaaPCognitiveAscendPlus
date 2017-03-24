<%@ Page Language="C#" MasterPageFile="~/MasterPages/WebForms.master" AutoEventWireup="true" CodeBehind="ManageParentAccount.aspx.cs" Inherits="Site.Areas.AccountManagement.Pages.ManageParentAccount" %>
<%@ OutputCache CacheProfile="User" %>
<%@ Import Namespace="Adxstudio.Xrm.Web.Mvc.Html" %>

<asp:Content  ContentPlaceHolderID="ContentBottom" ViewStateMode="Enabled" runat="server">
	<div class="row">
		<div class="col-md-8">
			<adx:Snippet ID="NoParentAccountError" SnippetName="accountmanagement/edit/NoParentAccountError" DefaultText="A parent account has not been assigned to you. Permission to manage parent account is denied." Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-danger" runat="server"/>
			<adx:Snippet ID="NoAccountAccessPermissionsRecordError" SnippetName="accountmanagement/edit/NoAccountAccessPermissionsRecordError" DefaultText="You do not have account access permissions. Permission to manage parent account is denied." Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-danger" runat="server"/>
			<adx:Snippet ID="AccountAccessPermissionsError" SnippetName="accountmanagement/edit/AccountAccessPermissionsError" DefaultText="Your account access permissions deny read and deny write rights. Permission to manage parent account is denied." Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-danger" runat="server"/>
			<adx:Snippet ID="NoAccountAccessPermissionsForParentAccountError" SnippetName="accountmanagement/edit/NoAccountAccessPermissionsForParentAccountError" DefaultText="You do not have account access permissions for your parent account. Permission to manage parent account is denied." Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-danger" runat="server"/>
			<adx:Snippet ID="AccountAccessWritePermissionDeniedMessage" SnippetName="accountmanagement/edit/AccountAccessWritePermissionDeniedMessage" DefaultText="Your account access permissions grant read rights but deny write rights. Permission to edit parent account is denied." Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-warning" runat="server"/>
			<adx:Snippet ID="UpdateSuccessMessage" SnippetName="accountmanagement/edit/UpdateSuccessMessage" DefaultText="Account information has been updated successfully." Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-success" runat="server" />
			<asp:Panel ID="AccountInformation" CssClass="content-panel panel panel-default" runat="server">
				<div class="panel-body">
					<asp:Panel ID="AccountEditForm" CssClass="crmEntityFormView" runat="server">
						<adx:CrmEntityFormView runat="server" 
							ID="AccountEditFormView" EntityName="account" 
							FormName="Account Web Form" 
							OnItemUpdating="OnItemUpdating" 
							OnItemUpdated="OnItemUpdated" 
							ValidationGroup="UpdateAccount" 
							RecommendedFieldsRequired="True" 
							ShowUnsupportedFields="False" 
							ToolTipEnabled="False" 
							ClientIDMode="Static"
							Mode="Edit"
							LanguageCode="<%$ SiteSetting: Language Code, 0 %>"
							ContextName="<%$ SiteSetting: Language Code %>">
							<UpdateItemTemplate>
							</UpdateItemTemplate>
						</adx:CrmEntityFormView>
						<div class="actions">
							<asp:Button ID="UpdateAccountButton" Text='<%$ Snippet: accountmanagement/edit/UpdateAccountButtonLabel, Update %>' CssClass="btn btn-primary" OnClick="UpdateAccountButton_Click" CausesValidation="true" ValidationGroup="UpdateAccount" runat="server" />
						</div>
					</asp:Panel>
					<asp:Panel ID="AccountReadOnlyForm" CssClass="crmEntityFormView" runat="server" Visible="false">
						<adx:CrmEntityFormView runat="server" 
							ID="AccountReadOnlyFormView"
							EntityName="account" 
							FormName="Account Web Form" 
							Mode="ReadOnly"
							LanguageCode="<%$ SiteSetting: Language Code, 0 %>"
							ContextName="<%$ SiteSetting: Language Code %>">
						</adx:CrmEntityFormView>
					</asp:Panel>
				</div>
			</asp:Panel>
		</div>
	</div>
	<asp:Panel ID="ContactsList" runat="server">
		<div class="row">
			<div class="col-md-12">
				<div class="content-panel panel panel-default">
					<div class="panel-heading">
						<h4>
							<span class="fa fa-user" aria-hidden="true"></span>
							<adx:Snippet runat="server" SnippetName="accountmanagement/edit/ContactsListHeading" DefaultText="Account Contacts" Editable="true" EditType="text" />
						</h4>
					</div>
					<div class="panel-body">
						<div class="content-caption">
							<asp:LinkButton ID="CreateContactButton" runat="server" CssClass="btn btn-success pull-right" OnClick="CreateContactButton_Click" >
								<span class="fa fa-plus-circle" aria-hidden="true"></span>
								<%: Html.SnippetLiteral("accountmanagement/edit/CreateNewContactButtonLabel", "Create New") %>
							</asp:LinkButton>
						</div>
						<adx:Snippet ID="AccountContactsListLabel" CssClass="content-caption" runat="server" SnippetName="accountmanagement/edit/ContactsListCaption" DefaultText="The following contacts are associated with this account:" Editable="true" EditType="html" />
						<asp:GridView ID="AccountContactsList" runat="server" CssClass="table table-striped" GridLines="None" AlternatingRowStyle-CssClass="alternate-row" AllowSorting="true" OnSorting="AccountContactsList_Sorting" OnRowDataBound="AccountContactsList_OnRowDataBound" >
							<EmptyDataRowStyle CssClass="empty" />
							<EmptyDataTemplate>
								<adx:Snippet runat="server" SnippetName="accountmanagement/edit/ContactsListEmptyMessage" DefaultText="There are no contacts to display." Editable="true" EditType="html" />
							</EmptyDataTemplate>
						</asp:GridView>
						<adx:Snippet ID="NoContactAccessPermissionsRecordMessage" SnippetName="accountmanagement/edit/NoContactAccessPermissionsRecordMessage" DefaultText="You do not have contact access permissions. Permission to manage parent account's contacts is denied." Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-warning" runat="server"/>
						<adx:Snippet ID="ContactAccessPermissionsMessage" SnippetName="accountmanagement/edit/ContactAccessPermissionsMessage" DefaultText="Your contact access permissions deny read rights. Permission to manage parent account's contacts is denied." Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-warning" runat="server"/>
						<adx:Snippet ID="NoContactAccessPermissionsForParentAccountMessage" SnippetName="accountmanagement/edit/NoContactAccessPermissionsForParentAccountMessage" DefaultText="You do not have contact access permissions for your parent account with scope set to account. Permission to manage parent account's contacts is denied." Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-warning" runat="server"/>
					</div>
				</div>
			</div>
		</div>
	</asp:Panel>
</asp:Content>

<asp:Content ContentPlaceHolderID="Scripts" runat="server">
	<script type="text/javascript">
		$(function () {
			$(".table tr").not(":has(th)").click(function () {
				window.location.href = $(this).find("a").attr("href");
			});
			$("form").submit(function () {
				blockUI();
			});
			$(".table th a").click(function () {
				blockUI();
			});
		});
		function blockUI() {
			$.blockUI({ message: null, overlayCSS: { opacity: .3 } });
		}
	</script>
</asp:Content>


