using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using System.Web.Security;
using Adxstudio.Xrm.Cms;
using Adxstudio.Xrm.Configuration;
using Adxstudio.Xrm.Json.JsonConverter;
using Adxstudio.Xrm.Metadata;
using Adxstudio.Xrm.Security;
using Adxstudio.Xrm.Web.Mvc;
using Adxstudio.Xrm.Web.UI;
using Adxstudio.Xrm.Web.UI.CrmEntityListView;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Client;
using Microsoft.Xrm.Client.Diagnostics;
using Microsoft.Xrm.Portal.Configuration;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Newtonsoft.Json;
using Site.Areas.Portal.ViewModels;

namespace Site.Areas.Portal.Controllers
{
	public class EntityGridController : Controller
	{
		private const int DefaultPageSize = 10;
		private const int DefaultMaxPageSize = 50;

		[HttpPost]
		[JsonHandlerError]
		public ActionResult GetSubgridData(string base64SecureConfiguration, string sortExpression, string search, int page,
			int pageSize = DefaultPageSize)
		{
			return GetData(ConvertSecureStringToViewConfiguration(base64SecureConfiguration), sortExpression, search, null, null, page, pageSize);
		}

		[HttpPost]
		[JsonHandlerError]
		public ActionResult GetGridData(string base64SecureConfiguration, string sortExpression, string search, string filter,
			string metaFilter, int page, int pageSize = DefaultPageSize)
		{
			return GetData(ConvertSecureStringToViewConfiguration(base64SecureConfiguration), sortExpression, search, filter, metaFilter, page, pageSize);
		}

		[HttpPost]
		[JsonHandlerError]
		public ActionResult GetLookupGridData(string base64SecureConfiguration, string sortExpression, string search, string filter,
			string metaFilter, int page, int pageSize = DefaultPageSize, bool applyRelatedRecordFilter = false,
			string filterRelationshipName = null, string filterEntityName = null, string filterAttributeName = null, string filterValue = null)
		{
			Guid? filterGuidValue = null;

			if (!string.IsNullOrWhiteSpace(filterValue))
			{
				Guid guidValue;
				if (Guid.TryParse(filterValue, out guidValue))
				{
					filterGuidValue = guidValue;
				}
			}

			return GetData(ConvertSecureStringToViewConfiguration(base64SecureConfiguration), sortExpression, search, filter, metaFilter, page, pageSize, true, applyRelatedRecordFilter,
				filterRelationshipName, filterEntityName, filterAttributeName, filterGuidValue);
		}

		private ViewConfiguration ConvertSecureStringToViewConfiguration(string base64SecureConfiguration)
		{
			var secureConfigurationByteArray = Convert.FromBase64String(base64SecureConfiguration);
			var unprotectedByteArray = MachineKey.Unprotect(secureConfigurationByteArray, "Secure View Configuration");
			if (unprotectedByteArray == null)
			{
				return null;
			}
			var configurationJson = Encoding.UTF8.GetString(unprotectedByteArray);
			var viewConfiguration = JsonConvert.DeserializeObject<ViewConfiguration>(configurationJson, new JsonSerializerSettings { Converters = new List<JsonConverter> { new UrlBuilderConverter() } });
			return viewConfiguration;
		}

		private ActionResult GetData(ViewConfiguration viewConfiguration, string sortExpression, string search, string filter,
			string metaFilter, int page, int pageSize = DefaultPageSize, bool applyRecordLevelFilters = true,
			bool applyRelatedRecordFilter = false, string filterRelationshipName = null, string filterEntityName = null,
			string filterAttributeName = null, Guid? filterValue = null, bool overrideMaxPageSize = false)
		{
			if (viewConfiguration == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Invalid Request.");
			}

			if (pageSize < 0)
			{
				pageSize = DefaultPageSize;
			}

			if (pageSize > DefaultMaxPageSize && !overrideMaxPageSize)
			{
				Tracing.FrameworkInformation(GetType().FullName, "GetData",
					"pageSize={0} is greater than the allowed maximum page size of {1}. Page size has been constrained to {1}.",
					pageSize, DefaultMaxPageSize);
				pageSize = DefaultMaxPageSize;
			}

			var dataAdapterDependencies = new PortalConfigurationDataAdapterDependencies(requestContext: Request.RequestContext, portalName: viewConfiguration.PortalName);
			
			var viewDataAdapter = applyRelatedRecordFilter &&
								  (!string.IsNullOrWhiteSpace(filterRelationshipName) &&
								   !string.IsNullOrWhiteSpace(filterEntityName))
				? new ViewDataAdapter(viewConfiguration, dataAdapterDependencies, filterRelationshipName, filterEntityName,
					filterAttributeName, filterValue ?? Guid.Empty, page, search, sortExpression, filter, metaFilter, applyRecordLevelFilters)
				: new ViewDataAdapter(viewConfiguration, dataAdapterDependencies, page, search, sortExpression, filter, metaFilter,
					applyRecordLevelFilters);

			var result = viewDataAdapter.FetchEntities();

			if (result.EntityPermissionDenied)
			{
				var permissionResult = new EntityPermissionResult(true);

				return Json(permissionResult);
			}

			IEnumerable<EntityRecord> records;

			if (viewConfiguration.EnableEntityPermissions && AdxstudioCrmConfigurationManager.GetCrmSection().ContentMap.Enabled)
			{
				var serviceContext = dataAdapterDependencies.GetServiceContext();
				var crmEntityPermissionProvider = new CrmEntityPermissionProvider();

				records = result.Records.Select(e => new EntityRecord(e, serviceContext, crmEntityPermissionProvider, viewDataAdapter.EntityMetadata, true));
			}
			else
			{
				records = result.Records.Select(e => new EntityRecord(e, viewDataAdapter.EntityMetadata));
			}

			var totalRecordCount = result.TotalRecordCount;

			var data = new PaginatedGridData(records, totalRecordCount, page, pageSize);

			var json = Json(data);
			json.MaxJsonLength = int.MaxValue;

			return json;
		}

		[HttpPost]
		[JsonHandlerError]
		public ActionResult Delete(EntityReference entityReference)
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
			var serviceContext = dataAdapterDependencies.GetServiceContext();
			var entityPermissionProvider = new CrmEntityPermissionProvider();

			if (!entityPermissionProvider.PermissionsExist)
			{
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Entity Permissions have not been defined. Your request could not be completed.");
			}

			var entityMetadata = serviceContext.GetEntityMetadata(entityReference.LogicalName, EntityFilters.All);
			var primaryKeyName = entityMetadata.PrimaryIdAttribute;
			var entity =
				serviceContext.CreateQuery(entityReference.LogicalName)
					.First(e => e.GetAttributeValue<Guid>(primaryKeyName) == entityReference.Id);
			var test = entityPermissionProvider.TryAssert(serviceContext, CrmEntityPermissionRight.Delete, entity);
				
			if (test)
			{
				serviceContext.DeleteObject(entity);
				serviceContext.SaveChanges();
			}
			else
			{
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Permission Denied. You do not have the appropriate Entity Permissions to delete this record.");
			}

			return new HttpStatusCodeResult(HttpStatusCode.NoContent);
		}

		[HttpPost]
		[JsonHandlerError]
		public ActionResult Associate(AssociateRequest request)
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
			var serviceContext = dataAdapterDependencies.GetServiceContext();
			var entityPermissionProvider = new CrmEntityPermissionProvider();

			if (!entityPermissionProvider.PermissionsExist)
			{
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Entity Permissions have not been defined. Your request could not be completed.");
			}

			var relatedEntities = request.RelatedEntities.Where(
				related => entityPermissionProvider.TryAssertAssociation(serviceContext, request.Target, request.Relationship, related)
			).ToList();

			if (relatedEntities.Any())
			{
				var filtered = new AssociateRequest { Target = request.Target, Relationship = request.Relationship, RelatedEntities = new EntityReferenceCollection(relatedEntities) };

				serviceContext.Execute(filtered);
			}
			else
			{
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Permission Denied. You do not have the appropriate Entity Permissions to associate these records.");
			}

			return new HttpStatusCodeResult(HttpStatusCode.NoContent);
		}

		[HttpPost]
		[JsonHandlerError]
		public ActionResult Disassociate(DisassociateRequest request)
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
			var serviceContext = dataAdapterDependencies.GetServiceContext();
			var entityPermissionProvider = new CrmEntityPermissionProvider();

			if (!entityPermissionProvider.PermissionsExist)
			{
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Entity Permissions have not been defined. Your request could not be completed.");
			}

			var relatedEntities =
				request.RelatedEntities.Where(
					related => entityPermissionProvider.TryAssertAssociation(serviceContext, request.Target, request.Relationship, related)).ToList();

			if (relatedEntities.Any())
			{
				var filtered = new DisassociateRequest { Target = request.Target, Relationship = request.Relationship, RelatedEntities = new EntityReferenceCollection(relatedEntities) };

				serviceContext.Execute(filtered);
			}
			else
			{
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Permission Denied. You do not have the appropriate Entity Permissions to disassociate the records.");
			}

			return new HttpStatusCodeResult(HttpStatusCode.NoContent);
		}

		[HttpPost]
		[JsonHandlerError]
		public ActionResult DownloadAsCsv(string viewName, IEnumerable<LayoutColumn> columns, string base64SecureConfiguration, string sortExpression, string search, string filter,
			string metaFilter, int page = 1, int pageSize = DefaultPageSize)
		{
			var viewConfiguration = ConvertSecureStringToViewConfiguration(base64SecureConfiguration);

			if (viewConfiguration == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Invalid Request.");
			}

			// override the page parameters to get up to 5000 records.
			page = 1;
			pageSize = 5000;
			viewConfiguration.PageSize = 5000;
			
			var json = GetData(viewConfiguration, sortExpression, search, filter, metaFilter, page, pageSize, true, false, null, null, null, null, true) as JsonResult;

			if (json == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.NoContent);
			}

			if (json.Data is EntityPermissionResult)
			{
				return json;
			}

			var data = json.Data as PaginatedGridData;

			if (data == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.NoContent);
			}

			var csv = new StringBuilder();

			var dataColumns = columns.Where(col => col.LogicalName != "col-action").ToArray();

			foreach (var column in dataColumns)
			{
				csv.Append(EncodeCommaSeperatedValue(column.Name));
			}

			csv.AppendLine();

			foreach (var record in data.Records)
			{
				foreach (var column in dataColumns)
				{
					var attribute = record.Attributes.FirstOrDefault(a => a.Name == column.LogicalName);

					if (attribute == null) continue;

					csv.Append(EncodeCommaSeperatedValue(attribute.DisplayValue as string));
				}

				csv.AppendLine();
			}

			var filename = new string(viewName.Where(c => !Path.GetInvalidFileNameChars().Contains(c)).ToArray());

			var sessionKey = "{0:s}|{1}.csv".FormatWith(DateTime.UtcNow, filename);

			Session[sessionKey] = csv.ToString();

			return Json(new { success = true, sessionKey }, JsonRequestBehavior.AllowGet);
		}

		[ActionName("DownloadAsCsv")]
		[HttpGet]
		public ActionResult GetCsvFile(string key)
		{
			var csv = Session[key] as string;

			if (string.IsNullOrEmpty(csv))
			{
				return new HttpStatusCodeResult(HttpStatusCode.NoContent);
			}

			Session[key] = null;
			
			return File(new UTF8Encoding().GetBytes(csv), "text/csv", key.Substring(key.IndexOf('|') + 1));
		}

		[HttpPost]
		[JsonHandlerError]
		public ActionResult DownloadAsExcel(string viewName, IEnumerable<LayoutColumn> columns, string base64SecureConfiguration, string sortExpression, string search, string filter,
			string metaFilter, int page = 1, int pageSize = DefaultPageSize)
		{
			var viewConfiguration = ConvertSecureStringToViewConfiguration(base64SecureConfiguration);

			if (viewConfiguration == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Invalid Request.");
			}

			// override the page parameters to get up to 5000 records.
			page = 1;
			pageSize = 5000;
			viewConfiguration.PageSize = 5000;
			
			var json = GetData(viewConfiguration, sortExpression, search, filter, metaFilter, page, pageSize, true, false, null, null, null, null, true) as JsonResult;

			if (json == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.NoContent);
			}

			if (json.Data is EntityPermissionResult)
			{
				return json;
			}

			var data = json.Data as PaginatedGridData;

			if (data == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.NoContent);
			}

			var stream = new MemoryStream();

			var spreadsheet = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook);

			var workbookPart = spreadsheet.AddWorkbookPart();
			workbookPart.Workbook = new Workbook();

			var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
			var sheet = new Sheet {Id = spreadsheet.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = viewName};

			var sheets = new Sheets();
			sheets.Append(sheet);

			var sheetData = new SheetData();

			var rowIndex = 1;
			var columnIndex = 1;

			var firstRow = new Row {RowIndex = (uint) rowIndex};

			var dataColumns = columns.Where(col => col.LogicalName != "col-action").ToArray();

			foreach (var column in dataColumns)
			{
				var cell = new Cell {CellReference = CreateCellReference(columnIndex) + rowIndex, DataType = CellValues.InlineString};

				var inlineString = new InlineString {Text = new Text {Text = column.Name}};

				cell.AppendChild(inlineString);

				firstRow.AppendChild(cell);

				columnIndex++;
			}

			sheetData.Append(firstRow);

			foreach (var record in data.Records)
			{
				var row = new Row {RowIndex = (uint) ++rowIndex};

				columnIndex = 0;

				foreach (var column in dataColumns)
				{
					columnIndex++;

					var attribute = record.Attributes.FirstOrDefault(a => a.Name == column.LogicalName);

					if (attribute == null) continue;

					var cell = new Cell { CellReference = CreateCellReference(columnIndex) + rowIndex, DataType = CellValues.InlineString };

					var inlineString = new InlineString { Text = new Text { Text = attribute.DisplayValue as string } };

					cell.AppendChild(inlineString);

					row.AppendChild(cell);
				}

				sheetData.Append(row);
			}

			worksheetPart.Worksheet = new Worksheet(sheetData);

			spreadsheet.WorkbookPart.Workbook.AppendChild(sheets);

			workbookPart.Workbook.Save();

			spreadsheet.Close();

			var filename = new string(viewName.Where(c => !Path.GetInvalidFileNameChars().Contains(c)).ToArray());

			var sessionKey = "{0:s}|{1}.xlsx".FormatWith(DateTime.UtcNow, filename);

			stream.Position = 0; // Reset the stream to the beginning and save to session.

			Session[sessionKey] = stream;

			return Json(new {success = true, sessionKey}, JsonRequestBehavior.AllowGet);
		}

		[ActionName("DownloadAsExcel")]
		[HttpGet]
		public ActionResult GetExcelFile(string key)
		{
			using (var stream = Session[key] as MemoryStream)
			{
				if (stream == null)
				{
					return new HttpStatusCodeResult(HttpStatusCode.NoContent);
				}

				Session[key] = null;

				return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", key.Substring(key.IndexOf('|') + 1));
			}
		}

		[HttpPost]
		[JsonHandlerError]
		public ActionResult ExecuteWorkflow(EntityReference workflow, EntityReference entity)
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
			var serviceContext = dataAdapterDependencies.GetServiceContext();

			var request = new ExecuteWorkflowRequest
			{
				WorkflowId = workflow.Id,
				EntityId = entity.Id
			};

			serviceContext.Execute(request);

			serviceContext.TryRemoveFromCache(entity);

			return new HttpStatusCodeResult(HttpStatusCode.NoContent);
		}

		private string CreateCellReference(int column)
		{
			// A, B, C...Z, AA, BB...ZZ, AAA, BBB...
			const char firstRef = 'A';
			const uint firstIndex = (uint)firstRef;
			
			var result = string.Empty;

			while (column > 0)
			{
				var mod = (column - 1) % 26;
				result += (char)(firstIndex + mod);
				column = (column - mod) / 26;
			}

			return result;
		}

		private static string EncodeCommaSeperatedValue(string value)
		{
			return !string.IsNullOrEmpty(value)
				? string.Format(@"""{0}"",", value.Replace(@"""", @""""""))
				: ",";
		}
	}
}
