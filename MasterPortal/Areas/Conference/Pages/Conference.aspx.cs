using System;
using Adxstudio.Xrm.Cms;
using Microsoft.Xrm.Portal.Web;

namespace Site.Areas.Conference.Pages
{
	public partial class Conference : ConferencePage
	{
		protected const string ConferenceIdQueryStringParameterName = "conferenceid";

		protected void Page_Load(object sender, EventArgs args)
		{
		}

		protected void Register_Click(object sender, EventArgs args)
		{
			if (PortalConference == null)
			{
				return;
			}

			var registrationUrl = GetRegistrationUrl(PortalConference.Id);

			Response.Redirect(registrationUrl);
		}

		protected string GetRegistrationUrl(Guid conferenceId)
		{
			var page = ServiceContext.GetPageBySiteMarkerName(Website, "Conference Registration");

			if (page == null)
			{
				throw new ApplicationException("Page could not be found for Site Marker named 'Conference Registration'");
			}

			var url = ServiceContext.GetUrl(page);

			if (string.IsNullOrWhiteSpace(url))
			{
				throw new ApplicationException("Url could not be determined for Site Marker named 'Conference Registration'");
			}

			var urlBuilder = new UrlBuilder(url);

			urlBuilder.QueryString.Add(ConferenceIdQueryStringParameterName, conferenceId.ToString());

			return urlBuilder.PathWithQueryString;
		}
	}
}
