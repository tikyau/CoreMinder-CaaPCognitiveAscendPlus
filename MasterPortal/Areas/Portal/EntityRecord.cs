using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Adxstudio.Xrm.Metadata;
using Adxstudio.Xrm.Security;
using Adxstudio.Xrm.Globalization;
using Microsoft.Xrm.Portal.Configuration;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Metadata;
using DateTimeFormat = Microsoft.Xrm.Sdk.Metadata.DateTimeFormat;

namespace Site.Areas.Portal
{
	[DataContract]
	public class EntityRecord
	{
		public const string DateTimeClientFormat = DateTimeFormatInfo.ISO8601Pattern;

		public EntityRecord(Entity entity, OrganizationServiceContext serviceContext, CrmEntityPermissionProvider provider, EntityMetadata entityMetadata = null, bool readGranted = false)
		{
			var entityPermissionResult = provider.TryAssert(serviceContext, entity, entityMetadata, readGranted);

			CanRead = entityPermissionResult.CanRead;
			CanWrite = entityPermissionResult.CanWrite;
			CanDelete = entityPermissionResult.CanDelete;
			CanAppend = entityPermissionResult.CanAppend;
			CanAppendTo = entityPermissionResult.CanAppendTo;

			var statecode = entity.GetAttributeValue<OptionSetValue>("statecode");

			if (statecode != null)
			{
				StateCode = statecode.Value;
			}

			var statuscode = entity.GetAttributeValue<OptionSetValue>("statuscode");

			if (statuscode != null)
			{
				StatusCode = statuscode.Value;
			}

			ConvertToEntityRecord(entity, entityMetadata, serviceContext);
		}

		public EntityRecord(Entity entity, EntityMetadata entityMetadata = null, OrganizationServiceContext serviceContext = null)
		{
			ConvertToEntityRecord(entity, entityMetadata, serviceContext);
		}

		protected void ConvertToEntityRecord(Entity entity, EntityMetadata entityMetadata = null, OrganizationServiceContext serviceContext = null)
		{
			var recordAttributes = new List<EntityRecordAttribute>();
			var attributes = entity.Attributes;
			var formattedAttributes = entity.FormattedValues;
			
			if (serviceContext == null)
			{
				serviceContext = PortalCrmConfigurationManager.CreateServiceContext();
			}

			foreach (var attribute in attributes)
			{
				var aliasedValue = attribute.Value as AliasedValue;
				var value = aliasedValue != null ? aliasedValue.Value : attribute.Value;
				var type = value.GetType().ToString();
				var formattedValue = string.Empty;
				var displayValue = value;
				AttributeMetadata attributeMetadata = null;

				if (formattedAttributes.Contains(attribute.Key))
				{
					formattedValue = formattedAttributes[attribute.Key];
					displayValue = formattedValue;
				}

				if (attribute.Value is AliasedValue)
				{
					var aliasedEntityMetadata = serviceContext.GetEntityMetadata(aliasedValue.EntityLogicalName, EntityFilters.Attributes);

					if (aliasedEntityMetadata != null)
					{
						attributeMetadata = aliasedEntityMetadata.Attributes.FirstOrDefault(a => a.LogicalName == aliasedValue.AttributeLogicalName);
					}
				}
				else
				{
					if (entityMetadata != null)
					{
						attributeMetadata = entityMetadata.Attributes.FirstOrDefault(a => a.LogicalName == attribute.Key);
					}
				}

				if (attributeMetadata != null)
				{
					switch (attributeMetadata.AttributeType)
					{
						case AttributeTypeCode.Customer:
						case AttributeTypeCode.Lookup:
						case AttributeTypeCode.Owner:
							var entityReference = attribute.Value as EntityReference;
							if (entityReference != null)
							{
								displayValue = entityReference.Name ?? string.Empty;
							}
							break;
						case AttributeTypeCode.DateTime:
							var datetimeAttributeMetadata = attributeMetadata as DateTimeAttributeMetadata;
							if (datetimeAttributeMetadata != null)
							{
								if (datetimeAttributeMetadata.Format.GetValueOrDefault(DateTimeFormat.DateAndTime) != DateTimeFormat.DateOnly)
								{
									// Don't use the formatted value, as the connection user's timezone is used to format the datetime value. Use the UTC value for display.
									var date = (DateTime)value;
									displayValue = date.ToString(DateTimeClientFormat);
								}
							}
							break;
					}
				}
				else
				{
					if (attribute.Value is EntityReference)
					{
						var entityReference = (EntityReference)attribute.Value;
						if (entityReference != null)
						{
							displayValue = entityReference.Name ?? string.Empty;
						}
					}
					else if (attribute.Value is DateTime)
					{
						// Don't use the formatted value, as the connection user's timezone is used to format the datetime value. Use the UTC value for display.
						if (formattedValue.Contains(":")) // Indicates this is not a date only attribute
						{
							var date = (DateTime)value;
							displayValue = date.ToString(DateTimeClientFormat);
						}
					}
				}
				
				recordAttributes.Add(new EntityRecordAttribute
				{
					Name = attribute.Key,
					Value = value,
					FormattedValue = formattedValue,
					DisplayValue = displayValue,
					Type = type,
					AttributeMetadata = attributeMetadata
				});
			}

			Id = entity.Id;
			EntityName = entity.LogicalName;
			Attributes = recordAttributes;
		}

		[DataMember(Name = "id")]
		public Guid Id { get; set; }

		[DataMember]
		public string EntityName { get; set; }

		[DataMember]
		public IEnumerable<EntityRecordAttribute> Attributes { get; set; }

		[DataMember]
		public bool CanRead { get; set; }

		[DataMember]
		public bool CanWrite { get; set; }

		[DataMember]
		public bool CanDelete { get; set; }

		[DataMember]
		public bool CanAppend { get; set; }

		[DataMember]
		public bool CanAppendTo { get; set; }

		[DataMember]
		public int StateCode { get; private set; }

		[DataMember]
		public int StatusCode { get; private set; }
	}

	[DataContract]
	public class EntityRecordAttribute
	{
		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Type { get; set; }

		[DataMember]
		public object Value { get; set; }

		[DataMember]
		public string FormattedValue { get; set; }

		[DataMember]
		public object DisplayValue { get; set; }

		[DataMember]
		public AttributeMetadata AttributeMetadata { get; set; }
	}

	public class PaginatedGridData
	{
		public PaginatedGridData(IEnumerable<EntityRecord> records, int itemCount, int page, int pageSize)
		{
			Records = records;
			ItemCount = itemCount;
			PageCount = itemCount > 0 ? (int)Math.Ceiling(itemCount / (double)pageSize) : 0;
			PageNumber = page;
			PageSize = pageSize;
		}

		public IEnumerable<EntityRecord> Records { get; set; }
		public int ItemCount { get; set; }
		public int PageCount { get; set; }
		public int PageNumber { get; set; }
		public int PageSize { get; set; }
	}

	public class EntityPermissionResult
	{
		public EntityPermissionResult(bool accessDenied)
		{
			AccessDenied = accessDenied;
		}

		public bool AccessDenied { get; set; }
	}
}