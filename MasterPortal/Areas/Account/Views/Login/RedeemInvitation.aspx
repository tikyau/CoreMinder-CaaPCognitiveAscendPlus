<%@ Page Title="" Language="C#" MasterPageFile="Account.Master" Inherits="System.Web.Mvc.ViewPage<Site.Areas.Account.ViewModels.RedeemInvitationViewModel>" %>

<asp:Content ContentPlaceHolderID="AccountNavBar" runat="server">
	<% Html.RenderPartial("AccountNavBar", new ViewDataDictionary(ViewData) { { "SubArea", "Redeem" } }); %>
</asp:Content>

<asp:Content ContentPlaceHolderID="PageCopy" runat="server">
	<%: Html.HtmlSnippet("Account/RedeemInvitation/PageCopy", "page-copy") %>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
	<% using (Html.BeginForm("RedeemInvitation", "Login", new { area = "Account", ReturnUrl = ViewBag.ReturnUrl })) { %>
		<%: Html.AntiForgeryToken() %>
		<div class="form-horizontal">
			<fieldset>
				<legend><%: Html.TextSnippet("Account/Redeem/InvitationCodeFormHeading", defaultValue: "Sign up with an invitation code", tagName: "span") %></legend>
				<%: Html.ValidationSummary(string.Empty, new { @class = "alert alert-block alert-danger" }) %>
				<div class="form-group">
					<label class="col-sm-2 control-label" for="InvitationCode"><%: Html.TextSnippet("Account/Redeem/InvitationCodeLabel", defaultValue: "Invitation Code", tagName: "span") %></label>
					<div class="col-sm-10">
						<%: Html.TextBoxFor(model => model.InvitationCode, new { @class = "form-control" }) %>
					</div>
				</div>
				<div class="form-group">
					<div class="col-sm-offset-2 col-sm-10">
						<div class="checkbox">
							<label>
								<%: Html.CheckBoxFor(model => model.RedeemByLogin) %>
								<%: Html.TextSnippet("Account/Redeem/RedeemByLoginLabel", defaultValue: "I have an existing account", tagName: "span") %>
							</label>
						</div>
					</div>
				</div>
				<div class="form-group">
					<div class="col-sm-offset-2 col-sm-10">
						<button id="submit-redeem-invitation" class="btn btn-primary"><%: Html.SnippetLiteral("Account/Redeem/InvitationCodeFormButtonText", "Register") %></button>
					</div>
				</div>
			</fieldset>
		</div>
	<% } %>
	<script type="text/javascript">
		$(function() {
			$("#submit-redeem-invitation").click(function () {
				$.blockUI({ message: null, overlayCSS: { opacity: .3 } });
			});
		});
	</script>
</asp:Content>
