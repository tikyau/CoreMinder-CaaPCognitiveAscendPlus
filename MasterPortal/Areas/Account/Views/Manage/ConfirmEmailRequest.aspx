<%@ Page Title="" Language="C#" MasterPageFile="Manage.Master" Inherits="System.Web.Mvc.ViewPage<Site.Areas.Account.ViewModels.RegisterViewModel>" %>

<asp:Content ContentPlaceHolderID="PageCopy" runat="server">
	<%: Html.HtmlSnippet("Account/ConfirmEmailRequest/PageCopy", "page-copy") %>
</asp:Content>

<asp:Content ContentPlaceHolderID="ProfileNavbar" runat="server">
	<% Html.RenderPartial("ProfileNavbar"); %>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
	<div class="form-horizontal">
		<fieldset>
			<legend><%: Html.TextSnippet("Account/ConfirmEmail/ConfirmEmailFormHeading", defaultValue: "Confirm Email", tagName: "span") %></legend>
			<%: Html.Partial("ProfileMessage", Request["Message"] ?? string.Empty) %>
			<div class="alert alert-info">
				<span class="fa fa-inbox" aria-hidden="true"></span> <%: Html.TextSnippet("Account/ConfirmEmail/ConfirmEmailInstructionsText", defaultValue: "Please check your email for confirmation instructions.", tagName: "span") %>
			</div>
			<div class="form-group">
				<label class="col-sm-2 control-label" for="Email"><%: Html.TextSnippet("Account/ConfirmEmail/EmailLabel", defaultValue: "Email", tagName: "span") %></label>
				<div class="col-sm-10">
					<%: Html.TextBoxFor(model => model.Email, new { @class = "form-control", @readonly = "readonly" }) %>
				</div>
			</div>
		</fieldset>
	</div>
	<% if (ViewBag.Settings.IsDemoMode && ViewBag.DemoModeLink != null) { %>
		<div class="panel panel-warning">
			<div class="panel-heading">
				<h3 class="panel-title"><span class="fa fa-wrench" aria-hidden="true"></span> DEMO MODE <small>LOCAL ONLY</small></h3>
			</div>
			<div class="panel-body">
				<a class="btn btn-default" href="<%: ViewBag.DemoModeLink %>"><%: Html.SnippetLiteral("Account/ConfirmEmail/ConfirmEmailButtonText", "Confirm Email") %></a>
			</div>
		</div>
	<% } %>
</asp:Content>
