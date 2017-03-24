<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/WebForms.master" AutoEventWireup="true" CodeBehind="RedemptionRequestDetails.aspx.cs" Inherits="Site.Areas.WeChat.Pages.RedemptionRequestDetails" %>

<%@ Register Src="~/Controls/ChildNavigation.ascx" TagName="ChildNavigation" TagPrefix="site" %>
<%@ Register Src="~/Controls/Comments.ascx" TagName="Comments" TagPrefix="site" %>
<%@ OutputCache CacheProfile="User" %>

<%--<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <adx:Snippet SnippetName="Social Share Widget Code Page Bottom" EditType="text" DefaultText="" HtmlTag="Div" runat="server" />
    <adx:CrmEntityListView runat="server" ID="ListView" CssClass="table-responsive" ListCssClass="table table-striped" ClientIDMode="Static" AlternatingRowStyle-CssClass="alternate-row" SelectMode="Multiple" LanguageCode="<%$ SiteSetting: Language Code, 0 %>" PortalName="<%$ SiteSetting: Language Code %>"></adx:CrmEntityListView>      
</asp:Content>--%>

<asp:Content ContentPlaceHolderID="EntityControls" runat="server">
    <div class="col-sm-2">
    <asp:Image ID=redeemQrImage CssClass="img-responsive" runat="server"  /> 
    </div>
    <div class="col-md-8">       
    <adx:EntityForm ID="EntityFormControl" runat="server" FormCssClass="crmEntityFormView readonly" PreviousButtonCssClass="btn btn-default" NextButtonCssClass="btn btn-primary" SubmitButtonCssClass="btn btn-primary" ClientIDMode="Static" LanguageCode="<%$ SiteSetting: Language Code, 0 %>" PortalName="<%$ SiteSetting: Language Code %>" />
    </div>    
    <site:ChildNavigation ID="ChildNavigation1" ShowDescriptions="true" runat="server" />
    <site:Comments RatingType="vote" EnableRatings="False" runat="server" />
</asp:Content>



