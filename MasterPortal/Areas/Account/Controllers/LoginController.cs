using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.WebPages;
using Adxstudio.Xrm.AspNet.Identity;
using Adxstudio.Xrm.AspNet.Mvc;
using Adxstudio.Xrm.Web;
using Adxstudio.Xrm.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Security.Application;
using Microsoft.Xrm.Sdk;
using Site.Areas.Account.Models;
using Site.Areas.Account.ViewModels;

namespace Site.Areas.Account.Controllers
{
	[Authorize]
	[PortalView]
	[OutputCache(NoStore = true, Duration = 0)]
	public class LoginController : Controller
	{
		public LoginController()
		{
		}

		public LoginController(
			ApplicationUserManager userManager,
			ApplicationSignInManager signInManager,
			ApplicationInvitationManager invitationManager,
			ApplicationOrganizationManager organizationManager,
			ApplicationWebsiteManager websiteManager)
		{
			UserManager = userManager;
			SignInManager = signInManager;
			InvitationManager = invitationManager;
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

		private ApplicationInvitationManager _invitationManager;
		public ApplicationInvitationManager InvitationManager
		{
			get
			{
				return _invitationManager ?? HttpContext.GetOwinContext().Get<ApplicationInvitationManager>();
			}
			private set
			{
				_invitationManager = value;
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

		//
		// GET: /Login/Login
		[HttpGet]
		[AllowAnonymous]
		public ActionResult Login(string returnUrl, string invitationCode)
		{
			return View(GetLoginViewModel(null, null, returnUrl, invitationCode));
		}

		private ApplicationSignInManager _signInManager;

		public ApplicationSignInManager SignInManager
		{
			get
			{
				return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
			}
			private set { _signInManager = value; }
		}

		protected override void Initialize(RequestContext requestContext)
		{
			base.Initialize(requestContext);

			var isLocal = requestContext.HttpContext.IsDebuggingEnabled && requestContext.HttpContext.Request.IsLocal;
			var website = WebsiteManager.Find(requestContext);

			ViewBag.Settings = website.GetAuthenticationSettings<ApplicationWebsite, string>(isLocal);
			ViewBag.IdentityErrors = website.GetIdentityErrors<ApplicationWebsite, string>();
		}

		//
		// POST: /Login/Login
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		[LocalLogin]
		public async Task<ActionResult> Login(LoginViewModel model, string returnUrl, string invitationCode)
		{
			if (!ModelState.IsValid
				|| (ViewBag.Settings.LocalLoginByEmail && string.IsNullOrWhiteSpace(model.Email))
				|| (!ViewBag.Settings.LocalLoginByEmail && string.IsNullOrWhiteSpace(model.Username)))
			{
				AddErrors(ViewBag.IdentityErrors.InvalidLogin());
				return View(GetLoginViewModel(model, null, returnUrl, invitationCode));
			}

			var rememberMe = ViewBag.Settings.RememberMeEnabled && model.RememberMe;

			// This doen't count login failures towards lockout only two factor authentication
			// To enable password failures to trigger lockout, change to shouldLockout: true
			SignInStatus result = ViewBag.Settings.LocalLoginByEmail
				? await SignInManager.PasswordSignInByEmailAsync(model.Email, model.Password, rememberMe, ViewBag.Settings.TriggerLockoutOnFailedPassword)
				: await SignInManager.PasswordSignInAsync(model.Username, model.Password, rememberMe, ViewBag.Settings.TriggerLockoutOnFailedPassword);

			switch (result)
			{
				case SignInStatus.Success:
					return await RedirectOnPostAuthenticate(returnUrl, invitationCode);
				case SignInStatus.LockedOut:
					AddErrors(ViewBag.IdentityErrors.UserLocked());
					return View(GetLoginViewModel(model, null, returnUrl, invitationCode));
				case SignInStatus.RequiresVerification:
					return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, InvitationCode = invitationCode, RememberMe = rememberMe });
				case SignInStatus.Failure:
				default:
					AddErrors(ViewBag.IdentityErrors.InvalidLogin());
					return View(GetLoginViewModel(model, null, returnUrl, invitationCode));
			}
		}

		//
		// GET: /Login/VerifyCode
		[HttpGet]
		[AllowAnonymous]
		public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe, string invitationCode)
		{
			// Require that the user has already logged in via username/password or external login
			if (!await SignInManager.HasBeenVerifiedAsync())
			{
				return HttpNotFound();
			}

			if (ViewBag.Settings.IsDemoMode)
			{
				var user = await UserManager.FindByIdAsync(await SignInManager.GetVerifiedUserIdAsync());

				if (user != null && user.LogonEnabled)
				{
					var code = await UserManager.GenerateTwoFactorTokenAsync(user.Id, provider);
					ViewBag.DemoModeCode = code;
				}
			}

			return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe, InvitationCode = invitationCode });
		}

		//
		// POST: /Login/VerifyCode
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var rememberMe = ViewBag.Settings.RememberMeEnabled && model.RememberMe;
			var rememberBrowser = ViewBag.Settings.TwoFactorEnabled && ViewBag.Settings.RememberBrowserEnabled && model.RememberBrowser;

			SignInStatus result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: rememberMe, rememberBrowser: rememberBrowser);

			switch (result)
			{
				case SignInStatus.Success:
					return await RedirectOnPostAuthenticate(model.ReturnUrl, model.InvitationCode);
				case SignInStatus.LockedOut:
					AddErrors(ViewBag.IdentityErrors.UserLocked());
					return View(model);
				case SignInStatus.Failure:
				default:
					AddErrors(ViewBag.IdentityErrors.InvalidTwoFactorCode());
					return View(model);
			}
		}

		//
		// GET: /Login/Register
		[HttpGet]
		[AllowAnonymous]
		public async Task<ActionResult> Register(string returnUrl, string invitationCode)
		{
			if (!ViewBag.Settings.RegistrationEnabled
				|| (!ViewBag.Settings.OpenRegistrationEnabled && !ViewBag.Settings.InvitationEnabled)
				|| (ViewBag.Settings.OpenRegistrationEnabled && !ViewBag.Settings.InvitationEnabled && !string.IsNullOrWhiteSpace(invitationCode))
				|| (!ViewBag.Settings.OpenRegistrationEnabled && ViewBag.Settings.InvitationEnabled && string.IsNullOrWhiteSpace(invitationCode)))
			{
				return HttpNotFound();
			}

			ViewBag.ReturnUrl = returnUrl;
			ViewBag.InvitationCode = invitationCode;
			var contactId = ToContactId(await FindInvitationByCodeAsync(invitationCode));
			var email = contactId != null ? contactId.Name : null;

			return View(GetRegisterViewModel(new RegisterViewModel { Email = email }, null));
		}

		//
		// POST: /Login/Register
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		[LocalLogin]
		public async Task<ActionResult> Register(RegisterViewModel model, string returnUrl, string invitationCode)
		{
			if (!ViewBag.Settings.RegistrationEnabled
				|| (!ViewBag.Settings.OpenRegistrationEnabled && !ViewBag.Settings.InvitationEnabled)
				|| (ViewBag.Settings.OpenRegistrationEnabled && !ViewBag.Settings.InvitationEnabled && !string.IsNullOrWhiteSpace(invitationCode))
				|| (!ViewBag.Settings.OpenRegistrationEnabled && ViewBag.Settings.InvitationEnabled && string.IsNullOrWhiteSpace(invitationCode)))
			{
				return HttpNotFound();
			}

			if ((ViewBag.Settings.LocalLoginByEmail || ViewBag.Settings.RequireUniqueEmail) && string.IsNullOrWhiteSpace(model.Email))
			{
				ModelState.AddModelError("Email", ViewBag.IdentityErrors.EmailRequired().Description);
			}

			if (!ViewBag.Settings.LocalLoginByEmail && string.IsNullOrWhiteSpace(model.Username))
			{
				ModelState.AddModelError("Username", ViewBag.IdentityErrors.UserNameRequired().Description);
			}

			if (!string.Equals(model.Password, model.ConfirmPassword))
			{
				ModelState.AddModelError("Password", ViewBag.IdentityErrors.PasswordConfirmationFailure().Description);
			}

			if (ModelState.IsValid)
			{
				var invitation = await FindInvitationByCodeAsync(invitationCode);
				var contactId = ToContactId(invitation);

				if (!string.IsNullOrWhiteSpace(invitationCode) && contactId == null)
				{
					AddErrors(ViewBag.IdentityErrors.InvalidInvitationCode());
				}

				if (ModelState.IsValid)
				{
					ApplicationUser user;
					IdentityResult result;

					if (contactId == null)
					{
						// Create a new user
						user = new ApplicationUser
						{
							UserName = ViewBag.Settings.LocalLoginByEmail ? model.Email : model.Username,
							Email = model.Email
						};

						result = await UserManager.CreateAsync(user, model.Password);
					}
					else
					{
						// Update the existing invited user
						user = await UserManager.FindByIdAsync(contactId.Id.ToString());

						if (user != null)
						{
							result = await UserManager.InitializeUserAsync(
								user,
								ViewBag.Settings.LocalLoginByEmail ? model.Email : model.Username,
								model.Password,
								model.Email,
								ViewBag.Settings.TriggerLockoutOnFailedPassword);
						}
						else
						{
							// Contact does not exist or login is disabled
							result = IdentityResult.Failed(ViewBag.IdentityErrors.InvalidInvitationCode().Description);
						}

						if (!result.Succeeded)
						{
							AddErrors(result);

							ViewBag.ReturnUrl = returnUrl;
							ViewBag.InvitationCode = invitationCode;

							return View("RedeemInvitation", new RedeemInvitationViewModel { InvitationCode = invitationCode });
						}
					}

					if (result.Succeeded)
					{
						if (invitation != null)
						{
							var redeemResult = await InvitationManager.RedeemAsync(invitation, user, Request.UserHostAddress);

							if (redeemResult.Succeeded)
							{
								return await SignInAsync(user, returnUrl);
							}
							else
							{
								AddErrors(redeemResult);
							}
						}
						else
						{
							return await SignInAsync(user, returnUrl);
						}
					}
					else
					{
						AddErrors(result);
					}
				}
			}

			// If we got this far, something failed, redisplay form
			ViewBag.ReturnUrl = returnUrl;
			ViewBag.InvitationCode = invitationCode;
			return View(GetRegisterViewModel(model, null));
		}

		//
		// GET: /Login/ForgotPassword
		[HttpGet]
		[AllowAnonymous]
		[LocalLogin]
		public ActionResult ForgotPassword()
		{
			if (!ViewBag.Settings.ResetPasswordEnabled)
			{
				return HttpNotFound();
			}

			return View();
		}

		//
		// POST: /Login/ForgotPassword
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		[LocalLogin]
		public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
		{
			if (!ViewBag.Settings.ResetPasswordEnabled)
			{
				return HttpNotFound();
			}

			if (ModelState.IsValid)
			{
				if (string.IsNullOrWhiteSpace(model.Email))
				{
					return HttpNotFound();
				}

				var user = await UserManager.FindByEmailAsync(model.Email);

				if (user == null || !user.LogonEnabled || (ViewBag.Settings.ResetPasswordRequiresConfirmedEmail && !(await UserManager.IsEmailConfirmedAsync(user.Id))))
				{
					// Don't reveal that the user does not exist or is not confirmed
					return View("ForgotPasswordConfirmation");
				}

				var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
				var callbackUrl = Url.Action("ResetPassword", "Login", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
				var parameters = new Dictionary<string, object> { { "UserId", user.Id }, { "Code", code }, { "UrlCode", Encoder.UrlEncode(code) }, { "CallbackUrl", callbackUrl }, { "Email", model.Email } };
				await OrganizationManager.InvokeProcessAsync("adx_SendPasswordResetToContact", user.ContactId, parameters);
				//await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking here: <a href=\"" + callbackUrl + "\">link</a>");

				if (ViewBag.Settings.IsDemoMode)
				{
					ViewBag.DemoModeLink = callbackUrl;
				}

				return View("ForgotPasswordConfirmation");
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		//
		// GET: /Login/ForgotPasswordConfirmation
		[HttpGet]
		[AllowAnonymous]
		[LocalLogin]
		public ActionResult ForgotPasswordConfirmation()
		{
			if (!ViewBag.Settings.ResetPasswordEnabled)
			{
				return HttpNotFound();
			}

			return View();
		}

		//
		// GET: /Login/ResetPassword
		[HttpGet]
		[AllowAnonymous]
		[LocalLogin]
		public ActionResult ResetPassword(string userId, string code)
		{
			if (!ViewBag.Settings.ResetPasswordEnabled)
			{
				return HttpNotFound();
			}

			if (userId == null || code == null)
			{
				return HttpNotFound();
			}

			return View();
		}

		//
		// POST: /Login/ResetPassword
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		[LocalLogin]
		public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
		{
			if (!ViewBag.Settings.ResetPasswordEnabled)
			{
				return HttpNotFound();
			}

			if (!string.Equals(model.Password, model.ConfirmPassword))
			{
				ModelState.AddModelError("Password", ViewBag.IdentityErrors.PasswordConfirmationFailure().Description);
			}

			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var user = await UserManager.FindByIdAsync(model.UserId);

			if (user == null || !user.LogonEnabled)
			{
				// Don't reveal that the user does not exist
				return RedirectToAction("ResetPasswordConfirmation", "Login");
			}

			var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
			if (result.Succeeded)
			{
				return RedirectToAction("ResetPasswordConfirmation", "Login");
			}

			AddErrors(result);
			return View();
		}

		//
		// GET: /Login/ResetPasswordConfirmation
		[HttpGet]
		[AllowAnonymous]
		[LocalLogin]
		public ActionResult ResetPasswordConfirmation()
		{
			if (!ViewBag.Settings.ResetPasswordEnabled)
			{
				return HttpNotFound();
			}

			return View();
		}

		//
		// GET: /Login/ConfirmEmail
		[HttpGet]
		[AllowAnonymous]
		public ActionResult ConfirmEmail()
		{
			if (User.Identity.IsAuthenticated)
			{
				return RedirectToProfile(null);
			}

			return View();
		}

		//
		// POST: /Login/ExternalLogin
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		[ExternalLogin]
		public ActionResult ExternalLogin(string provider, string returnUrl, string invitationCode)
		{
			// Request a redirect to the external login provider
			return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Login", new { ReturnUrl = returnUrl, InvitationCode = invitationCode }));
		}

		//
		// GET: /Login/ExternalLogin
		[HttpGet]
		[AllowAnonymous]
		[ExternalLogin]
		public ActionResult ExternalLogin(string provider, string returnUrl)
		{
			return ExternalLogin(provider, returnUrl, null);
		}

		//
		// GET: /Login/SendCode
		[HttpGet]
		[AllowAnonymous]
		public async Task<ActionResult> SendCode(string returnUrl, string invitationCode, bool rememberMe = false)
		{
			var userId = await SignInManager.GetVerifiedUserIdAsync();
			if (userId == null)
			{
				throw new ApplicationException("Account error.");
			}
			var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);

			if (userFactors.Count() == 1)
			{
				// Send the code directly for a single option
				return await SendCode(new SendCodeViewModel { SelectedProvider = userFactors.Single(), ReturnUrl = returnUrl, RememberMe = rememberMe, InvitationCode = invitationCode });
			}

			var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
			return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe, InvitationCode = invitationCode });
		}

		//
		// POST: /Login/SendCode
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> SendCode(SendCodeViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View();
			}

			// Generate the token and send it
			if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
			{
				throw new ApplicationException("Account error.");
			}
			return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe, InvitationCode = model.InvitationCode });
		}

		//
		// GET: /Login/ExternalLoginCallback
		[HttpGet]
		[AllowAnonymous]
		[ExternalLogin]
		public async Task<ActionResult> ExternalLoginCallback(string returnUrl, string invitationCode)
		{
			var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();

			if (loginInfo == null)
			{
				return RedirectToAction("Login");
			}

			// Sign in the user with this external login provider if the user already has a login
			var result = await SignInManager.ExternalSignInAsync(loginInfo, false, (bool) ViewBag.Settings.TriggerLockoutOnFailedPassword);

			switch (result)
			{
				case SignInStatus.Success:
					return await RedirectOnPostAuthenticate(returnUrl, invitationCode);
				case SignInStatus.LockedOut:
					AddErrors(ViewBag.IdentityErrors.UserLocked());
					return View("Login", GetLoginViewModel(null, null, returnUrl, invitationCode));
				case SignInStatus.RequiresVerification:
					return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, InvitationCode = invitationCode });
				case SignInStatus.Failure:
				default:
					// If the user does not have an account, then prompt the user to create an account
					ViewBag.ReturnUrl = returnUrl;
					ViewBag.InvitationCode = invitationCode;
					var contactId = ToContactId(await FindInvitationByCodeAsync(invitationCode));
					var email = (contactId != null ? contactId.Name : null) ?? loginInfo.Email;
					var username = loginInfo.Login.ProviderKey;

					return await ExternalLoginConfirmation(username, email, returnUrl, invitationCode, loginInfo);
			}
		}

		//
		// POST: /Login/ExternalLoginConfirmation
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		[ExternalLogin]
		public Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl, string invitationCode)
		{
			return ExternalLoginConfirmation(model.Email, model.Email, returnUrl, invitationCode, null);
		}

		protected virtual async Task<ActionResult> ExternalLoginConfirmation(string username, string email, string returnUrl, string invitationCode, ExternalLoginInfo loginInfo)
		{
			if (!ViewBag.Settings.RegistrationEnabled || (!ViewBag.Settings.OpenRegistrationEnabled && !ViewBag.Settings.InvitationEnabled))
			{
				AddErrors(ViewBag.IdentityErrors.InvalidLogin());

				// Registration is disabled
				return View("Login", GetLoginViewModel(null, null, returnUrl, invitationCode));
			}

			if (!ViewBag.Settings.OpenRegistrationEnabled && ViewBag.Settings.InvitationEnabled && string.IsNullOrWhiteSpace(invitationCode))
			{
				// Registration requires an invitation
				return RedirectToAction("RedeemInvitation", new { ReturnUrl = returnUrl });
			}

			if (User.Identity.IsAuthenticated)
			{
				return Redirect(returnUrl ?? "~/");
			}

			if (ModelState.IsValid)
			{
				var invitation = await FindInvitationByCodeAsync(invitationCode);
				var contactId = ToContactId(invitation);

				if (!string.IsNullOrWhiteSpace(invitationCode) && contactId == null)
				{
					AddErrors(ViewBag.IdentityErrors.InvalidInvitationCode());
				}

				if (ModelState.IsValid)
				{
					// Validate the username and email
					var user = contactId != null
						? new ApplicationUser { UserName = username, Email = email, Id = contactId.Id.ToString() }
						: new ApplicationUser { UserName = username, Email = email };

					var validateResult = await UserManager.UserValidator.ValidateAsync(user);

					if (validateResult.Succeeded)
					{
						// Get the information about the user from the external login provider
						var info = loginInfo ?? await AuthenticationManager.GetExternalLoginInfoAsync();

						if (info == null)
						{
							return View("ExternalLoginFailure");
						}

						IdentityResult result;

						if (contactId == null)
						{
							if (!ViewBag.Settings.OpenRegistrationEnabled)
							{
								throw new InvalidOperationException("Open registration is not enabled.");
							}

							// Create a new user
							result = await UserManager.CreateAsync(user);
						}
						else
						{
							// Update the existing invited user
							user = await UserManager.FindByIdAsync(contactId.Id.ToString());

							if (user != null)
							{
								result = await UserManager.InitializeUserAsync(user, username, null, email, ViewBag.Settings.TriggerLockoutOnFailedPassword);
							}
							else
							{
								// Contact does not exist or login is disabled
								result = IdentityResult.Failed(ViewBag.IdentityErrors.InvalidInvitationCode().Description);
							}

							if (!result.Succeeded)
							{
								AddErrors(result);

								ViewBag.ReturnUrl = returnUrl;
								ViewBag.InvitationCode = invitationCode;

								return View("RedeemInvitation", new RedeemInvitationViewModel { InvitationCode = invitationCode });
							}
						}

						if (result.Succeeded)
						{
							var addResult = await UserManager.AddLoginAsync(user.Id, info.Login);

							if (addResult.Succeeded)
							{
								if (invitation != null)
								{
									var redeemResult = await InvitationManager.RedeemAsync(invitation, user, Request.UserHostAddress);

									if (redeemResult.Succeeded)
									{
										return await SignInAsync(user, returnUrl);
									}
									else
									{
										AddErrors(redeemResult);
									}
								}
								else
								{
									return await SignInAsync(user, returnUrl);
								}
							}
							else
							{
								AddErrors(addResult);
							}
						}
						else
						{
							AddErrors(result);
						}
					}
					else
					{
						AddErrors(validateResult);
					}
				}
			}

			ViewBag.ReturnUrl = returnUrl;
			ViewBag.InvitationCode = invitationCode;
			return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = email });
		}

		//
		// GET: /Login/LogOff
		[HttpGet]
		public ActionResult LogOff(string returnUrl)
		{
			AuthenticationManager.SignOut(new AuthenticationProperties { RedirectUri = returnUrl });
			return Redirect(returnUrl ?? "~/");
		}

		//
		// POST: /Login/LogOff
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult LogOff()
		{
			AuthenticationManager.SignOut();
			return Redirect("~/");
		}

		//
		// GET: /Login/ExternalLoginFailure
		[HttpGet]
		[AllowAnonymous]
		[ExternalLogin]
		public ActionResult ExternalLoginFailure()
		{
			return View();
		}

		//
		// GET: /Login/RedeemInvitation
		[HttpGet]
		[AllowAnonymous]
		public ActionResult RedeemInvitation(string returnUrl, [Bind(Prefix="invitation")]string invitationCode, bool invalid = false)
		{
			if (!ViewBag.Settings.RegistrationEnabled || !ViewBag.Settings.InvitationEnabled)
			{
				return HttpNotFound();
			}

			if (invalid)
			{
				ModelState.AddModelError("InvitationCode", ViewBag.IdentityErrors.InvalidInvitationCode().Description);
			}

			ViewBag.ReturnUrl = returnUrl;
			ViewBag.InvitationCode = invitationCode;
			return View(new RedeemInvitationViewModel { InvitationCode = invitationCode });
		}

		//
		// POST: /Login/RedeemInvitation
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> RedeemInvitation(RedeemInvitationViewModel model, string returnUrl)
		{
			if (!ViewBag.Settings.RegistrationEnabled || !ViewBag.Settings.InvitationEnabled)
			{
				return HttpNotFound();
			}

			if (ModelState.IsValid)
			{
				var contactId = ToContactId(await FindInvitationByCodeAsync(model.InvitationCode));
				var email = contactId != null ? contactId.Name : null;

				if (contactId != null)
				{
					ViewBag.ReturnUrl = returnUrl;
					ViewBag.InvitationCode = model.InvitationCode;

					return model.RedeemByLogin
						? View("Login", GetLoginViewModel(null, null, returnUrl, model.InvitationCode))
						: View("Register", GetRegisterViewModel(new RegisterViewModel { Email = email }, null));
				}

				ModelState.AddModelError("InvitationCode", ViewBag.IdentityErrors.InvalidInvitationCode().Description);
			}

			ViewBag.ReturnUrl = returnUrl;
			return View(model);
		}

		[HttpGet]
		[AllowAnonymous]
		[ExternalLogin]
		public Task<ActionResult> FacebookExternalLogin()
		{
			Response.SuppressFormsAuthenticationRedirect = true;

			var website = HttpContext.GetOwinContext().Get<ApplicationWebsite>();
			var authenticationType = website.GetFacebookAuthenticationType<ApplicationWebsite, string>();
			var properties = website.GetFacebookWsFederationMessage<ApplicationWebsite, string>();

			var returnUrl = Url.Action("FacebookReloadParent", "Login");
			var redirectUri = Url.Action("ExternalLoginCallback", "Login", new { ReturnUrl = returnUrl });
			var result = new ChallengeResult(authenticationType.LoginProvider, redirectUri) { Properties = properties };

			return Task.FromResult(result as ActionResult);
		}

		[HttpGet]
		[AllowAnonymous]
		[ExternalLogin]
		public ActionResult FacebookReloadParent()
		{
			return View("LoginReloadParent");
		}

		[HttpPost]
		[AllowAnonymous]
		[ExternalLogin]
		public async Task<ActionResult> FacebookExternalLoginCallback(string signed_request, string returnUrl)
		{
			if (string.IsNullOrWhiteSpace(signed_request))
			{
				return HttpNotFound();
			}

			var website = HttpContext.GetOwinContext().Get<ApplicationWebsite>();
			var loginInfo = website.GetFacebookLoginInfo<ApplicationWebsite, string>(signed_request);

			if (loginInfo == null)
			{
				return RedirectToLocal(returnUrl);
			}

			// Sign in the user with this external login provider if the user already has a login
			var result = await SignInManager.ExternalSignInAsync(loginInfo, false, (bool) ViewBag.Settings.TriggerLockoutOnFailedPassword);

			switch (result)
			{
				case SignInStatus.Success:
				case SignInStatus.LockedOut:
					return RedirectToLocal(returnUrl);
				case SignInStatus.RequiresVerification:
					return RedirectToAction("SendCode", new { ReturnUrl = returnUrl });
				case SignInStatus.Failure:
				default:
					// If the user does not have an account, then prompt the user to create an account
					ViewBag.ReturnUrl = returnUrl;
					var email = loginInfo.Email;
					var username = loginInfo.Login.ProviderKey;

					return await ExternalLoginConfirmation(username, email, returnUrl, null, loginInfo);
			}
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

		private void AddErrors(IdentityError error)
		{
			AddErrors(IdentityResult.Failed(error.Description));
		}

		private void AddErrors(IdentityResult result)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError("", error);
			}
		}

		private async Task<ActionResult> RedirectOnPostAuthenticate(string returnUrl, string invitationCode)
		{
			var userId = AuthenticationManager.AuthenticationResponseGrant.Identity.GetUserId();
			var user = await UserManager.FindByIdAsync(userId);

			if (user != null)
			{
				IdentityResult redeemResult;
				var invitation = await FindInvitationByCodeAsync(invitationCode);

				if (invitation != null)
				{
					// Redeem invitation for the existing/registered contact
					redeemResult = await InvitationManager.RedeemAsync(invitation, user, Request.UserHostAddress);
				}
				else if (!string.IsNullOrWhiteSpace(invitationCode))
				{
					redeemResult = IdentityResult.Failed(ViewBag.IdentityErrors.InvalidInvitationCode().Description);
				}
				else
				{
					redeemResult = IdentityResult.Success;
				}

				if (!redeemResult.Succeeded)
				{
					return RedirectToAction("RedeemInvitation", new { ReturnUrl = returnUrl, invitation = invitationCode, invalid = true });
				}

				if (!DisplayModeIsActive() && (user.HasProfileAlert || user.ProfileModifiedOn == null))
				{
					return RedirectToProfile(returnUrl);
				}
			}

			return RedirectToLocal(returnUrl);
		}

		private bool DisplayModeIsActive()
		{
			return DisplayModeProvider.Instance
				.GetAvailableDisplayModesForContext(HttpContext, null)
				.OfType<HostNameSettingDisplayMode>()
				.Any();
		}

		private static ActionResult RedirectToProfile(string returnUrl)
		{
			var query = !string.IsNullOrWhiteSpace(returnUrl) ? new NameValueCollection {{"ReturnUrl", returnUrl}} : null;

			return new RedirectToSiteMarkerResult("Profile", query);
		}

		private ActionResult RedirectToLocal(string returnUrl)
		{
			if (Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}
			return Redirect("~/");
		}

		internal class ChallengeResult : HttpUnauthorizedResult
		{
			public ChallengeResult(string provider, string redirectUri)
				: this(provider, redirectUri, null)
			{
			}

			public ChallengeResult(string provider, string redirectUri, string userId)
			{
				LoginProvider = provider;
				RedirectUri = redirectUri;
				UserId = userId;
			}

			public string LoginProvider { get; set; }
			public string RedirectUri { get; set; }
			public string UserId { get; set; }
			public IDictionary<string, string> Properties { get; set; }

			public override void ExecuteResult(ControllerContext context)
			{
				var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
				if (UserId != null)
				{
					properties.Dictionary[XsrfKey] = UserId;
				}
				if (Properties != null)
				{
					foreach (var property in Properties)
					{
						properties.Dictionary.Add(property);
					}
				}
				context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
			}
		}

		private object GetLoginViewModel(LoginViewModel local, IEnumerable<AuthenticationDescription> external, string returnUrl, string invitationCode)
		{
			ViewData["local"] = local ?? new LoginViewModel();
			ViewData["external"] = external ?? GetExternalAuthenticationTypes();

			ViewBag.ReturnUrl = returnUrl;
			ViewBag.InvitationCode = invitationCode;

			return ViewData;
		}

		private object GetRegisterViewModel(RegisterViewModel local, IEnumerable<AuthenticationDescription> external)
		{
			if (ViewBag.Settings.RegistrationEnabled && (ViewBag.Settings.OpenRegistrationEnabled || ViewBag.Settings.InvitationEnabled))
			{
				ViewData["local"] = local ?? new RegisterViewModel();
				ViewData["external"] = external ?? GetExternalAuthenticationTypes();
			}

			return ViewData;
		}

		private IEnumerable<AuthenticationDescription> GetExternalAuthenticationTypes()
		{
			return HttpContext.GetOwinContext().Authentication.GetExternalAuthenticationTypes().OrderBy(type => type.Caption).ToList();
		}

		private async Task<ApplicationInvitation> FindInvitationByCodeAsync(string invitationCode)
		{
			if (string.IsNullOrWhiteSpace(invitationCode)) return null;
			if (!ViewBag.Settings.InvitationEnabled) return null;

			return await InvitationManager.FindByCodeAsync(invitationCode);
		}

		private static EntityReference ToContactId(ApplicationInvitation invitation)
		{
			return invitation != null && invitation.InvitedContact != null
				? new EntityReference(invitation.InvitedContact.LogicalName, invitation.InvitedContact.Id) { Name = invitation.Email }
				: null;
		}

		private async Task<ActionResult> SignInAsync(ApplicationUser user, string returnUrl, bool isPersistent = false, bool rememberBrowser = false)
		{
			await SignInManager.SignInAsync(user, isPersistent, rememberBrowser);
			return await RedirectOnPostAuthenticate(returnUrl, null);
		}
		#endregion
	}
}