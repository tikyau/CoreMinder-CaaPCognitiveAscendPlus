<%@ Page Language="C#" MasterPageFile="~/MasterPages/WebLinkNavigation.master" AutoEventWireup="true" CodeBehind="WebLinkNavigationPage.aspx.cs" Inherits="Site.Pages.WebLinkNavigationPage" %>
<%@ Register src="~/Controls/Comments.ascx" tagname="Comments" tagprefix="site" %>
<%@ Register src="~/Controls/MultiRatingControl.ascx" tagname="MultiRatingControl" tagprefix="site" %>
<%@ OutputCache CacheProfile="User" %>

<asp:Content ContentPlaceHolderID="ContentBottom" runat="server">
	<div class="page-metadata clearfix">
		<adx:Snippet SnippetName="Social Share Widget Code Page Bottom" EditType="text" DefaultText="" HtmlTag="Div" runat="server"/>
	</div>
	<site:MultiRatingControl ID="MultiRatingControl" RatingType="rating" IsPanel="True" runat="server" />
	<site:Comments RatingType="vote" EnableRatings="False" runat="server" />
</asp:Content>