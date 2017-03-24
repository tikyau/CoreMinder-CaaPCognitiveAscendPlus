using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Adxstudio.Xrm.AspNet.Mvc;
using Adxstudio.Xrm.Blogs;
using Adxstudio.Xrm.Cases;
using Adxstudio.Xrm.Cms;
using Adxstudio.Xrm.Configuration;
using Adxstudio.Xrm.Forums;
using Adxstudio.Xrm.Ideas;
using Adxstudio.Xrm.IdentityModel.Web.Modules;
using Adxstudio.Xrm.Issues;
using Adxstudio.Xrm.OpenAuth.Configuration;
using Adxstudio.Xrm.Web;
using Adxstudio.Xrm.Web.Mvc.Html;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Xrm.Portal.IdentityModel.Configuration;
using Microsoft.Xrm.Portal.IdentityModel.Web.Modules;
using Microsoft.Xrm.Sdk;
using Site.Areas.Account.Models;

namespace Site.Helpers
{
	public static class UrlHelpers
	{
		public static string ActionWithQueryString(this UrlHelper url, string actionName, object routeValues)
		{
			var routeDictionary = new RouteValueDictionary(routeValues);

			var queryString = url.RequestContext.HttpContext.Request.QueryString;

			foreach (var key in queryString.Cast<string>().Where(key => !routeDictionary.ContainsKey(key) && !string.IsNullOrWhiteSpace(queryString[key])))
			{
				routeDictionary[key] = queryString[key];
			}

			return url.Action(actionName, routeDictionary);
		}

		public static bool RegistrationEnabled(this UrlHelper url)
		{
			if (!OwinEnabled(url)) return false;
			var settings = GetAuthenticationSettings(url);
			return settings.RegistrationEnabled && settings.OpenRegistrationEnabled;
		}

		[Obsolete("Invoke Url.SignInUrl() instead.")]
		public static string SignInUrl(this HtmlHelper html, string siteMarkerName = "Login")
		{
			var returnUrlKey = FederationCrmConfigurationManager.GetUserRegistrationSettings().ReturnUrlKey ?? "returnurl";
			var returnUrl = html.ViewContext.RequestContext.HttpContext.Request[returnUrlKey] ?? html.ViewContext.RequestContext.HttpContext.Request.Url.PathAndQuery;
			var url = html.SiteMarkerUrl(siteMarkerName, new NameValueCollection { { returnUrlKey, returnUrl } });

			return url;
		}

		public static string SignInUrl(this UrlHelper url, string returnUrl = null)
		{
			return OwinEnabled(url)
				? SignInUrlOwin(url, returnUrl)
				: SignInUrlMembership(url, returnUrl);
		}

		private static string SignInUrlOwin(this UrlHelper url, string returnUrl = null)
		{
			var settings = GetAuthenticationSettings(url);

			return !string.IsNullOrWhiteSpace(settings.LoginButtonAuthenticationType)
				? url.Action("ExternalLogin", "Login", new { area = "Account", returnUrl = GetReturnUrl(url.RequestContext.HttpContext.Request, returnUrl), provider = settings.LoginButtonAuthenticationType })
				: LocalSignInUrl(url, returnUrl);
		}

		private static string SignInUrlMembership(this UrlHelper url, string returnUrl = null)
		{
			if (!AdxstudioCrmConfigurationManager.GetCrmSection().MembershipProviderEnabled
				&& !AdxstudioCrmConfigurationManager.GetCrmSection().IdentityModelEnabled
				&& !OpenAuthConfigurationManager.GetSection().Enabled)
			{
				return null;
			}

			var authenticationType = AdxstudioCrmConfigurationManager.GetCrmSection().LoginButtonAuthenticationType;

			if (_identityModelAuthenticationTypes.Contains(authenticationType, StringComparer.OrdinalIgnoreCase))
			{
				return FederatedSignInUrl(url, returnUrl);
			}

			if (!string.IsNullOrWhiteSpace(authenticationType))
			{
				return OpenAuthSignInUrl(url, authenticationType, returnUrl);
			}

			return LocalSignInUrl(url, returnUrl);
		}

		public static string LocalSignInUrl(this UrlHelper url, string returnUrl = null)
		{
			return OwinEnabled(url)
				? GetAccountUrl(url, "Login", "Login", returnUrl)
				: GetAccountUrl(url, "SignIn", "Account", returnUrl);
		}

		public static string FederatedSignInUrl(this UrlHelper url, string returnUrl = null)
		{
			if (!AdxstudioCrmConfigurationManager.GetCrmSection().IdentityModelEnabled) return LocalSignInUrl(url, returnUrl);

			var fam = new AdxstudioFederationAuthenticationModule(url.RequestContext.HttpContext);
			var signInUrl = fam.GetSignInRequestUrl(GetReturnUrl(url, returnUrl));

			return signInUrl;
		}

		public static string OpenAuthSignInUrl(this UrlHelper url, string authenticationType, string returnUrl)
		{
			if (!OpenAuthConfigurationManager.GetSection().Enabled) return LocalSignInUrl(url, returnUrl);

			return url.Action("SignInOpenAuth", "Account", new { provider = authenticationType, returnUrl = GetReturnUrl(url, returnUrl), area = "Account" });
		}

		public static string FacebookSignInUrl(this UrlHelper url)
		{
			if (!OpenAuthConfigurationManager.GetSection().Enabled) return LocalSignInUrl(url);

			return OwinEnabled(url)
				? url.Action("FacebookExternalLogin", "Login", new { area = "Account" })
				: url.Action("SignInOpenAuth", "Account", new { provider = "facebook", returnUrl = url.Action("SignInReloadParent", "Account", new { area = "Account" }), area = "Account" });
		}

		public static string SignOutUrl(this UrlHelper url, string returnUrl = null)
		{
			return OwinEnabled(url)
				? GetAccountUrl(url, "LogOff", "Login", returnUrl)
				: GetAccountUrl(url, "SignOut", "Account", returnUrl);
		}

		public static string RegisterUrl(this UrlHelper url, string returnUrl = null)
		{
			return OwinEnabled(url)
				? GetAccountUrl(url, "Register", "Login", returnUrl)
				: GetAccountUrl(url, "SignUp", "Account", returnUrl);
		}

		public static string RedeemUrl(this UrlHelper url, string returnUrl = null)
		{
			return OwinEnabled(url)
				? GetAccountUrl(url, "RedeemInvitation", "Login", returnUrl)
				: GetAccountUrl(url, "Redeem", "Account", returnUrl);
		}

		private static string GetAccountUrl(UrlHelper url, string actionName, string controllerName, string returnUrl)
		{
			return url.Action(actionName, controllerName, new { area = "Account", returnUrl = GetReturnUrl(url.RequestContext.HttpContext.Request, returnUrl) });
		}

		private static string GetReturnUrl(UrlHelper url, string returnUrl)
		{
			return GetReturnUrl(url.RequestContext.HttpContext.Request, returnUrl);
		}

		private static string GetReturnUrl(HttpRequestBase request, string returnUrl)
		{
			return request["ReturnUrl"] ?? returnUrl ?? request.RawUrl;
		}

		private static bool OwinEnabled(this UrlHelper url)
		{
			var manager = url.RequestContext.HttpContext.GetOwinContext().GetUserManager<ApplicationWebsiteManager>();
			return manager != null;
		}

		private static AuthenticationSettings GetAuthenticationSettings(this UrlHelper url)
		{
			var manager = url.RequestContext.HttpContext.GetOwinContext().GetUserManager<ApplicationWebsiteManager>();
			var website = manager.Find(url.RequestContext);
			return website.GetAuthenticationSettings<ApplicationWebsite, string>();
		}

		private static readonly string[] _identityModelAuthenticationTypes = { "Azure", "ACS", "ADFS" };

		private const string _defaultAuthorUrl = null;

		public static string AuthorUrl(this UrlHelper urlHelper, IBlogAuthor author)
		{
			try
			{
				return author == null
						   ? _defaultAuthorUrl
						   : urlHelper.RouteUrl("PublicProfileBlogPosts", new { contactId = author.Id });
			}
			catch
			{
				return _defaultAuthorUrl;
			}
		}

		public static string AuthorUrl(this UrlHelper urlHelper, ICase @case)
		{
			if (@case == null || @case.ResponsibleContact == null)
			{
				return _defaultAuthorUrl;
			}

			try
			{
				return urlHelper.RouteUrl("PublicProfileForumPosts", new { contactId = @case.ResponsibleContact.Id });
			}
			catch
			{
				return _defaultAuthorUrl;
			}
		}

		public static string AuthorUrl(this UrlHelper urlHelper, IComment comment)
		{
			if (comment == null || comment.Author == null)
			{
				return _defaultAuthorUrl;
			}

			try
			{
				return comment.Author.EntityReference == null
						   ? comment.Author.WebsiteUrl
					: urlHelper.RouteUrl("PublicProfileForumPosts", new { contactId = comment.Author.EntityReference.Id });
			}
			catch
			{
				return _defaultAuthorUrl;
			}
		}

		public static string AuthorUrl(this UrlHelper urlHelper, IIdea idea)
		{
			try
			{
				return idea == null || idea.AuthorId == null
					? _defaultAuthorUrl
					: urlHelper.RouteUrl("PublicProfileIdeas", new { contactId = idea.AuthorId.Value });
			}
			catch
			{
				return _defaultAuthorUrl;
			}
		}

		public static string AuthorUrl(this UrlHelper urlHelper, IIssue issue)
		{
			try
			{
				return issue == null || issue.AuthorId == null
				 ? _defaultAuthorUrl
				 : urlHelper.RouteUrl("PublicProfileForumPosts", new { contactId = issue.AuthorId.Value });
			}
			catch
			{
				return _defaultAuthorUrl;
			}
		}

		public static string AuthorUrl(this UrlHelper urlHelper, IForumAuthor author)
		{
			try
			{
				return author == null || author.EntityReference == null
						   ? _defaultAuthorUrl
						   : urlHelper.RouteUrl("PublicProfileForumPosts", new { contactId = author.EntityReference.Id });
			}
			catch
			{
				return _defaultAuthorUrl;
			}
		}

		public static string UserImageUrl(this UrlHelper urlHelper, IForumAuthor author, int? size = null)
		{
			return author == null ? null : urlHelper.UserImageUrl(author.EmailAddress, size);
		}

		public static string UserImageUrl(this UrlHelper urlHelper, ICase @case, int? size = null)
		{
			return @case == null || string.IsNullOrEmpty(@case.ResponsibleContactEmailAddress)
				? null
				: urlHelper.UserImageUrl(@case.ResponsibleContactEmailAddress, size);
		}

		public static string UserImageUrl(this UrlHelper urlHelper, Entity contact, int? size = null)
		{
			return contact == null ? null : urlHelper.UserImageUrl(contact.GetAttributeValue<string>("emailaddress1"), size);
		}

		public static string UserImageUrl(this UrlHelper urlHelper, object email, int? size = null)
		{
			return email == null ? null : urlHelper.UserImageUrl(email.ToString(), size);
		}

		public static string UserImageUrl(this UrlHelper urlHelper, string email, int? size = null)
		{
			var sizeValue = size.HasValue ? size.Value : GetGravatarDefaultSize(urlHelper);

			// Remove the leading "http:" from the Gravatar URL, so that the request will automatically use HTTPS in that context.
			return Gravatar.Url(email, size: sizeValue).Remove(0, 5);
		}

		private static int GetGravatarDefaultSize(UrlHelper urlHelper)
		{
			var settingDataAdapter = new SettingDataAdapter(new Adxstudio.Xrm.Cms.PortalConfigurationDataAdapterDependencies(requestContext: urlHelper.RequestContext));

			return settingDataAdapter.GetIntegerValue("Gravatar/Size").GetValueOrDefault(40);
		}
	}
}