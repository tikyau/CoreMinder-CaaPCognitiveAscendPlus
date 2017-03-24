<%@ Page Title="" Language="C#" MasterPageFile="Manage.Master" Inherits="System.Web.Mvc.ViewPage<Site.Areas.Account.ViewModels.ChangeEmailViewModel>" %>

<asp:Content ContentPlaceHolderID="PageCopy" runat="server">
	<%: Html.HtmlSnippet("Account/ChangeEmail/PageCopy", "page-copy") %>
</asp:Content>

<asp:Content ContentPlaceHolderID="ProfileNavbar" runat="server">
	<% Html.RenderPartial("ProfileNavbar"); %>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
	<% using (Html.BeginForm("ChangeEmail", "Manage")) { %>
		<%: Html.AntiForgeryToken() %>
		<div class="form-horizontal">
			<fieldset>
				<legend><%: Html.TextSnippet("Account/ChangeEmail/ChangeEmailFormHeading", defaultValue: "Change Email", tagName: "span") %></legend>
				<%: Html.ValidationSummary(string.Empty, new {@class = "alert alert-block alert-danger"}) %>
				<div class="form-group">
					<label class="col-sm-2 control-label" for="Email"><%: Html.TextSnippet("Account/ChangeEmail/EmailLabel", defaultValue: "Email", tagName: "span") %></label>
					<div class="col-sm-10">
						<%: Html.TextBoxFor(model => model.Email, new { @class = "form-control" }) %>
					</div>
				</div>
				<div class="form-group">
					<div class="col-sm-offset-2 col-sm-10">
						<button class="btn btn-primary"><span class="fa fa-envelope-o" aria-hidden="true"></span> <%: Html.SnippetLiteral("Account/ChangeEmail/ChangeEmailButtonText", ViewBag.Settings.EmailConfirmationEnabled ? "Change and Confirm Email" : "Change Email") %></button>
					</div>
				</div>
			</fieldset>
		</div>
	<% } %>
</asp:Content>
