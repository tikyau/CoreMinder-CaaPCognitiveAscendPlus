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
using System.Web.Http.Routing;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;

namespace Site.Areas.WeChat.Pages
{
    public partial class MyGiftList : PortalPage
    {
        OrganizationService service;

        private DataTable _redemreqs;

        private Entity _redemreq;

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

        public Entity RedemReqToEdit
        {
            get
            {
                if (_redemreq != null)
                {
                    return _redemreq;
                }

                Guid RedemReqId;

                if (!Guid.TryParse(Request["id"], out RedemReqId))
                {
                    return null;
                }

                _redemreq = XrmContext.CreateQuery("new_redemptionrequest").FirstOrDefault(c => c.GetAttributeValue<Guid>("new_redemptionrequestid") == RedemReqId);

                return _redemreq;
            }
        }


        protected void MyGiftList_OnRowDataBound(object sender, GridViewRowEventArgs e)
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

            var dataKey = MyGiftListView.DataKeys[e.Row.RowIndex].Value;

            e.Row.Cells[1].Text = string.Format(@"<a href=""{0}"" ><img src=""{1}""></a>",
                    MyGiftDetailsUrl(dataKey),
                    e.Row.Cells[1].Text);

            //e.Row.Cells[1].Attributes.Add("style", "white-space: nowrap;");

            foreach (var cell in e.Row.Cells.Cast<DataControlFieldCell>().Where(cell => cell.ContainingField.HeaderText == "Points Required"))
            {
                decimal parsedDecimal;

                if (decimal.TryParse(cell.Text, out parsedDecimal))
                {
                    cell.Text = parsedDecimal.ToString("C");
                }
            }
        }

        protected void AcceptedMyGiftList_Sorting(object sender, GridViewSortEventArgs e)
        {
            SortDirection = e.SortExpression == SortExpression
                ? (SortDirection == "ASC" ? "DESC" : "ASC")
                : "ASC";

            SortExpression = e.SortExpression;

            _redemreqs.DefaultView.Sort = string.Format("{0} {1}", SortExpression, SortDirection);

            MyGiftListView.DataSource = _redemreqs;
            MyGiftListView.DataBind();
        }

        protected string MyGiftDetailsUrl(object id)
        {
            var page = ServiceContext.GetPageBySiteMarkerName(Website, "My Gift Details");

            var url = new UrlBuilder(ServiceContext.GetUrl(page));

            url.QueryString.Set("id", id.ToString());

            return url.PathWithQueryString;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            RedirectToLoginIfAnonymous();
            RedirectToLoginIfNecessary();

            service = new OrganizationService("Xrm");

            var portalContext = Microsoft.Xrm.Portal.Configuration.PortalCrmConfigurationManager.CreatePortalContext();

            var contact = portalContext.User;

            //var redemreqs = Enumerable.Empty<Entity>();            
            //var searchQuery = Request["query"];                       
            //Guid giftid;

            string WebUrl = WebConfigurationManager.ConnectionStrings["Xrm"].ConnectionString;
            int index = WebUrl.IndexOf("=") + 1;
            int index2 = WebUrl.IndexOf(";");
            WebUrl = WebUrl.Substring(index, index2 - index);
            //UrlHelper urlhelper;

            var redemreqs =
                from r in XrmContext.CreateQuery("new_redemptionrequest")
                join g in XrmContext.CreateQuery("new_gift") on r.GetAttributeValue<EntityReference>("new_gift").Id equals
                    g.GetAttributeValue<Guid>("new_giftid")
                where r.GetAttributeValue<EntityReference>("new_gift") != null
                where
                    r.GetAttributeValue<EntityReference>("new_contact").Id == contact.Id &&
                    r.GetAttributeValue<bool>("new_isused") == false
                select new
                {
                    redemreqid = r.Id,
                    EntityImage = "data:image/png;base64," + Convert.ToBase64String(g.GetAttributeValue<byte[]>("entityimage")),
                    code = r.GetAttributeValue<string>("new_name"),
                    giftName = g.GetAttributeValue<string>("new_name")
                };
            _redemreqs = EnumerableExtensions.CopyToDataTable(redemreqs);
            /*
            _redemreqs = EnumerableExtensions.CopyToDataTable(redemreqs.Select(redemreq => new
            {
                redemreqid = redemreq.GetAttributeValue<Guid>("new_redemptionrequestid"),                
                //Image = gift.GetRelatedEntity(ServiceContext, new Relationship("new_gift_Annotations")) != null ? gift.GetRelatedEntity(ServiceContext, new Relationship("new_gift_Annotations")).GetAttributeValue<string>("documentbody") : " ",
                //EntityImage = WebUrl + redemreq.GetAttributeValue<string>("entityimage_url"),
                //EntityImage = WebUrl + redemreq.GetRelatedEntity(ServiceContext, new Relationship("new_new_gift_new_redemptionrequest_Gift")) != null ? redemreq.GetRelatedEntity(ServiceContext, new Relationship("new_new_gift_new_redemptionrequest_Gift")).GetAttributeValue<string>("entityimage_url") : " ",
                Code = redemreq.GetAttributeValue<string>("new_name"),
                GiftName = redemreq.GetRelatedEntity(ServiceContext, "new_new_gift_new_redemptionrequest_Gift") != null ? redemreq.GetRelatedEntity(ServiceContext, "new_new_gift_new_redemptionrequest_Gift").GetAttributeValue<string>("new_name") : " ",                
            }));//.OrderBy(gift => gift.GiftName));
            */
            _redemreqs.Columns["EntityImage"].ColumnName = "Gift Image";
            _redemreqs.Columns["Code"].ColumnName = "Redemption No.";
            _redemreqs.Columns["GiftName"].ColumnName = "Gift Name";

            MyGiftListView.DataKeyNames = new[] { "redemreqid" };
            MyGiftListView.DataSource = _redemreqs;
            MyGiftListView.DataBind();
        }

    }
}