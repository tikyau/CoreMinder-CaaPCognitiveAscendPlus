using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECommerceStoreBot
{
    class Mail
    {
        public string From { get; set; }
        public string To { get; set; }
      

    }
    /// <summary>
    /// Summary description for MailHandler1
    /// </summary>
    public class MailHandler1 : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string jsonString = String.Empty;
            HttpContext.Current.Request.InputStream.Position = 0;
            using (System.IO.StreamReader inputStream =
            new System.IO.StreamReader(HttpContext.Current.Request.InputStream))
            {
                jsonString = inputStream.ReadToEnd();
                System.Web.Script.Serialization.JavaScriptSerializer jSerialize =
                    new System.Web.Script.Serialization.JavaScriptSerializer();
                var email = jSerialize.Deserialize<Mail>(jsonString);

                if (email != null)
                {
                    string from = email.From;
                    MessagesController.UserEmail = email.From;
                    string to = email.To;
                    MessagesController.password = email.To;
                    //You can write here the code to send Email, 
                    //see ,the Class System.Net.Mail.MailMessage on MSDN
                    //Once the Mail is sent succefully, you can send back 
                    //a response to the Client informing him that everything is okay !
                    context.Response.Write(jSerialize.Serialize(
                         new
                         {
                             Response = "Message Has been sent successfully"
                         }));
                }
            }

            
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}