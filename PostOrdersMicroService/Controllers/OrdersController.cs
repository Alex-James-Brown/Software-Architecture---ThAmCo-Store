using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace PostOrdersMicroService.Controllers
{
    /// <summary>
    /// Class OrdersController.
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    public class OrdersController : ApiController
    {
        #region Globals
        private readonly HttpClient _client;
        private readonly BazzasBazaar.IStore _bazzasStore;
        private List<Models.Order> _successfulOrders = new List<Models.Order>();
        #endregion
        #region Constructors

        public OrdersController()
        {
            _client = new HttpClient();
            _bazzasStore = new BazzasBazaar.StoreClient();
        }
        public OrdersController(HttpClient client, BazzasBazaar.IStore bazzasStore)
        {
            _client = client;
            _bazzasStore = bazzasStore;
        }
        #endregion
        #region Post Orders
        /// <summary>
        /// post selection box order as an asynchronous operation.
        /// </summary>
        /// <param name="orders">Selection box order.</param>
        /// <returns>Task&lt;HttpResponseMessage&gt;.</returns>
        [Route("api/Orders")]
        [HttpPost]
        public async Task<HttpResponseMessage> PostOrderAsync(IEnumerable<Models.PostOrder> orders)
        {         
            List<Task<HttpResponseMessage>> tasks = null;

            try
            {               
                //process all orders in parallel/async 
                tasks = new List<Task<HttpResponseMessage>>();          
                foreach (var order in orders)
                {
                    tasks.Add(Task.Run(() => ProcessOrder(order)));
                }                    
                await Task.WhenAll(tasks);
                foreach (var task in tasks)
                {
                    if (task.Result.IsSuccessStatusCode)
                    {
                        _successfulOrders.Add(task.Result.Content.ReadAsAsync<Models.Order>().Result);
                    }                   
                }

                //if all tasks completed successfully
                if (tasks.Count() == _successfulOrders.Count())
                {
                    return Request.CreateResponse(HttpStatusCode.OK, _successfulOrders);
                }
                else
                {
                    var cancelledResponse = await CancelOrders();
                    return cancelledResponse;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("EXCEPTION CAUGHT: " + ex.Message);

                var cancelledResponse = await CancelOrders();
                return cancelledResponse;
            }
        }
        /// <summary>
        /// Processes the order items.
        /// </summary>
        /// <param name="order">order item.</param>
        /// <returns>Task&lt;HttpResponseMessage&gt;.</returns>
        public async Task<HttpResponseMessage> ProcessOrder(Models.PostOrder order)
        {
            try
            {
                if (order.StoreName.ToLower() == "bazzasbazaar")
                {
                    //retry pattern
                    for (int i = 0; i < 3; i++)
                    {
                        try
                        {
                            //post order
                            var response = await _bazzasStore.CreateOrderAsync(order.AccountName, order.CardNumber, order.ProductId, order.Quantity);
                            if (response != null)
                            {
                                var jObj = JObject.Parse(JsonConvert.SerializeObject(response));
                                jObj.Add("StoreName", "BazzasBazaar");
                                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.DeserializeObject<Models.Order>(jObj.ToString()));
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine("EXCEPTION CAUGHT: " + ex.Message);
                            if (i == 2)
                            {
                                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                            }
                        }
                    }
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                }
                else
                {
                    var response = await PostHttpOrder(JObject.Parse(JsonConvert.SerializeObject(order)));
                    if (response.IsSuccessStatusCode)
                    {
                        var result = response.Content.ReadAsAsync<JObject>().Result;
                        result.Add("StoreName", order.StoreName);
                        return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.DeserializeObject<Models.Order>(result.ToString()));
                    }
                    else
                    {
                        return response;
                    }                      
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("EXCEPTION CAUGHT: " + ex.Message);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }
        /// <summary>
        /// Posts the HTTP selection box order.
        /// </summary>
        /// <param name="order">order item.</param>
        /// <returns>Task&lt;HttpResponseMessage&gt;.</returns>
        private async Task<HttpResponseMessage> PostHttpOrder(JObject order)
        {
            string uri = "";
            if (order != null)
            {
                if (order.Property("StoreName").Value.ToString().ToLower() == "khanskwikimart")
                {
                    uri = "http://khanskwikimart.azurewebsites.net/api/giftwrapping/orders";                   
                }
                else
                {
                    uri = "http://" + order.Property("StoreName").Value.ToString() + ".azurewebsites.net/api/order";
                }
                order.Property("StoreName").Remove();
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            //retry pattern
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    //post order
                    var content = new StringContent(order.ToString(), Encoding.UTF8, "application/json");
                    var response = await _client.PostAsync(uri, content);
                    if (response.IsSuccessStatusCode)
                    {
                        return response;
                    }
                    else if (i == 2)
                    {
                        return response;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("EXCEPTION THROWN: " + ex.Message);
                    if (i == 2) { return new HttpResponseMessage(HttpStatusCode.InternalServerError); }
                }
            }
            return new HttpResponseMessage(HttpStatusCode.InternalServerError);
        }
        #endregion
        #region Cancel Orders
        /// <summary>
        /// Cancels the selection box order.
        /// </summary>
        /// <returns>Task&lt;HttpResponseMessage&gt;.</returns>
        private async Task<HttpResponseMessage> CancelOrders()
        {
            JObject orderError;
            var cancelled = await CancelOrderAsync();
            if (cancelled)
            {
                orderError = JObject.FromObject(new Models.OrderError()
                {
                    ErrorMessage = "Orders successfully cancelled"
                });
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(orderError.ToString(), Encoding.UTF8, "application/json")
                };
            }
            else
            {
                orderError = JObject.FromObject(new Models.OrderError()
                {
                    ErrorMessage = "Orders unsuccessfully cancelled",
                    UnsuccessfulOrders = _successfulOrders
                });
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(orderError.ToString(), Encoding.UTF8, "application/json")
                };
            }
        }
        /// <summary>
        /// cancels all item orders as an asynchronous operation.
        /// </summary>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        private async Task<bool> CancelOrderAsync()
        {
            try
            {
                //cancel orders in parallel/async
                var tasks = new List<Task<bool>>();
                foreach (var order in _successfulOrders)
                    tasks.Add(Task.Run(() => CancelOrder(order)));

                await Task.WhenAll(tasks);
                if (!tasks.Any(x => x.Result == false))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("EXCEPTION CAUGHT: " + ex.Message);
                return false;
            }
        }
        /// <summary>
        /// Cancels an order item.
        /// </summary>
        /// <param name="order">Order item</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        private async Task<bool> CancelOrder(Models.Order order)
        {
            if (order.StoreName.ToLower() == "bazzasbazaar")
            {
                _successfulOrders.Remove(order);
                return _bazzasStore.CancelOrderById(order.Id);
            }
            else if (order.StoreName.ToLower() == "khanskwikimart")
            {
                //khanskwikimart does not have a cancel order so dont waste processing time
                return false;
            }
            else
            {
                //retry pattern
                for (int i = 0; i < 3; i++)
                {
                    string uri = "http://" + order.StoreName + ".azurewebsites.net/api/order/" + order.Id;                    
                    var response = await _client.DeleteAsync(uri);
                    if (response.IsSuccessStatusCode)
                    {
                        _successfulOrders.Remove(order);
                        return true;
                    }
                    else if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        return false;
                    }
                }            
                return false;
            }
        }
        #endregion
    }
}