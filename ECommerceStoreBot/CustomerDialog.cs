using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace ECommerceStoreBot
{
    [Serializable]
    //[LuisModel("YourModelId", "YourSubscriptionKey")]
    //LUIS APP: CoreMinderCRMBot
    [LuisModel("00ad773e-2078-4b60-8e73-94aed34ee616", "f5a1c2fc5ea0407ba30aed8e2e27187c")]
    public class CustomerDialog : LuisDialog<object>
    {
        //override the StartAsync and make this to the root of the dialog
        public override async Task StartAsync(IDialogContext context)
        {
            //read the userData from context
            Debug.WriteLine("Channel: " + context.Activity.ChannelId + "\nUserID: " + context.UserData.Get<string>("userId") + "\nuserName: " + context.UserData.Get<string>("userName") + " \nuserMessage: " + context.UserData.Get<string>("userMessage"));
          
            //1st Bot message to user
            var welcomeMessage = context.MakeMessage();
            welcomeMessage.Text = context.UserData.Get<string>("userName") + " May I help you? \n\r 1. Raise an enquiry ticket \n\r 2. Check Order Status";
            await context.PostAsync(welcomeMessage);
            context.Wait((this.MessageReceived));
        }

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
            /*E.g. www.coreminder.com:9998/getorderstatus?orderid=1701 will return
            "salesorders": [
                {
                    "processid": "2ee86396-c8ff-e611-80ca-00155d120a00",
                    "ordernumber": "1701",
                    "new_shippingstatus": "Pending"
                }
            ]*/
            Debug.WriteLine("checking CRM.......");
            //
            //replacing the existing CRM web api to AzFn call...........
            //
            //string baseUrl = "http://www.coreminder.com:9998/getorderstatus";
            string baseUrl = "https://coremindercrmazfn.azurewebsites.net/api/getOrderStatus?code=lNsp7nd1cjVEdIFy4Qx5afLJGYKzo4G6OVHagTnpZ4F83OeV9OElJQ==";
            string prodid = result.Entities[0].Entity;
            var request = (HttpWebRequest)WebRequest.Create(baseUrl);
            //change to json format
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

        [LuisIntent("Help")]
        public async Task Help(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Hi! Try asking me things like 'check order status' or 'item checking'");

            context.Wait(this.MessageReceived);
        }

        [LuisIntent("None")]

        public async Task NoneHandler(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("I'm sorry, I don't understand");
            //pass to human agent?...........
            context.Wait(MessageReceived);

        }

    }
}