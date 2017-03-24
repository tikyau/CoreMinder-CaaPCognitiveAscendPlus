<%@ Page Language="C#" MasterPageFile="~/MasterPages/Profile.master" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="Site.Pages.ChangePassword" %>
<%@ Import namespace="Adxstudio.Xrm.Web.Mvc.Html" %>

<asp:Content ContentPlaceHolderID="ContentBottom" runat="server">
	<asp:Panel ID="ChangePasswordPanel" runat="server">
		<asp:ChangePassword ID="ChangePasswordControl" runat="server" MembershipProvider="Xrm" EnableViewState="false" RenderOuterTable="false">
			<ChangePasswordTemplate>
				<div class="form-horizontal">
					<fieldset>
						<asp:ValidationSummary CssClass="alert alert-danger alert-block" ValidationGroup="ChangePassword" runat="server" />
						<div class="form-group">
							<label class="col-sm-4 control-label required" for="CurrentPassword"><adx:Snippet runat="server" SnippetName="Profile Change Password Current Password Label" DefaultText="Current Password"/></label>
							<div class="col-sm-8">
								<asp:TextBox ID="CurrentPassword" CssClass="form-control" runat="server" textMode="Password"></asp:TextBox>
								<asp:RequiredFieldValidator ID="CurrentPasswordRequired" runat="server" ControlToValidate="CurrentPassword" ValidationGroup="ChangePassword" Display="None" ErrorMessage="Current Password is a required field." Text="" CssClass="help-inline error"></asp:RequiredFieldValidator>
							</div>
						</div>
						<div class="form-group">
							<label class="col-sm-4 control-label required" for="NewPassword"><adx:Snippet runat="server" SnippetName="Profile Change Password New Password Label" DefaultText="New Password"/></label>
							<div class="col-sm-8">
								<asp:TextBox ID="NewPassword" CssClass="form-control" runat="server" textMode="Password"></asp:TextBox>
								<asp:RequiredFieldValidator ID="NewPasswordRequired" runat="server" ControlToValidate="NewPassword" ValidationGroup="ChangePassword" Display="None" ErrorMessage="New Password is a required field." Text="" CssClass="help-inline error"></asp:RequiredFieldValidator>
							</div>
						</div>
						<div class="form-group">
							<label class="col-sm-4 control-label required" for="ConfirmNewPassword"><adx:Snippet runat="server" SnippetName="Profile Change Password Confirm New Password Label" DefaultText="Confirm New Password"/></label>
							<div class="col-sm-8">
								<asp:TextBox ID="ConfirmNewPassword" CssClass="form-control" runat="server" textMode="Password"></asp:TextBox>
								<asp:RequiredFieldValidator ID="ConfirmNewPasswordRequired" runat="server" ControlToValidate="ConfirmNewPassword" ValidationGroup="ChangePassword" Display="None" ErrorMessage="Confirm New Password is a required field." Text="" CssClass="help-inline error"></asp:RequiredFieldValidator>
								<asp:CompareValidator ID="PasswordCompare" runat="server" ControlToCompare="ConfirmNewPassword" ControlToValidate="NewPassword" ValidationGroup="ChangePassword" Display="None" ErrorMessage="The New Password and Confirm New Password must match." Text="" CssClass="help-inline error"/>
							</div>
						</div>
						<div class="form-group">
							<div class="col-sm-offset-4 col-sm-8">
								<asp:Button ID="CancelChangePasswordButton" CommandName="Cancel" runat="server" CausesValidation="False" CssClass="pull-right btn btn-default" Text="<%$ Snippet: Profile Change Password Cancel Button Label, Cancel %>"></asp:Button>
							<asp:Button ID="ChangePasswordButton" CommandName="ChangePassword" runat="server" ValidationGroup="ChangePassword" CssClass="btn btn-primary" Text="<%$ Snippet: Profile Change Password Button Label, Change Password %>"></asp:Button>
							</div>
						</div>
						<div class="form-group">
							<div class="col-sm-offset-4 col-sm-8">
								<div class="help-block error"><asp:Literal ID="FailureText" runat="server"/></div>
							</div>
						</div>
					</fieldset>
				</div>
			</ChangePasswordTemplate>
			<SuccessTemplate>
				<div class="form-horizontal">
					<fieldset>
						<legend><adx:Snippet runat="server" SnippetName="Profile Change Password Title" DefaultText="Change Password" /></legend>
						<div class="alert alert-block alert-success">
							<adx:Snippet runat="server" SnippetName="Profile Change Password Success Message" DefaultText="Your password has been changed successfully." Editable="True" EditType="html" />
						</div>
					</fieldset>
				</div>
			</SuccessTemplate>
		</asp:ChangePassword>
	</asp:Panel>
	<asp:Panel ID="FederatedIdentityMessage" Visible="False" CssClass="alert alert-block alert-info" runat="server">
		<%: Html.HtmlSnippet("Profile Change Password Federated Identity Message", defaultValue: "<p>You are signed in using an identity provider, rather than a local account. Please update your password through your identity provider, if applicable.</p>") %>
	</asp:Panel>
</asp:Content>
