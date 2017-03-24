﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Adxstudio.Xrm.Cms;
using Adxstudio.Xrm.SharePoint;
using Adxstudio.Xrm.Web.Mvc;
using Microsoft.Xrm.Portal.Configuration;
using Microsoft.Xrm.Sdk;

namespace Site.Areas.Portal.Controllers
{
	public class SharePointGridController : Controller
	{
		private const int DefaultPageSize = 10;

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult GetSharePointData(EntityReference regarding, string sortExpression, int page, int pageSize = DefaultPageSize, string pagingInfo = null, string folderPath = null)
		{
			string portalName = null;
			var portalContext = PortalCrmConfigurationManager.CreatePortalContext();
			var languageCodeSetting = portalContext.ServiceContext.GetSiteSettingValueByName(portalContext.Website, "Language Code");

			if (!string.IsNullOrWhiteSpace(languageCodeSetting))
			{
				int languageCode;
				if (int.TryParse(languageCodeSetting, out languageCode))
				{
					portalName = languageCode.ToString(CultureInfo.InvariantCulture);
				}
			}

			var dataAdapterDependencies = new PortalConfigurationDataAdapterDependencies(requestContext: Request.RequestContext, portalName: portalName);
			var dataAdapter = new SharePointDataAdapter(dataAdapterDependencies);
			var data = dataAdapter.GetFoldersAndFiles(regarding, sortExpression, page, pageSize, pagingInfo, folderPath);

			var json = Json(new
			{
				data.SharePointItems,
				PageSize = pageSize,
				PageNumber = page,
				data.PagingInfo,
				data.TotalCount,
				data.AccessDenied
			});
			return json;
		}

		[HttpPost]
		[JsonHandlerError]
		public ActionResult AddSharePointFiles(string regardingEntityLogicalName, string regardingEntityId, IList<HttpPostedFileBase> files, bool overwrite, string folderPath = null)
		{
			Guid regardingId;
			Guid.TryParse(regardingEntityId, out regardingId);
			var regarding = new EntityReference(regardingEntityLogicalName, regardingId);
			string portalName = null;
			var portalContext = PortalCrmConfigurationManager.CreatePortalContext();
			var languageCodeSetting = portalContext.ServiceContext.GetSiteSettingValueByName(portalContext.Website, "Language Code");

			if (!string.IsNullOrWhiteSpace(languageCodeSetting))
			{
				int languageCode;
				if (int.TryParse(languageCodeSetting, out languageCode))
				{
					portalName = languageCode.ToString(CultureInfo.InvariantCulture);
				}
			}

			var dataAdapterDependencies = new PortalConfigurationDataAdapterDependencies(requestContext: Request.RequestContext, portalName: portalName);
			var dataAdapter = new SharePointDataAdapter(dataAdapterDependencies);
			var result = dataAdapter.AddFiles(regarding, files, overwrite, folderPath);

			if (!result.PermissionsExist)
			{
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Entity Permissions have not been defined. Your request could not be completed.");
			}

			if (!result.CanCreate)
			{
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Permission Denied. You do not have the appropriate Entity Permissions to create SharePoint files.");
			}

			if (!result.CanAppendTo)
			{
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Permission Denied. You do not have the appropriate Entity Permissions to append to record.");
			}

			if (!result.CanAppend)
			{
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Permission Denied. You do not have the appropriate Entity Permissions to append SharePoint files.");
			}

			return new HttpStatusCodeResult(HttpStatusCode.NoContent);
		}

		[HttpPost]
		[JsonHandlerError]
		public ActionResult AddSharePointFolder(EntityReference regarding, string name, string folderPath = null)
		{
			string portalName = null;
			var portalContext = PortalCrmConfigurationManager.CreatePortalContext();
			var languageCodeSetting = portalContext.ServiceContext.GetSiteSettingValueByName(portalContext.Website, "Language Code");

			if (!string.IsNullOrWhiteSpace(languageCodeSetting))
			{
				int languageCode;
				if (int.TryParse(languageCodeSetting, out languageCode))
				{
					portalName = languageCode.ToString(CultureInfo.InvariantCulture);
				}
			}

			var dataAdapterDependencies = new PortalConfigurationDataAdapterDependencies(requestContext: Request.RequestContext, portalName: portalName);
			var dataAdapter = new SharePointDataAdapter(dataAdapterDependencies);
			var result = dataAdapter.AddFolder(regarding, name, folderPath);

			if (!result.PermissionsExist)
			{
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Entity Permissions have not been defined. Your request could not be completed.");
			}

			if (!result.CanCreate)
			{
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Permission Denied. You do not have the appropriate Entity Permissions to create SharePoint files.");
			}

			if (!result.CanAppendTo)
			{
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Permission Denied. You do not have the appropriate Entity Permissions to append to record.");
			}

			if (!result.CanAppend)
			{
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Permission Denied. You do not have the appropriate Entity Permissions to append SharePoint files.");
			}

			return new HttpStatusCodeResult(HttpStatusCode.NoContent);
		}

		[HttpPost]
		[JsonHandlerError]
		public ActionResult DeleteSharePointItem(EntityReference regarding, string id)
		{
			int itemId;
			int.TryParse(id, out itemId);
			string portalName = null;
			var portalContext = PortalCrmConfigurationManager.CreatePortalContext();
			var languageCodeSetting = portalContext.ServiceContext.GetSiteSettingValueByName(portalContext.Website, "Language Code");

			if (!string.IsNullOrWhiteSpace(languageCodeSetting))
			{
				int languageCode;
				if (int.TryParse(languageCodeSetting, out languageCode))
				{
					portalName = languageCode.ToString(CultureInfo.InvariantCulture);
				}
			}

			var dataAdapterDependencies = new PortalConfigurationDataAdapterDependencies(requestContext: Request.RequestContext, portalName: portalName);
			var dataAdapter = new SharePointDataAdapter(dataAdapterDependencies);

			var result = dataAdapter.DeleteItem(regarding, itemId);

			if (!result.PermissionsExist)
			{
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Entity Permissions have not been defined. Your request could not be completed.");
			}

			if (!result.CanDelete)
			{
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Permission Denied. You do not have the appropriate Entity Permissions to delete this record.");
			}

			return new HttpStatusCodeResult(HttpStatusCode.NoContent);
		}
	}
}
