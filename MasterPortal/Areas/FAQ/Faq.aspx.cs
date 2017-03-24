using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Site.Pages;
using System.Net;
using System.Web.Services;

namespace Site.Areas.FAQ
{
    public partial class Faq : PortalPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
            
        }

        [WebMethod]
        public static string Name()
        {
            string Name = "Hello Rohatash Kumar";
            return Name;
        }

        public string RequestAnswer(string query) {

            string responseString = string.Empty;
            //var query = "adx"; //User Query
            var knowledgebaseId = "edd7b405-c9c1-4f63-89c6-f32d8699ea8b"; // Use knowledge base id created.
            var qnamakerSubscriptionKey = "bf3ea99d19864402aa6e826892391d54"; //Use subscription key assigned to you.

            //Build the URI
            Uri qnamakerUriBase = new Uri("https://westus.api.cognitive.microsoft.com/qnamaker/v1.0");
            var builder = new UriBuilder($"{qnamakerUriBase}/knowledgebases/{knowledgebaseId}/generateAnswer");

            //Add the question as part of the body
            var postBody = $"{{\"question\": \"{query}\"}}";

            //Send the POST request
            using (WebClient client = new WebClient())
            {
                //Set the encoding to UTF8
                client.Encoding = System.Text.Encoding.UTF8;

                //Add the subscription key header
                client.Headers.Add("Ocp-Apim-Subscription-Key", qnamakerSubscriptionKey);
                client.Headers.Add("Content-Type", "application/json");
                responseString = client.UploadString(builder.Uri, postBody);
            }

            return responseString;

        }


    }
}