<%@ Control Language="C#" ViewStateMode="Enabled" EnableViewState="true" AutoEventWireup="true" CodeBehind="CaseDeflection.ascx.cs" Inherits="Site.Areas.HelpDesk.Controls.CaseDeflection" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="Adxstudio.Xrm.Web.Mvc.Html" %>

<div class="case-deflection">
	<div class="content-panel panel panel-default">
		<div class="panel-heading">
			<h4>
				<span class="fa fa-search" aria-hidden="true"></span>
				<adx:Snippet runat="server" SnippetName="cases/casedeflection/header" DefaultText="What can we help you with?" Editable="true" EditType="text"/>
			</h4>
		</div>
		<div class="panel-body">
			<asp:DropDownList ID="Product" CssClass="product form-control" runat="server"/>
			<div class="input-group">
				<asp:Panel ID="ClearSearchButton" Visible="False" CssClass="input-group-btn" runat="server">
					<asp:HyperLink CssClass="btn btn-default" ToolTip='<%$ Snippet: cases/casedeflection/searchresetbuttontext, Clear %>' NavigateUrl='<%$ CrmSiteMap: Current, Url %>' runat="server">
						<span class="fa fa-times" aria-hidden="true"></span>
					</asp:HyperLink>
				</asp:Panel>
				<asp:TextBox ID="Subject" CssClass="subject form-control" placeholder="e.g. User login is failing" runat="server"/>
				<div class="input-group-btn">
					<asp:LinkButton ID="SearchButton" CssClass="btn btn-default" ToolTip="<%$ Snippet: cases/casedeflection/searchbuttontext, Search %>" OnClick="Submit_Click" runat="server">
						<span class="fa fa-search" aria-hidden="true"></span>
					</asp:LinkButton>
				</div>
			</div>
		</div>

		<asp:PlaceHolder ID="Deflection" Visible="False" runat="server">
			<adx:SearchDataSource ID="SearchData" Query="(+_logicalname:incident~0.9^2 +statecode:@resolvedincidentstatecode +(@subject)) OR (-_logicalname:incident~0.9 +(@subject))" LogicalNames="incident,adx_issue,adx_webpage,adx_communityforumthread,adx_communityforumpost,adx_blogpost,kbarticle" OnSelected="SearchData_OnSelected" runat="server">
				<SelectParameters>
					<asp:QueryStringParameter Name="subject" QueryStringField="subject"/>
					<asp:QueryStringParameter Name="product" QueryStringField="product"/>
					<asp:Parameter Name="resolvedincidentstatecode" DefaultValue="1" />
					<asp:Parameter Name="PageNumber" DefaultValue="1" />
					<asp:Parameter Name="PageSize" DefaultValue="10" />
				</SelectParameters>
			</adx:SearchDataSource>
			<asp:ListView DataSourceID="SearchData" ID="SearchResults" runat="server">
				<LayoutTemplate>
					<ul class="list-group">
						<asp:PlaceHolder ID="itemPlaceholder" runat="server" />
					</ul>
				</LayoutTemplate>
				<ItemTemplate>
						<li class="list-group-item" runat="server">
							<h4 class="list-group-item-heading"><asp:HyperLink Text='<%# Eval("Title") %>' NavigateUrl='<%# Eval("Url") %>' runat="server" /></h4>
							<p class="list-group-item-text search-results fragment"><%# Eval("Fragment") %></p>
							<div class="content-metadata">
							<asp:Label Visible='<%# (string)Eval("EntityLogicalName") == "adx_communityforum" || (string)Eval("EntityLogicalName") == "adx_communityforumthread" || (string)Eval("EntityLogicalName") == "adx_communityforumpost" %>' CssClass="label" Text="Forums" runat="server"/>
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
				<EmptyDataTemplate>
					<ul class="list-group">
						<li class="list-group-item">
							<%: Html.HtmlSnippet("cases/casedeflection/noresultsmessage", defaultValue: "No information matching your query was found.") %>
						</li>
					</ul>
				</EmptyDataTemplate>
			</asp:ListView>
			
			<div class="panel-footer">
				<asp:PlaceHolder ID="NoCaseAccessWarning" Visible="False" runat="server">
					<asp:LoginView runat="server">
						<AnonymousTemplate>
							<div class="alert alert-block alert-info">
								<div class="pull-right">
									<% Html.RenderPartial("SignInLink"); %>
								</div>
								<adx:Snippet runat="server" SnippetName="cases/access/create/signin" DefaultText="You must sign in to open a new support request." Editable="true" EditType="html"/>
							</div>
						</AnonymousTemplate>
						<LoggedInTemplate>
							<div class="alert alert-block alert-warning">
								<adx:Snippet runat="server" SnippetName="cases/access/create/nopermissions" DefaultText="You do not have permission to create cases." Editable="true" EditType="html"/>
							</div>
						</LoggedInTemplate>
					</asp:LoginView>
				</asp:PlaceHolder>
				<asp:Button ID="OpenNewSupportRequest" CssClass="btn btn-block btn-lg btn-primary" runat="server" Text="<%$ Snippet: cases/casedeflection/createbuttontext, Open a New Support Request %>" OnClick="OpenNewSupportRequest_OnClick" />
			</div>
		</asp:PlaceHolder>
	</div>
</div>