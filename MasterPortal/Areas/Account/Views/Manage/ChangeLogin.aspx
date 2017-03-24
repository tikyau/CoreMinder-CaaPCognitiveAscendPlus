<%@ Page Title="" Language="C#" MasterPageFile="Manage.Master" Inherits="System.Web.Mvc.ViewPage<Site.Areas.Account.ViewModels.ChangeLoginViewModel>" %>

<asp:Content ContentPlaceHolderID="PageCopy" runat="server">
	<%: Html.HtmlSnippet("Account/ChangeLogin/PageCopy", "page-copy") %>
</asp:Content>

<asp:Content ContentPlaceHolderID="ProfileNavbar" runat="server">
	<% Html.RenderPartial("ProfileNavbar"); %>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
	<div class="form-horizontal">
		<fieldset>
			<legend><%: Html.TextSnippet("Account/ChangeLogin/ChangeLoginFormHeading", defaultValue: "Manage External Authentication", tagName: "span") %></legend>
			<%: Html.Partial("ProfileMessage", Request["Message"] ?? string.Empty) %>
			<ul class="list-group">
				<% foreach (var login in Model.Logins) { %>
					<li class="list-group-item clearfix <%: login.User == null ? "disabled" : "list-group-item-default" %>">
						<% if (login.User == null) { %>
							<% using (Html.BeginForm("LinkLogin", "Manage")) { %>
								<%: Html.AntiForgeryToken() %>
								<%: Html.Hidden("provider", login.Provider.AuthenticationType) %>
								<button type="submit" class="btn btn-default btn-xs pull-right"><span class="fa fa-plus" aria-hidden="true"></span> <%: Html.SnippetLiteral("Account/ChangeLogin/AddLoginButtonText", "Connect") %></button>
							<% } %>
						<% } else if (ViewBag.ShowRemoveButton) { %>
							<a class="btn btn-danger btn-xs pull-right" data-toggle="modal" data-target="#confirm-delete-<%: login.Id %>" href="#"><span class="fa fa-trash-o" aria-hidden="true"></span> <%: Html.SnippetLiteral("Account/ChangeLogin/RemoveLoginButtonText", "Disconnect") %></a>
							<div class="modal fade" id="confirm-delete-<%: login.Id %>" tabindex="-1" role="dialog" aria-labelledby="confirm-delete-label-<%: login.Id %>" aria-hidden="true">
								<div class="modal-dialog">
									<div class="modal-content">
										<div class="modal-header">
											<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
											<h4 class="modal-title" id="confirm-delete-label-<%: login.Id %>"><%: Html.TextSnippet("Account/ChangeLogin/ConfirmRemoveModalHeading", defaultValue: "Confirm Remove", tagName: "span") %></h4>
										</div>
										<div class="modal-body">
											<%: Html.TextSnippet("Account/ChangeLogin/ConfirmRemoveModalBody", defaultValue: string.Format("Remove the \"{0}\" external account?", login.Provider.Caption)) %>
										</div>
										<div class="modal-footer">
											<% using (Html.BeginForm("RemoveLogin", "Manage")) { %>
												<%: Html.AntiForgeryToken() %>
												<%: Html.Hidden("loginProvider", login.User.LoginProvider) %>
												<%: Html.Hidden("providerKey", login.User.ProviderKey) %>
												<button type="submit" class="btn btn-danger"><span class="fa fa-trash-o" aria-hidden="true"></span> <%: Html.SnippetLiteral("Account/ChangeLogin/RemoveLoginButtonText", "Disconnect") %></button>
												<button type="button" class="btn btn-default" data-dismiss="modal"><%: Html.SnippetLiteral("Account/ChangeLogin/ConfirmRemoveCancelButtonText", "Cancel") %></button>
											<% } %>
										</div>
									</div>
								</div>
							</div>
						<% } %>
						<%
							var authenticationTypeIcons = new []
							{
								new Tuple<string, string>("facebook",      "fa-facebook-official"),
								new Tuple<string, string>("google",        "fa-google"),
								new Tuple<string, string>("linkedin",      "fa-linkedin"),
								new Tuple<string, string>("microsoft",     "fa-windows"),
								new Tuple<string, string>("windowsliveid", "fa-windows"),
								new Tuple<string, string>("twitter",       "fa-twitter"),
								new Tuple<string, string>("yahoo",         "fa-yahoo"),
							};

							var icon = authenticationTypeIcons
								.FirstOrDefault(e => login.Provider.AuthenticationType.IndexOf(e.Item1, StringComparison.InvariantCultureIgnoreCase) >= 0);
						%>
						<span class="fa fa-fw <%: icon == null ? "fa-user" : icon.Item2 %>"></span>
						<%: login.Provider.Caption %>
					</li>
				<% } %>
			</ul>
		</fieldset>
	</div>
</asp:Content>
