using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace GetProductsMicroService.Controllers
{
    /// <summary>
    /// Class ProductController.
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    public class ProductController : ApiController
    {
        private readonly IProductRepo productRepo;

        #region Constructors
        private ProductController()
        {
            productRepo = new Repos.ProductRepo();
        }
        public ProductController(IProductRepo productRepo)
        {
            this.productRepo = productRepo;
        }
        #endregion

        /// <summary>
        /// get products as an asynchronous operation.
        /// </summary>
        /// <returns>Task&lt;HttpResponseMessage&gt;.</returns>
        [Route("Products")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetProductsAsync()
        {
            try
            {
                var httpProducts = await productRepo.GetHttpProducts();            
                var wcfProducts = await productRepo.GetWcfProducts();
                var products = new List<Dtos.Product>().AsEnumerable();
                products = (wcfProducts != Enumerable.Empty<Dtos.Product>()) ? products.Concat(wcfProducts) : products;
                products = (httpProducts != Enumerable.Empty<Dtos.Product>()) ? products.Concat(httpProducts) : products;
                if (products.Count() > 0)
                {
                    //remove the more expensive items
                    products = products.OrderBy(x => x.Price)
                                        .GroupBy(y => y.Id)
                                        .Select(group => group.First())
                                        .OrderBy(p => p.Id);
                    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                    response.Content = new StringContent(JsonConvert.SerializeObject(products), Encoding.UTF8, "application/json");
                    return response;
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.NoContent);
                }               
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("EXCEPTION CAUGHT: " + ex.Message);               
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }
    }
}
