<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WebFormSupportRequestSelectOrder.ascx.cs" Inherits="Site.Areas.HelpDesk.Controls.WebFormSupportRequestSelectOrder" %>

<adx:Snippet runat="server" SnippetName="Help Desk Select Order Message" DefaultText="<h3>Support Plan</h3><p>Please select the support package you would like to purchase:</p>" Editable="true" EditType="html"/>
<asp:RadioButtonList ID="PlanPackageList" runat="server" CssClass="radio-list" RepeatLayout="Flow">
</asp:RadioButtonList>
<asp:RequiredFieldValidator runat="server"
	ID="PlanPackageListRequiredFieldValidator"
	ControlToValidate="PlanPackageList"
	Display="Static"
	ErrorMessage="<%$ Snippet: Help Desk Select Order Required Text, Please select a support package %>"
	CssClass = "help-block error">
</asp:RequiredFieldValidator>