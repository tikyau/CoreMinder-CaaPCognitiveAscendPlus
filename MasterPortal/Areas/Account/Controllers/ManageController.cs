using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Adxstudio.Xrm.AspNet.Mvc;
using Adxstudio.Xrm.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Security.Application;
using Site.Areas.Account.Models;
using Site.Areas.Account.ViewModels;

namespace Site.Areas.Account.Controllers
{
	[Authorize]
	[PortalView]
	[OutputCache(NoStore = true, Duration = 0)]
	public class ManageController : Controller
	{
		public ManageController()
		{
		}

		public ManageController(ApplicationUserManager userManager, ApplicationOrganizationManager organizationManager, ApplicationWebsiteManager websiteManager)
		{
			UserManager = userManager;
			OrganizationManager = organizationManager;
			WebsiteManager = websiteManager;
		}

		private ApplicationUserManager _userManager;
		public ApplicationUserManager UserManager
		{
			get
			{
				return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
			}
			private set
			{
				_userManager = value;
			}
		}

		private ApplicationOrganizationManager _organizationManager;
		public ApplicationOrganizationManager OrganizationManager
		{
			get
			{
				return _organizationManager ?? HttpContext.GetOwinContext().Get<ApplicationOrganizationManager>();
			}
			private set
			{
				_organizationManager = value;
			}
		}

		private ApplicationWebsiteManager _websiteManager;
		public ApplicationWebsiteManager WebsiteManager
		{
			get
			{
				return _websiteManager ?? HttpContext.GetOwinContext().Get<ApplicationWebsiteManager>();
			}
			private set
			{
				_websiteManager = value;
			}
		}

		protected override void Initialize(RequestContext requestContext)
		{
			base.Initialize(requestContext);

			var isLocal = requestContext.HttpContext.IsDebuggingEnabled && requestContext.HttpContext.Request.IsLocal;
			var website = WebsiteManager.Find(requestContext);

			ViewBag.Settings = website.GetAuthenticationSettings<ApplicationWebsite, string>(isLocal);
			ViewBag.IdentityErrors = website.GetIdentityErrors<ApplicationWebsite, string>();

			var userId = User.Identity.GetUserId();
			var user = !string.IsNullOrWhiteSpace(userId) ? UserManager.FindById(userId) : null;

			ViewBag.Nav = user == null
				? new ManageNavSettings()
				: new ManageNavSettings
				{
					HasPassword = UserManager.HasPassword(user.Id),
					IsEmailConfirmed = string.IsNullOrWhiteSpace(user.Email) || user.EmailConfirmed,
					IsMobilePhoneConfirmed = string.IsNullOrWhiteSpace(user.PhoneNumber) || user.PhoneNumberConfirmed,
					IsTwoFactorEnabled = user.TwoFactorEnabled,
				};
		}

		//
		// GET: /Manage/Index
		[HttpGet]
		public ActionResult Index(ManageMessageId? message)
		{
			return RedirectToProfile(message);
		}

		//
		// POST: /Manage/RemoveLogin
		[HttpPost]
		[ValidateAntiForgeryToken]
		[ExternalLogin]
		public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
		{
			ManageMessageId? message;
			var userId = User.Identity.GetUserId();
			var user = await UserManager.FindByIdAsync(userId);
			var userLogins = await UserManager.GetLoginsAsync(userId);

			if (user != null && userLogins != null)
			{
				if (user.PasswordHash == null && userLogins.Count() <= 1)
				{
					return RedirectToAction("ChangeLogin", new { Message = ManageMessageId.RemoveLoginFailure });
				}
			}

			var result = await UserManager.RemoveLoginAsync(userId, new UserLoginInfo(loginProvider, providerKey));

			if (result.Succeeded)
			{
				if (user != null)
				{
					await SignInAsync(user, isPersistent: false);
				}

				message = ManageMessageId.RemoveLoginSuccess;
			}
			else
			{
				message = ManageMessageId.RemoveLoginFailure;
			}

			return RedirectToAction("ChangeLogin", new { Message = message });
		}

		//
		// GET: /Manage/ChangePhoneNumber
		[HttpGet]
		public async Task<ActionResult> ChangePhoneNumber()
		{
			if (!ViewBag.Settings.MobilePhoneEnabled)
			{
				return HttpNotFound();
			}

			var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

			if (user == null)
			{
				throw new ApplicationException("Account error.");
			}

			ViewBag.ShowRemoveButton = user.PhoneNumber != null;

			return View(new AddPhoneNumberViewModel { Number = user.PhoneNumber });
		}

		//
		// POST: /Manage/ChangePhoneNumber
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ChangePhoneNumber(AddPhoneNumberViewModel model)
		{
			if (!ViewBag.Settings.MobilePhoneEnabled)
			{
				return HttpNotFound();
			}

			if (!ModelState.IsValid)
			{
				return await ChangePhoneNumber();
			}

			var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

			if (user == null)
			{
				throw new ApplicationException("Account error.");
			}

			// Generate the token and send it
			var code = await UserManager.GenerateChangePhoneNumberTokenAsync(user.Id, model.Number);
			var parameters = new Dictionary<string, object> { { "Code", code }, { "PhoneNumber", model.Number } };
			await OrganizationManager.InvokeProcessAsync("adx_SendSmsConfirmationToContact", user.ContactId, parameters);

			//if (UserManager.SmsService != null)
			//{
			//	var message = new IdentityMessage
			//	{
			//		Destination = model.Number,
			//		Body = "Your security code is: " + code
			//	};
			//	await UserManager.SmsService.SendAsync(message);
			//}

			return RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = model.Number });
		}

		//
		// POST: /Manage/RememberBrowser
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult RememberBrowser()
		{
			if (!ViewBag.Settings.TwoFactorEnabled || !ViewBag.Settings.RememberBrowserEnabled)
			{
				return HttpNotFound();
			}

			var rememberBrowserIdentity = AuthenticationManager.CreateTwoFactorRememberBrowserIdentity(User.Identity.GetUserId());
			AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = true }, rememberBrowserIdentity);
			return RedirectToProfile(ManageMessageId.RememberBrowserSuccess);
		}

		//
		// POST: /Manage/ForgetBrowser
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult ForgetBrowser()
		{
			AuthenticationManager.SignOut(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);
			return RedirectToProfile(ManageMessageId.ForgetBrowserSuccess);
		}

		//
		// POST: /Manage/EnableTFA
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> EnableTFA()
		{
			if (!ViewBag.Settings.TwoFactorEnabled)
			{
				return HttpNotFound();
			}

			var userId = User.Identity.GetUserId();
			await UserManager.SetTwoFactorEnabledAsync(userId, true);
			var user = await UserManager.FindByIdAsync(userId);
			if (user != null)
			{
				await SignInAsync(user, isPersistent: false);
			}
			return RedirectToProfile(ManageMessageId.EnableTwoFactorSuccess);
		}

		//
		// POST: /Manage/DisableTFA
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> DisableTFA()
		{
			if (!ViewBag.Settings.TwoFactorEnabled)
			{
				return HttpNotFound();
			}

			var userId = User.Identity.GetUserId();
			await UserManager.SetTwoFactorEnabledAsync(userId, false);
			var user = await UserManager.FindByIdAsync(userId);
			if (user != null)
			{
				await SignInAsync(user, isPersistent: false);
			}
			return RedirectToProfile(ManageMessageId.DisableTwoFactorSuccess);
		}

		//
		// GET: /Manage/VerifyPhoneNumber
		[HttpGet]
		public async Task<ActionResult> VerifyPhoneNumber(string phoneNumber)
		{
			if (string.IsNullOrWhiteSpace(phoneNumber))
			{
				return HttpNotFound();
			}

			if (ViewBag.Settings.IsDemoMode)
			{
				// This code allows you exercise the flow without actually sending codes
				// For production use please register a SMS provider in IdentityConfig and generate a code here.
				var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), phoneNumber);
				ViewBag.DemoModeCode = code;
			}

			return View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
		}

		//
		// POST: /Manage/VerifyPhoneNumber
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
		{
			if (ModelState.IsValid)
			{
				var userId = User.Identity.GetUserId();
				var result = await UserManager.ChangePhoneNumberAsync(userId, model.PhoneNumber, model.Code);
				if (result.Succeeded)
				{
					var user = await UserManager.FindByIdAsync(userId);
					if (user != null)
					{
						await SignInAsync(user, isPersistent: false);
					}
					return RedirectToProfile(ManageMessageId.ChangePhoneNumberSuccess);
				}
			}

			// If we got this far, something failed, redisplay form
			ViewBag.Message = ManageMessageId.ChangePhoneNumberFailure.ToString();
			return await VerifyPhoneNumber(model.PhoneNumber);
		}

		//
		// GET: /Manage/RemovePhoneNumber
		[HttpGet]
		public async Task<ActionResult> RemovePhoneNumber()
		{
			var userId = User.Identity.GetUserId();
			var result = await UserManager.SetPhoneNumberAsync(userId, null);
			if (!result.Succeeded)
			{
				throw new ApplicationException("Account error.");
			}
			var user = await UserManager.FindByIdAsync(userId);
			if (user != null)
			{
				await SignInAsync(user, isPersistent: false);
			}
			return RedirectToProfile(ManageMessageId.RemovePhoneNumberSuccess);
		}

		//
		// GET: /Manage/ChangePassword
		[HttpGet]
		[LocalLogin]
		public async Task<ActionResult> ChangePassword()
		{
			var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

			if (user == null)
			{
				throw new ApplicationException("Account error.");
			}

			return View(new ChangePasswordViewModel { Username = user.UserName, Email = user.Email });
		}

		//
		// POST: /Manage/ChangePassword
		[HttpPost]
		[ValidateAntiForgeryToken]
		[LocalLogin]
		public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
		{
			if (!string.Equals(model.NewPassword, model.ConfirmPassword))
			{
				ModelState.AddModelError("Password", ViewBag.IdentityErrors.PasswordConfirmationFailure().Description);
			}

			if (ModelState.IsValid)
			{
				var userId = User.Identity.GetUserId();
				var result = await UserManager.ChangePasswordAsync(userId, model.OldPassword, model.NewPassword);
				if (result.Succeeded)
				{
					var user = await UserManager.FindByIdAsync(userId);
					if (user != null)
					{
						await SignInAsync(user, isPersistent: false);
					}
					return RedirectToProfile(ManageMessageId.ChangePasswordSuccess);
				}
				AddErrors(result);
			}

			return View(model);
		}

		//
		// GET: /Manage/SetPassword
		[HttpGet]
		[LocalLogin]
		public async Task<ActionResult> SetPassword()
		{
			var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

			if (user == null)
			{
				throw new ApplicationException("Account error.");
			}

			ViewBag.HasEmail = !string.IsNullOrWhiteSpace(user.Email);

			return View(new SetPasswordViewModel { Email = user.Email });
		}

		//
		// POST: /Manage/SetPassword
		[HttpPost]
		[ValidateAntiForgeryToken]
		[LocalLogin]
		public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
		{
			if (!ViewBag.Settings.LocalLoginByEmail && string.IsNullOrWhiteSpace(model.Username))
			{
				ModelState.AddModelError("Username", ViewBag.IdentityErrors.UserNameRequired().Description);
			}

			if (!string.Equals(model.NewPassword, model.ConfirmPassword))
			{
				ModelState.AddModelError("Password", ViewBag.IdentityErrors.PasswordConfirmationFailure().Description);
			}

			if (ModelState.IsValid)
			{
				var userId = User.Identity.GetUserId();

				var result = ViewBag.Settings.LocalLoginByEmail
					? await UserManager.AddPasswordAsync(userId, model.NewPassword)
					: await UserManager.AddUsernameAndPasswordAsync(userId, model.Username, model.NewPassword);

				if (result.Succeeded)
				{
					var user = await UserManager.FindByIdAsync(userId);
					if (user != null)
					{
						await SignInAsync(user, isPersistent: false);
					}
					return RedirectToProfile(ManageMessageId.SetPasswordSuccess);
				}
				AddErrors(result);
			}

			// If we got this far, something failed, redisplay form
			return await SetPassword();
		}

		//
		// POST: /Manage/LinkLogin
		[HttpPost]
		[ValidateAntiForgeryToken]
		[ExternalLogin]
		public ActionResult LinkLogin(string provider)
		{
			// Request a redirect to the external login provider to link a login for the current user
			return new LoginController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
		}

		//
		// GET: /Manage/LinkLoginCallback
		[ExternalLogin]
		public async Task<ActionResult> LinkLoginCallback()
		{
			var userId = User.Identity.GetUserId();
			var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, userId);
			if (loginInfo == null)
			{
				return RedirectToAction("ChangeLogin", new { Message = ManageMessageId.LinkLoginFailure });
			}
			var result = await UserManager.AddLoginAsync(userId, loginInfo.Login);
			return RedirectToAction("ChangeLogin", new { Message = result.Succeeded ? ManageMessageId.LinkLoginSuccess : ManageMessageId.LinkLoginFailure });
		}

		//
		// GET: /Manage/ConfirmEmailRequest
		[HttpGet]
		public async Task<ActionResult> ConfirmEmailRequest()
		{
			if (!ViewBag.Settings.EmailConfirmationEnabled)
			{
				return HttpNotFound();
			}

			var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

			if (user == null)
			{
				throw new ApplicationException("Account error.");
			}

			var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
			var callbackUrl = Url.Action("ConfirmEmail", "Manage", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
			var parameters = new Dictionary<string, object> { { "UserId", user.Id }, { "Code", code }, { "UrlCode", Encoder.UrlEncode(code) }, { "CallbackUrl", callbackUrl } };
			await OrganizationManager.InvokeProcessAsync("adx_SendEmailConfirmationToContact", user.ContactId, parameters);
			//await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking this link: <a href=\"" + callbackUrl + "\">link</a>");

			if (ViewBag.Settings.IsDemoMode)
			{
				ViewBag.DemoModeLink = callbackUrl;
			}

			return View(new RegisterViewModel { Email = user.Email });
		}

		//
		// GET: /Manage/ConfirmEmail
		[HttpGet]
		[AllowAnonymous]
		public async Task<ActionResult> ConfirmEmail(string userId, string code)
		{
			if (!ViewBag.Settings.EmailConfirmationEnabled)
			{
				return HttpNotFound();
			}

			if (userId == null || code == null)
			{
				return HttpNotFound();
			}

			var result = await UserManager.ConfirmEmailAsync(userId, code);
			var message = result.Succeeded ? ManageMessageId.ConfirmEmailSuccess : ManageMessageId.ConfirmEmailFailure;

			if (User.Identity.IsAuthenticated && userId == User.Identity.GetUserId())
			{
				return RedirectToProfile(message);
			}

			return RedirectToAction("ConfirmEmail", "Login", new { Message = message });
		}

		//
		// GET: /Manage/ChangeEmail
		[HttpGet]
		public async Task<ActionResult> ChangeEmail()
		{
			var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

			if (user == null)
			{
				throw new ApplicationException("Account error.");
			}

			return View(new ChangeEmailViewModel { Email = user.Email });
		}

		//
		// POST: /Manage/ChangeEmail
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ChangeEmail(ChangeEmailViewModel model)
		{
			if (ModelState.IsValid)
			{
				var userId = User.Identity.GetUserId();

				var result = ViewBag.Settings.LocalLoginByEmail
					? await UserManager.SetUsernameAndEmailAsync(userId, model.Email, model.Email)
					: await UserManager.SetEmailAsync(userId, model.Email);

				if (result.Succeeded)
				{
					var user = await UserManager.FindByIdAsync(userId);
					if (user != null)
					{
						await SignInAsync(user, isPersistent: false);
					}

					return ViewBag.Settings.EmailConfirmationEnabled
						? RedirectToAction("ConfirmEmailRequest", new { Message = ManageMessageId.ChangeEmailSuccess })
						: RedirectToProfile(ManageMessageId.ChangeEmailSuccess);
				}

				AddErrors(result);
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		//
		// GET: /Manage/ChangeTwoFactor
		[HttpGet]
		public async Task<ActionResult> ChangeTwoFactor()
		{
			if (!ViewBag.Settings.TwoFactorEnabled)
			{
				return HttpNotFound();
			}

			var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

			if (user == null)
			{
				throw new ApplicationException("Account error.");
			}

			ViewBag.TwoFactorBrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(user.Id);
			ViewBag.HasEmail = !string.IsNullOrWhiteSpace(user.Email);
			ViewBag.IsEmailConfirmed = user.EmailConfirmed;
			ViewBag.IsTwoFactorEnabled = user.TwoFactorEnabled;

			return View();
		}

		//
		// GET: /Manage/ChangeLogin
		[HttpGet]
		[ExternalLogin]
		public async Task<ActionResult> ChangeLogin()
		{
			var userId = User.Identity.GetUserId();
			var user = await UserManager.FindByIdAsync(userId);

			if (user == null)
			{
				throw new ApplicationException("Account error.");
			}

			var id = 0;
			var userLogins = await UserManager.GetLoginsAsync(userId);
			var logins = AuthenticationManager.GetExternalAuthenticationTypes()
				.Select(p => new LoginPair { Id = id++, Provider = p, User = userLogins.SingleOrDefault(u => u.LoginProvider == p.AuthenticationType) })
				.OrderBy(pair => pair.Provider.Caption)
				.ToList();

			ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count() > 1;

			return View(new ChangeLoginViewModel { Logins = logins });
		}

		#region Helpers

		// Used for XSRF protection when adding external logins
		private const string XsrfKey = "XsrfId";

		private IAuthenticationManager AuthenticationManager
		{
			get
			{
				return HttpContext.GetOwinContext().Authentication;
			}
		}

		private async Task SignInAsync(ApplicationUser user, bool isPersistent)
		{
			AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie, DefaultAuthenticationTypes.TwoFactorCookie);
			AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent }, await user.GenerateUserIdentityAsync(UserManager));
		}

		private void AddErrors(IdentityResult result)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError("", error);
			}
		}

		public enum ManageMessageId
		{
			SetPasswordSuccess,
			ChangePasswordSuccess,

			ChangeEmailSuccess,

			ConfirmEmailSuccess,
			ConfirmEmailFailure,

			ChangePhoneNumberSuccess,
			ChangePhoneNumberFailure,
			RemovePhoneNumberSuccess,

			ForgetBrowserSuccess,
			RememberBrowserSuccess,

			DisableTwoFactorSuccess,
			EnableTwoFactorSuccess,

			RemoveLoginSuccess,
			RemoveLoginFailure,
			LinkLoginSuccess,
			LinkLoginFailure,
		}

		private static ActionResult RedirectToProfile(ManageMessageId? message)
		{
			var query = message != null ? new NameValueCollection { { "Message", message.Value.ToString() } } : null;

			return new RedirectToSiteMarkerResult("Profile", query);
		}

		#endregion
	}
}