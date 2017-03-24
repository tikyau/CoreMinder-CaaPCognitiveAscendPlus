using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Adxstudio.Xrm.Account;
using Adxstudio.Xrm.Cms;
using Adxstudio.Xrm.Collections.Generic;
using Adxstudio.Xrm.Configuration;
using Adxstudio.Xrm.IdentityModel;
using Adxstudio.Xrm.IdentityModel.Configuration;
using Adxstudio.Xrm.IdentityModel.Web.Modules;
using Adxstudio.Xrm.OpenAuth;
using Adxstudio.Xrm.OpenAuth.Clients;
using Adxstudio.Xrm.OpenAuth.Configuration;
using Adxstudio.Xrm.Parature;
using Adxstudio.Xrm.Services;
using Adxstudio.Xrm.Web.Mvc;
using DotNetOpenAuth.Messaging;
using Microsoft.Security.Application;
using Microsoft.Xrm.Client;
using Microsoft.Xrm.Portal;
using Microsoft.Xrm.Portal.Configuration;
using Microsoft.Xrm.Portal.IdentityModel.Configuration;
using Microsoft.Xrm.Portal.IdentityModel.Web.Modules;
using Microsoft.Xrm.Portal.Web;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Site.Areas.Account.ViewModels;

namespace Site.Areas.Account.Controllers
{
	[PortalView]
	public class AccountController : Controller
	{
		private static readonly string _defaultEmailConfirmationWorkflowName = "ADX Sign Up Email";
		private static readonly string _defaultPasswordRecoveryWorkflowName = "ADX Sign In Password Recovery";

		private static readonly string _errorLockedOut = "Your account has been locked out because of either too many invalid login attempts or an administrator has locked the account. Please contact the administrator to have your account unlocked.";
		private static readonly string _errorNotApproved = "Your account has not yet been approved. You cannot login until an administrator has enabled login for your account.";
		private static readonly string _errorUnknown = "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

		protected override void OnAuthorization(AuthorizationContext filterContext)
		{
			base.OnAuthorization(filterContext);

			var resolutionSettings = FederationCrmConfigurationManager.GetUserResolutionSettings();

			ViewBag.MemberEntityName = resolutionSettings.MemberEntityName ?? "contact";
			ViewBag.AttributeMapUsername = resolutionSettings.AttributeMapUsername ?? "adx_username";

			var registrationSettings = FederationCrmConfigurationManager.GetUserRegistrationSettings();

			ViewBag.PortalName = registrationSettings.PortalName;
			ViewBag.RegistrationEnabled = registrationSettings.Enabled;
			ViewBag.ReturnUrlKey = registrationSettings.ReturnUrlKey ?? "returnurl";
			ViewBag.InvitationCodeKey = registrationSettings.InvitationCodeKey ?? "invitation";
			ViewBag.ChallengeAnswerKey = registrationSettings.ChallengeAnswerKey ?? "answer";
			ViewBag.ResultCodeKey = registrationSettings.ResultCodeKey ?? "result-code";
			ViewBag.DefaultReturnPath = registrationSettings.DefaultReturnPath ?? "~/";
			ViewBag.ProfilePath = registrationSettings.ProfilePath ?? GetPathBySiteMarker("Profile") ?? ViewBag.DefaultReturnPath;

			ViewBag.AttributeMapIdentityProvider = registrationSettings.AttributeMapIdentityProvider ?? "adx_identityprovidername";
			ViewBag.RequiresInvitation = registrationSettings.RequiresInvitation;
			ViewBag.RequiresChallengeAnswer = registrationSettings.RequiresChallengeAnswer;
			ViewBag.OpenRegistrationEnabled = !registrationSettings.RequiresInvitation;
			ViewBag.RequiresConfirmation = registrationSettings.RequiresConfirmation;
			ViewBag.InvitationCodeDuration = registrationSettings.InvitationCodeDuration;
			ViewBag.RequiresQuestionAndAnswer = Membership.RequiresQuestionAndAnswer;
			ViewBag.RequiresUniqueEmail = Membership.Provider.RequiresUniqueEmail;

			ViewBag.IdentityModelServices = IdentityModelServicesConfigurationManager.GetSection();
			ViewBag.IdentityModelEnabled = AdxstudioCrmConfigurationManager.GetCrmSection().IdentityModelEnabled;
			ViewBag.OpenAuthEnabled = OpenAuthConfigurationManager.GetSection().Enabled;
			ViewBag.MembershipProviderEnabled = AdxstudioCrmConfigurationManager.GetCrmSection().MembershipProviderEnabled;
			ViewBag.FederatedAuthEnabled = ViewBag.IdentityModelEnabled || ViewBag.OpenAuthEnabled;

			ViewBag.DisplayRememberMe = true;

			ViewBag.ResultCode = filterContext.RequestContext.HttpContext.Request[ViewBag.ResultCodeKey];
			ViewBag.ReturnUrl = filterContext.RequestContext.HttpContext.Request[ViewBag.ReturnUrlKey];

			ViewBag.ReturnUrlRouteValues = !string.IsNullOrWhiteSpace(ViewBag.ReturnUrl)
				? new RouteValueDictionary { { ViewBag.ReturnUrlKey, ViewBag.ReturnUrl } }
				: new RouteValueDictionary();

			ViewBag.ReturnUrlQueryString = !string.IsNullOrWhiteSpace(ViewBag.ReturnUrl)
				? new NameValueCollection { { ViewBag.ReturnUrlKey, ViewBag.ReturnUrl } }
				: new NameValueCollection();
		}

		protected ActionResult AuthorizeResult
		{
			get { return ControllerContext.RouteData.Values["AuthorizeResult"] as ActionResult; }
		}

		// GET: /login
		[HttpGet]
		[NoCache]
		public ActionResult SignIn()
		{
			ViewBag.PasswordRecoveryQueryString = new NameValueCollection { { ViewBag.ReturnUrlKey, ViewBag.ReturnUrl ?? VirtualPathUtility.ToAbsolute(ViewBag.DefaultReturnPath) } };

			return View("SignIn", GetSignInViewModel(null, null));
		}

		// POST: /login/SignInLocal
		[HttpPost]
		[LocalAuthActionFilter]
		[ValidateAntiForgeryToken]
		public ActionResult SignInLocal(SignInLocalViewModel model)
		{
			if (ModelState.IsValid)
			{
				if (string.IsNullOrWhiteSpace(model.UserName))
				{
					ModelState.AddModelError("UserName", "Username is a required field.");
				}

				if (string.IsNullOrWhiteSpace(model.Password))
				{
					ModelState.AddModelError("Password", "Password is a required field.");
				}

				if (ModelState.IsValid)
				{
					// authenticate the credentials

					var result = ExecuteSignInLocal(model);

					if (result != null)
					{
						return result;
					}
				}
			}

			return View("SignIn", GetSignInViewModel(model, null));
		}

		private ActionResult ExecuteSignInLocal(SignInLocalViewModel model)
		{
			if (Membership.ValidateUser(model.UserName, model.Password))
			{
				FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);

				// if the contact is missing required field values, redirect to the profile page

				var user = Membership.GetUser(model.UserName);

				Guid contactId;

				if (TryConvertToContactId(user, out contactId))
				{
					PostSignIn(contactId);

					using (var context = CreateServiceContext())
					{
						var returnUrl = context.ValidateProfileSuccessfullySaved(contactId)
							? GetReturnUrl()
							: GetProfileUrl();

						return Redirect(returnUrl);
					}
				}

				return Redirect(GetReturnUrl());
			}

			// determine the reason for the validation failure

			var membershipUser = Membership.GetUser(model.UserName);

			if (membershipUser != null && membershipUser.IsLockedOut)
			{
				var errorMessage = GetErrorMessage("LockedOut", _errorLockedOut);

				ModelState.AddModelError(string.Empty, errorMessage);
			}
			else if (membershipUser != null && !membershipUser.IsApproved)
			{
				var errorMessage = GetErrorMessage("NotApproved", _errorNotApproved);

				ModelState.AddModelError(string.Empty, errorMessage);
			}
			else
			{
				ModelState.AddModelError(string.Empty, "Invalid username or password.");
			}

			return null;
		}

		// GET: /login/SignInReloadParent
		[HttpGet]
		[NoCache]
		public ActionResult SignInReloadParent()
		{
			return View("SignInReloadParent");
		}

		// POST: /auth/wsfederation
		[HttpPost]
		[NoCache]
		[FederationAuthorize]
		public ActionResult SignInWsFederation()
		{
			return AuthorizeResult ?? new RedirectResult(ViewBag.DefaultReturnPath);
		}

		[HttpGet]
		[NoCache]
		public async Task<ActionResult> SignInOpenAuth(string provider)
			{
			var authorizeResult = await SignInOpenAuthAsync(provider);

			return authorizeResult ?? new RedirectResult(ViewBag.DefaultReturnPath);
		}

		private async Task<ActionResult> SignInOpenAuthAsync(string provider)
				{
			if (string.IsNullOrWhiteSpace(provider)) return new HttpNotFoundResult();

			var portalContext = CreatePortalContext();
			var adapter = new OpenAuthDataAdapter(new PortalContextDataAdapterDependencies(portalContext));

			var openId = adapter.SelectOpenIdProviders().FirstOrDefault(p => p.IdentityProvider == provider);

			if (openId != null)
			{
				var response = await openId.Client.GetResponseAsync(Request, Response.ClientDisconnectedToken);

				if (response != null)
				{
					var graph = await openId.Client.GetGraphAsync(response, Response.ClientDisconnectedToken);
					var resolutionSettings = portalContext as IUserResolutionSettings;
					var attribute = new OpenAuthAuthorizeAttribute(FederationCrmConfigurationManager.GetUserRegistrationSettings(), resolutionSettings);
					var signInContext = CreateSignInContext(Request[ViewBag.ReturnUrlKey], Request[ViewBag.InvitationCodeKey], Request[ViewBag.ChallengeAnswerKey]);

					return attribute.GetActionResult(ControllerContext, signInContext, openId, graph);
				}
			}

			var oauth2 = adapter.SelectOAuth2Providers().FirstOrDefault(p => p.IdentityProvider == provider);

			if (oauth2 != null)
			{
				var authorization = await oauth2.Client.ProcessUserAuthorizationAsync(Request, Response.ClientDisconnectedToken);

				if (authorization != null && !string.IsNullOrWhiteSpace(authorization.AccessToken))
		{
					var graph = await oauth2.Client.GetGraphAsync(authorization, null, Response.ClientDisconnectedToken);
					var resolutionSettings = portalContext as IUserResolutionSettings;
					var attribute = new OpenAuthAuthorizeAttribute(FederationCrmConfigurationManager.GetUserRegistrationSettings(), resolutionSettings);

					return attribute.GetActionResult(ControllerContext, authorization.ClientState, oauth2, graph);
				}
			}

			var oauth1 = adapter.SelectOAuthProviders().FirstOrDefault(p => p.IdentityProvider == provider);

			if (oauth1 != null)
		{
				var accessTokenResponse = await oauth1.Client.ProcessUserAuthorizationAsync(Request.Url, Response.ClientDisconnectedToken);

				if (accessTokenResponse != null)
		{
					var graph = await oauth1.Client.GetGraphAsync(accessTokenResponse, Response.ClientDisconnectedToken);
					var resolutionSettings = portalContext as IUserResolutionSettings;
					var attribute = new OpenAuthAuthorizeAttribute(FederationCrmConfigurationManager.GetUserRegistrationSettings(), resolutionSettings);
					var signInContext = CreateSignInContext(Request[ViewBag.ReturnUrlKey], Request[ViewBag.InvitationCodeKey], Request[ViewBag.ChallengeAnswerKey]);

					return attribute.GetActionResult(ControllerContext, signInContext, oauth1, graph);
				}
		}

			return await SignInFederatedAsync(provider, Request[ViewBag.InvitationCodeKey], Request[ViewBag.ChallengeAnswerKey]);
		}

		// POST: /app/facebook
		[HttpPost]
		[NoCache]
		public async Task<ActionResult> FacebookApp(string provider)
		{
			var authorizeResult = await FacebookAppAsync(provider);

			return authorizeResult ?? new RedirectResult(ViewBag.DefaultReturnPath);
		}

		private async Task<ActionResult> FacebookAppAsync(string provider)
		{
			if (string.IsNullOrWhiteSpace(provider)) return new HttpNotFoundResult();

			var portalContext = CreatePortalContext();
			var adapter = new OpenAuthDataAdapter(new PortalContextDataAdapterDependencies(portalContext));
			var oauth2 = adapter.SelectOAuth2Providers().FirstOrDefault(p => p.IdentityProvider == provider);

			if (oauth2 == null) return new HttpNotFoundResult();

			var client = oauth2.Client as Adxstudio.Xrm.OpenAuth.Clients.Facebook.FacebookClient;

			if (client == null) return new HttpNotFoundResult();

			var authorization = client.ProcessSignedRequest(Request);

			if (authorization != null && !string.IsNullOrWhiteSpace(authorization.AccessToken))
		{
				var graph = await client.GetGraphAsync(authorization, null, Response.ClientDisconnectedToken);
				var resolutionSettings = portalContext as IUserResolutionSettings;
				var attribute = new OpenAuthAuthorizeAttribute(FederationCrmConfigurationManager.GetUserRegistrationSettings(), resolutionSettings);

				return attribute.GetActionResult(ControllerContext, authorization.ClientState, oauth2, graph);
			}

			return new RedirectResult(ViewBag.DefaultReturnPath);
		}

		// GET: /login/PasswordRecovery
		[HttpGet]
		[NoCache]
		public ActionResult PasswordRecovery()
		{
			return View("PasswordRecoveryEmail");
		}

		// POST: /login/PasswordRecoveryEmail
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult PasswordRecoveryEmail(PasswordRecoveryViewModel model)
		{
			if (ModelState.IsValid)
			{
				if (string.IsNullOrWhiteSpace(model.Username))
				{
					ModelState.AddModelError("Username", "Username is a required field.");
				}

				if (ModelState.IsValid)
				{
					var workflowName = GetPasswordRecoveryWorkflowName();
					var user = Membership.GetUser(model.Username);
					var email = ConvertToEmail(user);

					if (user == null || user.IsLockedOut || string.IsNullOrWhiteSpace(email) || !Membership.EnablePasswordReset)
					{
						ModelState.AddModelError("Username", "Invalid username.");

						return View("PasswordRecoveryEmail", model);
					}

					if (!string.IsNullOrWhiteSpace(user.PasswordQuestion) || Membership.RequiresQuestionAndAnswer)
					{
						// the security answer is required

						if (!string.IsNullOrWhiteSpace(user.PasswordQuestion))
						{
							model.Question = user.PasswordQuestion;

							return View("PasswordRecoverySecurityQuestion", model);
						}
						else
						{
							// answer is required but the user does not have a question/answer specified

							ModelState.AddModelError("Username", "Invalid username.");

							return View("PasswordRecoveryEmail", model);
						}
					}
					else
					{
						if (!string.IsNullOrWhiteSpace(email) && Membership.EnablePasswordReset)
						{
							var password = ResetPassword(user);

							if (!string.IsNullOrWhiteSpace(password))
							{
								Guid contactId;

								if (TryConvertToContactId(user, out contactId))
								{
									using (var context = CreateServiceContext())
									{
										context.SendPasswordRecoveryEmail(contactId, password, workflowName);
									}
								}
								else
								{
									SendPasswordRecoveryEmail(user, password);
								}

								return View("PasswordRecoveryConfirmation");
							}
						}
					}
				}
			}

			return View("PasswordRecoveryEmail", model);
		}

		// POST: /login/PasswordRecoverySecurityAnswer
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult PasswordRecoverySecurityAnswer(PasswordRecoveryViewModel model)
		{
			if (ModelState.IsValid)
			{
				if (string.IsNullOrWhiteSpace(model.Answer))
				{
					ModelState.AddModelError("Answer", "Answer is a required field.");
				}

				if (ModelState.IsValid)
				{
					var workflowName = GetPasswordRecoveryWorkflowName();
					var user = Membership.GetUser(model.Username);
					var email = ConvertToEmail(user);

					if (user == null || user.IsLockedOut || string.IsNullOrWhiteSpace(email) || !Membership.EnablePasswordReset)
					{
						ModelState.AddModelError("Username", "Invalid username.");

						return View("PasswordRecoveryEmail", model);
					}

					var password = ResetPassword(user, model.Answer);

					if (!string.IsNullOrWhiteSpace(password))
					{
						Guid contactId;

						if (TryConvertToContactId(user, out contactId))
						{
							using (var context = CreateServiceContext())
							{
								context.SendPasswordRecoveryEmail(contactId, password, workflowName);
							}
						}
						else
						{
							SendPasswordRecoveryEmail(user, password);
						}

						return View("PasswordRecoveryConfirmation");
					}

					ModelState.AddModelError("Answer", "Invalid security answer.");
				}
			}

			return View("PasswordRecoverySecurityQuestion", model);
		}

		private static string ResetPassword(MembershipUser user, string answer = null)
		{
			try
			{
				return user.ResetPassword(answer);
			}
			catch (ArgumentException)
			{
			}
			catch (MembershipPasswordException)
			{
			}
			catch (ProviderException)
			{
			}

			return null;
		}

		private string GetPasswordRecoveryWorkflowName()
		{
			var portalContext = CreatePortalContext();
			var workflowName = portalContext.ServiceContext.GetSiteSettingValueByName(portalContext.Website, "Account/PasswordRecovery/WorkflowName") ?? _defaultPasswordRecoveryWorkflowName;

			using (var context = CreateServiceContext())
			{
				var workflow = context.GetWorkflowByName(workflowName);

				if (workflow == null)
				{
					throw new InvalidOperationException(string.Format("Password recovery is not configured. The workflow named '{0}' does not exist or is not activated. Ensure you have installed the correct solutions and followed the authentication setup instructions.", workflowName));
				}
			}

			return workflowName;
		}

		// GET: /account-signout
		[NoCache]
		public ActionResult SignOut()
		{
			if (ViewBag.FederatedAuthEnabled)
			{
				new AdxstudioFederationAuthenticationModule(HttpContext.ApplicationInstance).SignOut(false);
			}

			if (ViewBag.MembershipProviderEnabled)
			{
				FormsAuthentication.SignOut();
			}

			return Redirect(GetReturnUrl());
		}

		// GET: /Register
		[HttpGet]
		[NoCache]
		[RegistrationActionFilter]
		public ActionResult Redeem()
		{
			var invitation = Request[ViewBag.InvitationCodeKey];

			if (string.IsNullOrWhiteSpace(invitation))
			{
				return View("RedeemInvitationCode", new RedeemViewModel());
			}

			return CheckInvitationCode(new RedeemViewModel { InvitationCode = invitation });
		}

		// POST: /Register
		[HttpPost]
		[RegistrationActionFilter]
		[ValidateAntiForgeryToken]
		public ActionResult Redeem(RedeemViewModel model)
		{
			return View("RedeemInvitationCode", model);
		}

		// POST: /Register/RedeemInvitationCode
		[HttpPost]
		[RegistrationActionFilter]
		[ValidateAntiForgeryToken]
		public ActionResult RedeemInvitationCode(RedeemViewModel model)
		{
			return CheckInvitationCode(model);
		}

		private ActionResult CheckInvitationCode(RedeemViewModel model)
		{
			if (ModelState.IsValid)
			{
				if (string.IsNullOrWhiteSpace(model.InvitationCode))
				{
					AddModelErrorForInvitationCode();
				}

				if (ModelState.IsValid)
				{
					using (var context = CreateServiceContext())
					{
						var invitation = context.GetInvitation(model.InvitationCode);

						if (invitation != null)
						{
							model.InvitationType = (InvitationType)invitation.GetAttributeValue<OptionSetValue>("adx_type").Value;

							if (Request.IsAuthenticated)
							{
								var portalContextUser = CreatePortalContext().User;

								var user = context.CreateQuery("contact").First(entity => entity.GetAttributeValue<Guid>("contactid") == portalContextUser.Id);

								context.RedeemInvitationAndSave(user, model.InvitationCode, Request.UserHostAddress);

								model.Redeemed = true;

								return View("RedeemInvitationCode", model);
							}

							var contact = context.GetContactByInvitation(model.InvitationCode);

							if (contact == null)
							{
								return View("RedeemAccount", AddIdentityProviders(model));
							}
							
							var username = contact.GetAttributeValue<string>("adx_username");

							if (!string.IsNullOrWhiteSpace(username))
							{
								model.UserExists = true;

								return View("RedeemAccount", AddIdentityProviders(model));
							}

							var question = contact.GetAttributeValue<string>("adx_challengequestion");

							if (!string.IsNullOrWhiteSpace(question))
							{
								return View("RedeemChallengeQuestion", new RedeemViewModel {InvitationCode = model.InvitationCode, ChallengeQuestion = question});
							}

							if (!ViewBag.RequiresChallengeAnswer)
							{
								return GetActionResult(model) ?? View("RedeemAccount", AddIdentityProviders(model));
							}
						}
					}

					AddModelErrorForInvitationCode();
				}
			}

			return View("RedeemInvitationCode", model);
		}

		// POST: /Register/RedeemChallengeAnswer
		[HttpPost]
		[RegistrationActionFilter]
		[ValidateAntiForgeryToken]
		public ActionResult RedeemChallengeAnswer(RedeemViewModel model)
		{
			if (ModelState.IsValid)
			{
				if (string.IsNullOrWhiteSpace(model.InvitationCode))
				{
					AddModelErrorForInvitationCode();

					return View("RedeemInvitationCode", model);
				}

				if (string.IsNullOrWhiteSpace(model.ChallengeAnswer))
				{
					AddModelErrorForChallengeAnswer();
				}

				if (ModelState.IsValid)
				{
					using (var context = CreateServiceContext())
					{
						var contact = context.GetContactByInvitationAndAnswer(model.InvitationCode, model.ChallengeAnswer);

						if (contact != null)
						{
							return GetActionResult(model) ?? View("RedeemAccount", AddIdentityProviders(model));
						}
					}

					AddModelErrorForChallengeAnswer();
				}
			}

			return View("RedeemChallengeQuestion", model);
		}

		private ActionResult GetActionResult(RedeemViewModel model)
		{
			// if the user is signed in prior to registration, reuse the security token

			if (string.Equals(model.wa, "openauth"))
			{
				var registrationSettings = FederationCrmConfigurationManager.GetUserRegistrationSettings();
				var attribute = new OpenAuthAuthorizeAttribute(registrationSettings);

				return attribute.GetActionResult(ControllerContext, ViewBag.ReturnUrl as string, model.InvitationCode, model.ChallengeAnswer);
			}
			
			if (string.Equals(model.wa, "wsignin1.0"))
			{
				var registrationSettings = FederationCrmConfigurationManager.GetUserRegistrationSettings();
				var attribute = new FederationAuthorizeAttribute(registrationSettings);

				return attribute.GetActionResult(ControllerContext, ViewBag.ReturnUrl as string, model.InvitationCode, model.ChallengeAnswer);
			}

			return null;
		}

		private RedeemViewModel AddIdentityProviders(RedeemViewModel model)
		{
			model.IdentityProviders = GetIdentityProviders(model.InvitationCode, model.ChallengeAnswer).ToList();
			model.HomeRealmDiscoveryMetadataFeedUrl = GetHomeRealmDiscoveryMetadataFeedUrl(model.InvitationCode, model.ChallengeAnswer);

			return model;
		}

		private SignInFederatedViewModel AddIdentityProviders(SignInFederatedViewModel model)
		{
			model.IdentityProviders = GetIdentityProviders().ToList();
			model.HomeRealmDiscoveryMetadataFeedUrl = GetHomeRealmDiscoveryMetadataFeedUrl();

			return model;
		}

		// POST: /Register/RedeemLocal
		[HttpPost]
		[RegistrationActionFilter]
		[LocalAuthActionFilter]
		[ValidateAntiForgeryToken]
		public ActionResult RedeemLocal(RedeemViewModel model)
		{
			if (ModelState.IsValid)
			{
				if (string.IsNullOrWhiteSpace(model.InvitationCode))
				{
					AddModelErrorForInvitationCode();

					return View("RedeemInvitationCode", model);
				}

				if (ViewBag.RequiresChallengeAnswer && string.IsNullOrWhiteSpace(model.ChallengeAnswer))
				{
					AddModelErrorForChallengeAnswer();

					return View("RedeemChallengeQuestion", model);
				}

				// if user exists or it's a group invite and there's a sign in username, assume signing in
				if (model.UserExists || (model.InvitationType == InvitationType.Group && !string.IsNullOrWhiteSpace(model.SignInUsername)))
				{
					if (string.IsNullOrWhiteSpace(model.SignInPassword))
					{
						ModelState.AddModelError("Password", "Password is a required field.");
					}

					if (ModelState.IsValid)
					{
						// authenticate the credentials

						var result = ExecuteSignInLocal(new SignInLocalViewModel {UserName = model.SignInUsername, Password = model.SignInPassword});

						if (result != null)
						{
							var user = Membership.GetUser(model.SignInUsername);

							Guid contactId;

							if (TryConvertToContactId(user, out contactId))
							{
								using (var context = CreateServiceContext())
								{
									var contact = context.CreateQuery("contact").First(entity => entity.GetAttributeValue<Guid>("contactid") == contactId);

									context.RedeemInvitationAndSave(contact, model.InvitationCode, Request.UserHostAddress);
								}

								return result;
							}
						}
					}
				}
				else
				{
					ValidateLocalForm(model);

					if (ModelState.IsValid)
					{
						using (var context = CreateServiceContext())
						{
							var invitation = context.GetInvitation(model.InvitationCode);

							if (invitation != null)
							{
								var contact = context.GetContactByInvitationAndAnswer(model.InvitationCode, model.ChallengeAnswer, (bool)ViewBag.RequiresChallengeAnswer);

								var email = ViewBag.RequiresConfirmation ? null : model.Email;

								if (contact == null)
								{
									context.RedeemInvitationAndSave(model.InvitationCode, model.Username, model.Password, email, model.Question, model.Answer, Request.UserHostAddress);									
								}

								if (contact != null)
								{
									context.RedeemInvitationAndSave(contact, model.InvitationCode, model.Username, model.Password, email, model.Question, model.Answer, Request.UserHostAddress);
								}

								var result = ExecuteSignInLocal(new SignInLocalViewModel { UserName = model.Username, Password = model.Password });

								if (result != null)
								{
									return result;
								}
							}
						}
					}
				}
			}

			return View("RedeemAccount", AddIdentityProviders(model));
		}

		// POST: /Register/RedeemFederated
		[HttpPost]
		[RegistrationActionFilter]
		[FederatedAuthActionFilter]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> RedeemFederated(RedeemViewModel model)
		{
			if (ModelState.IsValid)
			{
				if (string.IsNullOrWhiteSpace(model.InvitationCode))
				{
					AddModelErrorForInvitationCode();

					return View("RedeemInvitationCode", model);
				}

				if (ViewBag.RequiresChallengeAnswer && string.IsNullOrWhiteSpace(model.ChallengeAnswer))
				{
					AddModelErrorForChallengeAnswer();

					return View("RedeemChallengeQuestion", model);
				}

				var result = await SignInFederatedAsync(model.IdentityProviderName, model.InvitationCode, model.ChallengeAnswer);

				if (result != null)
				{
					return result;
				}
			}

			return View("RedeemAccount", AddIdentityProviders(model));
		}

		// GET: /Sign-Up
		[HttpGet]
		[NoCache]
		[RegistrationActionFilter]
		public ActionResult SignUp()
		{
			return View("SignUp", GetSignUpViewModel(null, null, null));
		}

		// POST: /Sign-Up/SignUpLocal
		[HttpPost]
		[RegistrationActionFilter]
		[LocalAuthActionFilter]
		[ValidateAntiForgeryToken]
		public ActionResult SignUpLocal(SignUpLocalViewModel model)
		{
			if (ModelState.IsValid)
			{
				ValidateLocalForm(model);

				if (ModelState.IsValid)
				{
					// create the user

					MembershipCreateStatus status;

					var membershipUser = Membership.CreateUser(model.Username, model.Password, model.Email, model.Question, model.Answer, true, null, out status);

					if (status == MembershipCreateStatus.Success)
					{
						if (membershipUser == null)
						{
							throw new InvalidOperationException("Failed to create a new user.");
						}

						FormsAuthentication.SetAuthCookie(membershipUser.UserName, false);

						var returnUrl = GetProfileUrl();

						return Redirect(returnUrl);
					}

					var errorMessage = GetErrorMessage(status, ErrorCodeToString(status));

					ModelState.AddModelError(string.Empty, errorMessage);
				}
			}

			return View("SignUp", GetSignUpViewModel(model, null, null));
		}

		// POST: /Sign-Up/SignUpByEmail
		[HttpPost]
		[RegistrationActionFilter]
		[ValidateAntiForgeryToken]
		public ActionResult SignUpByEmail(EmailConfirmationViewModel model)
		{
			if (ModelState.IsValid)
			{
				if (string.IsNullOrWhiteSpace(model.Email))
				{
					ModelState.AddModelError("Email", "Email is a required field.");
				}
				else if (!ValidateEmail(model.Email))
				{
					ModelState.AddModelError("Email", "Email is invalid.");
				}
				else if (ViewBag.RequiresUniqueEmail)
				{
					try
					{
						var membershipUser = Membership.GetUserNameByEmail(model.Email);

						if (membershipUser != null)
						{
							ModelState.AddModelError("Email", "Email is invalid.");
						}
					}
					catch (ProviderException)
					{
						ModelState.AddModelError("Email", "Email is invalid.");
					}
				}

				if (ModelState.IsValid)
				{
					var portalContext = CreatePortalContext();
					var workflowName = portalContext.ServiceContext.GetSiteSettingValueByName(portalContext.Website, "Account/EmailConfirmation/WorkflowName") ?? _defaultEmailConfirmationWorkflowName;

					using (var context = CreateServiceContext())
					{
						var contact = context.CreateContactWithEmailConfirmation(model.Email, ViewBag.InvitationCodeDuration as TimeSpan?, workflowName);

						if (contact != null)
						{
							var routeValues = ViewBag.ReturnUrlRouteValues as RouteValueDictionary;
							routeValues.Add(ViewBag.ResultCodeKey, "confirm");

							return Redirect(Url.Action("Redeem", "Account", routeValues));
						}
					}

					var errorMessage = GetErrorMessage("Unknown", _errorUnknown);

					ModelState.AddModelError(string.Empty, errorMessage);
				}
			}

			return View("SignUp", GetSignUpViewModel(null, null, model));
		}

		private object GetSignInViewModel(SignInLocalViewModel local, SignInFederatedViewModel federated)
		{
			ViewData["local"] = local ?? new SignInLocalViewModel();
			ViewData["federated"] = federated ?? AddIdentityProviders(new SignInFederatedViewModel());

			return ViewData;
		}

		private object GetSignUpViewModel(SignUpLocalViewModel local, SignInFederatedViewModel federated, EmailConfirmationViewModel confirmation)
		{
			if (ViewBag.RequiresConfirmation)
			{
				ViewData["confirmation"] = confirmation ?? new EmailConfirmationViewModel();
			}
			else if (ViewBag.OpenRegistrationEnabled)
			{
				ViewData["local"] = local ?? new SignUpLocalViewModel();
				ViewData["federated"] = federated ?? AddIdentityProviders(new SignInFederatedViewModel());
			}

			return ViewData;
		}

		private void AddModelErrorForInvitationCode()
		{
			ModelState.AddModelError("RedeemInvitationCode", "Invalid invitation code.");
		}

		private void AddModelErrorForChallengeAnswer()
		{
			ModelState.AddModelError("ChallengeAnswer", "Invalid answer.");
		}

		private string GetProfileUrl()
		{
			var returnUrl = Encoder.UrlEncode(GetReturnUrl());
			var profilePath = ViewBag.ProfilePath as string;
			var profileUrl = "{0}{1}{2}={3}".FormatWith(profilePath, profilePath.Contains("?") ? "&" : "?", ViewBag.ReturnUrlKey as string, returnUrl);

			return profileUrl;
		}

		private string GetReturnUrl()
		{
			var returnUrl = Request[ViewBag.ReturnUrlKey];

			if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
			{
				return returnUrl;
			}

			return GetRootUrl();
		}

		private string GetRootUrl()
		{
			var portalContext = CreatePortalContext();
			return WebsitePathUtility.ToAbsolute(portalContext.Website, VirtualPathUtility.ToAbsolute(ViewBag.DefaultReturnPath));
		}

		private string GetErrorMessage(object key, string defaultMessage)
		{
			var portalContext = CreatePortalContext();
			var errorMessage = portalContext.ServiceContext.GetSnippetValueByName(portalContext.Website, "Account/ErrorMessage/{0}".FormatWith(key));

			return !string.IsNullOrWhiteSpace(errorMessage) ? errorMessage : defaultMessage;
		}

		private IEnumerable<IdentityProviderViewModel> GetIdentityProviders(string invitation = null, string answer = null)
		{
			var returnUrl = GetReturnUrl();
			var signInContext = CreateSignInContext(returnUrl, invitation, answer);

			if (ViewBag.OpenAuthEnabled)
			{
				var portalContext = CreatePortalContext();
				var adapter = new OpenAuthDataAdapter(new PortalContextDataAdapterDependencies(portalContext));

				var openIdProviders = adapter.SelectOpenIdProviders().Select(p => new IdentityProviderViewModel
			{
					Name = p.DisplayName,
					SignInUrl = Url.Action("SignInOpenAuth", new { provider = p.IdentityProvider }).AppendQueryString(signInContext),
				});

				var oauth2Providers = adapter.SelectOAuth2Providers().Where(p => !p.IsHidden).Select(p => new IdentityProviderViewModel
				{
					Name = p.DisplayName,
					SignInUrl = Url.Action("SignInOpenAuth", new { provider = p.IdentityProvider }).AppendQueryString(signInContext),
				});

				var oauthProviders = adapter.SelectOAuthProviders().Where(p => !p.IsHidden).Select(p => new IdentityProviderViewModel
		{
					Name = p.DisplayName,
					SignInUrl = Url.Action("SignInOpenAuth", new { provider = p.IdentityProvider }).AppendQueryString(signInContext),
				});

				foreach (var provider in openIdProviders.Concat(oauth2Providers).Concat(oauthProviders))
				{
					yield return provider;
		}
			}

			if (ViewBag.IdentityModelEnabled)
		{
				var fam = new AdxstudioFederationAuthenticationModule(HttpContext);
				var issuer = new Uri(fam.Issuer);

				if (!IsAcsIssuer(issuer))
				{
					var signInUrl = fam.GetSignInRequestUrl(signInContext, null, null);
					yield return new IdentityProviderViewModel { Name = fam.Name ?? "Sign In", SignInUrl = signInUrl };
		}
			}
		}

		private string GetHomeRealmDiscoveryMetadataFeedUrl(string invitation = null, string answer = null)
		{
			if (!ViewBag.IdentityModelEnabled) return null;

			var fam = new AdxstudioFederationAuthenticationModule(HttpContext);
			var issuer = new Uri(fam.Issuer);

			if (IsAcsIssuer(issuer))
			{
				var returnUrl = GetReturnUrl();
				var signInContext = CreateSignInContext(returnUrl, invitation, answer);
				var url = fam.GetHomeRealmDiscoveryMetadataFeedUrl(context: signInContext, callback: "ShowSignInPage");

				return url;
			}

			return null;
		}

		private static bool IsAcsIssuer(Uri issuer)
		{
			return issuer.Host.EndsWith(".accesscontrol.windows.net");
		}

		private async Task<ActionResult> SignInFederatedAsync(string identityProvider, string invitation = null, string answer = null)
		{
			if (!ViewBag.OpenAuthEnabled || string.IsNullOrWhiteSpace(identityProvider)) return new HttpNotFoundResult();

			var returnUrl = GetReturnUrl();
			var url = new UrlHelper(ControllerContext.RequestContext);
			var redirectPath = url.Action("SignInOpenAuth", "Account", new { provider = identityProvider });
			var signInContext = CreateSignInContext(returnUrl, invitation, answer);
			var portalContext = CreatePortalContext();
			var adapter = new OpenAuthDataAdapter(new PortalContextDataAdapterDependencies(portalContext));

			var openId = adapter.SelectOpenIdProviders().FirstOrDefault(p => p.IdentityProvider == identityProvider);

			if (openId != null)
			{
				var callback = new Uri(Request.Url, redirectPath.AppendQueryString(signInContext));
				var request = await openId.Client.CreateRequestAsync(Request, callback, Response.ClientDisconnectedToken);
				var response = await request.GetRedirectingResponseAsync(Response.ClientDisconnectedToken);
				await response.SendAsync(HttpContext);

				return new HttpStatusCodeResult(response.StatusCode);
		}

			var oauth2 = adapter.SelectOAuth2Providers().FirstOrDefault(p => p.IdentityProvider == identityProvider);

			if (oauth2 != null)
			{
				var openIdRealm = GetOpenIdRealm(oauth2);
				var request = await oauth2.Client.PrepareRequestUserAuthorizationAsync(oauth2.Scope, new Uri(Request.Url, redirectPath), openIdRealm, signInContext, Response.ClientDisconnectedToken);
				await request.SendAsync(HttpContext, Response.ClientDisconnectedToken);

				return new HttpStatusCodeResult(request.StatusCode);
					}

			var oauth1 = adapter.SelectOAuthProviders().FirstOrDefault(p => p.IdentityProvider == identityProvider);

			if (oauth1 != null)
					{
				var callback = new Uri(Request.Url, redirectPath.AppendQueryString(signInContext));
				var uri = await oauth1.Client.RequestUserAuthorizationAsync(callback, null, Response.ClientDisconnectedToken);
				return Redirect(uri.AbsoluteUri);
					}

			return null;
		}

		private IDictionary<string, string> CreateSignInContext(string returnUrl, string invitation, string answer)
		{
					var signInContext = new Dictionary<string, string>();

					if (!string.IsNullOrWhiteSpace(returnUrl))
					{
						signInContext.Add(ViewBag.ReturnUrlKey, returnUrl);
					}

					if (!string.IsNullOrWhiteSpace(invitation))
					{
						signInContext.Add(ViewBag.InvitationCodeKey, invitation);
					}

					if (!string.IsNullOrWhiteSpace(answer))
					{
						signInContext.Add(ViewBag.ChallengeAnswerKey, answer);
					}

			return signInContext;
		}

		private void ValidateLocalForm(SignUpLocalViewModel model)
		{
			if (string.IsNullOrWhiteSpace(model.Username))
			{
				ModelState.AddModelError("Username", "Username is a required field.");
			}
			else
			{
				try
				{
					var membershipUser = Membership.GetUser(model.Username, false);

					if (membershipUser != null)
					{
						ModelState.AddModelError("Username", "Username is invalid.");
					}
				}
				catch (ProviderException)
				{
					ModelState.AddModelError("Username", "Username is invalid.");
				}
			}

			if (string.IsNullOrWhiteSpace(model.Password))
			{
				ModelState.AddModelError("Password", "Password is a required field.");
			}
			else
			{
				if (model.Password.Length < Membership.MinRequiredPasswordLength)
				{
					ModelState.AddModelError("Password", "The password requires a minimum of {0} characters.".FormatWith(Membership.MinRequiredPasswordLength));
				}

				var regex = new Regex(@"\W");
				var matches = regex.Matches(model.Password);

				if (matches.Count < Membership.MinRequiredNonAlphanumericCharacters)
				{
					ModelState.AddModelError("Password", "The password requires a minimum of {0} non-alphanumeric characters.".FormatWith(Membership.MinRequiredNonAlphanumericCharacters));
				}

				if (!string.IsNullOrEmpty(Membership.PasswordStrengthRegularExpression) && !Regex.IsMatch(model.Password, Membership.PasswordStrengthRegularExpression))
				{
					ModelState.AddModelError("Password", "The password does not meet the requirements.");
				}
			}

			if (string.IsNullOrWhiteSpace(model.ConfirmPassword))
			{
				ModelState.AddModelError("ConfirmPassword", "Confirm Password is a required field.");
			}

			if (!Equals(model.Password, model.ConfirmPassword))
			{
				ModelState.AddModelError("Password", "Password and Confirm Password fields need to match.");
			}

			if (ViewBag.RequiresQuestionAndAnswer)
			{
				if (string.IsNullOrWhiteSpace(model.Question))
				{
					ModelState.AddModelError("Question", "Question is a required field.");
				}

				if (string.IsNullOrWhiteSpace(model.Answer))
				{
					ModelState.AddModelError("Answer", "Answer is a required field.");
				}
			}

			if (ViewBag.RequiresUniqueEmail && !ViewBag.RequiresConfirmation)
			{
				if (string.IsNullOrWhiteSpace(model.Email))
				{
					ModelState.AddModelError("Email", "Email is a required field.");
				}
				else if (!ValidateEmail(model.Email))
				{
					ModelState.AddModelError("Email", "Email is invalid.");
				}
				else
				{
					try
					{
						var membershipUser = Membership.GetUserNameByEmail(model.Email);

						if (membershipUser != null)
						{
							ModelState.AddModelError("Email", "Email is invalid.");
						}
					}
					catch (ProviderException)
					{
						ModelState.AddModelError("Email", "Email is invalid.");
					}
				}
			}
			else if (!string.IsNullOrWhiteSpace(model.Email) && !ValidateEmail(model.Email))
			{
				ModelState.AddModelError("Email", "Email is invalid.");
			}
		}

		private static string ErrorCodeToString(MembershipCreateStatus createStatus)
		{
			// See http://go.microsoft.com/fwlink/?LinkID=177550 for
			// a full list of status codes.
			switch (createStatus)
			{
				case MembershipCreateStatus.DuplicateUserName:
					return "User name already exists. Please enter a different user name.";

				case MembershipCreateStatus.DuplicateEmail:
					return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

				case MembershipCreateStatus.InvalidPassword:
					return "The password provided is invalid. Please enter a valid password value.";

				case MembershipCreateStatus.InvalidEmail:
					return "The e-mail address provided is invalid. Please check the value and try again.";

				case MembershipCreateStatus.InvalidAnswer:
					return "The password retrieval answer provided is invalid. Please check the value and try again.";

				case MembershipCreateStatus.InvalidQuestion:
					return "The password retrieval question provided is invalid. Please check the value and try again.";

				case MembershipCreateStatus.InvalidUserName:
					return "The user name provided is invalid. Please check the value and try again.";

				case MembershipCreateStatus.ProviderError:
					return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

				case MembershipCreateStatus.UserRejected:
					return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

				default:
					return _errorUnknown;
			}
		}

		private static bool ValidateEmail(string email)
		{
			try
			{
				new MailAddress(email);
				return true;
			}
			catch
			{
				return false;
			}
		}

		private OrganizationServiceContext CreateServiceContext()
		{
			return PortalCrmConfigurationManager.CreateServiceContext(ViewBag.PortalName);
		}

		private IPortalContext CreatePortalContext()
		{
			return PortalCrmConfigurationManager.CreatePortalContext(ViewBag.PortalName, Request.RequestContext);
		}

		private string GetPathBySiteMarker(string siteMarkerName)
		{
			var portal = CreatePortalContext();

			var page = portal.ServiceContext.GetPageBySiteMarkerName(portal.Website, siteMarkerName);

			return page != null ? portal.ServiceContext.GetUrl(page) : null;
		}

		private string ConvertToEmail(MembershipUser user)
		{
			// add custom handling here to convert a MembershipUser to a valid email

			if (user == null)
			{
				return null;
			}

			var adUser = user as ActiveDirectoryMembershipUser;

			if (adUser != null)
			{
				Entity contact;

				if (TryConvertToContact(adUser, out contact))
				{
					var email = contact.GetAttributeValue<string>("emailaddress1");

					if (!string.IsNullOrWhiteSpace(email))
					{
						return email;
					}
				}
			}
			else
			{
				return user.Email;
			}

			return null;
		}

		private bool TryConvertToContactId(MembershipUser user, out Guid contactId)
		{
			// add custom handling here to convert a MembershipUser to a valid CRM contact ID

			var adUser = user as ActiveDirectoryMembershipUser;

			if (adUser != null)
			{
				Entity contact;

				if (TryConvertToContact(adUser, out contact))
				{
					contactId = contact.Id;
					return true;
				}

				contactId = default(Guid);
				return false;
			}

			if (user.ProviderUserKey is Guid)
			{
				contactId = (Guid)user.ProviderUserKey;
				return true;
			}

			contactId = default(Guid);
			return false;
		}

		private bool TryConvertToContact(ActiveDirectoryMembershipUser user, out Entity contact)
		{
			using (var context = CreateServiceContext())
			{
				var memberEntityName = (string)ViewBag.MemberEntityName;
				var attributeMapUsername = (string)ViewBag.AttributeMapUsername;
				var attributeMapIdentityProvider = (string)ViewBag.AttributeMapIdentityProvider;

				var findContact = context
					.CreateQuery(memberEntityName)
					.Where(
						c => c.GetAttributeValue<OptionSetValue>("statecode") != null && c.GetAttributeValue<OptionSetValue>("statecode").Value == 0
						&& c.GetAttributeValue<string>(attributeMapIdentityProvider) == null
						&& c.GetAttributeValue<string>(attributeMapUsername) == user.UserName);

				contact = findContact.SingleOrDefault();

				if (contact != null)
				{
					return true;
				}
			}

			return false;
		}

		private static void SendPasswordRecoveryEmail(MembershipUser user, string password)
		{
			if (user == null) throw new ArgumentNullException("user");
			if (string.IsNullOrWhiteSpace(password)) throw new ArgumentNullException("password");

			// for custom MembershipProvider implementations where a contact ID does not exist, the password recovery email cannot be sent by workflow

			throw new NotImplementedException("Send a password recovery email.");
		}

		private Uri GetOpenIdRealm(IOAuthProvider<GraphWebServerClient> oauth2)
		{
			if (!string.IsNullOrWhiteSpace(oauth2.OpenIdRealm)) return new Uri(oauth2.OpenIdRealm);

			// auto-detect the realm

			var registrationSettings = FederationCrmConfigurationManager.GetUserRegistrationSettings();
			var attribute = new OpenAuthAuthorizeAttribute(registrationSettings);
			return attribute.GetOpenIdRealm(ControllerContext, oauth2);
		}

		private static void PostFederatedSignIn(ActionResult authorizeResult)
		{
			if (!(authorizeResult is FederationReturnResult))
			{
				return;
			}

			var federatedResult = (FederationReturnResult) authorizeResult;

			if (federatedResult.User == null)
			{
				return;
			}

			PostSignIn(federatedResult.User.Id);
		}

		private static void PostSignIn(Guid contactId)
		{
			// If Parature is enabled ensure a customer record exists otherwise create it.

			var paratureContext = new ParatureContext();

			if (paratureContext.IsEnabled && paratureContext.IsTicketingSettingsValid)
			{
				paratureContext.SyncContactCustomer(contactId);
			}
		}
	}
}