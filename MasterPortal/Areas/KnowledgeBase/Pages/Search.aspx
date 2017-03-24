<%@ Page Language="C#" MasterPageFile="~/MasterPages/WebFormsContent.master" AutoEventWireup="true" CodeBehind="Search.aspx.cs" Inherits="Site.Areas.KnowledgeBase.Pages.Search" %>
<%@ OutputCache CacheProfile="User" %>
<%@ Import Namespace="Microsoft.Security.Application" %>
<%@ Register src="../Controls/Search.ascx" tagname="Search" tagprefix="adx" %>

<asp:Content ContentPlaceHolderID="Head" runat="server">
	<link rel="stylesheet" href="<%: Url.Content("~/Areas/KnowledgeBase/css/knowledgebase.css") %>">
</asp:Content>

<asp:Content ContentPlaceHolderID="SidebarTop" runat="server"/>

<asp:Content ContentPlaceHolderID="ContentBottom" runat="server">
	<adx:Search runat="server" />
	
	<div class="kb-search-results search-results">
		<adx:SearchDataSource ID="SearchData" LogicalNames="kbarticle" OnSelected="SearchData_OnSelected" runat="server">
			<SelectParameters>
				<asp:QueryStringParameter Name="Query" QueryStringField="kbquery" />
				<asp:QueryStringParameter Name="PageNumber" QueryStringField="page" />
				<asp:Parameter Name="PageSize" DefaultValue="10" />
			</SelectParameters>
		</adx:SearchDataSource>
		
		<asp:Repeater DataMember="Info" DataSourceID="SearchData" runat="server">
			<ItemTemplate>
				<asp:Panel Visible='<%# ((int)Eval("Count")) > 0 %>' runat="server">
					<div class="page-header">
						<h3>Results <%# Eval("FirstResultNumber") %>&ndash;<%# Eval("LastResultNumber") %> of <%# Eval("ApproximateTotalHits") %> for query <em class="querytext"><%# AntiXss.HtmlEncode((Eval("[Query]") ?? string.Empty).ToString()) %></em></h3>
					</div>
					
					<asp:ListView DataSourceID="SearchData" ID="SearchResults" OnDataBound="SearchResults_DataBound" runat="server">
						<LayoutTemplate>
							<ul>
								<asp:PlaceHolder ID="itemPlaceholder" runat="server" />
							</ul>
						</LayoutTemplate>
						<ItemTemplate>
							<li runat="server">
								<h4 class="title">
									<asp:HyperLink Text='<%# Eval("Title") %>' NavigateUrl='<%# Eval("Url") %>' runat="server" />
								</h4>
								<p class="fragment"><%# Eval("Fragment") %></p>
							</li>
						</ItemTemplate>
					</asp:ListView>
					
					<adx:UnorderedListDataPager ID="SearchResultPager" CssClass="pagination" PagedControlID="SearchResults" QueryStringField="page" PageSize="10" runat="server">
						<Fields>
							<adx:ListItemNextPreviousPagerField ShowNextPageButton="false" ShowFirstPageButton="True" FirstPageText="&laquo;" PreviousPageText="&lsaquo;" />
							<adx:ListItemNumericPagerField ButtonCount="10" PreviousPageText="&hellip;" NextPageText="&hellip;" />
							<adx:ListItemNextPreviousPagerField ShowPreviousPageButton="false" ShowLastPageButton="True" LastPageText="&raquo;" NextPageText="&rsaquo;" />
						</Fields>
					</adx:UnorderedListDataPager>
				</asp:Panel>
				<asp:Panel Visible='<%# ((int)Eval("Count")) == 0 && ((int)Eval("PageNumber")) == 1 %>' runat="server">
					<h3>No results found for query <em class="querytext"><%# AntiXss.HtmlEncode((Eval("[Query]") ?? string.Empty).ToString()) %></em></h3>
				</asp:Panel>
			</ItemTemplate>
		</asp:Repeater>
	</div>
</asp:Content>
