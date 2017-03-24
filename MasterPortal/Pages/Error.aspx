<%@ Page Language="C#" MasterPageFile="~/MasterPages/WebForms.master" AutoEventWireup="true" CodeBehind="Error.aspx.cs" Inherits="Site.Pages.Error" %>

<asp:Content ContentPlaceHolderID="ContentBottom" runat="server">
	<asp:Panel ID="FederationError" Visible="false" runat="server">
		<h2>Details</h2>
		<asp:GridView ID="ErrorDetails" runat="server" AutoGenerateColumns="true" />
		<h2>Errors</h2>
		<asp:GridView ID="ErrorList" runat="server" AutoGenerateColumns="true" />
	</asp:Panel> 

	<b><asp:Label ID="AuthorizeNetErrorMessage" runat="server" Visible="False"></asp:Label></b> 
</asp:Content>
