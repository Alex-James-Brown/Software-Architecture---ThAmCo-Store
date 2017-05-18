using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PostOrdersMicroService.Tests
{
    class MockData
    {
        public IEnumerable<Models.PostOrder> MockSelectionBox()
        {
            var items = new List<Models.PostOrder>();
            items.Add(new Models.PostOrder
            {
                AccountName = "mock", CardNumber = "1234567890", ProductId = 1, Quantity = 1, StoreName = "BazzasBazaar"
            });
            items.Add(new Models.PostOrder
            {
                AccountName = "mock", CardNumber = "1234567890", ProductId = 1, Quantity = 1, StoreName = "Undercutters"
            });
            items.Add(new Models.PostOrder
            {
                AccountName = "mock", CardNumber = "1234567890", ProductId = 1, Quantity = 1, StoreName = "DodgyDealers"
            });
            items.Add(new Models.PostOrder
            {
                AccountName = "mock", CardNumber = "1234567890", ProductId = 1, Quantity = 1, StoreName = "KhansKwikiMart"
            });
            return items.AsEnumerable();
        }
        public BazzasBazaar.Order BazzaOrder()
        {
            return new BazzasBazaar.Order
            {
                AccountName = "mock", CardNumber = "1234567890", ProductId = 1, Quantity = 1, Id = 1, ProductEan = "5 01946 61",
                ProductName = "mockProd", TotalPrice = 1.76, When = Convert.ToDateTime("04/01/2017 16:00:00.00")
            };
        }
        public HttpResponseMessage HttpOrder()
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(new
                {
                    AccountName = "mock",
                    CardNumber = "1234567890",
                    ProductId = 1,
                    Quantity = 1,
                    Id = 1,
                    ProductEan = "5 01946 61",
                    ProductName = "mockProd",
                    TotalPrice = 1.76,
                    When = Convert.ToDateTime("04/01/2017 16:00:00.00")
                }), Encoding.UTF8, "application/json")
            };
        }
        public HttpResponseMessage Expected()
        {
            var responses = new List<Models.Order>();
            responses.Add(new Models.Order
            {
                AccountName = "mock", CardNumber = "1234567890", ProductId = 1, Quantity = 1, Id = 1, ProductEan = "5 01946 61",
                ProductName = "mockProd", TotalPrice = 1.76, When = Convert.ToDateTime("04/01/2017 16:00:00.00"), StoreName = "BazzasBazaar"
            });
            responses.Add(new Models.Order
            {
                AccountName = "mock", CardNumber = "1234567890", ProductId = 1, Quantity = 1, Id = 1, ProductEan = "5 01946 61",
                ProductName = "mockProd", TotalPrice = 1.76, When = Convert.ToDateTime("04/01/2017 16:00:00.00"), StoreName = "Undercutters"
            });
            responses.Add(new Models.Order
            {
                AccountName = "mock", CardNumber = "1234567890", ProductId = 1, Quantity = 1, Id = 1, ProductEan = "5 01946 61",
                ProductName = "mockProd", TotalPrice = 1.76, When = Convert.ToDateTime("04/01/2017 16:00:00.00"), StoreName = "DodgyDealers"
            });
            responses.Add(new Models.Order
            {
                AccountName = "mock", CardNumber = "1234567890", ProductId = 1, Quantity = 1, Id = 1, ProductEan = "5 01946 61",
                ProductName = "mockProd", TotalPrice = 1.76, When = Convert.ToDateTime("04/01/2017 16:00:00.00"), StoreName = "KhansKwikiMart"
            });

            var jStr = JsonConvert.SerializeObject(responses.AsEnumerable());
            return new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent(jStr, Encoding.UTF8, "application/json")
            };
        }

        public HttpResponseMessage FailedKwikiCancelMock()
        {
            var response = new List<Models.Order>();
            response.Add(new Models.Order()
            {
                AccountName = "mock",
                CardNumber = "1234567890",
                ProductId = 1,
                Quantity = 1,
                Id = 1,
                ProductEan = "5 01946 61",
                ProductName = "mockProd",
                TotalPrice = 1.76,
                When = Convert.ToDateTime("04/01/2017 16:00:00.00"),
                StoreName = "KhansKwikiMart"
            });
            return new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(JsonConvert.SerializeObject(response.AsEnumerable()), Encoding.UTF8, "application/json")
            };
        }
        public HttpResponseMessage SuccessfulCancelMock()
        {
            return new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent("Orders successfully cancelled")
            };
        }
    }
}
