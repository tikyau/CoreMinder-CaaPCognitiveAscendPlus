﻿using System.Web.Mvc;

namespace Site.Areas.Service311
{
	public class Service311AreaRegistration : AreaRegistration
	{
		public override string AreaName
		{
			get
			{
				return "Service311";
			}
		}

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.MapRoute("ServiceRequestsMap", "Service311/Map/{action}" , new { controller = "Map", action = "Search" });

		}
	}
}
