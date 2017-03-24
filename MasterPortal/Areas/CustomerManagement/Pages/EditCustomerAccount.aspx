<%@ Page Language="C#" MasterPageFile="~/MasterPages/WebForms.master" AutoEventWireup="true" CodeBehind="EditCustomerAccount.aspx.cs" Inherits="Site.Areas.CustomerManagement.Pages.EditCustomerAccount" %>

<asp:Content  ContentPlaceHolderID="ContentBottom" ViewStateMode="Enabled" runat="server">
	<adx:Snippet ID="RecordNotFoundError" SnippetName="customermanagement/account/edit/RecordNotFoundError" DefaultText="Customer Account could not be found. Ensure a valid customer account ID has been specified." Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-danger" runat="server"/>
	<adx:Snippet ID="NoChannelPermissionsRecordError" SnippetName="customermanagement/account/edit/NoChannelPermissionsRecordError" DefaultText="You do not have channel permissions. Permission to edit customer accounts is denied." Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-danger" runat="server"/>
	<adx:Snippet ID="ChannelPermissionsError" SnippetName="customermanagement/account/edit/ChannelPermissionsError" DefaultText="Your channel permissions deny write rights. Permission to edit customer accounts is denied." Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-danger" runat="server"/>
	<adx:Snippet ID="NoParentAccountError" SnippetName="customermanagement/account/edit/NoParentAccountError" DefaultText="A parent customer account has not been assigned to you. Permission to edit customer accounts is denied." Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-danger" runat="server"/>
	<adx:Snippet ID="NoChannelPermissionsForParentAccountError" SnippetName="customermanagement/account/edit/NoChannelPermissionsForParentAccountError" DefaultText="You do not have channel permissions for your parent customer account. Permission to edit customer accounts is denied." Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-danger" runat="server"/>
	<adx:Snippet ID="ParentAccountClassificationCodeError" SnippetName="customermanagement/account/edit/ParentAccountClassificationCodeError" DefaultText="<p>The parent customer account assigned to you has an invalid Classification Code. Permission to edit customer accounts is denied.</p><p>Account Classification Code must be set to 'Partner'.</p>" Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-danger" runat="server"/>
	<adx:Snippet ID="SuccessMessage" SnippetName="customermanagement/account/edit/SuccessMessage" DefaultText="Account information has been updated successfully." Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-success" runat="server"/>
	<asp:Panel ID="EditAccountForm" CssClass="panel panel-default" runat="server">
		<div class="panel-body">
			<asp:Panel ID="AccountForm" CssClass="crmEntityFormView" runat="server" Visible="true">
				<adx:CrmEntityFormView runat="server" 
					ID="FormView" 
					EntityName="account" 
					FormName="Account Web Form" 
					OnItemUpdating="OnItemUpdating" 
					OnItemUpdated="OnItemUpdated" 
					ValidationGroup="UpdateAccount" 
					RecommendedFieldsRequired="True" 
					ShowUnsupportedFields="False" 
					ToolTipEnabled="False" 
					Mode="Edit"
					LanguageCode="<%$ SiteSetting: Language Code, 0 %>"
					ContextName="<%$ SiteSetting: Language Code %>">
					<UpdateItemTemplate>
					</UpdateItemTemplate>
				</adx:CrmEntityFormView>
				<div class="well">
					<adx:Snippet ID="NoContactsExistWarningMessage" SnippetName="customermanagement/account/edit/CreateContactMessage" DefaultText="No contacts exist yet. To set the Primary Contact, please create the appropriate contact for this customer account first, then assign the Primary Contact." Editable="true" EditType="html"  Visible="false" CssClass="alert alert-block alert-warning" runat="server"/>
					<asp:Label ID="PrimaryContactLabel" AssociatedControlID="PrimaryContactList" runat="server">
						<adx:Snippet runat="server" SnippetName="customermanagement/account/edit/PrimaryContactFieldLabel" DefaultText="Set the Primary Contact for this Account:" />
					</asp:Label>
					<asp:DropDownList ID="PrimaryContactList" runat="server" ClientIDMode="Static" CssClass="form-control" />
				</div>
				<div class="actions">
					<asp:Button ID="UpdateAccountButton" 
						Text='<%$ Snippet: customermanagement/account/edit/AccountUpdateButtonLabel, Update %>' 
						CssClass="btn btn-primary" 
						OnClick="UpdateAccountButton_Click" 
						ValidationGroup="UpdateAccount" 
						runat="server" />
				</div>
			</asp:Panel>
		</div>
	</asp:Panel>
	<asp:Panel ID="ContactsList" CssClass="panel panel-default" runat="server">
		<div class="panel-heading">
			<div class="pull-right">
				<adx:SiteMarkerLinkButton ID="CreateContactButton" SiteMarkerName="Create Customer Contact" runat="server" CssClass="btn btn-success btn-xs">
					<span class="fa fa-plus-circle" aria-hidden="true"></span>
					<adx:Snippet runat="server" SnippetName="customermanagement/account/edit/CreateContactButtonLabel" DefaultText="Create New" Editable="true" EditType="text" />
				</adx:SiteMarkerLinkButton>
			</div>
			<h4 class="panel-title">
				<span class="fa fa-user" aria-hidden="true"></span>
				<adx:Snippet runat="server" SnippetName="customermanagement/account/edit/ContactsListTitle" DefaultText="Account Contacts" Editable="true" EditType="text" />
			</h4>
		</div>
		<div class="panel-body">
			<div id="account-contacts">
				<asp:GridView ID="AccountContactsList" runat="server" CssClass="table table-striped" GridLines="None"  AlternatingRowStyle-CssClass="alternate-row" AllowSorting="true" OnSorting="AccountContactsList_Sorting" OnRowDataBound="AccountContactsList_OnRowDataBound" >
					<EmptyDataRowStyle CssClass="empty" />
					<EmptyDataTemplate>
						<adx:Snippet runat="server" SnippetName="customermanagement/account/edit/ContactsListEmptyText" DefaultText="There are no contact records to display." Editable="true" EditType="html" />
					</EmptyDataTemplate>
				</asp:GridView>
			</div>
		</div>
	</asp:Panel>
</asp:Content>

<asp:Content ContentPlaceHolderID="Scripts" runat="server">
	<script type="text/javascript">
		$(function () {
			$("form").submit(function () {
				if (Page_IsValid) {
					$.blockUI({ message: null, overlayCSS: { opacity: .3 } });
				}
			});
		});
	</script>
</asp:Content>
