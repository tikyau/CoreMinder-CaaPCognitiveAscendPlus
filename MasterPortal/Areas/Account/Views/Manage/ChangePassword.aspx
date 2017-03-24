<%@ Page Language="C#" MasterPageFile="Manage.Master" Inherits="System.Web.Mvc.ViewPage<Site.Areas.Account.ViewModels.ChangePasswordViewModel>" %>

<asp:Content ContentPlaceHolderID="PageCopy" runat="server">
	<%: Html.HtmlSnippet("Account/ChangePassword/PageCopy", "page-copy") %>
</asp:Content>

<asp:Content ContentPlaceHolderID="ProfileNavbar" runat="server">
	<% Html.RenderPartial("ProfileNavbar"); %>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
	<% using (Html.BeginForm("ChangePassword", "Manage", new { area = "Account" })) { %>
		<%: Html.AntiForgeryToken() %>
		<div class="form-horizontal">
			<fieldset>
				<legend><%: Html.TextSnippet("Account/ChangePassword/ChangePasswordFormHeading", defaultValue: "Change Password", tagName: "span") %></legend>
				<%: Html.ValidationSummary(string.Empty, new {@class = "alert alert-block alert-danger"}) %>
				<% if (ViewBag.Settings.LocalLoginByEmail) { %>
					<div class="form-group">
						<label class="col-sm-4 control-label" for="Email"><%: Html.TextSnippet("Account/ChangePassword/Email", defaultValue: "Email", tagName: "span") %></label>
						<div class="col-sm-8">
							<%: Html.TextBoxFor(model => model.Email, new { @class = "form-control", @readonly = "readonly" }) %>
						</div>
					</div>
				<% } else { %>
					<div class="form-group">
						<label class="col-sm-4 control-label" for="Username"><%: Html.TextSnippet("Account/ChangePassword/Username", defaultValue: "Username", tagName: "span") %></label>
						<div class="col-sm-8">
							<%: Html.TextBoxFor(model => model.Username, new { @class = "form-control", @readonly = "readonly" }) %>
						</div>
					</div>
				<% } %>
				<div class="form-group">
					<label class="col-sm-4 control-label" for="OldPassword"><%: Html.TextSnippet("Account/ChangePassword/OldPassword", defaultValue: "Old Password", tagName: "span") %></label>
					<div class="col-sm-8">
						<%: Html.PasswordFor(model => model.OldPassword, new { @class = "form-control" }) %>
					</div>
				</div>
				<div class="form-group">
					<label class="col-sm-4 control-label" for="NewPassword"><%: Html.TextSnippet("Account/ChangePassword/NewPassword", defaultValue: "New Password", tagName: "span") %></label>
					<div class="col-sm-8">
						<%: Html.PasswordFor(model => model.NewPassword, new { @class = "form-control" }) %>
					</div>
				</div>
				<div class="form-group">
					<label class="col-sm-4 control-label" for="ConfirmPassword"><%: Html.TextSnippet("Account/ChangePassword/ConfirmPassword", defaultValue: "Confirm Password", tagName: "span") %></label>
					<div class="col-sm-8">
						<%: Html.PasswordFor(model => model.ConfirmPassword, new { @class = "form-control" }) %>
					</div>
				</div>
				<div class="form-group">
					<div class="col-sm-offset-4 col-sm-8">
						<button class="btn btn-primary"><%: Html.SnippetLiteral("Account/ChangePassword/ChangePasswordButtonText", "Change Password") %></button>
					</div>
				</div>
			</fieldset>
		</div>
	<% } %>
</asp:Content>
