using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Site.Areas.Account.ViewModels
{
	public class ExternalLoginConfirmationViewModel
	{
		[Required]
		public string Email { get; set; }
	}

	public class ExternalLoginListViewModel
	{
		public string ReturnUrl { get; set; }
	}

	public class SendCodeViewModel
	{
		public string SelectedProvider { get; set; }
		public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
		public string ReturnUrl { get; set; }
		public bool RememberMe { get; set; }
		public string InvitationCode { get; set; }
	}

	public class VerifyCodeViewModel
	{
		[Required]
		public string Provider { get; set; }

		[Required]
		public string Code { get; set; }
		public string ReturnUrl { get; set; }

		[Display(Name = "Remember this browser?")]
		public bool RememberBrowser { get; set; }

		public bool RememberMe { get; set; }
		public string InvitationCode { get; set; }
	}

	public class ForgotViewModel
	{
		[Required]
		public string Email { get; set; }
	}

	public class LoginViewModel
	{
		[EmailAddress]
		public string Email { get; set; }

		public string Username { get; set; }

		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[Display(Name = "Remember me?")]
		public bool RememberMe { get; set; }
	}

	public class RegisterViewModel
	{
		[EmailAddress]
		public string Email { get; set; }

		public string Username { get; set; }

		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Confirm password")]
		public string ConfirmPassword { get; set; }
	}

	public class ResetPasswordViewModel
	{
		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Confirm password")]
		public string ConfirmPassword { get; set; }

		public string UserId { get; set; }
		public string Code { get; set; }
	}

	public class ForgotPasswordViewModel
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; }
	}

	public class RedeemInvitationViewModel
	{
		[Required]
		[Display(Name = "Invitation Code")]
		public string InvitationCode { get; set; }

		[Display(Name = "I have an existing account.")]
		public bool RedeemByLogin { get; set; }
	}
}