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
[**CoreMinder**](http://www.coreminder.com/chat.html) Core Minder is Microsoft HK gold partner in providing CRM and O365 enterprise & e-commerce integrated solutions to small and medium size companies.  Its Chat management is an add-on solution for retailers, which integrates Dynamics CRM and social app (FaceBook, WeChat). It allows retailers' customers to communicate with the Human Sale Agent and their CRM system at the back-end will capture all these conversation, and therefore the cases can be followed up closely. They see the opportunity to
adopt Micrsoft BotFramework, Cognitive Services embeded into webchat and Skype channel to provide instant customer query and product recommendation.

This is the team that was involved with the project:
-   William Dam – Microsoft, Technical Evangelist
-   Delon Yau – Microsoft, Technical Evangelist
-   John Cheng - CoreMinder, Architect
-   Tom Mok - CopreMinder, CTO

Problem statement:
--------------------

**Scenario**

Currently **CoreMinder Chat Management** The webchat solution can track all converstaions in Dynamic CRM from social app but still relying on human agent to interact with/respond to cusotmer via an ASP.Net web portal which conecting to the CRM.  Thus Coreminder like to automate the FAQ, product queries using a ChatBot to provide better and instant respond to customers and also product recommendation to their ecommerce web store partners.

From our solution architecture whiteboard discussion, we selected the C# BotFramework to build the bot and leverage the existing CRM web api(ASP.net) for the Bot/CRM connection, but will modify a bit to improve the exisiting CRM web api performance which host on permise by move its to Azure Function for better dynamic scaling. To preserve their existing close customer engagement experience, the chat bot will use LUIS for natural language experience.  And we'll also use QnAmaker for most common FAQ and Recommendation API for item-to-item recommendation instead of existing Human Sale agent to recommend based on their CRM total sale records.

Key technologies:
- BotFramework, QnAMaker
- Cognitive Services: LUIS, Recommendation API
- Web Service: ASP.net, Azure Function
- Dynamic CRM 365
- Client: webchat, Skype

![Whiteboard Architecture]({{site.baseurl}}/images/CoreMinderImages/Whiteboard_Coreminder.JPG)

Solutions, steps and delivery
-------------------------------

**LUIS - Dialog design**

We start with defining the LUIS intents and breaking into 3 categories:
 1. Return the result from CRM : taking the order status as example
 2. Create a new case in CRM: e.g. reporting issue, like shipping problem
 3. Handling most common FAQ using QnAMaker

And we 1st discover one of FAQ we use above has some overlap of the LUIS intent e.g. order status enquiry.  To avoid the confusion to LUIS, we decide to separate the FAQ from the LUIS bot and emneded in an existing FAQ page instead.

![Here's the simple LUIS]({{site.baseurl}}/images/CoreMinderImages/LUIS.JPG)

**Bot Implementation**

C# code that handle the customer order status enquiry from LUIS
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
            //replacing with AzFn call...........
            string baseUrl = "http://www.coreminder.com:9998/getorderstatus";
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

The BotFramework coding part is startight forward, and if we can have more example of how to preserve the user data in C# woud help as we later realize the BotData can be extracted from the IDialogContext.
And to continue teh CRM connection, we did explore logicApp's Dynamic CRM 635 initially but find that its always use another cached authencicated token if the PC previously authenicated another credential.  Thus its very confusing when we 1st tred to add the CoreMinderCRM URL as its always taken another credential

![LogicApp Logon Fail] ({{site.baseurl}}/images/CoreMinderImages/LogicAppCRMLogOnFailAsUsingDXCRMNoCRMUrlEnterOption.JPG}})

So we decide to use Azure Function and  using CRM and XRM sdk instead whic is more generic and can work on both Dynamic 365 on perm and on cloud.

This is tha Azure Function that connect to Dynamic CRM to retrive the order status
```
using System.Net;
using Microsoft.Crm.Sdk;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;

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

    //CrmServiceClient conn = new Microsoft.Xrm.Tooling.Connector.CrmServiceClient("https://coredemo.crm5.dynamics.com/"); 
    //fill the crm logic
    CrmServiceClient crmSvc = new Microsoft.Xrm.Tooling.Connector.CrmServiceClient("Url=https://coredemo.crm5.dynamics.com/; UserName=cms.ws12@coreminder.com; Password=P@ssw0rd1234");
    if (crmSvc != null & orderID != null) {
        log.Info("CRM connected.....");
        //get the oder status..........
        orderStatus = "This is the status";
    }

    return orderID == null
        ? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass an orderID in the request body")
        : req.CreateResponse(HttpStatusCode.OK, "Order ID: " + orderID + " Status: " + orderStatus);
}
```
One suggestion we've is perhaps the Azure Function can have a freindly user interface to download the nuget package in C# as currently the way to specify the nuget package in the project.json file is too mamual.

And .............CRM write case & QnAMaker


**Product recommendation**

Next, we move to the product recommendation and we simply use the Cognitive Service's recommendation API portal to create an item recommendation model with based on the shopping cart feature (add, purchase, remove) thus its make more sense to embeded its in to the website instead of the bot

.......next in improving the ML using custom based to include time range as feature to refelct latest custoemr behaivor

????????
-------------------


Conclusion
----------

This combined effort from Microsoft and CoreMinder has delivered the POC
that gave the partner a glance on how Microsoft CaaP, Cognitive Services and Azure Function can
provide easy scaling and integration thier existing Dynamic CRM solution.

We accomplished our goal of making a CRM BOT to help the CRM Agent to handle majority the cusotmer request with this POC. 
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
