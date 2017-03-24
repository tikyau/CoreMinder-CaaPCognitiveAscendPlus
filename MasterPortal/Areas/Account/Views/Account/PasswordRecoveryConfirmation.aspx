<%@ Page Language="C#" MasterPageFile="../Shared/Account.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
	<div class="form-horizontal">
		<fieldset>
			<legend><%: Html.SnippetLiteral("Account/PasswordRecovery/ConfirmationHeading", "Reset your password") %></legend>
			<div class="alert alert-success">
				<%: Html.SnippetLiteral("Account/PasswordRecovery/SuccessText", "Your password has been sent to your email address.") %>
			</div>
		</fieldset>
	</div>
</asp:Content>
