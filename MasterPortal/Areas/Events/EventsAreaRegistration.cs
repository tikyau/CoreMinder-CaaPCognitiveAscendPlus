﻿using System.Web.Mvc;
using System.Web.Routing;
using Adxstudio.Xrm.Events;

namespace Site.Areas.Events
{
	public class EventsAreaRegistration : AreaRegistration
	{
		public override string AreaName
		{
			get { return "Events"; }
		}

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.Routes.Add("iCalendar", new Route("_services/icalendar/{eventScheduleId}", new CalendarRouteHandler()));
		}
	}
}