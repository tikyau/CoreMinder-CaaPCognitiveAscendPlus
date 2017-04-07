#r "System.Data"
#load ".\ToolingListener.csx"

using System;
using System.Data;
using System.Net;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Web.Script.Serialization;

using Microsoft.Crm.Sdk;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
//using Microsoft.Xrm.Client;
//using Microsoft.Xrm.Client.Services;
using Microsoft.Xrm.Tooling.Connector;

//using Microsoft.Xrm.Tooling.CrmConnectControl;

using Microsoft.Xrm.Sdk.Deployment;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info("C# HTTP trigger function processed a request.");

    TraceControlSettings.TraceLevel = SourceLevels.All;
    TraceControlSettings.AddTraceListener(new Toolinglistener("Microsoft.Xrm.Tooling.Connector", log));

    // parse query parameter
    string orderID = req.GetQueryNameValuePairs()
        .FirstOrDefault(q => string.Compare(q.Key, "orderID", true) == 0)
        .Value;

    // Get request body
    dynamic data = await req.Content.ReadAsAsync<object>();

    // Set name to query string or body data
    orderID = orderID ?? data?.orderID;
    string orderStatus = "no order status";

    //temporary get rid of the SSL trust connection issue as this CRM using self signed cert....
    System.Net.ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
    //Fill the crm logic
    //CoreMinder Dynamic CRM On-premises with provided user credentials
    //string connectionString = "Url=https://coreloy.coreminder.com:10443/coreloy; Domain=bechelon; Username=john.cheng@bechelon; Password=bs!9!dga; AuthType=AD;";
    string connectionString = System.Environment.GetEnvironmentVariable("CoreMinderCRMConnectionString");
    Dictionary<string, object> result = new Dictionary<string, object>();
    string json = "";
    //CrmServiceClient crmSvc = new Microsoft.Xrm.Tooling.Connector.CrmServiceClient(connectionString);
    CrmServiceClient crmSvc = new CrmServiceClient(connectionString);
    log.Info("crmSvc : " + crmSvc );

    // Cast the proxy client to the IOrganizationService interface.
    IOrganizationService _orgService = (IOrganizationService)crmSvc.OrganizationWebProxyClient != null ? (IOrganizationService)crmSvc.OrganizationWebProxyClient : (IOrganizationService)crmSvc.OrganizationServiceProxy;
    log.Info("_orgService : " + _orgService );

    if (crmSvc != null & orderID != null & _orgService != null) {
        log.Info("CRM connected.....");
        //get the oder status..........
        DataSet dataSet = new DataSet("salesorder");
        DataTable table = new DataTable("salesorders");
        DataColumn col_order_id = new DataColumn("processid");            
        DataColumn col_order_number = new DataColumn("ordernumber");
        DataColumn col_order_status = new DataColumn("new_shippingstatus");
        table.Columns.Add(col_order_id);
        table.Columns.Add(col_order_number);
        table.Columns.Add(col_order_status);
        dataSet.Tables.Add(table);

        QueryExpression qe = new QueryExpression("salesorder");
        qe.ColumnSet = new ColumnSet("ordernumber", "new_shippingstatus", "name");
        qe.Criteria.AddCondition("name", ConditionOperator.Equal, orderID);
        EntityCollection results = _orgService.RetrieveMultiple(qe);

        log.Info("after results.....");

        if (results.Entities != null)
        {
            foreach (Entity order in results.Entities)
            {
                log.Info("Entity.....");
                DataRow newRow = table.NewRow();
                newRow["processid"] = order.Id.ToString();
                newRow["ordernumber"] = order["name"];
                newRow["new_shippingstatus"] = order.FormattedValues["new_shippingstatus"].ToString();
                table.Rows.Add(newRow);
            }
        }
        json = JsonConvert.SerializeObject(dataSet, Formatting.Indented);
        orderStatus = json;
        log.Info("orderStatus: " + orderStatus );
    }
    
    return orderID == null
        ? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass an orderID in the request body")
        : req.CreateResponse(HttpStatusCode.OK, orderStatus);
}