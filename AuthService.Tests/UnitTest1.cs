using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Web;
using System.Text;
using System.Net;
using System.Net.Http.Headers;

namespace AuthService.Tests
{
    [TestClass]
    public class UnitTest1
    {
        private MockData _mockData = new MockData();

        //must have AuthService debugging in another VS to use these tests
        private HttpResponseMessage GetOAuthToken(string username, string password)
        {
            HttpResponseMessage response;
            using (HttpClient client = new HttpClient())
            {
                string uri = "http://localhost:4020/oauth/token";
                var contentStr = string.Format("grant_type=password&username={0}&password={1}",
                                HttpUtility.UrlEncode(username),
                                HttpUtility.UrlEncode(password));
                var content = new StringContent(contentStr, Encoding.UTF8, "application/x-www-form-urlencoded");

                response = client.PostAsync(uri, content).Result;
            }
            return response;
        }

        [TestMethod]
        public void OAuthToken_ValidCredentials_JToken()
        {
            var response = GetOAuthToken("SuperUser", "SuperP@ss");
            var tokenStr = response.Content.ReadAsStringAsync().Result;
            var token = JObject.Parse(tokenStr);

            Assert.IsTrue(token.ToString().Contains("access_token") &&
                            token.ToString().Contains("token_type") &&
                            token.ToString().Contains("expires_in"));
            Assert.IsTrue(token["token_type"].ToString() == "bearer");
            Assert.IsTrue(response.IsSuccessStatusCode);
        }

        [TestMethod]
        public void OAuthToken_InvalidUsername_ErrorMessage()
        {
            var response = GetOAuthToken("WrongUser", "SuperP@ss");
            var error = response.Content.ReadAsStringAsync().Result;

            var expected = "{\"error\":\"invalid_grant\",\"error_description\":\"The user name or password is incorrect.\"}";

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.IsFalse(response.IsSuccessStatusCode);
            Assert.AreEqual(expected, error);
        }

        [TestMethod]
        public void OAuthToken_InvalidPassword_ErrorMessage()
        {
            var response = GetOAuthToken("SuperUser", "WrongPass");
            var error = response.Content.ReadAsStringAsync().Result;

            var expected = "{\"error\":\"invalid_grant\",\"error_description\":\"The user name or password is incorrect.\"}";

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.IsFalse(response.IsSuccessStatusCode);
            Assert.AreEqual(expected, error);
        }

        private HttpResponseMessage GetCurrentUser(JObject token)
        {
            string uri = "http://localhost:4020/api/accounts/user";
            string accessToken = JObject.Parse(token.ToString())["access_token"].ToString();

            HttpResponseMessage response;
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                response = client.GetAsync(uri).Result;
            }
            return response;
        }

        [TestMethod]
        public void GetCurrentUser_ValidToken_UserInfo()
        {
            var token = JObject.Parse(GetOAuthToken("SuperUser", "SuperP@ss").Content.ReadAsStringAsync().Result);

            var response = GetCurrentUser(token);
            var user = JObject.Parse(response.Content.ReadAsStringAsync().Result);

            Assert.IsTrue(response.IsSuccessStatusCode);
            Assert.AreEqual(user["url"].ToString(), "http://localhost:4020/api/accounts/user/dbaf6509-39ce-4105-9589-b84a955ffeaa");
            Assert.AreEqual(user["id"].ToString(), "dbaf6509-39ce-4105-9589-b84a955ffeaa");
            Assert.AreEqual(user["userName"].ToString(), "SuperUser");
            Assert.AreEqual(user["fullName"].ToString(), "Super User");
            Assert.AreEqual(user["email"].ToString(), "superuser@ThAmCo.com");
            Assert.AreEqual(user["emailConfirmed"].ToString(), "True");
            Assert.AreEqual(user["level"].ToString(), "1");
            Assert.AreEqual(user["joinDate"].ToString(), "11/01/2017 18:53:15");
            Assert.AreEqual(user["roles"].ToString(), JArray.Parse("[\"SuperAdmin\",\"Admin\"]").ToString());
            Assert.AreEqual(user["claims"].ToString(), "[]");
        }

        [TestMethod]
        public void GetCurrentUser_InvalidToken_Unauthorized()
        {
            var token = JObject.Parse("{ \"access_token\":\"eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJuYW1laWQiOiJiNTQyYmNhYi1mZDBiLTQzZGYtODc0MC1lODJkYzA1MjZkNTYiLCJ1bmlxdWVfbmFtZSI6IlN1cGVyVXNlciIsImh0dHA6Ly9zY2hlbWFzLm1pY3JvcHJvbHNlcnZpY2UvMjAxMC8wNy9jbGFpbXMvaWRlbnRpdHlwcm92aWRlciI6IkFTUC5ORVQgSWRlbnRpdHkiLCJBc3BOZXQuSWRlbnRpdHkuU2VjdXJpdHlTdGFtcCI6ImRjYzQ0NDgxLWQ0NjMtNDA5Ny04YTBlLWU0NGFmOGVmOWQ0ZSIsInJvbGUiOlsiU3VwZXJBZG1pbiIsIkFkbWluIl0sImlzcyI6Imh0dHA6Ly9sb2NhbGhvc3Q6NDAyMCIsImF1ZCI6IjQxNGUxOTI3YTM4ODRmNjhhYmM3OWY3MjgzODM3ZmQxIiwiZXhwIjoxNDgzNzUyNDMxLCJuYmYiOjE0ODM2NjYwMzF9.MgFD9Lt2fA0nCQHUMK9W0u5YCOt1L_ipunngZKNA2WU\",\"token_type\":\"bearer\",\"expires_in\":86399}");

            var response = GetCurrentUser(token);
            var user = JObject.Parse(response.Content.ReadAsStringAsync().Result);

            Assert.IsFalse(response.IsSuccessStatusCode);
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.AreEqual(JObject.Parse("{\"message\":\"Authorization has been denied for this request.\"}").ToString(), user.ToString());
        }

        [TestMethod]
        public void GetCurrentUser_NoToken_Unauthorized()
        {
            HttpResponseMessage response;
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string uri = "http://localhost:4020/api/accounts/user";
                response = client.GetAsync(uri).Result;
            }
            var error = JObject.Parse(response.Content.ReadAsStringAsync().Result);

            Assert.IsFalse(response.IsSuccessStatusCode);
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.AreEqual(JObject.Parse("{\"message\":\"Authorization has been denied for this request.\"}").ToString(), error.ToString());
        }

        [TestMethod]
        public void CreateUser_NewUser_Success()
        {
            HttpResponseMessage response;

            using (HttpClient client = new HttpClient())
            {
                string uri = "http://localhost:4020/api/accounts/create";

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var content = new StringContent(_mockData.CreateUserMock().ToString(), Encoding.UTF8, "application/json");
                response = client.PostAsync(uri, content).Result;
            }

            var result = response.Content.ReadAsStringAsync();

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var tresponse = GetOAuthToken("mock", "MockP@ss");
            var tokenStr = response.Content.ReadAsStringAsync().Result;
            var token = JObject.Parse(tokenStr);

            Assert.IsTrue(response.IsSuccessStatusCode);
        }

        [TestMethod]
        public void DeleteUser_ValidUser_Success()
        {
            var adminToken = JObject.Parse(GetOAuthToken("SuperUser", "SuperP@ss").Content.ReadAsStringAsync().Result);
            var mockToken = JObject.Parse(GetOAuthToken("Mock", "MockP@ss").Content.ReadAsStringAsync().Result);
            var mockUser = JObject.Parse(GetCurrentUser(mockToken).Content.ReadAsStringAsync().Result);

            HttpResponseMessage response;
            using (HttpClient client = new HttpClient())
            {
                string uri = "http://localhost:4020/api/accounts/user/";

                var accessToken = JObject.Parse(adminToken.ToString())["access_token"].ToString();

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var userId = mockUser["id"].ToString();
                response = client.DeleteAsync(uri + userId).Result;
            }
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.IsTrue(response.IsSuccessStatusCode);

            var tresponse = GetOAuthToken("mock", "MockP@ss");
            Assert.IsFalse(tresponse.IsSuccessStatusCode);
        }
    }
}
