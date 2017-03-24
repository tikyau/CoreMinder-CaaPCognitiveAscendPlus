using System;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using Microsoft.Xrm.Sdk;

namespace Site.Pages
{
	public partial class PollArchives : PortalPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (IsPostBack)
			{
				return;
			}

			var polls = ServiceContext.CreateQuery("adx_poll")
				.Where(p => p.GetAttributeValue<EntityReference>("adx_websiteid") == Website.ToEntityReference() && p.GetAttributeValue<DateTime?>("adx_expirationdate").GetValueOrDefault() <= DateTime.UtcNow)
				.ToList();

			PollsArchiveListView.DataSource = polls;
			PollsArchiveListView.DataBind();
		}

		protected void PollsArchiveListView_ItemDataBound(object sender, ListViewItemEventArgs e)
		{
			var listItem = e.Item as ListViewDataItem;

			if (listItem == null || listItem.DataItem == null)
			{
				return;
			}

			var poll = listItem.DataItem as Entity;

			var listView = (ListView)e.Item.FindControl("PollResponsesListView");
			var totalLabel = (System.Web.UI.WebControls.Label)e.Item.FindControl("Total");

			var pollResponses = ServiceContext.CreateQuery("adx_polloption").Where(p => p.GetAttributeValue<EntityReference>("adx_pollid") == poll.ToEntityReference()).ToList();

			var totalVotes = pollResponses.Sum(p => p.GetAttributeValue<int?>("adx_votes").GetValueOrDefault(0));

			totalLabel.Text = totalVotes.ToString(CultureInfo.InvariantCulture);

			if (totalVotes <= 0)
			{
				return;
			}

			var results = from response in pollResponses
				select new
				{
					Response = response.GetAttributeValue<string>("adx_answer"),
					Count = response.GetAttributeValue<int?>("adx_votes").GetValueOrDefault(0),
					Percentage = Convert.ToInt32((response.GetAttributeValue<int?>("adx_votes").GetValueOrDefault(0)) / ((float)totalVotes) * (100))
				};

			listView.DataSource = results;

			listView.DataBind();
		}
	}
}