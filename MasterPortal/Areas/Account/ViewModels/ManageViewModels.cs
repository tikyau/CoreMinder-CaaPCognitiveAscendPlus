using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Site.Areas.Account.ViewModels
{
	public class IndexViewModel
	{
		public bool HasPassword { get; set; }
		public IList<UserLoginInfo> Logins { get; set; }
		public string PhoneNumber { get; set; }
		public bool TwoFactor { get; set; }
		public bool BrowserRemembered { get; set; }
	}

	public class ManageLoginsViewModel
	{
		public IList<UserLoginInfo> CurrentLogins { get; set; }
		public IList<AuthenticationDescription> OtherLogins { get; set; }
	}

	public class FactorViewModel
	{
		public string Purpose { get; set; }
	}

	public class SetPasswordViewModel
	{
		[EmailAddress]
		public string Email { get; set; }

		public string Username { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "New password")]
		public string NewPassword { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Confirm new password")]
		public string ConfirmPassword { get; set; }
	}

	public class ChangePasswordViewModel
	{
		[EmailAddress]
		public string Email { get; set; }

		public string Username { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Current password")]
		public string OldPassword { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "New password")]
		public string NewPassword { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Confirm new password")]
		public string ConfirmPassword { get; set; }
	}


	public class AddPhoneNumberViewModel
	{
		[Required]
		[Phone]
		[Display(Name = "Phone Number")]
		public string Number { get; set; }
	}

	public class VerifyPhoneNumberViewModel
	{
		[Required]
		public string Code { get; set; }

		[Required]
		[Phone]
		[Display(Name = "Phone Number")]
		public string PhoneNumber { get; set; }
	}

	public class ConfigureTwoFactorViewModel
	{
		public string SelectedProvider { get; set; }
		public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
	}

	public class ChangeEmailViewModel
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; }
	}

	public class LoginPair
	{
		public int Id { get; set; }
		public AuthenticationDescription Provider { get; set; }
		public UserLoginInfo User { get; set; }
	}

	public class ChangeLoginViewModel
	{
		public IList<LoginPair> Logins { get; set; }
	}

	public class ManageNavSettings
	{
		public bool HasPassword { get; set; }
		public bool IsEmailConfirmed { get; set; }
		public bool IsMobilePhoneConfirmed { get; set; }
		public bool IsTwoFactorEnabled { get; set; }
	}
}
