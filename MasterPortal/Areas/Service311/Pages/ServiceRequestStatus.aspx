<%@ Page Language="C#" MasterPageFile="~/MasterPages/WebFormsContent.master" AutoEventWireup="true" CodeBehind="ServiceRequestStatus.aspx.cs" Inherits="Site.Areas.Service311.Pages.ServiceRequestStatus" %>

<asp:Content runat="server" ContentPlaceHolderID="Head">
	<link rel="stylesheet" href="<%: Url.Content("~/Areas/Service311/css/311.css") %>" />
</asp:Content>

<asp:Content ContentPlaceHolderID="ContentBottom" runat="server">
	<div id="service-request-status">
		<asp:Panel runat="server" ID="ErrorPanel" CssClass="alert alert-block alert-danger">
			<adx:Snippet ID="ErrorMessage" runat="server" SnippetName="311 Service Request Status Error Message" EditType="html" Editable="true" DefaultText="<h4>Service request not found</h4><p>Please check that you have entered your reference number correctly. You may call 311 at any time to inquire about the status of your service request.</p>" ClientIDMode="Static" />
		</asp:Panel>
	</div>
</asp:Content>
