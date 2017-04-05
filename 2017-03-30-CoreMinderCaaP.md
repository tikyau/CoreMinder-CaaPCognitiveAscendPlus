---
layout: post
title: "Extending CRM solution on E-commerce website with Bot Framkework and Cognitive Services"  
author: "William Dam, Delon Yau"
author-link: "#"
#author-image: "{{site.baseurl}}/images/authors/photo.jpg"
date: 2017-03-26
categories: [CaaP and Cognitive Services]
color: "blue"
#image: "{{site.baseurl}}/images/CoreMinderImages/logo.png" #should be ~350px tall
excerpt: Microsoft work with CoreMinder to extend its CRM Ecommerce platform solution wuith CaaP
language: English
verticals: “Reatil, Consumer Products & Services”
---

Customer profile
----------------

Company: CoreMinder
---------------------------------
Location: Hong Kong
-------------------
Description:
------------
[**CoreMinder**](http://www.coreminder.com/chat.html) is Microsoft HK gold partner in providing Dynamic CRM and O365 enterprise & e-commerce integrated solutions to small and medium size companies.  Its Chat management is an add-on solution for retailers, which integrates Dynamics CRM and social app (FaceBook, WeChat). It allows retailers' customers to communicate with the Human Sale Agent and their CRM system at the back-end will capture all these conversation, and therefore the cases can be followed up closely. They see the opportunity to adopt Micrsoft BotFramework, Cognitive Services to provide 24x7 instant customer respond and product recommendation to enhace their Dynamic CRM platfrom solution.

This is the team that was involved with the project:
-   William Dam – Microsoft, Technical Evangelist
-   Delon Yau – Microsoft, Technical Evangelist
-   John Cheng - CoreMinder, Architect
-   Tom Mok - CopreMinder, CTO

Problem statement:
--------------------

**Scenario**

Currently **CoreMinder Chat Management** provide human agent to interact/respond to cusotmer enquires via an ASP.Net web portal which connecting to the backend Dynamic CRM (on-premises).  CoreMinder like to handle the customer queries, FAQ using a ChatBot which directly connect to their Dynamic CRM (on-premises) or in future, Dynamics 365 (online).

From our solution architecture whiteboard discussion, we start with the BOT using BotFramework as our building block and try to leverage the existing CRM Web API (Dynamic CRM's low-level interaction and wrapper methods) running on permises that handle the CRM communication for the Bot/CRM connection, but we will migrate thier CRM Web API to Azure Function to ehance its performance and better dynamic scaling. To preserve their existing close customer engagement experience, the chat bot will use LUIS for natural language experience. And we'll also use QnAmaker for most common FAQ, and Recommendation API for item-to-item recommendation instead of existing human sale agent to recommend based on their CRM total sale records.

Key technologies:
- BotFramework, QnAMaker
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
 2. Create a new case in CRM(e.g. reporting issue, like shipping problem)
 3. Handling most common FAQ using QnAMaker

And we 1st discover few of the FAQ have some overlap of the LUIS intent e.g. order status enquiry.  To avoid the confusion to LUIS, we decide to separate the FAQ from the LUIS bot and emneded in an existing FAQ page instead.

![Here's the simple LUIS]({{site.baseurl}}/images/CoreMinderImages/LUIS.JPG)

**Bot Implementation**

Since its an ecommerce site, instead of using the Sign-In card inside the bot, we will leverage the ecommerce web portal(SSL protected) memeber login procedure than extract the user name from email as an identifier (User ID) for BoT/CRM communcation. And store its into the user data store below before starting the luis dialog, "CustomerDialog".  In production, we'll need to ensure it'll successfully sign on before assign the UserEmail as the userId.

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

Once the user  state been created, we can now start the luis dialog.

Here's the sample code to process the order status enquiry from LUIS intent.
```
public class CustomerDialog : LuisDialog<object>
    {
        [LuisIntent("OrderEnquiry")]
        public async Task OrderEnquiry(IDialogContext context, LuisResult result)
        {     
            //Prompt for missing order number  
            while (result.Entities.Count < 1) {
                new PromptDialog.PromptString("What is the order number?", "Please enter the order number", attempts: 3);
            }
            Debug.WriteLine("Order Number: ", result.Entities[0].Entity);
            await context.PostAsync($"Please wait a moment, we are searching your order '{result.Entities[0].Entity}'");
            
            //Calling the Dynamic CRM AzureFunction to retrieve the order status
            string baseUrl = "https://coremindercrmazfn.azurewebsites.net/api/getOrderStatus?code=IkZbgayre6Bi/RKMW2Itacyb/FZ3wRv3jdzVYqIgTV3GHlQfQo3MPg==";
            string prodid = result.Entities[0].Entity;
            var request = (HttpWebRequest)WebRequest.Create(baseUrl);
            var postData = string.Format("orderid={0}", prodid);
            var data = Encoding.UTF8.GetBytes(postData);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;
            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }
            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            // TODO: Parse the responseString to json object
            RootObject r = JsonConvert.DeserializeObject<RootObject>(responseString);
            Debug.WriteLine("return from CRM......");
            if (r.salesorders.Count != 0)
            {
                var replyToConversation = context.MakeMessage();
                if (r.salesorders[0].new_shippingstatus.ToString() == "Pending")
                {
                    HeroCard replyCard = new HeroCard()
                    {
                        Title = "Order Status",
                        Subtitle = "Status of your order " + result.Entities[0].Entity + " is " + r.salesorders[0].new_shippingstatus.ToString(),
                        Text = "Your order will be shipped soon. Notification email will be sent to you once shipped."
                    };
                    replyToConversation.Attachments.Add(replyCard.ToAttachment());
                } else {.........
```

For the above CRM connection, we did play around logicApp's Dynamic CRM 365 (on-line) conector initially but find that its always use another cached authencicated token if the PC previously authenicated another credential.  Thus its very confusing when we 1st treid to add the CoreMinderCRM URL as its always taken another credential

![LogicApp Logon Fail] ({{site.baseurl}}/images/CoreMinderImages/LogicAppCRMLogOnFailAsUsingDXCRMNoCRMUrlEnterOption.JPG}})

So we decide to use Azure Function and using Dynamic CRM's low-level interaction and wrapper methods instead which can work with both Dynamic 365 on permises and on-line.  But when migrating the code, it take quite an effort to figure out all the nuget package dependencies and also specific version. E.g. you need to add additional "Microsoft.IdentityModel.Clients.ActiveDirectory" nuget package cause our scenario is connecting to on-permises Dynamic 365 using Active Directory as Auth Type which you would not need to if using the XRM dll etc.  During the hack, we try to completely move all the CRM/XRM dlls by calling [**external assemblies reference**](https://docs.microsoft.com/en-us/azure/azure-functions/functions-reference-csharp#referencing-external-assemblies) in Azure Functions but got complaint with "Not Found". so we focus back on resolivng the nuget packages and eventually got all dependencies sort out and aldo help from adding [**TraceListener**](http://crmtipoftheday.com/2017/01/05/tracing-with-xrm-tooling-in-azure-functions/) for the XRM connector logs on Azure Function.

This is the Azure Function that connect to Dynamic CRM to retrive the order status
```
#r "System.Data"

using System.Net;
using Microsoft.Crm.Sdk;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System.Data;
using Newtonsoft.Json;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info("C# HTTP trigger function processed a request.");

    // parse query parameter
    string orderID = req.GetQueryNameValuePairs()
        .FirstOrDefault(q => string.Compare(q.Key, "orderID", true) == 0)
        .Value;

    // Get request body
    dynamic data = await req.Content.ReadAsAsync<object>();

    // Set name to query string or body data
    orderID = orderID ?? data?.orderID;
    string orderStatus = "no order status";

    //fill the crm logic
    string connectionString = "Url=https://coreloy.coreminder.com:10443/coreloy; Domain=bechelon; Username=bechelon'\'john.cheng; Password=bs!9!dga; authtype=AD";
    Dictionary<string, object> result = new Dictionary<string, object>();
    string json = "";
    CrmServiceClient crmSvc = new Microsoft.Xrm.Tooling.Connector.CrmServiceClient(connectionString);
    IOrganizationService _orgService = (IOrganizationService)crmSvc.OrganizationWebProxyClient != null ? (IOrganizationService)crmSvc.OrganizationWebProxyClient : (IOrganizationService)crmSvc.OrganizationServiceProxy;
    if (crmSvc != null & orderID != null & _orgService != null) {
        log.Info("CRM connected.....");
        //get the oder status..........
        orderStatus = "This is the status";
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
        //Response.ContentType = "application/json; charset=utf-8";
        //Response.Write(json);
    }
    
    return orderID == null
        ? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass an orderID in the request body")
        : req.CreateResponse(HttpStatusCode.OK, "Order ID: " + orderID + " Status: " + orderStatus);
}
```

One suggestion we've is perhaps the Azure Function can have a freindly user interface to download the nuget package in C# as currently the way to specify the nuget package in the project.json file is too manual. In additon, if there's Dynamic CRM connector on Azure function compatiable with [**CrmServiceClient**](https://msdn.microsoft.com/en-us/library/dn688177.aspx_would make its much easier.

![screen shoot of the Bot in getting the order status successfulyl from CRM ] ({{site.baseurl}}/images/CoreMinderImages/xxxxxxxxxxxxxxxxxxxxxxxxx.JPG}})

The same logic will be applied for other similar LUIS intent to read from Dynamic CRM and below is the Azure Function sample code that write to the Dynamic CRM by taking the ReportShippingProblems LUIS intent example

```
code insert xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
```

![screen shoot of the Bot creating a new case to CRM ] ({{site.baseurl}}/images/CoreMinderImages/xxxxxxxxxxxxxxxxxxxxxxxxx.JPG}})

Since CoreMnder CRM backend is capable to assigin the Human Agent to follow up the new case created, we're not implementing the bot handoveer in particular the botframewrok don't have any offical api but we found some [**sample code**](https://github.com/tompaana/intermediator-bot-sample) from github, a bit raw but good for reference.  Ideally we like the framework can official support it.

**FAQ Handling**
Since the eCommercise site sample taken from the CoreMinder client has some overlapped of its FAQ and LUIS intent e.g. get order status.  We decide to separate the FAQ that use QnAMaker from the LUIS Bot and and renew the FAQ tab with QnAMaker bot embeded.

![screen shoot of renew FAQ page with QnAMaker ] ({{site.baseurl}}/images/CoreMinderImages/xxxxxxxxxxxxxxxxxxxxxxxxx.JPG}})

**Product recommendation**

So far we've enhanced the ecommerce portal with new Bot channel created and to allow CoreMinder client to up or cross sell more items from the site, we're able to get some real data from their client to generate an training mdoel for item recommendation useing the Cognitive Service's recommendation API portal to create including the shopping cart feature (add, purchase, remove).  Its make more sense to embeded its in to the website instead of the bot

![screen shoot of product recommendation] ({{site.baseurl}}/images/CoreMinderImages/xxxxxxxxxxxxxxxxxxxxxxxxx.JPG}})


.......next in improving the ML using custom based to include time range as feature to refelct latest custoemr behaivor



Conclusion
----------

This combined effort from Microsoft and CoreMinder has delivered the POC
that gave the partner a glance on how Microsoft CaaP, Cognitive Services and Azure Function can
provide easy scaling and integration of thier existing Dynamic CRM solution.

We've also invitied CoreMinder to particptate the MSX event with the early protyotype of this POC and has attract 

CoreMinder is willing to commit to work ith us to explore furhter Machine Learning capability....... 

Tofugear is setting high expectations and committing to work with us
together towards a production launch of the thier client Ecommerrce site. While many of our
frameworks and solutions solve the business or technical problem a
customer has, we’re appreciated the resources and bandwidth the customer
has to maintain, debug, or troubleshoot what we put together.

![CoreMinder team]({{site.baseurl}}/images/)

Here is a quote from the customer:

“Our partnership with Microsoft on this CoreMinder CRM BOT project has
bring us close relationship to work as partner. This new architecture
significantly add intelligent to the CRM solution, while without significant architecture changes. It brings
performance and cost benefits and will definitively leverage our sales
in this segment. This is what the market needs: solutions that add value
while at the same time reducing the complexity of the integration to our
platform would let us more focusing to deliver more customer value and feature delivery.”
