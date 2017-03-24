﻿using System.Web.Http;
using System.Web.Http.OData.Routing;
using System.Web.Http.OData.Routing.Conventions;
using System.Web.Mvc;
using Adxstudio.Xrm.Web.UI.EntityList.OData;

namespace Site.Areas.EntityList
{
	public class EntityListAreaRegistration : AreaRegistration
	{
		public override string AreaName
		{
			get { return "EntityList"; }
		}

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.MapRoute("EntityListMapSearch", "EntityList/Map/Search/{longitude}/{latitude}/{distance}/{units}/{id}", new { controller = "Map", action = "Search", longitude = UrlParameter.Optional, latitude = UrlParameter.Optional, distance = UrlParameter.Optional, units = UrlParameter.Optional, id = UrlParameter.Optional });
			context.MapRoute("EntityListCalendar", "EntityList/Calendar/{__portalScopeId__}/{entityListId}/{viewId}", new { controller = "Calendar", action = "Index" });
			context.MapRoute("EntityListCalendarDownload", "EntityList/Calendar/{__portalScopeId__}/{entityListId}/{viewId}/icalendar", new { controller = "Calendar", action = "Download" });
			context.MapRoute("EntityListPackageRepository", "EntityList/PackageRepository/{__portalScopeId__}/{entityListId}/{viewId}/repository.json", new { controller = "PackageRepository", action = "Index" });
			context.MapRoute("EntityListPackageRepositoryByPartialUrl", "EntityList/PackageRepository/Repositories/{__portalScopeId__}/{repositoryPartialUrl}", new { controller = "PackageRepository", action = "IndexByPartialUrl" });
			context.MapRoute("EntityListPackageRepositoryImage", "EntityList/PackageRepository/Images/{__portalScopeId__}/{packageImageId}", new { controller = "PackageRepository", action = "PackageImage" });
			context.MapRoute("EntityListPackageRepositoryVersion", "EntityList/PackageRepository/Versions/{__portalScopeId__}/{packageVersionId}", new { controller = "PackageRepository", action = "PackageVersion" });
			context.MapRoute("EntityListPackageRepositoryDiscovery", "_installer.json", new { controller = "PackageRepository", action = "GetRepositories" });

			RegisterEntityListODataRoute(GlobalConfiguration.Configuration);
		}

		public void RegisterEntityListODataRoute(HttpConfiguration config)
		{
			config.MessageHandlers.Add(new EntityListFormatQueryMessageHandler());
			
			var routingConventions = ODataRoutingConventions.CreateDefault();
			
			routingConventions.Insert(0, new EntitySetODataRoutingConvention());
			
			const string routeName = "EntityListOData";
			const string routePrefix = "_odata";

			var routeConstraint = new EntityListODataPathRouteConstraint(new DefaultODataPathHandler(), routeName, routingConventions);
			
			config.Routes.Add(routeName, new ODataRoute(routePrefix, routeConstraint));
		}
	}
}
