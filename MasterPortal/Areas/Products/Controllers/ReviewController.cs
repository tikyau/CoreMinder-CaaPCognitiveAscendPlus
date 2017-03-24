﻿using System;
using System.Linq;
using System.Web.Mvc;
using Adxstudio.Xrm.Products;
using Microsoft.Xrm.Client.Services;
using Microsoft.Xrm.Portal.Configuration;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;

namespace Site.Areas.Products.Controllers
{
	public class ReviewController : Controller
	{
		[HttpPost, ValidateInput(false)]
		public ActionResult ReportAbuse(string reviewid, string remarks)
		{
			Guid reviewID;
			if (string.IsNullOrEmpty(reviewid) || !Guid.TryParse(reviewid, out reviewID))
			{
				throw new ArgumentException("Please provide a valid review id.");
			}
			var portal = PortalCrmConfigurationManager.CreatePortalContext();
			var context = new OrganizationServiceContext(new OrganizationService("Xrm"));
			var review = GetReview(reviewID, context);
			var reviewDataAdapter = new ReviewDataAdapter(review, new PortalContextDataAdapterDependencies(portal, null, Request.RequestContext));
			reviewDataAdapter.ReportAbuse(remarks);

			return Json(new { success = true });
		}

		private static Entity GetReview(Guid reviewId, OrganizationServiceContext context)
		{
			var product = context.CreateQuery("adx_review").FirstOrDefault(p => p.GetAttributeValue<Guid>("adx_reviewid") == reviewId);

			return product;
		}
	}
}
