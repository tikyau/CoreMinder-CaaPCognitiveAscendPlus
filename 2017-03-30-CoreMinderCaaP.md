---
layout: post
title: "Extending CRM solution on E-commerce website with Bot Framework and Cognitive Services"  
author: "William Dam, Delon Yau"
author-link: "#"
#author-image: "{{site.baseurl}}/images/authors/photo.jpg"
date: 2017-03-26
categories: [CaaP and Cognitive Services]
color: "blue"
#image: "{{site.baseurl}}/images/CoreMinderImages/logo.png" #should be ~350px tall
excerpt: Microsoft work with CoreMinder to extend its CRM Ecommerce platform solution with CaaP
language: English
verticals: “Retail, Consumer Products & Services”
---

Customer profile
----------------

Company: CoreMinder
---------------------------------
Location: Hong Kong
-------------------
Description:
------------
[**CoreMinder**](http://www.coreminder.com/chat.html) is Microsoft HK gold partner in providing Dynamic CRM and O365 enterprise & e-commerce integrated solutions to small and medium size companies.  Its Chat management is an add-on solution for retailers, which integrates Dynamics CRM and social app (Facebook, WeChat). It allows retailers' customers to communicate with the Human Sale Agent and their CRM system at the back-end, and therefore the cases can be followed up closely. They see the opportunity to adopt Microsoft Bot Framework, Cognitive Services to provide 24x7 instant customer respond and product recommendation to enhance their Dynamic CRM platform value.

This is the team that was involved with the project:
-   William Dam – Microsoft, Technical Evangelist
-   Delon Yau – Microsoft, Technical Evangelist
-   John Cheng - CoreMinder, Architect
-   Tom Mok - CoreMinder, CTO

Problem statement:
--------------------

**Scenario**

Currently **CoreMinder Chat Management** allow human agent to interact/respond to customer enquires raised from the social channels which connected with an CRM Web Gateway (ASP.Net) to a backend Dynamic CRM (both are on-premises in their typical client deployment).  CoreMinder like to improve their client Sale Agent efficiency by handling majority of the customer queries, FAQ using a Chatbot instead of the Human Agent to review every case into the CRM portal.  The Bot deployment will need to be directly connected to their Dynamic CRM (on-premises) or Dynamics 365 (online) in future.

From our solution architecture whiteboard discussion, we start with the BOT using Bot Framework as our building block and try to leverage the existing CRM Web API (Dynamic CRM's low-level interaction and wrapper methods) running on premises that handle the CRM communication for the Bot/CRM connection, but we will migrate their CRM Web API to Azure Function to enhance its performance and better dynamic scaling. To preserve their existing close customer engagement experience, the chat bot will use LUIS for natural language experience. And we'll also use QnAmaker for most common FAQ, and Recommendation API for item-to-item recommendation instead of existing human sale agent to recommend based on their CRM total sale records.

Key technologies:
- Bot Framework, QnAMaker
- Cognitive Services: LUIS, Recommendation API
- Web Service: Azure Function, ASP.Net
- Dynamic CRM 
- Client: webchat, Skype

![Whiteboard Architecture]({{site.baseurl}}/images/CoreMinderImages/Whiteboard_Coreminder.JPG)

Solutions, steps and delivery
-------------------------------

**LUIS - Dialog design**

We start with defining the LUIS intents and breaking into 3 categories:
 1. Retrieve data from CRM (e.g. order status check)
 2. Create a new case in CRM (e.g. reporting issue, like shipping problem)
 3. Handling most common FAQ using QnAMaker

And we 1st discover few of the FAQ have some overlapped of the LUIS intent e.g. order status enquiry.  To avoid the confusion to LUIS, we decide to separate the FAQ from the LUIS bot and embedded in an existing FAQ page instead.

![Here's the simple LUIS]({{site.baseurl}}/images/CoreMinderImages/LUIS.JPG)

**Bot Implementation**

Since it’s an ecommerce site, instead of using the Sign-In card inside the bot, we will leverage the ecommerce web portal (SSL protected) member login procedure than extract the user name from email logon as an identifier (User ID) for Bot/CRM communication. And store it into the user data store below before starting the Luis dialog, "CustomerDialog".  In production, we'll need to ensure it'll successfully sign on before assigning the UserEmail as the userId

This is the sample code is storing the bot state with user email as the user identifier.
```
public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            //initialize the user data store
            stateClient = activity.GetStateClient();

            userData = await stateClient.BotState.GetUserDataAsync(activity.ChannelId, activity.From.Id);
            userData.SetProperty("userId", UserEmail);
            userData.SetProperty("userChannel", activity.ChannelId);
            userData.SetProperty("userMessage", activity.Text);
            
            await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);    

            if (activity.Type == ActivityTypes.Message)
            {
                await Microsoft.Bot.Builder.Dialogs.Conversation.SendAsync(activity, () => new CustomerDialog());
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }
```

![Here's the POC website sign on page]({{site.baseurl}}/images/CoreMinderImages/useLogOn.JPG)

Once the user state been created, we can now start the Luis dialog.

Here's the sample code to process the order status enquiry from LUIS intent.
```
 [LuisIntent("OrderEnquiry")]
        //public async Task OrderEnquiry(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        public async Task OrderEnquiry(IDialogContext context, LuisResult result)
        {
            //Prompt for missing order number  
            while (result.Entities.Count < 1) {
                new PromptDialog.PromptString("What is the order number?", "Please enter the order number", attempts: 3);
            }
            Debug.WriteLine("Order Number: " + result.Entities[0].Entity);
            await context.PostAsync($"Please wait a moment, we are searching your order '{result.Entities[0].Entity}'");
            Debug.WriteLine("checking CRM.......");
            //
            //replacing the existing CRM web api to AzFn call...........
            string baseUrl = "https://coremindercrmazfn.azurewebsites.net/api/getOrderStatus?code=lNsp7nd1cjVEdIFy4Qx5afLJGYKzo4G6OVHagTnpZ4F83OeV9OElJQ==";
            string prodid = result.Entities[0].Entity;
            var request = (HttpWebRequest)WebRequest.Create(baseUrl);
            request.ContentType = "application/json";
            request.Method = "POST";
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                string json = new JavaScriptSerializer().Serialize(new
                {
                    orderID = prodid
                });
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }
            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            // TODO: Parse the responseString to json object
            RootObject r = JsonConvert.DeserializeObject<RootObject>(responseString);
            Debug.WriteLine("return from CRM......");
            if (r.salesorders.Count != 0)
            {
                var replyToConversation = context.MakeMessage();
                List<CardImage> cardImages = new List<CardImage>();
                //the products associated with the order number return from CRM........
                cardImages.Add(new CardImage(url: "https://rfpicdemo.blob.core.windows.net/rfdemo/AS-APRON-1L.jpg"));
                if (r.salesorders[0].new_shippingstatus.ToString() == "Pending")
                {
                    HeroCard replyCard = new HeroCard()
                    {
                        Title = "Order Status",
                        Images = cardImages,
                        Subtitle = "Status of your order " + result.Entities[0].Entity + " is " + r.salesorders[0].new_shippingstatus.ToString(),
                        Text = "Your order will be shipped soon. Notification email will be sent to you once shipped."
                    };
                    replyToConversation.Attachments.Add(replyCard.ToAttachment());
                }
                else
                {
                    HeroCard replyCard = new HeroCard()
                    {
                        Title = "Order Status",
                        Images = cardImages,
                        Subtitle = $"Status of your order {result.Entities[0].Entity} is {r.salesorders[0].new_shippingstatus.ToString()}.",
                        Text = "Your order is shipped. Please check your email."
                    };
                    replyToConversation.Attachments.Add(replyCard.ToAttachment());
                }
                await context.PostAsync(replyToConversation);
            }
            else
            {
                var message1 = context.MakeMessage();
                message1.Text = "Cannot find order";
                await context.PostAsync(message1);
            }
        }
```

For the above CRM connection, we did play around logicApp's Dynamic CRM 365 (on-line) connector initially but find that logicApp always use another cached authenticated token if the PC have another credential.  It’s very confusing when we 1st tried to add the CoreMinder CRM URL as its always taken another credential.

![This is what we see from LogicApp logon fail] ({{site.baseurl}}/images/CoreMinderImages/LogicAppCRMLogOnFailAsUsingDXCRMNoCRMUrlEnterOption.JPG}})

Anyway, we decided to use Azure Function with Dynamic CRM's low-level interaction and wrapper methods instead as its can work with both Dynamic 365 on premises and on-line.  But when migrating the XRM SDK code, it takes quite an effort to figure out all the correct nuget package dependencies and specific version to get its compiled. During the hack, there're few times that we experience Azure Function are cached no matter what changes putting in but will resume normal sometime after?  This is another reason it takes us longer time to debug/change.

Alternatively, we did also try to completely move all the CRM/XRM dlls by [**referencing external assemblies **](https://docs.microsoft.com/en-us/azure/azure-functions/functions-reference-csharp#referencing-external-assemblies) in Azure Functions but got complaint with the specific external assemblies "Not Found", likely not able to figure out the path.  So, we focus back on resolving the nuget packages and eventually got all dependencies sort out.

One recommendation to CRM developer if working on Azure Function, you might find adding this [**TraceListener**](http://crmtipoftheday.com/2017/01/05/tracing-with-xrm-tooling-in-azure-functions/) helpful as we use to capture XRM connector logs on Azure Function to help troubleshooting.

In addition, on April 5th I also observed the Azure Function URL will return "404" as its somehow not able to take the default Host Keys?  But if replacing the master host key will get it work! And the problem goes away next day.  We like to feed this back to product team and hope continue to improve the quality of its delivery.

This is the Azure Function that connect to Dynamic CRM (on-premises) to retrieve the order status
```
#r "System.Data"
#load ".\ToolingListener.csx"

using System;
using System.Data;
using System.Net;
using System.Diagnostics;
using Newtonsoft.Json;

using Microsoft.Crm.Sdk;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;

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

    //temporary get rid of the SSL trust connection issue due to this CRM using self signed cert....
    System.Net.ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

    //Fill the crm logic
    //CoreMinder Dynamic CRM On-premises with provided user credentials
    string connectionString = System.Environment.GetEnvironmentVariable("CoreMinderCRMConnectionString");
    Dictionary<string, object> result = new Dictionary<string, object>();
    string json = "";
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
```

One suggestion we've is perhaps the Azure Function can have a friendly user interface to download the nuget package as currently the way to find the nuget package and add to the project.json file is too manual. In addition, if there's Dynamic CRM built-in connector on Azure function equivalent with [**CrmServiceClient**](https://msdn.microsoft.com/en-us/library/dn688177.aspx_would will make it much easier.

![screen shoot of the Bot in getting the order status successfully from CRM ] ({{site.baseurl}}/images/CoreMinderImages/orderNumberChecking.JPG}})

The same logic will be applied for other similar LUIS intent to read from Dynamic CRM and below is the Azure Function sample code that write to the Dynamic CRM by taking the ReportShippingProblems LUIS intent example

This is the Azure Function to create a new case in Dynamic CRM
```
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
            Guid _case_id = _orgService.Create(_case);
            caseCreated = true;
        }
    }
    else
    {
        var display = "Invalid information";
        //will be handle by the HttpStatusCode.BadRequest
    }
    
    return caseCreated == false
        ? req.CreateResponse(HttpStatusCode.BadRequest, "Case submitted Fail - Invalid Info")
        : req.CreateResponse(HttpStatusCode.OK, "Case submitted successfully - " + userDescription);
}
```

And this is the Bot code to handle the write to CRM
```
private async Task ResumeAfterPrompt(IDialogContext context, IAwaitable<string> result)
        {
            Debug.WriteLine("1 ");
            var parameter = await result;
            Debug.WriteLine("2 ");
            string problemDescription = parameter;
            string userID = context.UserData.Get<string>("userId");
   
            await context.PostAsync("creating case in CRM with description: " + problemDescription);
            Debug.WriteLine("Recording to CRM.......");

            //Fill the CRM logic here instead
            string baseUrl = "https://coremindercrmazfn.azurewebsites.net/api/caseSubmit?code=AQbsQ9L7OH42Uj3boEcsjpHqJKV6mCbseK1RD1NS56CoCFrGz98EFw==";
            var request = (HttpWebRequest)WebRequest.Create(baseUrl);
            //change to json format
            request.ContentType = "application/json";
            request.Method = "POST";
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                string json = new JavaScriptSerializer().Serialize(new
                {
                    userID = userID,
                    userDescription = problemDescription
                });
                //now should clear the problemDescription for next use
                newProblemDescription = null;
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }
            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                await context.PostAsync("Case Submitted, our Agent will contact you shortly");
            }
            else
            {
                await context.PostAsync("Case Submitted failed, Invalid user profile or info, please submit again");
            }
        }

        [LuisIntent("ReportShippingProblems")]
        public async Task ReportShippingProblems(IDialogContext context, LuisResult result)
        {

            //get the user description
            if (result.Entities.Count == 0)
            {
                var dialog = new PromptDialog.PromptString("Please provide your description: ", "please try again", 2);
                context.Call(dialog, ResumeAfterPrompt);
            }
            else
            {
                context.Wait(MessageReceived);
            }
     
        }
```
This shipping problem LUIS has no entity defined and we just need to prompt the user to provide the problem description to open the case in CRM.  We've some challenge to get this prompt dialog work inside LUIS, as most often we got compliant on the IAwaitable<string> null reference in runtime if not handling properly.  In addition, the line context.call(dialog, ResumeAfterPrompt) would not wait for user input and will continue to run the code, so we move the CRM logic inside the ResumeAfterPrompt.  In general, we like to have better api support in [**taking simple user input**](https://github.com/Microsoft/BotBuilder/issues/428)

![Screen shoot of the Bot creating a new case to CRM ] ({{site.baseurl}}/images/CoreMinderImages/shippingProblemCaseRaise.JPG}})

Since CoreMinder CRM backend is capable to assign the Human Agent to follow up the new case created, we're not implementing the bot handover the Bot Framework don't have any official api but we found some [**sample code**](https://github.com/tompaana/intermediator-bot-sample) from GitHub, a bit raw but good for reference.  Ideally, we like the framework can official support it.

**FAQ Handling**
Due to overlapping of this client sample website FAQ and LUIS intent e.g. get order status.  We decide to separate the FAQ that use QnAMaker from the LUIS Bot and renew the FAQ tab with QnAMaker bot embedded.

![screen shoot of renew FAQ page with QnAMaker ] ({{site.baseurl}}/images/CoreMinderImages/QnA_Portal.JPG}})
![screen shoot of renew FAQ page with QnAMaker ] ({{site.baseurl}}/images/CoreMinderImages/QnA_QnA_Azure.JPG}})
![screen shoot of renew FAQ page with QnAMaker ] ({{site.baseurl}}/images/CoreMinderImages/QnA_Azure_TestEnv.JPG}})

**Product recommendation**

So far, we've enhanced the ecommerce portal with new Bot channel created and to enhance the portal netter selling to end customer, we're able to get some sample data from their client to generate a training model for item recommendation using the Cognitive Service's recommendation API portal.  These data include the shopping cart feature (add, purchase, remove).  Its make more sense to embedded its in to the website instead of the bot

![screen shoot of product recommendation] ({{site.baseurl}}/images/CoreMinderImages/ProdRecommendation.JPG}})
![screen shoot of product recommendation] ({{site.baseurl}}/images/CoreMinderImages/ML_AzureSiteView.JPG}})


Conclusion
----------

This combined effort from Microsoft and CoreMinder has delivered the POC
that gave the partner a glance on how Microsoft CaaP, Cognitive Services and Azure Function can enrich their platform solution with easy scaling and integration of their existing on premises Dynamic CRM platform.

We've also invited CoreMinder to participate the MSX event with the early prototype of this POC and has attracted 12% of sale lead which rank at the 3rd among other solution show case.

![CoreMinder team]({{site.baseurl}}/images/CoreMinderImages/MSXEvent.jpg)

CoreMinder is willing to commit to work it us to explore further Machine Learning capability and prepare to move their existing customer portal revamp and moving to hybrid cloud model instead on purely on premises deployment.

![CoreMinder team]({{site.baseurl}}/images/CoreMinderImages/team.JPG)

Here is a quote from the customer:

“Our partnership with Microsoft on this CoreMinder CRM BOT project has
bring us close relationship to work as partner. This new architecture
significantly add intelligent to the CRM solution, while without significant architecture changes to our CRM platform. It brings
performance and cost benefits to our client sale support and will definitively leverage more the Cognitive Service and Machine Learning capabilities in our upcoming client projects. This is what the market needs: solutions that add value while at the same time reducing the complexity of the integration to us
platform would let us more focusing to deliver more customer value and feature delivery.”
