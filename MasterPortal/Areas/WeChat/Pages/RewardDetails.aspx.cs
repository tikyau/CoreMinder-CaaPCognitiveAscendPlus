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


namespace Site.Areas.WeChat.Pages
{
    public partial class RewardDetails : PortalPage
    {
        OrganizationService service;        

        protected void Page_Load(object sender, EventArgs e)
        {
            RedirectToLoginIfAnonymous();
            RedirectToLoginIfNecessary();

            service = new OrganizationService("Xrm");
            Guid id;
            if (Guid.TryParse(Request.QueryString["id"], out id))
            {
                XrmContext.CreateQuery("new_gift").FirstOrDefault(c => c.GetAttributeValue<Guid>("new_giftid") == id);
            }

            Guid giftGuid = id;
            QueryExpression qe = new QueryExpression("annotation");
            qe.ColumnSet.AllColumns = true;
            qe.Criteria.AddCondition("objectid", ConditionOperator.Equal, giftGuid);
            qe.ColumnSet.AddColumn("documentbody");
            EntityCollection ec = service.RetrieveMultiple(qe);
            if (ec.Entities.Count > 0)
            {
                byte[] fil = Convert.FromBase64String(ec.Entities[0].Attributes["documentbody"].ToString());
                string base64String = Convert.ToBase64String(fil, 0, fil.Length);
                giftimage.ImageUrl = "data:image/jpeg;base64," + base64String;
            }


        }

    }
}