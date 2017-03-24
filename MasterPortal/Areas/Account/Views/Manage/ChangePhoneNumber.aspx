<%@ Page Title="" Language="C#" MasterPageFile="Manage.Master" Inherits="System.Web.Mvc.ViewPage<Site.Areas.Account.ViewModels.AddPhoneNumberViewModel>" %>

<asp:Content ContentPlaceHolderID="PageCopy" runat="server">
	<%: Html.HtmlSnippet("Account/ChangePhoneNumber/PageCopy", "page-copy") %>
</asp:Content>

<asp:Content ContentPlaceHolderID="ProfileNavbar" runat="server">
	<% Html.RenderPartial("ProfileNavbar"); %>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
	<% using (Html.BeginForm("ChangePhoneNumber", "Manage")) { %>
		<%: Html.AntiForgeryToken() %>
		<div class="form-horizontal">
			<fieldset>
				<legend><%: Html.TextSnippet("Account/ChangePhoneNumber/ChangePhoneNumberFormHeading", defaultValue: "Change Mobile Phone", tagName: "span") %></legend>
				<%: Html.ValidationSummary(string.Empty, new {@class = "alert alert-block alert-danger"}) %>
				<div class="form-group">
					<label class="col-sm-2 control-label" for="Number"><%: Html.TextSnippet("Account/ChangePhoneNumber/PhoneNumberLabel", defaultValue: "Mobile Phone", tagName: "span") %></label>
					<div class="col-sm-10">
						<%: Html.TextBoxFor(model => model.Number, new { @class = "form-control" }) %>
					</div>
				</div>
				<div class="form-group">
					<div class="col-sm-offset-2 col-sm-10">
						<% if (ViewBag.ShowRemoveButton) { %>
							<a class="btn btn-default pull-right"data-toggle="modal" data-target="#confirm-remove" href="#"><span class="fa fa-trash-o" aria-hidden="true"></span> <%: Html.SnippetLiteral("Account/ChangePhoneNumber/RemovePhoneNumberButtonText", "Remove") %></a>
							<div class="modal fade" id="confirm-remove" tabindex="-1" role="dialog" aria-labelledby="confirm-remove" aria-hidden="true">
								<div class="modal-dialog">
									<div class="modal-content">
										<div class="modal-header">
											<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
											<h4 class="modal-title" id="confirm-remove"><%: Html.TextSnippet("Account/ChangePhoneNumber/ConfirmRemoveModalHeading", defaultValue: "Confirm Remove", tagName: "span") %></h4>
										</div>
										<div class="modal-body">
											<%: Html.TextSnippet("Account/ChangePhoneNumber/ConfirmRemoveModalBody", defaultValue: "Remove mobile phone number?") %>
										</div>
										<div class="modal-footer">
											<a class="btn btn-primary" href="<%: Url.Action("RemovePhoneNumber") %>"><span class="fa fa-trash-o" aria-hidden="true"></span> <%: Html.SnippetLiteral("Account/ChangePhoneNumber/RemovePhoneNumberButtonText", "Remove") %></a>
											<button type="button" class="btn btn-default" data-dismiss="modal"><%: Html.SnippetLiteral("Account/ChangePhoneNumber/ConfirmRemoveCancelButtonText", "Cancel") %></button>
										</div>
									</div>
								</div>
							</div>
						<% } %>
						<button type="submit" class="btn btn-primary"><span class="fa fa-mobile" aria-hidden="true"></span> <%: Html.SnippetLiteral("Account/ChangePhoneNumber/ChangePhoneNumberButtonText", "Change and Confirm Number") %></button>
					</div>
				</div>
			</fieldset>
		</div>
	<% } %>
</asp:Content>
