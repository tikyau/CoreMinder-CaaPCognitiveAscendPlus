<%@ Page Title="" Language="C#" MasterPageFile="Account.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ContentPlaceHolderID="PageCopy" runat="server">
	<%: Html.HtmlSnippet("Account/ExternalLoginFailure/PageCopy", "page-copy") %>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
	<div class="alert alert-danger">
		<%: Html.HtmlSnippet("Account/SignIn/ExternalLoginFailureText", defaultValue: "Unable to authenticate with the external account provider.") %>
	</div>
	<% Html.RenderPartial("SignInLink", new ViewDataDictionary(ViewData) { { "class", "btn btn-primary" } }); %>
</asp:Content>
