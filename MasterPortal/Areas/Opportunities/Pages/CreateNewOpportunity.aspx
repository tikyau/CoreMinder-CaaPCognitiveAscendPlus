<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/WebForms.master" CodeBehind="CreateNewOpportunity.aspx.cs" Inherits="Site.Areas.Opportunities.Pages.CreateNewOpportunity" %>

<asp:Content ContentPlaceHolderID="ContentBottom" ViewStateMode="Enabled" runat="server">
	<adx:Snippet ID="NoOpportunityPermissionsRecordError" SnippetName="CreateOpp/NoOpportunityPermissionsRecordError" DefaultText="You do not have opportunity permissions. Permission to create opportunities is denied." Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-danger" runat="server"/>
	<adx:Snippet ID="OpportunityPermissionsError" SnippetName="CreateOpp/OpportunityPermissionsError" DefaultText="Your opportunity permissions deny create rights. Permission to create opportunities is denied." Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-danger" runat="server"/>
	<div id="create-opportunity-section">
		<asp:Panel ID="OpportunityDetailsPanel" runat="server">
			<div class="form-group">
				<asp:Label ID="InstructionTxt" AssociatedControlID="Account_dropdown" runat="server">
					<adx:Snippet ID="SelectClientMessage" runat="server" SnippetName="CreateOpp/SelectClientMsg" DefaultText="Select a customer account as the potential client for the opportunity:" />
				</asp:Label>
				<div class="input-group">
					<asp:DropDownList runat="server" ID="Account_dropdown" ClientIDMode="Static" AppendDataBoundItems="true" CssClass="form-control">
						<asp:ListItem Selected="True" Text="- Accounts -" Value="" />
					</asp:DropDownList>
					<div class="input-group-btn">
						<asp:LinkButton ID="CreateCustomerButton"  
							runat="server" 
							CssClass="btn btn-success" 
							OnClick="CreateCustomerButton_Click" ><span class="fa fa-plus-circle" aria-hidden="true"></span>
							<asp:Literal runat="server" Text="<%$ Snippet: CreateOpp/CreateNewCustomer, Create New Customer %>"/>
						</asp:LinkButton>
						<asp:LinkButton ID="ManageCustomersButton" Visible="false"  
							runat="server" 
							CssClass="btn btn-default" 
							OnClick="ManageCustomerButton_Click" >
							<asp:Literal runat="server" Text="<%$ Snippet: CreateOpp/ManageCustomersButtonLabel, Manage Customers %>"/>
						</asp:LinkButton>
					</div>
				</div>
			</div>
			<adx:Snippet ID="NoManagingPartnerCustomerAccountsMessage" SnippetName="CreateOpp/NoManagingPartnerCustomerAccountsMessage" DefaultText="No partner customer accounts exist that you manage. Please create a new customer to continue." Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-warning" runat="server"/>
			<adx:Snippet ID="NoPrimaryContactOnManagingPartnerCustomerAccountsMessage" SnippetName="CreateOpp/NoPrimaryContactOnManagingPartnerCustomerAccountsMessage" DefaultText="The customer account(s) you manage do not have a Primary Contact. Either create a new customer account or click 'Manage Customers' and edit an existing account to be associated with the new opportunity and assign a Primary Contact to continue." Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-warning" runat="server"/>
			<adx:Snippet ID="NoChannelPermissionsRecordError" SnippetName="CreateOpp/NoChannelPermissionsRecordError" DefaultText="You do not have channel permissions. Permission to create customer accounts is denied." Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-warning" runat="server"/>
			<adx:Snippet ID="ChannelPermissionsError" SnippetName="CreateOpp/ChannelPermissionsError" DefaultText="Your channel permissions deny create rights. Permission to create customer accounts is denied." Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-warning" runat="server"/>
			<adx:Snippet ID="NoParentAccountError" SnippetName="CreateOpp/NoParentAccountError" DefaultText="A parent customer account has not been assigned to you. Permission to create customer accounts is denied." Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-warning" runat="server"/>
			<adx:Snippet ID="ParentAccountClassificationCodeError" SnippetName="CreateOpp/ParentAccountClassificationCodeError" DefaultText="<p>The parent customer account assigned to you has an invalid Classification Code. Permission to create customer accounts is denied.</p><p>Account Classification Code must be set to 'Partner'.</p>" Editable="true" EditType="html" Visible="false" CssClass="alert alert-block alert-warning" runat="server"/>
			<div id="createOpportunity" style="display: none;">
				<adx:CrmDataSource ID="WebFormDataSource" runat="server" CrmDataContextName="<%$ SiteSetting: Language Code %>" />
				<adx:CrmEntityFormView ID="createOpp" runat="server" DataSourceID="WebFormDataSource" EntityName="opportunity" FormName="Opportunity Create Form"
					ValidationGroup="CreateOpportunity" OnItemInserting="OnItemInserting" CssClass="crmEntityFormView" OnItemInserted="OnItemInserted" Mode="Insert" LanguageCode="<%$ SiteSetting: Language Code, 0 %>" ContextName="<%$ SiteSetting: Language Code %>">
					<InsertItemTemplate>
						<div class="actions">
							<asp:Button Text='<%$ Snippet: CreateOpp/Submit, Submit %>' CssClass="btn btn-primary" CommandName="Insert" CausesValidation="true" ValidationGroup="CreateOpportunity" runat="server" />
						</div>
					</InsertItemTemplate>
				</adx:CrmEntityFormView>
			</div>
		</asp:Panel>
	</div>
	<script type="text/javascript">
		$(function () {
			if ($("#Account_dropdown").val()) {
				$("#createOpportunity").show();
			} else {
				$("#createOpportunity").hide();
			}

			$("#Account_dropdown").change(function () {
				if ($(this).val()) {
					$("#createOpportunity").show("slide");
				} else {
					$("#createOpportunity").hide("slide");
				}
			});
		});
	</script>
</asp:Content>
