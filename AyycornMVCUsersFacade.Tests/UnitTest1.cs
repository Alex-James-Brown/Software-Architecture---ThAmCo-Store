using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using RichardSzalay.MockHttp;
using System.Net.Http;
using System.Net;
using System.Text;

namespace AyycornMVCUsersFacade.Tests
{
    [TestClass]
    public class UnitTest1
    {
        private MockData _mockData = new MockData();

        #region Token Return Tests
        [TestMethod]
        public void GetToken_ValidateUserLogin_Success()
        {
            var mockHttp = new MockHttpMessageHandler();
            var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(_mockData.TokenMock().ToString(), Encoding.UTF8, "application/json")
            };

            mockHttp.When("http://ayycornauthservice.azurewebsites.net/oauth/token").Respond(mockResponse);
            var client = new HttpClient(mockHttp);
            var usersFacade = new Facades.UsersFacade(client);

            var response = usersFacade.ValidateUserLogin("", "").Result;

            Assert.AreEqual(typeof(Models.JReturnModel), response.GetType());
            Assert.IsTrue(response.Success);
            Assert.AreEqual(_mockData.TokenMock().ToString(), response.Json.ToString());
        }
        [TestMethod]
        public void GetToken_ValidateUserLogin_InvalidCredentials()
        {
            var mockHttp = new MockHttpMessageHandler();
            var mockResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(_mockData.InvalidCredentialsMock().ToString(), Encoding.UTF8, "application/json")
            };

            mockHttp.When("http://ayycornauthservice.azurewebsites.net/oauth/token").Respond(mockResponse);
            var client = new HttpClient(mockHttp);
            var usersFacade = new Facades.UsersFacade(client);

            var response = usersFacade.ValidateUserLogin("", "").Result;

            Assert.AreEqual(typeof(Models.JReturnModel), response.GetType());
            Assert.IsFalse(response.Success);
            Assert.AreEqual(_mockData.InvalidCredentialsMock().ToString(), response.Json.ToString());
        }
        #endregion
        #region Get User Tests
        [TestMethod]
        public void GetUserInfo_ValidUser_UserInfo()
        {
            var mockHttp = new MockHttpMessageHandler();
            var mockRespnse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(_mockData.UserInfoMock().ToString(), Encoding.UTF8, "application/json")
            };
            mockHttp.When("http://ayycornauthservice.azurewebsites.net/api/accounts/user").Respond(mockRespnse);

            var client = new HttpClient(mockHttp);
            var userFacade = new Facades.UsersFacade(client);

            var response = userFacade.GetCurrentUserInfoAsync(_mockData.TokenMock()).Result;

            Assert.AreEqual(typeof(Models.JReturnModel), response.GetType());
            Assert.IsTrue(response.Success);
            Assert.AreEqual(_mockData.UserInfoMock().ToString(), response.Json.ToString());
        }
        [TestMethod]
        public void GetUserInfo_InvalidUser_Denied()
        {
            var mockHttp = new MockHttpMessageHandler();
            var mockRespnse = new HttpResponseMessage(HttpStatusCode.Unauthorized)
            {
                Content = new StringContent(_mockData.AuthDenied().ToString(), Encoding.UTF8, "application/json")
            };
            mockHttp.When("http://ayycornauthservice.azurewebsites.net/api/accounts/user").Respond(mockRespnse);

            var client = new HttpClient(mockHttp);
            var userFacade = new Facades.UsersFacade(client);

            var response = userFacade.GetCurrentUserInfoAsync(_mockData.TokenMock()).Result;

            Assert.AreEqual(typeof(Models.JReturnModel), response.GetType());
            Assert.IsFalse(response.Success);
            Assert.AreEqual(_mockData.AuthDenied().ToString(), response.Json.ToString());
        }
        #endregion
    }
}
