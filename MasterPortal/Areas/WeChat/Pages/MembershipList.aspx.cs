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
    public partial class MembershipList : PortalPage
    {
        OrganizationService service;

        private DataTable _memberships;

        private Entity _membership;

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
                if (_membership != null)
                {
                    return _membership;
                }

                Guid membershipId;

                if (!Guid.TryParse(Request["id"], out membershipId))
                {
                    return null;
                }

                _membership = XrmContext.CreateQuery("new_redemptionrequest").FirstOrDefault(c => c.GetAttributeValue<Guid>("new_redemptionrequestid") == membershipId);

                return _membership;
            }
        }


        protected void Membership_OnRowDataBound(object sender, GridViewRowEventArgs e)
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

            var dataKey = MembershipListView.DataKeys[e.Row.RowIndex].Value;

            e.Row.Cells[1].Text = string.Format(@"<a href=""{0}"" ><img src=""{1}""></a>",
                    MembershipDetailsUrl(dataKey),
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

        protected void AcceptedMembershipList_Sorting(object sender, GridViewSortEventArgs e)
        {
            SortDirection = e.SortExpression == SortExpression
                ? (SortDirection == "ASC" ? "DESC" : "ASC")
                : "ASC";

            SortExpression = e.SortExpression;

            _memberships.DefaultView.Sort = string.Format("{0} {1}", SortExpression, SortDirection);

            MembershipListView.DataSource = _memberships;
            MembershipListView.DataBind();
        }

        protected string MembershipDetailsUrl(object id)
        {
            var page = ServiceContext.GetPageBySiteMarkerName(Website, "Membership Details");

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

            var memberships =
                from m in XrmContext.CreateQuery("adx_membership")
                join l in XrmContext.CreateQuery("new_loyaltyprogram") on m.GetAttributeValue<EntityReference>("new_loyaltyprogram").Id equals
                    l.GetAttributeValue<Guid>("new_loyaltyprogramid")
                join t in XrmContext.CreateQuery("new_tier") on m.GetAttributeValue<EntityReference>("new_membershiplevel").Id equals
                    t.GetAttributeValue<Guid>("new_tierid")
                where m.GetAttributeValue<EntityReference>("new_loyaltyprogram") != null
                where
                    m.GetAttributeValue<EntityReference>("adx_administrativecontactid").Id == contact.Id
                select new
                {
                    membershipid = m.Id,
                    EntityImage = "data:image/png;base64," + Convert.ToBase64String(l.GetAttributeValue<byte[]>("entityimage")),
                    LPName = l.GetAttributeValue<string>("new_name"),
                    Tier = t.GetAttributeValue<string>("new_name"),
                    PointBalance = Convert.ToInt32(m.GetAttributeValue<decimal?>("new_pointbalance").GetValueOrDefault(0)).ToString("N2")
                };
            _memberships = EnumerableExtensions.CopyToDataTable(memberships);

            //new_new_loyaltyprogram_adx_membership_LoyaltyProgram
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
            _memberships.Columns["EntityImage"].ColumnName = "Loyalty Image";
            _memberships.Columns["LPName"].ColumnName = "Loyalty Program Name";
            _memberships.Columns["Tier"].ColumnName = "Membership Tier";
            _memberships.Columns["PointBalance"].ColumnName = "Points Balance";

            MembershipListView.DataKeyNames = new[] { "membershipid" };
            MembershipListView.DataSource = _memberships;
            MembershipListView.DataBind();
        }

    }
}