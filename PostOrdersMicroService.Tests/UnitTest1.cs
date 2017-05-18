using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;
using RichardSzalay.MockHttp;
using System.Net.Http;
using System.Collections.Generic;
using FluentAssertions;
using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace PostOrdersMicroService.Tests
{
    [TestClass]
    public class UnitTest1
    {
        MockData _mockData = new MockData();

        #region Selection Box Order Tests
        [TestMethod]
        public void PostOrderAsync_SuccessfulOrder_OK()
        {
            var bazzaMock = new Mock<BazzasBazaar.IStore>();
            var httpMock = new MockHttpMessageHandler();
                  
            bazzaMock.Setup(m => m.CreateOrderAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                        .ReturnsAsync(_mockData.BazzaOrder);   
                    
            httpMock.When("http://khanskwikimart.azurewebsites.net/*").Respond(_mockData.HttpOrder());
            httpMock.When("http://dodgydealers.azurewebsites.net/*").Respond(_mockData.HttpOrder());
            httpMock.When("http://undercutters.azurewebsites.net/*").Respond(_mockData.HttpOrder());

            var client = new HttpClient(httpMock);
            var controller = new Controllers.OrdersController(client, bazzaMock.Object)
            {
                Request = new HttpRequestMessage { RequestUri = new System.Uri("http://unittest.com") },
                Configuration = new System.Web.Http.HttpConfiguration()
            };

            var json = JsonConvert.SerializeObject(_mockData.MockSelectionBox());

            var actualResponse = controller.PostOrderAsync(_mockData.MockSelectionBox()).Result;
            var actualResults = actualResponse.Content.ReadAsAsync<IEnumerable<Models.Order>>().Result;

            var expectedResponse = _mockData.Expected();
            var expectedResults = expectedResponse.Content.ReadAsAsync<IEnumerable<Models.Order>>().Result;

            expectedResults.ShouldBeEquivalentTo(actualResults, "Expected not equal to actual.");
            Assert.AreEqual(expectedResponse.StatusCode, actualResponse.StatusCode);
        }

        #region Exception Tests
        [TestMethod]
        public void PostOrderAsync_BazzaException_InternalServerError()
        {
            var bazzaMock = new Mock<BazzasBazaar.IStore>();
            var httpMock = new MockHttpMessageHandler();
            
            bazzaMock.Setup(m => m.CreateOrderAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                        .Throws(new System.Exception());

            //POST mocks
            httpMock.When("http://khanskwikimart.azurewebsites.net/api/giftwrapping/orders").Respond(_mockData.HttpOrder());
            httpMock.When("http://dodgydealers.azurewebsites.net/api/order").Respond(_mockData.HttpOrder());
            httpMock.When("http://undercutters.azurewebsites.net/api/order").Respond(_mockData.HttpOrder());

            //DELETE mocks
            httpMock.When("http://dodgydealers.azurewebsites.net/api/order/*").Respond(new HttpResponseMessage(System.Net.HttpStatusCode.OK));
            httpMock.When("http://undercutters.azurewebsites.net/api/order/*").Respond(new HttpResponseMessage(System.Net.HttpStatusCode.OK));

            var client = new HttpClient(httpMock);
            var controller = new Controllers.OrdersController(client, bazzaMock.Object)
            {
                Request = new HttpRequestMessage { RequestUri = new System.Uri("http://unittest.com") },
                Configuration = new System.Web.Http.HttpConfiguration()
            };

            var actualResponse = controller.PostOrderAsync(_mockData.MockSelectionBox()).Result;
            var actualResult = actualResponse.Content.ReadAsAsync<Models.OrderError>().Result;

            var expectedResponse = _mockData.FailedKwikiCancelMock();
            var expectedOrders = expectedResponse.Content.ReadAsAsync<IEnumerable<Models.Order>>().Result;

            Assert.AreEqual(expectedResponse.StatusCode, actualResponse.StatusCode);
            Assert.AreEqual("Orders unsuccessfully cancelled", actualResult.ErrorMessage);
            expectedOrders.ShouldBeEquivalentTo(actualResult.UnsuccessfulOrders);
        }

        [TestMethod]
        public void PostOrderAsync_KwikiException_InternalServerError()
        {
            var bazzaMock = new Mock<BazzasBazaar.IStore>();
            var httpMock = new MockHttpMessageHandler();
            
            bazzaMock.Setup(m => m.CreateOrderAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                         .ReturnsAsync(_mockData.BazzaOrder);
            bazzaMock.Setup(m => m.CancelOrderById(It.IsAny<int>())).Returns(true);

            //POST mocks
            httpMock.When("http://khanskwikimart.azurewebsites.net/api/giftwrapping/orders").Throw(new Exception());
            httpMock.When("http://dodgydealers.azurewebsites.net/api/order").Respond(_mockData.HttpOrder());
            httpMock.When("http://undercutters.azurewebsites.net/api/order").Respond(_mockData.HttpOrder());

            //DELETE mocks
            httpMock.When("http://dodgydealers.azurewebsites.net/api/order/*").Respond(new HttpResponseMessage(System.Net.HttpStatusCode.OK));
            httpMock.When("http://undercutters.azurewebsites.net/api/order/*").Respond(new HttpResponseMessage(System.Net.HttpStatusCode.OK));

            var client = new HttpClient(httpMock);
            var controller = new Controllers.OrdersController(client, bazzaMock.Object)
            {
                Request = new HttpRequestMessage { RequestUri = new System.Uri("http://unittest.com") },
                Configuration = new System.Web.Http.HttpConfiguration()
            };

            var actualResponse = controller.PostOrderAsync(_mockData.MockSelectionBox()).Result;
            var actualResult = actualResponse.Content.ReadAsAsync<Models.OrderError>().Result;

            Assert.AreEqual(System.Net.HttpStatusCode.InternalServerError, actualResponse.StatusCode);
            Assert.AreEqual("Orders successfully cancelled", actualResult.ErrorMessage);
            Assert.IsNull(actualResult.UnsuccessfulOrders);
        }

        [TestMethod]
        public void PostOrderAsync_DodgyException_InternalServerError()
        {
            var bazzaMock = new Mock<BazzasBazaar.IStore>();
            var httpMock = new MockHttpMessageHandler();
           
            bazzaMock.Setup(m => m.CreateOrderAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                         .ReturnsAsync(_mockData.BazzaOrder);
            bazzaMock.Setup(m => m.CancelOrderById(It.IsAny<int>())).Returns(true);

            //POST mocks
            httpMock.When("http://khanskwikimart.azurewebsites.net/api/giftwrapping/orders").Respond(_mockData.HttpOrder());
            httpMock.When("http://dodgydealers.azurewebsites.net/api/order").Throw(new Exception());
            httpMock.When("http://undercutters.azurewebsites.net/api/order").Respond(_mockData.HttpOrder());

            //DELETE mocks
            httpMock.When("http://dodgydealers.azurewebsites.net/api/order/*").Respond(new HttpResponseMessage(System.Net.HttpStatusCode.OK));
            httpMock.When("http://undercutters.azurewebsites.net/api/order/*").Respond(new HttpResponseMessage(System.Net.HttpStatusCode.OK));

            var client = new HttpClient(httpMock);
            var controller = new Controllers.OrdersController(client, bazzaMock.Object)
            {
                Request = new HttpRequestMessage { RequestUri = new System.Uri("http://unittest.com") },
                Configuration = new System.Web.Http.HttpConfiguration()
            };

            var actualResponse = controller.PostOrderAsync(_mockData.MockSelectionBox()).Result;
            var actualResult = actualResponse.Content.ReadAsAsync<Models.OrderError>().Result;

            var expectedResponse = _mockData.FailedKwikiCancelMock();
            var expectedResult = expectedResponse.Content.ReadAsAsync<IEnumerable<Models.Order>>().Result;

            Assert.AreEqual(expectedResponse.StatusCode, actualResponse.StatusCode);
            Assert.AreEqual("Orders unsuccessfully cancelled", actualResult.ErrorMessage);
            expectedResult.ShouldBeEquivalentTo(actualResult.UnsuccessfulOrders, "Expected not equal to actual");
            
        }

        [TestMethod]
        public void PostOrderAsync_UndercuttersException_InternalServerError()
        {
            var bazzaMock = new Mock<BazzasBazaar.IStore>();
            var httpMock = new MockHttpMessageHandler();
           
            bazzaMock.Setup(m => m.CreateOrderAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                        .ReturnsAsync(_mockData.BazzaOrder);
            bazzaMock.Setup(m => m.CancelOrderById(It.IsAny<int>())).Returns(true);

            //POST mocks
            httpMock.When("http://khanskwikimart.azurewebsites.net/api/giftwrapping/orders").Respond(_mockData.HttpOrder());
            httpMock.When("http://dodgydealers.azurewebsites.net/api/order").Respond(_mockData.HttpOrder());
            httpMock.When("http://undercutters.azurewebsites.net/api/order").Throw(new Exception());

            //DELETE mocks
            httpMock.When("http://dodgydealers.azurewebsites.net/api/order/*").Respond(new HttpResponseMessage(System.Net.HttpStatusCode.OK));
            httpMock.When("http://undercutters.azurewebsites.net/api/order/*").Respond(new HttpResponseMessage(System.Net.HttpStatusCode.OK));

            var client = new HttpClient(httpMock);
            var controller = new Controllers.OrdersController(client, bazzaMock.Object)
            {
                Request = new HttpRequestMessage { RequestUri = new System.Uri("http://unittest.com") },
                Configuration = new System.Web.Http.HttpConfiguration()
            };

            var actualResponse = controller.PostOrderAsync(_mockData.MockSelectionBox()).Result;
            var actualResult = actualResponse.Content.ReadAsAsync<Models.OrderError>().Result;

            var expectedResponse = _mockData.FailedKwikiCancelMock();
            var expectedResult = expectedResponse.Content.ReadAsAsync<IEnumerable<Models.Order>>().Result;

            Assert.AreEqual(expectedResponse.StatusCode, actualResponse.StatusCode);
            Assert.AreEqual("Orders unsuccessfully cancelled", actualResult.ErrorMessage);
            expectedResult.ShouldBeEquivalentTo(actualResult.UnsuccessfulOrders, "Expected not equal to actual");         
        }
        #endregion

        #region Http Error Responses
        [TestMethod]
        public void PostOrderAsync_KwikiBadRequest_InternalServerError()
        {
            var bazzaMock = new Mock<BazzasBazaar.IStore>();
            var httpMock = new MockHttpMessageHandler();

            bazzaMock.Setup(m => m.CreateOrderAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                        .ReturnsAsync(_mockData.BazzaOrder);
            bazzaMock.Setup(m => m.CancelOrderById(It.IsAny<int>())).Returns(true);

            //POST mocks
            httpMock.When("http://khanskwikimart.azurewebsites.net/api/giftwrapping/orders").Respond(new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest));
            httpMock.When("http://dodgydealers.azurewebsites.net/api/order").Respond(_mockData.HttpOrder());
            httpMock.When("http://undercutters.azurewebsites.net/api/order").Respond(_mockData.HttpOrder());

            //DELETE mocks
            httpMock.When("http://dodgydealers.azurewebsites.net/api/order/*").Respond(new HttpResponseMessage(System.Net.HttpStatusCode.OK));
            httpMock.When("http://undercutters.azurewebsites.net/api/order/*").Respond(new HttpResponseMessage(System.Net.HttpStatusCode.OK));

            var client = new HttpClient(httpMock);
            var controller = new Controllers.OrdersController(client, bazzaMock.Object)
            {
                Request = new HttpRequestMessage { RequestUri = new System.Uri("http://unittest.com") },
                Configuration = new System.Web.Http.HttpConfiguration()
            };

            var actualResponse = controller.PostOrderAsync(_mockData.MockSelectionBox()).Result;
            var actualResult = actualResponse.Content.ReadAsAsync<Models.OrderError>().Result;

            Assert.AreEqual("Orders successfully cancelled", actualResult.ErrorMessage);
            Assert.AreEqual(System.Net.HttpStatusCode.InternalServerError, actualResponse.StatusCode);
            Assert.IsNull(actualResult.UnsuccessfulOrders);
        }

        [TestMethod]
        public void PostOrderAsync_DodgyBadRequest_InternalServerError()
        {
            var bazzaMock = new Mock<BazzasBazaar.IStore>();
            var httpMock = new MockHttpMessageHandler();

            bazzaMock.Setup(m => m.CreateOrderAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                        .ReturnsAsync(_mockData.BazzaOrder);
            bazzaMock.Setup(m => m.CancelOrderById(It.IsAny<int>())).Returns(true);

            //POST mocks
            httpMock.When("http://khanskwikimart.azurewebsites.net/api/giftwrapping/orders").Respond(_mockData.HttpOrder());
            httpMock.When("http://dodgydealers.azurewebsites.net/api/order").Respond(new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest));
            httpMock.When("http://undercutters.azurewebsites.net/api/order").Respond(_mockData.HttpOrder());

            //DELETE mocks
            httpMock.When("http://dodgydealers.azurewebsites.net/api/order/*").Respond(new HttpResponseMessage(System.Net.HttpStatusCode.OK));
            httpMock.When("http://undercutters.azurewebsites.net/api/order/*").Respond(new HttpResponseMessage(System.Net.HttpStatusCode.OK));

            var client = new HttpClient(httpMock);
            var controller = new Controllers.OrdersController(client, bazzaMock.Object)
            {
                Request = new HttpRequestMessage { RequestUri = new System.Uri("http://unittest.com") },
                Configuration = new System.Web.Http.HttpConfiguration()
            };

            var actualResponse = controller.PostOrderAsync(_mockData.MockSelectionBox()).Result;
            var actualResult = actualResponse.Content.ReadAsAsync<Models.OrderError>().Result;

            var expectedResponse = _mockData.FailedKwikiCancelMock();
            var expectedResult = expectedResponse.Content.ReadAsAsync<IEnumerable<Models.Order>>().Result;

            Assert.AreEqual(expectedResponse.StatusCode, actualResponse.StatusCode);
            Assert.AreEqual("Orders unsuccessfully cancelled", actualResult.ErrorMessage);
            expectedResult.ShouldBeEquivalentTo(actualResult.UnsuccessfulOrders, "Expected not equal to actual");            
        }

        [TestMethod]
        public void PostOrderAsync_UndercuttersBadRequest_InternalServerError()
        {
            var bazzaMock = new Mock<BazzasBazaar.IStore>();
            var httpMock = new MockHttpMessageHandler();

            bazzaMock.Setup(m => m.CreateOrderAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                        .ReturnsAsync(_mockData.BazzaOrder);
            bazzaMock.Setup(m => m.CancelOrderById(It.IsAny<int>())).Returns(true);

            //POST mocks
            httpMock.When("http://khanskwikimart.azurewebsites.net/api/giftwrapping/orders").Respond(_mockData.HttpOrder());
            httpMock.When("http://dodgydealers.azurewebsites.net/api/order").Respond(_mockData.HttpOrder());
            httpMock.When("http://undercutters.azurewebsites.net/api/order").Respond(new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest));

            //DELETE mocks
            httpMock.When("http://dodgydealers.azurewebsites.net/api/order/*").Respond(new HttpResponseMessage(System.Net.HttpStatusCode.OK));
            httpMock.When("http://undercutters.azurewebsites.net/api/order/*").Respond(new HttpResponseMessage(System.Net.HttpStatusCode.OK));

            var client = new HttpClient(httpMock);
            var controller = new Controllers.OrdersController(client, bazzaMock.Object)
            {
                Request = new HttpRequestMessage { RequestUri = new System.Uri("http://unittest.com") },
                Configuration = new System.Web.Http.HttpConfiguration()
            };

            var actualResponse = controller.PostOrderAsync(_mockData.MockSelectionBox()).Result;
            var actualResult = actualResponse.Content.ReadAsAsync<Models.OrderError>().Result;

            var expectedResponse = _mockData.FailedKwikiCancelMock();
            var expectedResult = expectedResponse.Content.ReadAsAsync<IEnumerable<Models.Order>>().Result;

            Assert.AreEqual(expectedResponse.StatusCode, actualResponse.StatusCode);
            Assert.AreEqual("Orders unsuccessfully cancelled", actualResult.ErrorMessage);
            expectedResult.ShouldBeEquivalentTo(actualResult.UnsuccessfulOrders, "Expected not equal to actual");            
        }
        #endregion

        #endregion

    }
}
