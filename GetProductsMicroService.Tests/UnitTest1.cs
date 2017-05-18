using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using RichardSzalay.MockHttp;
using Newtonsoft.Json;
using System.Text;
using System.ServiceModel;

namespace GetProductsMicroService.Tests
{
    [TestClass]
    public class UnitTest1
    {
        private MockData mockData = new MockData();

        #region DataReturnTests           
        [TestMethod]
        public void GetProducts_NoErrors_CheapestProducts()
        {
            var mockRepo = new Mock<IProductRepo>();
            mockRepo.Setup(m => m.GetWcfProducts()).Returns(mockData.WcfData);
            mockRepo.Setup(m => m.GetHttpProducts()).Returns(mockData.HttpData);
            var service = new Controllers.ProductController(mockRepo.Object);
            var resultResponse = service.GetProductsAsync().Result;
            var result = resultResponse.Content.ReadAsAsync<IEnumerable<Dtos.Product>>().Result;
            var expected = mockData.TestGetCheapestProductsData().Content.ReadAsAsync<IEnumerable<Dtos.Product>>().Result;
            expected.ShouldBeEquivalentTo(result, "Expected response not the same as actual response.");
        }
        [TestMethod]
        public void GetProducts_HttpNoContent_HttpNoContent()
        {
            var mockRepo = new Mock<IProductRepo>();
            var nullProducts = Task.FromResult(Enumerable.Empty<Dtos.Product>());
            mockRepo.Setup(m => m.GetWcfProducts()).Returns(nullProducts);
            mockRepo.Setup(m => m.GetHttpProducts()).Returns(nullProducts);
            var service = new Controllers.ProductController(mockRepo.Object);
            var result = service.GetProductsAsync().Result;
            Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode);
        }
        [TestMethod]
        public void GetProducts_OnlyWcfProducts_WcfProducts()
        {
            var mockRepo = new Mock<IProductRepo>();
            mockRepo.Setup(m => m.GetHttpProducts()).Returns(Task.FromResult(Enumerable.Empty<Dtos.Product>()));
            mockRepo.Setup(m => m.GetWcfProducts()).Returns(mockData.WcfData());
            var service = new Controllers.ProductController(mockRepo.Object);
            var response = service.GetProductsAsync().Result;
            var result = response.Content.ReadAsAsync<IEnumerable<Dtos.Product>>().Result;
            var expected = mockData.WcfData().Result;
            expected.ShouldBeEquivalentTo(result, "Expected result not equal to actual result.");
        }
        [TestMethod]
        public void GetProducts_OnlyHttpProducts_HttpProducts()
        {
            var mockRepo = new Mock<IProductRepo>();
            mockRepo.Setup(m => m.GetHttpProducts()).Returns(mockData.HttpData());
            mockRepo.Setup(m => m.GetWcfProducts()).Returns(Task.FromResult(Enumerable.Empty<Dtos.Product>()));
            var service = new Controllers.ProductController(mockRepo.Object);
            var response = service.GetProductsAsync().Result;
            var result = response.Content.ReadAsAsync<IEnumerable<Dtos.Product>>().Result;
            var expected = mockData.HttpData().Result;
            expected.ShouldBeEquivalentTo(result, "Expected result not equal to actual result.");
        }
        [TestMethod]
        public void GetProducts_InternalServerError_InternalServerError()
        {
            var mockRepo = new Mock<IProductRepo>();
            mockRepo.Setup(m => m.GetHttpProducts()).Throws(new Exception());
            var service = new Controllers.ProductController(mockRepo.Object);
            var result = service.GetProductsAsync().Result;
            var expected = HttpStatusCode.InternalServerError;
            Assert.AreEqual(expected, result.StatusCode);
        }
        #endregion
        #region WcfExceptionTests
        [TestMethod]
        public void GetProducts_WcfTimeoutException_HttpProducts()
        {
            var mockStore = new Mock<BazzasBazaar.IStore>();
            mockStore.Setup(m => m.GetFilteredProducts(null, null, null, null)).Throws(new TimeoutException());
            MockHttpMessageHandler mockHttp = new MockHttpMessageHandler();
            HttpResponseMessage fakeResponse = new HttpResponseMessage(HttpStatusCode.OK);
            fakeResponse.Content = new StringContent(JsonConvert.SerializeObject(mockData.HttpData().Result), Encoding.UTF8, "application/json");
            mockHttp.When("http://localhost:24697/api/*").Respond(fakeResponse);
            HttpClient client = new HttpClient(mockHttp);
            Dictionary<string, Uri> uri = new Dictionary<string, Uri> { { "Fake", new Uri("http://localhost:24697/api/Products") } };

            var repo = new Repos.ProductRepo(client, mockStore.Object, uri);        
            var service = new Controllers.ProductController(repo);
            var response = service.GetProductsAsync().Result;
            var results = response.Content.ReadAsAsync<IEnumerable<Dtos.Product>>().Result;
            var expected = mockData.HttpDataFake().Result;
            expected.ShouldBeEquivalentTo(results, "Expected results not equal to actual results.");
        }
        [TestMethod]
        public void GetProducts_WcfFaultException_HttpProducts()
        {
            var mockStore = new Mock<BazzasBazaar.IStore>();
            mockStore.Setup(m => m.GetFilteredProducts(null, null, null, null)).Throws(new FaultException());
            MockHttpMessageHandler mockHttp = new MockHttpMessageHandler();
            HttpResponseMessage fakeResponse = new HttpResponseMessage(HttpStatusCode.OK);
            fakeResponse.Content = new StringContent(JsonConvert.SerializeObject(mockData.HttpData().Result), Encoding.UTF8, "application/json");
            mockHttp.When("http://localhost:24697/api/*").Respond(fakeResponse);
            HttpClient client = new HttpClient(mockHttp);
            Dictionary<string, Uri> uri = new Dictionary<string, Uri> { { "Fake", new Uri("http://localhost:24697/api/Products") } };

            var repo = new Repos.ProductRepo(client, mockStore.Object, uri);
            var service = new Controllers.ProductController(repo);
            var response = service.GetProductsAsync().Result;
            var results = response.Content.ReadAsAsync<IEnumerable<Dtos.Product>>().Result;
            var expected = mockData.HttpDataFake().Result;
            expected.ShouldBeEquivalentTo(results, "Expected results not equal to actual results.");
        }
        [TestMethod]
        public void GetProducts_WcfCommunicationException_HttpProducts()
        {
            var mockStore = new Mock<BazzasBazaar.IStore>();
            mockStore.Setup(m => m.GetFilteredProducts(null, null, null, null)).Throws(new CommunicationException());
            MockHttpMessageHandler mockHttp = new MockHttpMessageHandler();
            HttpResponseMessage fakeResponse = new HttpResponseMessage(HttpStatusCode.OK);
            fakeResponse.Content = new StringContent(JsonConvert.SerializeObject(mockData.HttpData().Result), Encoding.UTF8, "application/json");
            mockHttp.When("http://localhost:24697/api/*").Respond(fakeResponse);
            HttpClient client = new HttpClient(mockHttp);
            Dictionary<string, Uri> uri = new Dictionary<string, Uri> { { "Fake", new Uri("http://localhost:24697/api/Products") } };

            var repo = new Repos.ProductRepo(client, mockStore.Object, uri);
            var service = new Controllers.ProductController(repo);
            var response = service.GetProductsAsync().Result;
            var results = response.Content.ReadAsAsync<IEnumerable<Dtos.Product>>().Result;
            var expected = mockData.HttpDataFake().Result;
            expected.ShouldBeEquivalentTo(results, "Expected results not equal to actual results.");
        }
        [TestMethod]
        public void GetProductsWcf_TimeoutException_EmptyEnumerable()
        {
            var mockStore = new Mock<BazzasBazaar.IStore>();
            mockStore.Setup(m => m.GetFilteredProducts(null, null, null, null)).Throws(new TimeoutException());
            var service = new Repos.ProductRepo(mockStore.Object);
            var result = service.GetWcfProducts().Result;
            var expected = Enumerable.Empty<Dtos.Product>();
            expected.ShouldBeEquivalentTo(result, "Expected result not equal to actual result.");
        }
        [TestMethod]
        public void GetProductsWcf_FaultException_EmptyEnumerable()
        {
            var mockStore = new Mock<BazzasBazaar.IStore>();
            mockStore.Setup(m => m.GetFilteredProducts(null, null, null, null)).Throws(new FaultException());
            var service = new Repos.ProductRepo(mockStore.Object);
            var result = service.GetWcfProducts().Result;
            var expected = Enumerable.Empty<Dtos.Product>();
            expected.ShouldBeEquivalentTo(result, "Expected result not equal to actual result.");
        }
        [TestMethod]
        public void GetProductsWcf_CommunicationException_EmptyEnumerable()
        {
            var mockStore = new Mock<BazzasBazaar.IStore>();
            mockStore.Setup(m => m.GetFilteredProducts(null, null, null, null)).Throws(new CommunicationException());
            var service = new Repos.ProductRepo(mockStore.Object);
            var result = service.GetWcfProducts().Result;
            var expected = Enumerable.Empty<Dtos.Product>();
            expected.ShouldBeEquivalentTo(result, "Expected result not equal to actual result.");
        }
        #endregion
        #region HttpResponseTests
        private IEnumerable<Dtos.Product> _GetResults(HttpStatusCode statusCode)
        {
            MockHttpMessageHandler mockHttp = new MockHttpMessageHandler();
            mockHttp.When("http://localhost:24697/api/*").Respond(statusCode);
            HttpClient client = new HttpClient(mockHttp);
            client.BaseAddress = new Uri("http://localhost/api/");
            Dictionary<string, Uri> uri = new Dictionary<string, Uri> { { "Fake", new Uri("http://localhost:24697/api/Products") } };
            var service = new Repos.ProductRepo(client, uri);
            return service.GetHttpProducts().Result;
        }
        [TestMethod]
        public void GetProductsHttp_BadGateway_EmptyEnumerable()
        {
            var result = _GetResults(HttpStatusCode.BadGateway);
            var expected = Enumerable.Empty<Dtos.Product>();
            expected.ShouldBeEquivalentTo(result, "Expected result not equal to expected result");
        }

        [TestMethod]
        public void GetProductsHttp_BadRequest_EmptyEnumerable()
        {
            var result = _GetResults(HttpStatusCode.BadRequest);
            var expected = Enumerable.Empty<Dtos.Product>();
            expected.ShouldBeEquivalentTo(result, "Expected result not equal to expected result");
        }

        [TestMethod]
        public void GetProductsHttp_Conflict_EmptyEnumerable()
        {
            var result = _GetResults(HttpStatusCode.Conflict);
            var expected = Enumerable.Empty<Dtos.Product>();
            expected.ShouldBeEquivalentTo(result, "Expected result not equal to expected result");
        }

        [TestMethod]
        public void GetProductsHttp_Forbidden_EmptyEnumerable()
        {
            var result = _GetResults(HttpStatusCode.Forbidden);
            var expected = Enumerable.Empty<Dtos.Product>();
            expected.ShouldBeEquivalentTo(result, "Expected result not equal to expected result");
        }

        [TestMethod]
        public void GetProductsHttp_GatewayTimeout_EmptyEnumerable()
        {
            var result = _GetResults(HttpStatusCode.GatewayTimeout);
            var expected = Enumerable.Empty<Dtos.Product>();
            expected.ShouldBeEquivalentTo(result, "Expected result not equal to expected result");
        }

        [TestMethod]
        public void GetProductsHttp_Gone_EmptyEnumerable()
        {
            var result = _GetResults(HttpStatusCode.Gone);
            var expected = Enumerable.Empty<Dtos.Product>();
            expected.ShouldBeEquivalentTo(result, "Expected result not equal to expected result");
        }

        [TestMethod]
        public void GetProductsHttp_InteralServerError_EmptyEnumerable()
        {
            var result = _GetResults(HttpStatusCode.InternalServerError);
            var expected = Enumerable.Empty<Dtos.Product>();
            expected.ShouldBeEquivalentTo(result, "Expected result not equal to expected result");
        }

        [TestMethod]
        public void GetProductsHttp_MethodNotAllowed_EmptyEnumerable()
        {
            var result = _GetResults(HttpStatusCode.MethodNotAllowed);
            var expected = Enumerable.Empty<Dtos.Product>();
            expected.ShouldBeEquivalentTo(result, "Expected result not equal to expected result");
        }

        [TestMethod]
        public void GetProductsHttp_Moved_EmptyEnumerable()
        {
            var result = _GetResults(HttpStatusCode.Moved);
            var expected = Enumerable.Empty<Dtos.Product>();
            expected.ShouldBeEquivalentTo(result, "Expected result not equal to expected result");
        }

        [TestMethod]
        public void GetProductsHttp_MovedPermanently_EmptyEnumerable()
        {
            var result = _GetResults(HttpStatusCode.MovedPermanently);
            var expected = Enumerable.Empty<Dtos.Product>();
            expected.ShouldBeEquivalentTo(result, "Expected result not equal to expected result");
        }

        [TestMethod]
        public void GetProductsHttp_NoContent_EmptyEnumerable()
        {
            var result = _GetResults(HttpStatusCode.NoContent);
            var expected = Enumerable.Empty<Dtos.Product>();
            expected.ShouldBeEquivalentTo(result, "Expected result not equal to expected result");
        }

        [TestMethod]
        public void GetProductsHttp_NonAuthoritativeInformation_EmptyEnumerable()
        {
            var result = _GetResults(HttpStatusCode.NonAuthoritativeInformation);
            var expected = Enumerable.Empty<Dtos.Product>();
            expected.ShouldBeEquivalentTo(result, "Expected result not equal to expected result");
        }

        [TestMethod]
        public void GetProductsHttp_NotFound_EmptyEnumerable()
        {
            var result = _GetResults(HttpStatusCode.NotFound);
            var expected = Enumerable.Empty<Dtos.Product>();
            expected.ShouldBeEquivalentTo(result, "Expected result not equal to expected result");
        }

        [TestMethod]
        public void GetProductsHttp_NotImplemented_EmptyEnumerable()
        {
            var result = _GetResults(HttpStatusCode.NotImplemented);
            var expected = Enumerable.Empty<Dtos.Product>();
            expected.ShouldBeEquivalentTo(result, "Expected result not equal to expected result");
        }

        [TestMethod]
        public void GetProductsHttp_OKWithNoContent_NoContent()
        {
            var result = _GetResults(HttpStatusCode.OK);
            var expected = Enumerable.Empty<Dtos.Product>();
            expected.ShouldBeEquivalentTo(result, "Expected result not equal to expected result");
        }

        [TestMethod]
        public void GetProductsHttp_OKWithContent_HttpProducts()
        {
            var mockHttp = new MockHttpMessageHandler();
            var fakeResponse = new HttpResponseMessage(HttpStatusCode.OK);
            fakeResponse.Content = new StringContent(JsonConvert.SerializeObject(mockData.HttpDataFake().Result), Encoding.UTF8, "application/json");
            mockHttp.When("http://localhost:24697/api/*").Respond(fakeResponse);
            var client = new HttpClient(mockHttp);
            Dictionary<string, Uri> uri = new Dictionary<string, Uri> { { "Fake", new Uri("http://localhost:24697/api/Products") } };
            var service = new Repos.ProductRepo(client, uri);
            var result = service.GetHttpProducts().Result;
            var expected = mockData.HttpDataFake().Result;
            expected.ShouldBeEquivalentTo(result, "Expected result not equal to actual result.");
        }

        [TestMethod]
        public void GetProductsHttp_ServiceUnavailable_EmptyEnumerable()
        {
            var result = _GetResults(HttpStatusCode.ServiceUnavailable);
            var expected = Enumerable.Empty<Dtos.Product>();
            expected.ShouldBeEquivalentTo(result, "Expected result not equal to expected result");
        }

        [TestMethod]
        public void GetProductsHttp_Unauthorized_EmptyEnumerable()
        {
            var result = _GetResults(HttpStatusCode.Unauthorized);
            var expected = Enumerable.Empty<Dtos.Product>();
            expected.ShouldBeEquivalentTo(result, "Expected result not equal to expected result");
        }
        #endregion
    }
}
