using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web.Mvc;

namespace Site.Areas.Social.Controllers
{
	public class TwitterController : Controller
	{
		//
		// GET: /Social/Twitter/

		public ActionResult Index()
		{
			return null;
		}

		[OutputCache(Duration = 30, VaryByParam = "query")]
		public ActionResult Search(string query)
		{
			//https://dev.twitter.com/docs/api/1.1/get/search/tweets
			//https://dev.twitter.com/docs/auth/application-only-auth
			//https://dev.twitter.com/docs/platform-objects/tweets

			const string oauthConsumerKey = "42fQDJ2i3s9hIlXYPFWnfQ";
			const string oauthConsumerSecret = "eTTxSDEo8syn1gpWlUZSMGKzM8TzWzWYLUTqvR7W5A";
			var url = "https://api.twitter.com/1.1/search/tweets.json?q=" + Uri.EscapeDataString(query);
			
			var accessToken = GetAccessToken(oauthConsumerKey, oauthConsumerSecret);
			
			var request = (HttpWebRequest) WebRequest.Create(url);
			request.Headers.Add("Authorization", "Bearer " + accessToken);
			request.Method = "GET";

			var response = request.GetResponse();

			string responseData;

			using (var reader = new StreamReader(response.GetResponseStream()))
			{
				responseData = reader.ReadToEnd();
			}

			//var json = Json(responseData, JsonRequestBehavior.AllowGet);
			//return json;

			return new JsonpResult(responseData, "success");
		}

		private static string GetAccessToken(string consumerKey, string consumerSecret)
		{
			if (string.IsNullOrWhiteSpace(consumerKey))
			{
				throw new ArgumentNullException("consumerKey");
			}

			if (string.IsNullOrWhiteSpace(consumerSecret))
			{
				throw new ArgumentNullException("consumerSecret");
			}

			const string oauthUrl = "https://api.twitter.com/oauth2/token";
			const string bearerTokenRequestBody = "grant_type=client_credentials";
			var encodedKey = string.Concat(Uri.EscapeDataString(consumerKey), ":", Uri.EscapeDataString(consumerSecret));
			var toEncodeAsBytes = Encoding.ASCII.GetBytes(encodedKey);
			var base64Key = Convert.ToBase64String(toEncodeAsBytes);
			var authorizationHeader = "Basic " + base64Key;
			string accessToken;

			try
			{
				var bearerTokenRequest = (HttpWebRequest)WebRequest.Create(oauthUrl);

				bearerTokenRequest.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
				bearerTokenRequest.Headers.Add("Authorization", authorizationHeader);
				bearerTokenRequest.Method = "POST";

				var bearerTokenRequestBodyData = Encoding.ASCII.GetBytes(bearerTokenRequestBody);
				var bearerTokenRequestStream = bearerTokenRequest.GetRequestStream();
				bearerTokenRequestStream.Write(bearerTokenRequestBodyData, 0, bearerTokenRequestBodyData.Length);
				bearerTokenRequestStream.Close();

				TokenResponse tokenResponse;

				using (var bearerTokenResponse = (HttpWebResponse)bearerTokenRequest.GetResponse())
				{
					tokenResponse = GetResult(bearerTokenResponse);
				}

				if (tokenResponse == null || tokenResponse.TokenType != "bearer")
				{
					throw new ApplicationException("OAuth bearer token could not be acquired.");
				}

				accessToken = tokenResponse.AccessToken;
			}
			catch (Exception e)
			{
				throw new ApplicationException("OAuth bearer token could not be acquired.", e.InnerException);
			}
			return accessToken;
		}

		private static TokenResponse GetResult(HttpWebResponse response)
		{
			TokenResponse result = null;

			if (response != null && response.StatusCode == HttpStatusCode.OK)
			{
				using (var stream = response.GetResponseStream())
				{
					var serialiser = new DataContractJsonSerializer(typeof(TokenResponse));

					if (stream != null)
					{
						result = serialiser.ReadObject(stream) as TokenResponse;
					}
				}
			}

			return result;
		}

		[DataContract]
		public class TokenResponse
		{
			[DataMember(Name = "token_type")]
			public string TokenType { get; set; }

			[DataMember(Name = "access_token")]
			public string AccessToken { get; set; }
		}

		public class JsonpResult : ActionResult
		{
			public string CallbackFunction { get; set; }
			public Encoding ContentEncoding { get; set; }
			public string ContentType { get; set; }
			public object Data { get; set; }

			public JsonpResult(object data) : this(data, null) { }
			public JsonpResult(object data, string callbackFunction)
			{
				Data = data;
				CallbackFunction = callbackFunction;
			}

			public override void ExecuteResult(ControllerContext context)
			{
				if (context == null) throw new ArgumentNullException("context");

				var response = context.HttpContext.Response;

				response.ContentType = "application/json";

				if (ContentEncoding != null) response.ContentEncoding = ContentEncoding;

				if (Data != null)
				{
					var request = context.HttpContext.Request;

					var callback = CallbackFunction ?? request.Params["callback"] ?? "callback";

					//var serializer = new JavaScriptSerializer();

					//var jsonp = string.Format("{0}({1});", callback, serializer.Serialize(Data));

					var jsonp = string.Format("{0}({1});", callback, Data);
					
					response.Write(jsonp);
				}
			}
		}
	}
}
