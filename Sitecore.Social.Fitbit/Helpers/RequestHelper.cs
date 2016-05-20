using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Sitecore.Social.Fitbit.Model;
using Sitecore.Social.NetworkProviders;

namespace Sitecore.Social.Fitbit.Helpers
{
    public class RequestHelper
    {
        public static T ExecuteRequest<T>(string url, AuthenticationHeaderValue authHeader, Func<string, T> parseResponse = null, FormUrlEncodedContent content = null)
        {
            Task<string> responseString;
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = authHeader;
                httpClient.Timeout = TimeSpan.FromMinutes(1);
                
                Task<HttpResponseMessage> response = content != null ? httpClient.PostAsync(url, content) : httpClient.GetAsync(url);
                response.Wait();
                responseString = response.Result.Content.ReadAsStringAsync();
                responseString.Wait();
            }

            T result = parseResponse != null ? parseResponse(responseString.Result) : ParseResponse<T>(responseString.Result);

            return result;
        }

        public static T ExecuteRequest<T>(string url, Application application, Func<string, T> parseResponse = null, FormUrlEncodedContent content = null)
        {
            string authHeaderValue = GetAuthHeaderValue(application.ApplicationKey, application.ApplicationSecret);
            AuthenticationHeaderValue authHeader = new AuthenticationHeaderValue("Basic", authHeaderValue);

            return ExecuteRequest(url, authHeader, parseResponse, content);
        }

        public static T ExecuteRequest<T>(string url, Account account, Func<string, T> parseResponse = null, FormUrlEncodedContent content = null)
        {
            AuthenticationHeaderValue authHeader = new AuthenticationHeaderValue("Bearer", account.AccessTokenSecret);

            return ExecuteRequest(url, authHeader, parseResponse, content);
        }

        public static T ExecuteRequest<T>(string url, string token, Func<string, T> parseResponse = null, FormUrlEncodedContent content = null)
        {
            AuthenticationHeaderValue authHeader = new AuthenticationHeaderValue("Bearer", token);

            return ExecuteRequest(url, authHeader, parseResponse, content);
        }

        private static string GetAuthHeaderValue(string appKey, string appSecret)
        {
            return Base64Encode(appKey + ":" + appSecret);
        }

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        private static T ParseResponse<T>(string response)
        {
            JObject responseObject = JObject.Parse(response);
            JsonSerializer serializer = JsonSerializer.CreateDefault();

            JToken errors = responseObject["errors"];
            if (errors != null)
            {
                ErrorsResponse errorscollection = serializer.Deserialize<ErrorsResponse>(responseObject.CreateReader());
                throw new Exception(errorscollection.Errors.First().Message);
            }

            return serializer.Deserialize<T>(responseObject.CreateReader());
        }
    }
}
