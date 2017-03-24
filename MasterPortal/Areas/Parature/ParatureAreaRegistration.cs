using System.Web.Mvc;

namespace Site.Areas.Parature
{
	public class ParatureAreaRegistration : AreaRegistration
	{
		public override string AreaName
		{
			get { return "Parature"; }
		}

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.MapRoute("ParatureCreateTicket", "_parature-services/ticket/create", new { controller = "Api", action = "CreateTicket" }, new [] { "Adxstudio.Xrm.Parature.Controllers" });
			context.MapRoute("ParatureRunTicketAction", "_parature-services/ticket/action", new { controller = "Api", action = "RunTicketAction" }, new[] { "Adxstudio.Xrm.Parature.Controllers" });
			context.MapRoute("ParatureAddTicketAttachments", "_parature-services/ticket/attachments/add", new { controller = "Api", action = "AddTicketAttachments" }, new[] { "Adxstudio.Xrm.Parature.Controllers" });
			context.MapRoute("ParatureSearchArticles", "_parature-services/articles/search", new { controller = "Api", action = "SearchArticles" }, new [] { "Adxstudio.Xrm.Parature.Controllers" });
		}
	}
}
