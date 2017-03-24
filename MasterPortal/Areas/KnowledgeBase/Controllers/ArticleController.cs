﻿using System.Linq;
using System.Web.Mvc;
using Adxstudio.Xrm.Web.Mvc;
using Microsoft.Xrm.Client;
using Microsoft.Xrm.Client.Security;
using Microsoft.Xrm.Portal.Configuration;
using Microsoft.Xrm.Portal.Web.Providers;
using Microsoft.Xrm.Sdk;
using Site.Areas.KnowledgeBase.ViewModels;

namespace Site.Areas.KnowledgeBase.Controllers
{
	[PortalView, PortalSecurity]
	public class ArticleController : Controller
	{
		public ActionResult Index(string number)
		{
			var serviceContext = PortalCrmConfigurationManager.CreateServiceContext();

			var kbarticle = serviceContext.CreateQuery("kbarticle")
				.FirstOrDefault(e => e.GetAttributeValue<string>("number") == number);

			if (kbarticle == null)
			{
				return HttpNotFound(@"Knowledge base article ""{0}"" not found.".FormatWith(number));
			}

			var securityProvider = PortalCrmConfigurationManager.CreateCrmEntitySecurityProvider();

			if (!securityProvider.TryAssert(serviceContext, kbarticle, CrmEntityRight.Read))
			{
				return HttpNotFound(@"Knowledge base article ""{0}"" not found.".FormatWith(number));
			}

			var urlProvider = PortalCrmConfigurationManager.CreateDependencyProvider().GetDependency<IEntityUrlProvider>();

			var adx_kbarticle_kbarticle = kbarticle.GetRelatedEntities(serviceContext, new Relationship("adx_kbarticle_kbarticle")
			{
				PrimaryEntityRole = EntityRole.Referenced
			});

			var relatedArticles = adx_kbarticle_kbarticle
				.Where(e => securityProvider.TryAssert(serviceContext, e, CrmEntityRight.Read))
				.Select(e => new
				{
					Title = e.GetAttributeValue<string>("title"),
					Url = urlProvider.GetUrl(serviceContext, e)
				})
				.Where(e => !(string.IsNullOrEmpty(e.Title) || string.IsNullOrEmpty(e.Url)))
				.Select(e => new RelatedArticle(e.Title, e.Url))
				.OrderBy(e => e.Title);

			return View(new ArticleViewModel(kbarticle, relatedArticles));
		}
	}
}