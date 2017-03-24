<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WebFormServiceRequestKbArticles.ascx.cs" Inherits="Site.Areas.Service311.Controls.WebFormServiceRequestKbArticles" %>
<%@ Import Namespace="Microsoft.Xrm.Sdk" %>
<asp:Panel ID="PanelKbArticles" runat="server" CssClass="row">
	<asp:ListView ID="LatestArticlesList" runat="server">
		<LayoutTemplate>
			<div class="activity activity-grid">
				<div class="header">
					<h2>
						<adx:Snippet runat="server" SnippetName="311 Service Request Latest Knowledge Base Articles Title Text" DefaultText="Latest Knowledge Base Articles" />
					</h2>
				</div>
				<ul>
					<asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
				</ul>
		</LayoutTemplate>
		<ItemTemplate>
			<li>
				<asp:HyperLink NavigateUrl='<%# GetKbArticleUrl((Entity)Container.DataItem) %>' Text='<%# HtmlEncode(Eval("Title")) %>' runat="server" />
			</li>
		</ItemTemplate>
		<EmptyDataTemplate>
			<adx:Snippet runat="server" SnippetName="311 Service Request Latest Knowledge Base Articles Empty Text" DefaultText="There are no articles to display." />
		</EmptyDataTemplate>
	</asp:ListView>
</asp:Panel>