<%@ Page Language="C#" MasterPageFile="~/MasterPages/WebFormsContent.master" AutoEventWireup="True" CodeBehind="Page.aspx.cs" Inherits="Site.Pages.Page" ValidateRequest="false" EnableEventValidation="false" %>
<%@ Register src="~/Controls/Comments.ascx" tagname="Comments" tagprefix="site" %>
<%@ Register src="~/Controls/MultiRatingControl.ascx" tagname="MultiRatingControl" tagprefix="site" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ OutputCache CacheProfile="User" %>

<asp:Content ContentPlaceHolderID="ContentBottom" runat="server">
	<div class="page-metadata clearfix">
		<adx:Snippet SnippetName="Social Share Widget Code Page Bottom" EditType="text" HtmlTag="Div" DefaultText="" runat="server"/>
	</div>
	<site:Comments RatingType="vote" EnableRatings="False" runat="server" />
</asp:Content>

<asp:Content ContentPlaceHolderID="SidebarBottom" runat="server">
	<site:MultiRatingControl ID="MultiRatingControl" RatingType="rating" IsPanel="True" runat="server" />
	<% Html.RenderAction("PollPlacement", "Poll", new {Area = "Cms", id = "Sidebar", __portalScopeId__ = Website.Id}); %>
	<% Html.RenderAction("AdPlacement", "Ad", new { Area = "Cms", id = "Sidebar Bottom", __portalScopeId__ = Website.Id }); %>
</asp:Content>