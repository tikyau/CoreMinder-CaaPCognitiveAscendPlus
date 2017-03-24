using System;
using System.Linq;
using Adxstudio.Xrm.Events;
using Adxstudio.Xrm.Web.Mvc.Html;

namespace Site.Controls
{
	public partial class EventsPanel : PortalUserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			var dataAdapter = new WebsiteEventDataAdapter(new Adxstudio.Xrm.Cms.PortalContextDataAdapterDependencies(Portal));
			var now = DateTime.UtcNow;

			var future = Html.TimeSpanSetting("Events/DisplayTimeSpan/Future").GetValueOrDefault(TimeSpan.FromDays(90));

			UpcomingEvents.DataSource = dataAdapter.SelectEventOccurrences(now, now.Add(future)).Take(Html.IntegerSetting("Home Event Count").GetValueOrDefault(3));
			UpcomingEvents.DataBind();
		}
	}
}
