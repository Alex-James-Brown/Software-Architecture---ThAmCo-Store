using Microsoft.VisualStudio.TestTools.UnitTesting;
using RichardSzalay.MockHttp;
using System;
using System.Net;
using System.Net.Http;

namespace GiftWrapMicroService.Tests
{
    [TestClass]
    public class UnitTest1
    {
        #region HttpResponseTests
        private HttpResponseMessage _GetResults(HttpStatusCode statusCode)
            {
                MockHttpMessageHandler mockHttp = new MockHttpMessageHandler();
                mockHttp.When("http://localhost/api/*").Respond(statusCode);
                HttpClient client = new HttpClient(mockHttp);
                client.BaseAddress = new Uri("http://localhost/api/");
                Controllers.GiftWrapController controller = new Controllers.GiftWrapController(client);
                return controller.GetGiftWrapAsync().Result;
            }
            
            [TestMethod]
            public void GetGiftWrapAsync_HttpAmbiguous_HttpAmbiguous()
            {
                //HttpStatusCode.Ambiguous;
                //Need to program the service to follow the location header
                var statusCode = HttpStatusCode.Ambiguous;
                var results = _GetResults(statusCode);
                Assert.AreEqual(statusCode, results.StatusCode);
            }

            [TestMethod]
            public void GetGiftWrapAsync_BadGateway_BadGateway()
            {
                var statusCode = HttpStatusCode.BadGateway;
                var results = _GetResults(statusCode);
                Assert.AreEqual(statusCode, results.StatusCode);
            }
        
            [TestMethod]
            public void GetGiftWrapAsync_BadRequest_BadRequest()
            {
                //HttpStatusCode.BadRequest;
                var statusCode = HttpStatusCode.BadRequest;
                var results = _GetResults(statusCode);
                Assert.AreEqual(statusCode, results.StatusCode);
            }
            
            [TestMethod]
            public void GetGiftWrapAsync_Conflict_Conflict()
            {
                //HttpStatusCode.Conflict;
                var statusCode = HttpStatusCode.Conflict;
                var results = _GetResults(statusCode);
                Assert.AreEqual(statusCode, results.StatusCode);
            }
            
            [TestMethod]
            public void GetGiftWrapAsync_Forbidden_Forbidden()
            {
                //HttpStatusCode.Forbidden;
                var statusCode = HttpStatusCode.Forbidden;
                var results = _GetResults(statusCode);
                Assert.AreEqual(statusCode, results.StatusCode);
            }
            
            [TestMethod]
            public void GetGiftWrapAsync_GatewayTimeout_GatewayTimeout()
            {
                //HttpStatusCode.GatewayTimeout;
                var statusCode = HttpStatusCode.GatewayTimeout;
                var results = _GetResults(statusCode);
                Assert.AreEqual(statusCode, results.StatusCode);
            }
            
            [TestMethod]
            public void GetGiftWrapAsync_Gone_Gone()
            {
                //HttpStatusCode.Gone;
                var statusCode = HttpStatusCode.Gone;
                var results = _GetResults(statusCode);
                Assert.AreEqual(statusCode, results.StatusCode);
            }

            [TestMethod]
            public void GetGiftWrapAsync_InteralServerError_InternalServerError()
            {
                var statusCode = HttpStatusCode.InternalServerError;
                var results = _GetResults(statusCode);
                Assert.AreEqual(statusCode, results.StatusCode);
            }
            
            [TestMethod]
            public void GetGiftWrapAsync_MethodNotAllowed_MethodNotAllowed()
            {
                //HttpStatusCode.MethodNotAllowed;
                var statusCode = HttpStatusCode.MethodNotAllowed;
                var results = _GetResults(statusCode);
                Assert.AreEqual(statusCode, results.StatusCode);
            }
            
            [TestMethod]
            public void GetGiftWrapAsync_Moved_Moved()
            {
                //HttpStatusCode.Moved;
                //also need to do the same as ambiguous
                var statusCode = HttpStatusCode.Moved;
                var results = _GetResults(statusCode);
                Assert.AreEqual(statusCode, results.StatusCode);
            }
            
            [TestMethod]
            public void GetGiftWrapAsync_MovedPermanently_MovedPermanently()
            {
                //HttpStatusCode.MovedPermanently;
                //do same as moved
                var statusCode = HttpStatusCode.MovedPermanently;
                var results = _GetResults(statusCode);
                Assert.AreEqual(statusCode, results.StatusCode);
            }
            
            [TestMethod]
            public void GetGiftWrapAsync_NoContent_NoContent()
            {
                //HttpStatusCode.NoContent;
                var statusCode = HttpStatusCode.NoContent;
                var results = _GetResults(statusCode);
                Assert.AreEqual(statusCode, results.StatusCode);
            }
            
            [TestMethod]
            public void GetGiftWrapAsync_NonAuthoritativeInformation_NonAuthoritativeInformation()
            {
                //HttpStatusCode.NonAuthoritativeInformation;
                var statusCode = HttpStatusCode.NonAuthoritativeInformation;
                var results = _GetResults(statusCode);
                Assert.AreEqual(statusCode, results.StatusCode);
            }
            
            [TestMethod]
            public void GetGiftWrapAsync_NotFound_NotFound()
            {
                var statusCode = HttpStatusCode.NotFound;
                var results = _GetResults(statusCode);
                Assert.AreEqual(statusCode, results.StatusCode);
            }
        
            [TestMethod]
            public void GetGiftWrapAsync_NotImplemented_NotImplemented()
            {
                //HttpStatusCode.NotImplemented;
                var statusCode = HttpStatusCode.NotImplemented;
                var results = _GetResults(statusCode);
                Assert.AreEqual(statusCode, results.StatusCode);
            }
            
            [TestMethod]
            public void GetGiftWrapAsync_OK_OK()
            {
                var statusCode = HttpStatusCode.OK;
                var results = _GetResults(statusCode);
                Assert.AreEqual(statusCode, results.StatusCode);
            }
            
            [TestMethod]
            public void GetGiftWrapAsync_RequestUriTooLong_RequestUriTooLong()
            {
                //HttpStatusCode.RequestUriTooLong;
                var statusCode = HttpStatusCode.RequestUriTooLong;
                var results = _GetResults(statusCode);
                Assert.AreEqual(statusCode, results.StatusCode);
            }
            
            [TestMethod]
            public void GetGiftWrapAsync_ServiceUnavailable_ServiceUnavailable()
            {
                //HttpStatusCode.ServiceUnavailable;
                //possibly need to program something to deal with this
                var statusCode = HttpStatusCode.ServiceUnavailable;
                var results = _GetResults(statusCode);
                Assert.AreEqual(statusCode, results.StatusCode);
            }
            
            [TestMethod]
            public void GetGiftWrapAsync_Unauthorized_Unauthorized()
            {
                //HttpStatusCode.Unauthorized;
                var statusCode = HttpStatusCode.Unauthorized;
                var results = _GetResults(statusCode);
                Assert.AreEqual(statusCode, results.StatusCode);
            }
        #endregion
    }
}
