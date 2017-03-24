<%@ Page  Language="C#" MasterPageFile="../MasterPages/Blogs.master" AutoEventWireup="true" ValidateRequest="false" CodeBehind="Search.aspx.cs" Inherits="Site.Areas.Blogs.Pages.Search" %>
<%@ Import Namespace="Microsoft.Security.Application" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
	<div class="search-results">
		<adx:SearchDataSource ID="SearchData" LogicalNames="adx_blog,adx_blogpost,adx_blogpostcomment" OnSelected="SearchData_OnSelected" runat="server">
			<SelectParameters>
				<asp:QueryStringParameter Name="Query" QueryStringField="q" />
				<asp:QueryStringParameter Name="PageNumber" QueryStringField="page" />
				<asp:Parameter Name="PageSize" DefaultValue="10" />
			</SelectParameters>
		</adx:SearchDataSource>
		
		<asp:Repeater DataMember="Info" DataSourceID="SearchData" runat="server">
			<ItemTemplate>
				<asp:Panel Visible='<%# ((int)Eval("Count")) > 0 %>' runat="server">
					<h2>Results <%# Eval("FirstResultNumber") %>&ndash;<%# Eval("LastResultNumber") %> of <%# Eval("ApproximateTotalHits") %> for query <em class="querytext"><%# AntiXss.HtmlEncode((Eval("[Query]") ?? string.Empty) as string) %></em></h2>
					
					<asp:ListView DataSourceID="SearchData" ID="SearchResults" runat="server">
						<LayoutTemplate>
							<ul>
								<asp:PlaceHolder ID="itemPlaceholder" runat="server" />
							</ul>
						</LayoutTemplate>
						<ItemTemplate>
							<li runat="server">
								<h3><asp:HyperLink Text='<%# Eval("Title") %>' NavigateUrl='<%# Eval("Url") %>' runat="server" /></h3>
								<p class="fragment"><%# Eval("Fragment") %></p>
								<asp:HyperLink Text='<%# GetDisplayUrl(Eval("Url")) %>' NavigateUrl='<%# Eval("Url") %>' runat="server" />
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
					<h2>No results found for query <em class="querytext"><%# AntiXss.HtmlEncode((Eval("[Query]") ?? string.Empty) as string) %></em></h2>
				</asp:Panel>
			</ItemTemplate>
		</asp:Repeater>
	</div>
</asp:Content>
