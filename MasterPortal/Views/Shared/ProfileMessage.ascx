<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>
<%@ Import Namespace="Site.Areas.Account.Controllers" %>

<% var alerts = new[] {
	new { Id = ManageController.ManageMessageId.SetPasswordSuccess, SnippetName = "Profile/Message/SetPasswordSuccess", DefaultText = "Your password has been set successfully.", Type = "success", Icon = "fa-check-circle" },
	new { Id = ManageController.ManageMessageId.ChangePasswordSuccess, SnippetName = "Profile/Message/ChangePasswordSuccess", DefaultText = "Your password has been changed successfully.", Type = "success", Icon = "fa-check-circle" },

	new { Id = ManageController.ManageMessageId.ChangeEmailSuccess, SnippetName = "Profile/Message/ChangeEmailSuccess", DefaultText = "Your email has been changed successfully.", Type = "success", Icon = "fa-check-circle" },

	new { Id = ManageController.ManageMessageId.ConfirmEmailSuccess, SnippetName = "Profile/Message/ConfirmEmailSuccess", DefaultText = "Your email has been confirmed successfully.", Type = "success", Icon = "fa-check-circle" },
	new { Id = ManageController.ManageMessageId.ConfirmEmailFailure, SnippetName = "Profile/Message/ConfirmEmailFailure", DefaultText = "Failed to confirm email.", Type = "danger", Icon = "fa-times-circle" },

	new { Id = ManageController.ManageMessageId.ChangePhoneNumberSuccess, SnippetName = "Profile/Message/ChangePhoneNumberSuccess", DefaultText = "Your mobile phone number has been changed successfully.", Type = "success", Icon = "fa-check-circle" },
	new { Id = ManageController.ManageMessageId.ChangePhoneNumberFailure, SnippetName = "Profile/Message/ChangePhoneNumberFailure", DefaultText = "Failed to change mobile phone number.", Type = "danger", Icon = "fa-times-circle" },
	new { Id = ManageController.ManageMessageId.RemovePhoneNumberSuccess, SnippetName = "Profile/Message/RemovePhoneNumberSuccess", DefaultText = "Your mobile phone number has been removed successfully.", Type = "success", Icon = "fa-check-circle" },

	new { Id = ManageController.ManageMessageId.ForgetBrowserSuccess, SnippetName = "Profile/Message/ForgetBrowserSuccess", DefaultText = "The two-factor sign-in validation is now required for this browser.", Type = "success", Icon = "fa-check-circle" },
	new { Id = ManageController.ManageMessageId.RememberBrowserSuccess, SnippetName = "Profile/Message/RememberBrowserSuccess", DefaultText = "The two-factor authentication session is now remembered for this browser.", Type = "success", Icon = "fa-check-circle" },

	new { Id = ManageController.ManageMessageId.DisableTwoFactorSuccess, SnippetName = "Profile/Message/DisableTwoFactorSuccess", DefaultText = "Two-factor authentication has been disabled successfully.", Type = "success", Icon = "fa-check-circle" },
	new { Id = ManageController.ManageMessageId.EnableTwoFactorSuccess, SnippetName = "Profile/Message/EnableTwoFactorSuccess", DefaultText = "Two-factor authentication has been enabled successfully.", Type = "success", Icon = "fa-check-circle" },
	
	new { Id = ManageController.ManageMessageId.RemoveLoginSuccess, SnippetName = "Profile/Message/RemoveLoginSuccessText", DefaultText = "The external account was removed successfully.", Type = "success", Icon = "fa-check-circle" },
	new { Id = ManageController.ManageMessageId.RemoveLoginFailure, SnippetName = "Profile/Message/RemoveLoginFailureText", DefaultText = "Failed to remove the external account.", Type = "danger", Icon = "fa-times-circle" },
	new { Id = ManageController.ManageMessageId.LinkLoginSuccess, SnippetName = "Profile/Message/LinkLoginSuccessText", DefaultText = "The external account was added successfully.", Type = "success", Icon = "fa-check-circle" },
	new { Id = ManageController.ManageMessageId.LinkLoginFailure, SnippetName = "Profile/Message/LinkLoginFailureText", DefaultText = "Failed to add the external account.", Type = "danger", Icon = "fa-times-circle" },
}; %>
<% if (!string.IsNullOrWhiteSpace(Model)) {
		foreach (var alert in alerts) {
			if (Model == alert.Id.ToString()) { %>
				<div class="alert alert-<%: alert.Type %>">
					<a class="close" data-dismiss="alert" href="#">&times;</a>
					<span class="fa <%: alert.Icon %>" aria-hidden="true"></span> <%: Html.TextSnippet(alert.SnippetName, defaultValue: alert.DefaultText, tagName: "span") %>
				</div>
			<% }
		}
} %>
