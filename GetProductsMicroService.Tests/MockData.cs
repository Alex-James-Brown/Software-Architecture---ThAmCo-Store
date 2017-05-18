using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GetProductsMicroService.Tests
{
    class MockData
    {
        public Task<IEnumerable<Dtos.Product>> WcfData()
        {
            List<Dtos.Product> mockData = new List<Dtos.Product>();
                
            mockData.Add(new Dtos.Product
            {
                Id = 1,
                Ean = "5 102310 300103",
                CategoryId = 3,
                CategoryName = "Covers",
                Name = "Cloth Cover",
                Description = "Lamely adapted used and dirty teatowel",
                Price = 3.45,
                ExpectedRestock = null,
                StoreName = "BazzasBazaar"
            });
            mockData.Add(new Dtos.Product
            {
                Id = 2,
                Ean = "5 102310 300100",
                CategoryId = 3,
                CategoryName = "Covers",
                Name = "Leather Cover",
                Description = "A leather cover",
                Price = 5.00,
                ExpectedRestock = null,
                StoreName = "BazzasBazaar"
            });
            return Task.FromResult(mockData.AsEnumerable());
        }
        public Task<IEnumerable<Dtos.Product>> HttpData()
        {
            List<Dtos.Product> mockData = new List<Dtos.Product>();

            mockData.Add(new Dtos.Product
            {
                Id = 1,
                Ean = "5 102310 300103",
                CategoryId = 3,
                CategoryName = "Covers",
                Name = "Cloth Cover",
                Description = "Lamely adapted used and dirty teatowel",
                Price = 3.50,
                ExpectedRestock = null,
                StoreName = "UnderCutters"
            });
            mockData.Add(new Dtos.Product
            {
                Id = 2,
                Ean = "5 102310 300100",
                CategoryId = 3,
                CategoryName = "Covers",
                Name = "Leather Cover",
                Description = "A leather cover",
                Price = 4.00,
                ExpectedRestock = null,
                StoreName = "DodgyDealers"
            });
            return Task.FromResult(mockData.AsEnumerable());
        }
        public Task<IEnumerable<Dtos.Product>> HttpDataFake()
        {
            List<Dtos.Product> mockData = new List<Dtos.Product>();

            mockData.Add(new Dtos.Product
            {
                Id = 1,
                Ean = "5 102310 300103",
                CategoryId = 3,
                CategoryName = "Covers",
                Name = "Cloth Cover",
                Description = "Lamely adapted used and dirty teatowel",
                Price = 3.50,
                ExpectedRestock = null,
                StoreName = "Fake"
            });
            mockData.Add(new Dtos.Product
            {
                Id = 2,
                Ean = "5 102310 300100",
                CategoryId = 3,
                CategoryName = "Covers",
                Name = "Leather Cover",
                Description = "A leather cover",
                Price = 4.00,
                ExpectedRestock = null,
                StoreName = "Fake"
            });
            return Task.FromResult(mockData.AsEnumerable());
        }
        public HttpResponseMessage TestGetCheapestProductsData()
        {
            List<Dtos.Product> mockData = new List<Dtos.Product>();

            mockData.Add(new Dtos.Product
            {
                Id = 1,
                Ean = "5 102310 300103",
                CategoryId = 3,
                CategoryName = "Covers",
                Name = "Cloth Cover",
                Description = "Lamely adapted used and dirty teatowel",
                Price = 3.45,
                ExpectedRestock = null,
                StoreName = "BazzasBazaar"
            });
            mockData.Add(new Dtos.Product
            {
                Id = 2,
                Ean = "5 102310 300100",
                CategoryId = 3,
                CategoryName = "Covers",
                Name = "Leather Cover",
                Description = "A leather cover",
                Price = 4.00,
                ExpectedRestock = null,
                StoreName = "DodgyDealers"
            });
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(JsonConvert.SerializeObject(mockData), Encoding.UTF8, "application/json");
            return response;
        }
    }
}
