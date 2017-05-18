using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AyycornMVCUsersFacade.Facades
{
    /// <summary>
    /// Class UsersFacade.
    /// </summary>
    public class UsersFacade
    {
        private readonly HttpClient _client;

        #region constructors
        public UsersFacade()
        {
            _client = new HttpClient();
        }
        public UsersFacade(HttpClient client)
        {
            _client = client;
        }
        #endregion

        /// <summary>
        /// Validates the user login.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>Task&lt;Models.JReturnModel&gt;.</returns>
        public async Task<Models.JReturnModel> ValidateUserLogin(string username, string password)
        {
            HttpResponseMessage response;

            var uri = "http://ayycornauthservice.azurewebsites.net/oauth/token";
            var contentStr = string.Format("grant_type=password&username={0}&password={1}",
                                            HttpUtility.UrlEncode(username),
                                            HttpUtility.UrlEncode(password));
            var content = new StringContent(contentStr, Encoding.UTF8, "application/x-www-form-urlencoded");

            response = await _client.PostAsync(uri, content);
            var responseStr = await response.Content.ReadAsStringAsync();
            var responseJObj = JObject.Parse(responseStr);

            return new Models.JReturnModel()
            {
                Success = response.IsSuccessStatusCode,
                Json = responseJObj
            };
        }
        /// <summary>
        /// get current user information as an asynchronous operation.
        /// </summary>
        /// <param name="token">JWT.</param>
        /// <returns>Task&lt;Models.JReturnModel&gt;.</returns>
        public async Task<Models.JReturnModel> GetCurrentUserInfoAsync(JObject token)
        {
            HttpResponseMessage response;

            string uri = "http://ayycornauthservice.azurewebsites.net/api/accounts/user";
            string accessToken = JObject.Parse(token.ToString())["access_token"].ToString();

            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var request = new HttpRequestMessage(HttpMethod.Get, uri)
            {
                Content = new StringContent("", Encoding.UTF8, "application/json")
            };

            response = await _client.SendAsync(request);

            var responseStr = await response.Content.ReadAsStringAsync();
            var responseJObj = JObject.Parse(responseStr);
  
            return new Models.JReturnModel()
            {
                Success = response.IsSuccessStatusCode,
                Json = responseJObj
            };
        }
    }
}