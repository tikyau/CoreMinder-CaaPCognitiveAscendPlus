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
using static System.Net.Mime.MediaTypeNames;

namespace ECommerceStoreBot
{
    [Serializable]
    //[LuisModel("YourModelId", "YourSubscriptionKey")]
    //redwolfdemo
    [LuisModel("00ad773e-2078-4b60-8e73-94aed34ee616", "f5a1c2fc5ea0407ba30aed8e2e27187c")]
    public class CustomerDialog : LuisDialog<object>
    {
        [LuisIntent("OrderEnquiry")]
        //public async Task OrderEnquiry(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        public async Task OrderEnquiry(IDialogContext context, LuisResult result)
        {
            //var message = await activity;
            //Activity replyToConversation = message.CreateReply("Should go to conversation, with a hero card");
 
            while (result.Entities.Count < 1) {
                new PromptDialog.PromptString("What is the order number?", "Please enter the order number", attempts: 3);
            }
            Debug.WriteLine(result.Entities[0]);
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
            //add wait in order to continue dialog for HeroCard reply below! github.com/Microsoft/BotBuilder/issues/602
            await context.PostAsync(responseString);

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
                        Subtitle = "Status of your order {result.Entities[0].Entity} is {r.salesorders[0].new_shippingstatus.ToString()}.",
                        Text = "Your order will be shipped soon. Notification email will be sent to you once shipped."
                    };
                    replyToConversation.Attachments.Add(replyCard.ToAttachment());
                }
                else
                {
                    HeroCard replyCard = new HeroCard()
                    {
                        Title = "Order Status",
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
            context.Wait(MessageReceived);
            /*var message2 = context.MakeMessage();
            message2.Text = "Hot Items";
            message2.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            prodid = "AS-APRON";
            message2.Attachments.Add(GetHeroCard(prodid, new CardImage(url: "https://rfpicdemo.blob.core.windows.net/rfdemo/AS-APRON-1L.jpg")));
            prodid = "TMC2565-MC-M";
            message2.Attachments.Add(GetHeroCard(prodid, new CardImage(url: "https://rfpicdemo.blob.core.windows.net/rfdemo/TMC2565-MC-M-1L.jpg")));
            prodid = "AEG030-TM";
            message2.Attachments.Add(GetHeroCard(prodid, new CardImage(url: "https://rfpicdemo.blob.core.windows.net/rfdemo/AEG030-TM-1L.jpg")));
            prodid = "GP-AEG081";
            message2.Attachments.Add(GetHeroCard(prodid, new CardImage(url: "https://rfpicdemo.blob.core.windows.net/rfdemo/GP-AEG081-1L.jpg")));
            await context.PostAsync(message2);
            context.Wait(this.MessageReceived);
            */
            var message3 = context.MakeMessage();
            message3.Text = $"May I help you? \n\r 1. Item checking \n\r 2. Check Order Status";
            await context.PostAsync(message3);
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