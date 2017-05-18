using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RichardSzalay.MockHttp;
using System.Net.Http;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using FluentAssertions;

namespace AyycornApiPostOrdersFacade.Tests
{
    [TestClass]
    public class UnitTest1
    {
        private MockData _mockData = new MockData();

        [TestMethod]
        public void PostOrderAsync_SuccessfulOrder_OK()
        {
            var mockHttp = new MockHttpMessageHandler();
            var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(_mockData.ExpectedOrderMock().ToString(), Encoding.UTF8, "application/json")
            };
            mockHttp.When("http://AyycornPostOrdersMicroService.azurewebsites.net/api/order").Respond(mockResponse);

            var client = new HttpClient(mockHttp);
            var facade = new Facades.PostOrderFacade(client);

            var response = facade.PostOrderAsync(_mockData.SelectionBoxMock()).Result;

            Assert.AreEqual(typeof(Models.JReturnModel), response.GetType());
            Assert.IsTrue(response.Success);
            _mockData.ExpectedOrderMock().ShouldBeEquivalentTo(response.Json);
        }

        [TestMethod]
        public void PostOrderAsync_UnsuccessfulOrder_InternalServerErrorSuccessfulCancel()
        {
            var mockHttp = new MockHttpMessageHandler();
            var mockResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(_mockData.UnsuccessfulOrderResponseMock().ToString(), Encoding.UTF8, "application/json")
            };
            mockHttp.When("http://AyycornPostOrdersMicroService.azurewebsites.net/api/order").Respond(mockResponse);

            var client = new HttpClient(mockHttp);
            var facade = new Facades.PostOrderFacade(client);

            var response = facade.PostOrderAsync(_mockData.SelectionBoxMock()).Result;

            var expected = _mockData.UnsuccessfulOrderMock();

            Assert.AreEqual(typeof(Models.JReturnModel), response.GetType());
            Assert.IsFalse(response.Success);
            Assert.AreEqual(expected.ErrorMessage, response.ErrorMessage);
            Assert.AreEqual(expected.Json, response.Json);
        }

        [TestMethod]
        public void PostOrderAsync_UnsuccessfulOrder_InternalServerErrorUnsuccessfulCancel()
        {
            var mockHttp = new MockHttpMessageHandler();
            var mockResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(_mockData.UnsuccessfulOrderCancelResponseMock().ToString(), Encoding.UTF8, "application/json")
            };
            mockHttp.When("http://AyycornPostOrdersMicroService.azurewebsites.net/api/order").Respond(mockResponse);

            var client = new HttpClient(mockHttp);
            var facade = new Facades.PostOrderFacade(client);

            var response = facade.PostOrderAsync(_mockData.SelectionBoxMock()).Result;
            var expected = _mockData.UnsuccessfulOrderCancelMock();

            Assert.AreEqual(typeof(Models.JReturnModel), response.GetType());
            Assert.IsFalse(response.Success);
            Assert.AreEqual(expected.ErrorMessage, response.ErrorMessage);
            expected.Json.ShouldBeEquivalentTo(response.Json);
        }
    }
}
