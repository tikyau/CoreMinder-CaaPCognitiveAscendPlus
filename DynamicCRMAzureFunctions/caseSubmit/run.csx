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

using Microsoft.Xrm.Tooling.Connector;

using Microsoft.Xrm.Sdk.Deployment;
//using Microsoft.IdentityModel.Clients.ActiveDirectory;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info("C# HTTP trigger function processed a request.");

    TraceControlSettings.TraceLevel = SourceLevels.All;
    TraceControlSettings.AddTraceListener(new Toolinglistener("Microsoft.Xrm.Tooling.Connector", log));

    // parse query parameter
    string queryParameter = req.GetQueryNameValuePairs()
        .FirstOrDefault(q => string.Compare(q.Key, "userID", true) == 0)
        .Value;

    // Get request body
    dynamic data = await req.Content.ReadAsAsync<object>();

    // Set name to query string or body data
    queryParameter  = queryParameter  ?? data?.queryParameter ;

    //temporary get rid of the SSL trust connection issue as this CRM using self signed cert....
    System.Net.ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
    
    //Fill the crm logic
    string userID = data.userID;
    string userDescription = data.userDescription;
    bool caseCreated = false;

    //CoreMinder Dynamic CRM On-premises with provided user credentials
    //string connectionString = "Url=https://coreloy.coreminder.com:10443/coreloy; Domain=bechelon; Username=john.cheng@bechelon; Password=bs!9!dga; AuthType=AD;";
    string connectionString = System.Environment.GetEnvironmentVariable("CoreMinderCRMConnectionString");
    Dictionary<string, object> result = new Dictionary<string, object>();

    CrmServiceClient crmSvc = new CrmServiceClient(connectionString);
    log.Info("crmSvc: " + crmSvc );

    // Cast the proxy client to the IOrganizationService interface.
    IOrganizationService _orgService = (IOrganizationService)crmSvc.OrganizationWebProxyClient != null ? (IOrganizationService)crmSvc.OrganizationWebProxyClient : (IOrganizationService)crmSvc.OrganizationServiceProxy;
    log.Info("_orgService: " + _orgService );

    log.Info("log: " + userID + userDescription);
    if (crmSvc != null & userID != null & userDescription != null & _orgService != null) {
        log.Info("CRM connected.....submitting case");
        //submitting case..........
        QueryExpression qe = new QueryExpression("contact");
        qe.Criteria.AddCondition("adx_identity_username", ConditionOperator.Equal, userID);
        qe.ColumnSet = new ColumnSet(new string[] { "fullname" });
        EntityCollection ec = _orgService.RetrieveMultiple(qe);
        EntityReference _contact_ref;
        if (ec.Entities.Count == 1)
        {
            Entity _contact = ec.Entities[0];
            _contact_ref = new EntityReference(_contact.LogicalName, _contact.Id);
            Entity _case = new Entity("incident");
            _case["customerid"] = _contact_ref;
            _case["description"] = userDescription;
            _case["title"] = "Shipping Problem";
            _case["caseorigincode"] = new OptionSetValue(3); // Web
            //_case["new_createdbyportalparsonsmusicenquiry"] = true;
            Guid _case_id = _orgService.Create(_case);
            caseCreated = true;
        }
    }
    else
    {
        var display = "Invalid information";
        /////csm.RegisterClientScriptBlock(this.GetType(), "Pop", "alert('" + display + "');", true);
    }
    
    //return userID == null
    return caseCreated == false
        ? req.CreateResponse(HttpStatusCode.BadRequest, "Case submitted Fail - Invalid Info")
        : req.CreateResponse(HttpStatusCode.OK, "Case submitted successfully - " + userDescription);
}