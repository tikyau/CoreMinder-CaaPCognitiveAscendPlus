<%@ Page Language="C#" AutoEventWireup="True" MasterPageFile="~/MasterPages/WebForms.master" ValidateRequest="false" CodeBehind="Search.aspx.cs" Inherits="Site.Pages.Search" %>
<%@ OutputCache CacheProfile="User" %>
<%@ Import Namespace="Microsoft.Security.Application"%>

<asp:Content ContentPlaceHolderID="PageHeader" runat="server"/>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
	<div class="search-results">
		<adx:SearchDataSource ID="SearchData" Query='<%$ SiteSetting: search/query %>' OnSelected="SearchData_OnSelected" runat="server">
			<SelectParameters>
				<asp:QueryStringParameter Name="Query" QueryStringField="q" />
				<asp:QueryStringParameter Name="LogicalNames" QueryStringField="filter" />
				<asp:QueryStringParameter Name="PageNumber" QueryStringField="page" />
				<asp:Parameter Name="PageSize" DefaultValue="10" />
			</SelectParameters>
		</adx:SearchDataSource>
		
		<asp:Repeater DataMember="Info" DataSourceID="SearchData" runat="server">
			<ItemTemplate>
				<asp:Panel Visible='<%# ((int)Eval("Count")) > 0 %>' runat="server">
					<h2>Results <%# Eval("FirstResultNumber") %>&ndash;<%# Eval("LastResultNumber") %> of <%# Eval("ApproximateTotalHits") %> for query <em class="querytext"><%# AntiXss.HtmlEncode((Eval("[Query]") ?? string.Empty).ToString()) %></em></h2>
					
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
								<div>
									<asp:Label Visible='<%# (string)Eval("EntityLogicalName") == "adx_communityforum" || (string)Eval("EntityLogicalName") == "adx_communityforumthread" || (string)Eval("EntityLogicalName") == "adx_communityforumpost" %>' CssClass="label label-default" Text="Forums" runat="server"/>
									<asp:Label Visible='<%# (string)Eval("EntityLogicalName") == "adx_blog" || (string)Eval("EntityLogicalName") == "adx_blogpost" || (string)Eval("EntityLogicalName") == "adx_blogpostcomment" %>' CssClass="label label-info" Text="Blogs" runat="server"/>
									<asp:Label Visible='<%# (string)Eval("EntityLogicalName") == "adx_event" || (string)Eval("EntityLogicalName") == "adx_eventschedule" %>' CssClass="label label-info" Text="Events" runat="server"/>
									<asp:Label Visible='<%# (string)Eval("EntityLogicalName") == "adx_idea" %>' CssClass="label label-success" Text="Ideas" runat="server"/>
									<asp:Label Visible='<%# (string)Eval("EntityLogicalName") == "adx_issue" %>' CssClass="label label-danger" Text="Issues" runat="server"/>
									<asp:Label Visible='<%# (string)Eval("EntityLogicalName") == "incident" %>' CssClass="label label-warning" Text="Help Desk" runat="server"/>
									<asp:Label Visible='<%# (string)Eval("EntityLogicalName") == "kbarticle" %>' CssClass="label label-info" Text="Knowledge Base" runat="server"/>
									<asp:HyperLink Text='<%# GetDisplayUrl(Eval("Url")) %>' NavigateUrl='<%# Eval("Url") %>' runat="server" />
								</div>
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
					<h2>No results found for query <em class="querytext"><%# AntiXss.HtmlEncode((Eval("[Query]") ?? string.Empty).ToString()) %></em></h2>
				</asp:Panel>
			</ItemTemplate>
		</asp:Repeater>
	</div>
</asp:Content>
