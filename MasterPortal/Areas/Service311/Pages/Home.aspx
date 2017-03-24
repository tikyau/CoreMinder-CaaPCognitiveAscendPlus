<%@ Page Language="C#" MasterPageFile="~/MasterPages/WebFormsContent.master" AutoEventWireup="True" CodeBehind="Home.aspx.cs" Inherits="Site.Areas.Service311.Pages.Home" %>
<%@ OutputCache CacheProfile="User" %>
<%@ Import Namespace="Adxstudio.Xrm.Web.Mvc.Html" %>

<asp:Content ContentPlaceHolderID="Head" runat="server">
	<link rel="stylesheet" href="<%: Url.Content("~/Areas/Service311/css/311.css") %>" />
</asp:Content>

<asp:Content ContentPlaceHolderID="Scripts" runat="server">
	<script src="<%: Url.Content("~/Areas/Service311/js/service311.js") %>"></script>
</asp:Content>

<asp:Content ContentPlaceHolderID="ContentBottom" ViewStateMode="Enabled" runat="server">
	<asp:ListView ID="ServiceRequestsListView" runat="server">
		<LayoutTemplate>
			<div class="service311 row">
				<asp:PlaceHolder ID="itemPlaceholder" runat="server"/>
			</div>
		</LayoutTemplate>
		<ItemTemplate>
			<div class="col-xs-6 col-sm-4 col-md-3">
				<a class="well thumbnail service text-center" href="<%# Eval("Url") %>">
					<asp:Image ImageUrl='<%# GetThumbnailUrl(Eval("Entity")) %>' runat="server"/>
					<h6><%# Eval("Title") %></h6>
				</a>
			</div>
		</ItemTemplate>
	</asp:ListView>
	<br/>
	<div class="page-metadata clearfix">
		<adx:Snippet SnippetName="Social Share Widget Code Page Bottom" EditType="text" HtmlTag="Div" DefaultText="" runat="server"/>
	</div>
</asp:Content>

<asp:Content ContentPlaceHolderID="SidebarTop" runat="server">
	<div id="status-tracker" class="content-panel panel panel-default">
		<div class="panel-heading">
			<h4>
				<span class="fa fa-info-circle" aria-hidden="true"></span>
				<adx:Snippet runat="server" SnippetName="311 Service Request Status Tracker Title" EditType="text" Editable="true" DefaultText="Track Your Service Request" />
			</h4>
		</div>
		<div class="panel-body">
			<div class="input-group">
				<input id="ServiceRequestTrackingNumber" class="form-control" type="text" placeholder="Service Request #" onkeypress="return ADX.service311.disableEnterKey(event);" />
				<div class="input-group-btn">
					<a href="#" class="btn btn-default" onclick="<%= string.Format("ADX.service311.checkStatus('ServiceRequestTrackingNumber', '{0}', 'refnum');", Html.SiteMarkerUrl("check-status")) %>"><span class="fa fa-search" aria-hidden="true"></span></a>
				</div>
			</div>
		</div>
	</div>
	<div class="content-panel panel panel-default">
		<div class="panel-heading">
			<h4>
				<span class="fa fa-map-marker" aria-hidden="true"></span>
				<adx:Snippet runat="server" SnippetName="311 Home Service Requests Map Heading Text" EditType="text" Editable="true" DefaultText="Service Requests Map" />
			</h4>
		</div>
		<div class="panel-body">
			<div class="content-caption">
				<adx:Snippet runat="server" SnippetName="311 Home View Service Requests Map Caption" EditType="html" Editable="true" DefaultText="<p>Service requests can be searched for and rendered on a map.</p>" />			
			</div>
			<a class="btn btn-info btn-block btn-lg" href="<%= Html.SiteMarkerUrl("Service Requests Map") %>"><span class="fa fa-map-marker" aria-hidden="true"></span> <adx:Snippet runat="server" SnippetName="311 Home View Service Requests Map Button" EditType="text" Editable="true" DefaultText="View Map" /></a>
		</div>
	</div>
</asp:Content>

<asp:Content ContentPlaceHolderID="SidebarBottom" runat="server">
	<asp:Panel ID="TwitterFeedPanel" runat="server">
		<adx:Snippet SnippetName="311 Home Twitter Widget" EditType="text" Literal="True" DefaultText="311 Home Twitter Widget" runat="server"/>
	</asp:Panel>
</asp:Content>
