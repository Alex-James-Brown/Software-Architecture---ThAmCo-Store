using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace GiftWrapMicroService.Controllers
{
    /// <summary>
    /// Class GiftWrapController.
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    public class GiftWrapController : ApiController
    {
        private readonly HttpClient _client;
        public GiftWrapController()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri("http://khanskwikimart.azurewebsites.net/api/");
        }
        public GiftWrapController (HttpClient client)
        {
            _client = client;
        }


        /// <summary>
        /// get gift wrap as an asynchronous operation.
        /// </summary>
        /// <returns>Task&lt;HttpResponseMessage&gt;.</returns>
        [Route("giftwrapping")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetGiftWrapAsync()
        {
            HttpResponseMessage response = new HttpResponseMessage();
            using (_client)
            {
                //retry pattern
                for (int i = 0; i < 5; i++)
                {
                    try
                    {    
                        //get giftwrappings                   
                        response = await _client.GetAsync("giftwrapping/products");
                        if (response.IsSuccessStatusCode)
                        {
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        response = Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
                    }
                }
            }
            return response;
        }
    }
}
