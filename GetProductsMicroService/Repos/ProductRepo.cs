using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.ServiceModel;
using System.Threading.Tasks;

namespace GetProductsMicroService.Repos
{
    /// <summary>
    /// Class ProductRepo.
    /// </summary>
    /// <seealso cref="GetProductsMicroService.IProductRepo" />
    public class ProductRepo : IProductRepo
    {
        #region Globals
        private readonly HttpClient _httpClient;
        private readonly BazzasBazaar.IStore _storeClient;
        private readonly Dictionary<string, Uri> _serviceUris;
        #endregion
        #region Constructors
        public ProductRepo()
        {
            _httpClient = new HttpClient();
            _storeClient = new BazzasBazaar.StoreClient();
            _serviceUris = new Dictionary<string, Uri>
            {
                { "DodgyDealers", new Uri("http://dodgydealers.azurewebsites.net/api/product") },
                { "UnderCutters", new Uri("http://undercutters.azurewebsites.net/api/product") }         
            };
        }
        public ProductRepo(BazzasBazaar.IStore storeClient)
        {
            _storeClient = storeClient;
        }
        public ProductRepo(HttpClient httpClient, Dictionary<string, Uri> serviceUris)
        {
            _httpClient = httpClient;
            _serviceUris = serviceUris;
        }
        public ProductRepo(HttpClient httpClient, BazzasBazaar.IStore storeClient, Dictionary<string, Uri> serviceUris)
        {
            _httpClient = httpClient;
            _storeClient = storeClient;
            _serviceUris = serviceUris;
        }
        #endregion
        #region Get Wcf Products
        /// <summary>
        /// Gets the WCF products.
        /// </summary>
        /// <returns>Task&lt;IEnumerable&lt;Dtos.Product&gt;&gt;.</returns>
        public Task<IEnumerable<Dtos.Product>> GetWcfProducts()
        {
            //retry pattern
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    //get all products
                    BazzasBazaar.Product[] bazzasProducts = _storeClient.GetFilteredProducts(null, null, null, null);
                    if (bazzasProducts.Count() > 0)
                    {
                        IEnumerable<Dtos.Product> products = bazzasProducts.Select(p => new Dtos.Product
                        {
                            Id = p.Id,
                            Ean = p.Ean,
                            CategoryId = p.CategoryId,
                            CategoryName = p.CategoryName,
                            Name = p.Name,
                            Description = p.Description,
                            Price = p.PriceForOne,
                            InStock = p.InStock,
                            ExpectedRestock = p.ExpectedRestock,
                            StoreName = "BazzasBazaar"
                        });
                        return Task.FromResult(products);
                    }
                }
                catch (TimeoutException ex)
                {
                    System.Diagnostics.Debug.WriteLine("EXCEPTION CAUGHT: " + ex.Message);
                    if (i == 2) { return Task.FromResult(Enumerable.Empty<Dtos.Product>()); }
                }
                catch (FaultException ex)
                {
                    System.Diagnostics.Debug.WriteLine("EXCEPTION CAUGHT: " + ex.Message);
                    if (i == 2) { return Task.FromResult(Enumerable.Empty<Dtos.Product>()); }
                }
                catch (CommunicationException ex)
                {
                    System.Diagnostics.Debug.WriteLine("EXCEPTION CAUGHT: " + ex.Message);
                    if (i == 2) { return Task.FromResult(Enumerable.Empty<Dtos.Product>()); }
                }
            }
            return Task.FromResult(Enumerable.Empty<Dtos.Product>());
        }
        #endregion
        #region Get Http Products
        /// <summary>
        /// Gets the HTTP products.
        /// </summary>
        /// <returns>Task&lt;IEnumerable&lt;Dtos.Product&gt;&gt;.</returns>
        public Task<IEnumerable<Dtos.Product>> GetHttpProducts()
        {
            using (_httpClient)
            {
                //retry pattern
                for (int i = 0; i < 3; i++)
                {
                    try
                    {
                        IEnumerable<Dtos.Product> allProducts = new List<Dtos.Product>().AsEnumerable();

                        foreach (var item in _serviceUris)
                        {
                            //get products from store
                            HttpResponseMessage response = new HttpResponseMessage();
                            response = _httpClient.GetAsync(item.Value).Result;
                            if (response.IsSuccessStatusCode)
                            {
                                var serviceProducts = response.Content.ReadAsAsync<IEnumerable<Dtos.Product>>().Result;
                                if (serviceProducts.Count() > 0)
                                {
                                    //add store name to the products
                                    serviceProducts = serviceProducts.Select(p =>
                                    {
                                        p.StoreName = item.Key;
                                        return p;
                                    });
                                    allProducts = allProducts.Concat(serviceProducts);
                                }
                            }
                        }
                        return Task.FromResult(allProducts);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("EXCEPTION CAUGHT: " + ex.Message);
                        if (i == 2) { return Task.FromResult(Enumerable.Empty<Dtos.Product>()); }
                    }
                }
            }
            return Task.FromResult(Enumerable.Empty<Dtos.Product>());
        }
        #endregion
    }
}