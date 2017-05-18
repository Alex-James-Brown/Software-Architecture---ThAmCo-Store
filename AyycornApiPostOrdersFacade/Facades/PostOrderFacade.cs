using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace AyycornApiPostOrdersFacade.Facades
{
    /// <summary>
    /// Class PostOrderFacade.
    /// </summary>
    public class PostOrderFacade
    {
        private readonly HttpClient _client;

        #region constructors
        public PostOrderFacade()
        {
            _client = new HttpClient();
        }
        public PostOrderFacade(HttpClient client)
        {
            _client = client;
        }
        #endregion

        /// <summary>
        /// post orders as an asynchronous operation.
        /// </summary>
        /// <param name="orders">The selection box orders.</param>
        /// <returns>Task&lt;Models.JReturnModel&gt;.</returns>
        public async Task<Models.JReturnModel> PostOrderAsync(IEnumerable<Models.PostOrder> orders)
        {
            HttpResponseMessage response;
            using (_client)
            {
                string uri = "http://AyycornPostOrdersMicroService.azurewebsites.net/api/order";
                var content = new StringContent(JsonConvert.SerializeObject(orders), Encoding.UTF8, "application/json");
                response = await _client.PostAsync(uri, content);
            }

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsAsync<IEnumerable<Models.Order>>();
                return new Models.JReturnModel()
                {
                    Success = true,
                    Json = JArray.FromObject(result)
                };
            }
            else
            {
                var result = await response.Content.ReadAsAsync<Models.OrderError>();
                return new Models.JReturnModel()
                {
                    Success = response.IsSuccessStatusCode,
                    ErrorMessage = result.ErrorMessage,
                    Json = result.UnsuccessfulOrders != null ? JArray.FromObject(result.UnsuccessfulOrders) : null
                };
            }

            
        }
    }
}