using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Adxstudio.Xrm.Mapping;
using Microsoft.Xrm.Portal.Configuration;
using Microsoft.Xrm.Portal.Web;

namespace Site.Areas.EntityList.Controllers
{
	public class MapController : Controller
	{
		private const int DefaultMapPushpinWidth = 32;
		private const int DefaultMapPushpinHeight = 39;

		public class MapNode
		{
			public MapNode(Guid id, string url, string title, string description, double latitude, double longitude, string location, double distance, string pushpinImageUrl, int pushpinImageHeight, int pushpinImageWidth)
			{
				Description = description;
				Distance = distance;
				Id = id;
				Latitude = (decimal)latitude;
				Location = location;
				Longitude = (decimal)longitude;
				PushpinImageHeight = pushpinImageHeight;
				PushpinImageUrl = pushpinImageUrl;
				PushpinImageWidth = pushpinImageWidth;
				Title = title;
				Url = url;
			}

			public string Description { get; private set; }
			public double Distance { get; private set; }
			public Guid Id { get; private set; }
			public decimal Latitude { get; private set; }
			public string Location { get; private set; }
			public decimal Longitude { get; private set; }
			public int PushpinImageHeight { get; private set; }
			public string PushpinImageUrl { get; private set; }
			public int PushpinImageWidth { get; private set; }
			public string Title { get; private set; }
			public string Url { get; private set; }
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult Search(double longitude, double latitude, int distance, string units, Guid id)
		{
			var originLatitude = latitude;
			var originLongitude = longitude;
			var radius = distance <= 0 ? 5 : distance;

			var uom = string.IsNullOrWhiteSpace(units)
						? GeoHelpers.Units.Miles
						: units == "km" ? GeoHelpers.Units.Kilometers : GeoHelpers.Units.Miles;
			var context = PortalCrmConfigurationManager.CreateServiceContext();

			var entityList = context.CreateQuery("adx_entitylist").FirstOrDefault(e => e.GetAttributeValue<Guid>("adx_entitylistid") == id);

			if (entityList == null)
			{
				return null;
			}

			var entityName = entityList.GetAttributeValue<string>("adx_entityname");
			var latitudeFieldName = entityList.GetAttributeValue<string>("adx_map_latitudefieldname");
			var longitudeFieldName = entityList.GetAttributeValue<string>("adx_map_longitudefieldname");
			var titleFieldName = entityList.GetAttributeValue<string>("adx_map_infoboxtitlefieldname");
			var descriptionFieldName = entityList.GetAttributeValue<string>("adx_map_infoboxdescriptionfieldname");
			var pushpinImageUrl = entityList.GetAttributeValue<string>("adx_map_pushpinurl") ?? string.Empty;
			var pushpinImageWidth = entityList.GetAttributeValue<int?>("adx_map_pushpinwidth").GetValueOrDefault(DefaultMapPushpinWidth);
			var pushpinImageHeight = entityList.GetAttributeValue<int?>("adx_map_pushpinheight").GetValueOrDefault(DefaultMapPushpinHeight);

			if (!string.IsNullOrWhiteSpace(pushpinImageUrl) && !pushpinImageUrl.StartsWith("http", true, CultureInfo.InvariantCulture))
			{
				var portalContext = PortalCrmConfigurationManager.CreatePortalContext();
				pushpinImageUrl = WebsitePathUtility.ToAbsolute(portalContext.Website, pushpinImageUrl);
			}

			var entities = context.CreateQuery(entityName)
				.Where(e => e.GetAttributeValue<double?>(latitudeFieldName) != null && e.GetAttributeValue<double?>(longitudeFieldName) != null)
				.FilterByBoxDistance(originLatitude, originLongitude, longitudeFieldName, latitudeFieldName, "double", radius, uom);

			var nodes = entities.DistanceQueryWithResult(originLatitude, originLongitude, longitudeFieldName, latitudeFieldName, radius, uom).ToList();

			var mapNodes = new List<MapNode>();

			if (nodes.Any())
			{
				mapNodes = nodes.Select(s =>
					new MapNode(s.Item1.Id, string.Empty,
						s.Item1.GetAttributeValue<string>(titleFieldName),
						s.Item1.GetAttributeValue<string>(descriptionFieldName),
						s.Item1.GetAttributeValue<Double?>(latitudeFieldName) ?? 0,
						s.Item1.GetAttributeValue<Double?>(longitudeFieldName) ?? 0,
						string.Format("{0},{1}", s.Item1.GetAttributeValue<Double?>(latitudeFieldName) ?? 0, s.Item1.GetAttributeValue<Double?>(longitudeFieldName) ?? 0),
						Math.Round(s.Item2, 1), pushpinImageUrl, pushpinImageHeight, pushpinImageWidth)).ToList();
			}

			var json = Json(mapNodes);

			return json;
		}
	}
}
