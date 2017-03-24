using System;
using System.Collections.Generic;
using Adxstudio.Xrm.Web.UI.CrmEntityListView;
using Site.Pages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Client.Services;
using Microsoft.Xrm.Sdk.Query;
using Site.Areas.EntityList.Helpers;
using Microsoft.Xrm.Client.Diagnostics;
using Adxstudio.Xrm.Web.UI.JsonConfiguration;
using Adxstudio.Xrm.Web.UI;
using System.Linq;
using System.Web.Script.Serialization;
using System.Globalization;
using Adxstudio.Xrm.Data;
using System.Data;
using System.Web.UI.WebControls;
using Adxstudio.Xrm.Cms;
using Microsoft.Xrm.Portal.Web;
using Microsoft.Xrm.Client;
using Adxstudio.Xrm.Notes;
using Adxstudio.Xrm.Services.Query;
using System.Web.Configuration;

namespace Site.Areas.WeChat.Pages
{
    public partial class GiftList : PortalPage
    {
        OrganizationService service;

        private DataTable _gifts;

        private Entity _gift;

        protected string SortDirection
        {
            get { return ViewState["SortDirection"] as string ?? "ASC"; }
            set { ViewState["SortDirection"] = value; }
        }
        protected string SortExpression
        {
            get { return ViewState["SortExpression"] as string ?? "Accepted"; }
            set { ViewState["SortExpression"] = value; }
        }

        public Entity GiftToEdit
        {
            get
            {
                if (_gift != null)
                {
                    return _gift;
                }

                Guid giftId;

                if (!Guid.TryParse(Request["id"], out giftId))
                {
                    return null;
                }

                _gift = XrmContext.CreateQuery("new_gift").FirstOrDefault(c => c.GetAttributeValue<Guid>("new_giftid") == giftId);

                return _gift;
            }
        }


        protected void GiftList_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header ||
                e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[0].Visible = false;
            }

            if (e.Row.RowType != DataControlRowType.DataRow)
            {
                return;
            }

            var dataKey = GiftListView.DataKeys[e.Row.RowIndex].Value;

            e.Row.Cells[1].Text = string.Format(@"<a href=""{0}"" ><img src=""{1}""></a>",
                    GiftDetailsUrl(dataKey),
                    e.Row.Cells[1].Text);

            //e.Row.Cells[1].Attributes.Add("style", "white-space: nowrap;");

            //foreach (var cell in e.Row.Cells.Cast<DataControlFieldCell>().Where(cell => cell.ContainingField.HeaderText == "Points Required"))
            //{
            //    decimal parsedDecimal;

            //    if (decimal.TryParse(cell.Text, out parsedDecimal))
            //    {
            //        cell.Text = parsedDecimal.ToString("C");
            //    }
            //}
        }

        protected void AcceptedGiftList_Sorting(object sender, GridViewSortEventArgs e)
        {
            SortDirection = e.SortExpression == SortExpression
                ? (SortDirection == "ASC" ? "DESC" : "ASC")
                : "ASC";

            SortExpression = e.SortExpression;

            _gifts.DefaultView.Sort = string.Format("{0} {1}", SortExpression, SortDirection);

            GiftListView.DataSource = _gifts;
            GiftListView.DataBind();
        }

        protected string GiftDetailsUrl(object id)
        {
            var page = ServiceContext.GetPageBySiteMarkerName(Website, "Gift Details");

            var url = new UrlBuilder(ServiceContext.GetUrl(page));

            url.QueryString.Set("id", id.ToString());

            return url.PathWithQueryString;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            RedirectToLoginIfAnonymous();
            RedirectToLoginIfNecessary();

            service = new OrganizationService("Xrm");

            //var gifts = Enumerable.Empty<Entity>();

            //var searchQuery = Request["query"];                       
            //Guid giftid;

            DateTime startdate = new DateTime(DateTime.Today.Year, 1, 1, 0, 0, 1);

            DateTime enddate = new DateTime(DateTime.Today.Year, 12, 31, 23, 59, 50);

            string WebUrl = WebConfigurationManager.ConnectionStrings["Xrm"].ConnectionString;
            int index = WebUrl.IndexOf("=") + 1;
            int index2 = WebUrl.IndexOf(";");
            WebUrl = WebUrl.Substring(index, index2 - index);
            var gifts = from g in XrmContext.CreateQuery("new_gift")
                        join n in XrmContext.CreateQuery("annotation")
                        on g.GetAttributeValue<Guid>("new_giftid") equals n.GetAttributeValue<EntityReference>("objectid").Id
                        where g.GetAttributeValue<DateTime>("new_redemptionstartdate") >= startdate &&
                        g.GetAttributeValue<DateTime>("new_redemptionenddate") <= enddate
                        select new
                        {
                            giftid = g.Id,
                            EntityImage = "data:image/png;base64," + Convert.ToBase64String(g.GetAttributeValue<byte[]>("entityimage")),
                            GiftName = g.GetAttributeValue<string>("new_name"),
                            PointRequired = Convert.ToInt32(g.GetAttributeValue<decimal>("new_pointsrequired")).ToString("N2")
                        };
            _gifts = EnumerableExtensions.CopyToDataTable(gifts);

            _gifts.Columns["EntityImage"].ColumnName = "Gift Image";
            _gifts.Columns["GiftName"].ColumnName = "Gift Name";
            _gifts.Columns["PointRequired"].ColumnName = "Points Required";

            GiftListView.DataKeyNames = new[] { "giftid" };
            GiftListView.DataSource = _gifts;
            GiftListView.DataBind();

        }

    }
}