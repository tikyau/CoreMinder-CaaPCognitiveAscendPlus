using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System.Diagnostics;

namespace ECommerceStoreBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        private StateClient stateClient;
        public BotData userData;

        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            //persist botstate here..https://docs.botframework.com/en-us/csharp/builder/sdkreference/stateapi.html
            //initialize the user data store
            stateClient = activity.GetStateClient();
            userData = await stateClient.BotState.GetUserDataAsync(activity.ChannelId, activity.From.Id);
            //
            //Tom, Delon: If we can get user sign in from web, we can replace the user ID/Name below from the activitiy
            //
            userData.SetProperty("userId", activity.From.Id);
            userData.SetProperty("userName", activity.From.Name);
            userData.SetProperty("userChannel", activity.ChannelId);
            userData.SetProperty("userMessage", activity.Text);
            await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
            //Debug.WriteLine("Channel: " + activity.ChannelId + "UserID :" + activity.From.Id + "userData :" + userData.Data.ToString());

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

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }
            return null;
        }
    }
}