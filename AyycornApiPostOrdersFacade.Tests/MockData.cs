using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AyycornApiPostOrdersFacade.Tests
{
    class MockData
    {
        public IEnumerable<Models.PostOrder> SelectionBoxMock()
        {
            var mock = new List<Models.PostOrder>();
            mock.Add(new Models.PostOrder
            {
                AccountName = "mock",
                CardNumber = "1234567890",
                ProductId = 1,
                Quantity = 1,
                StoreName = "BazzasBazaar"
            });
            mock.Add(new Models.PostOrder
            {
                AccountName = "mock",
                CardNumber = "1234567890",
                ProductId = 1,
                Quantity = 1,
                StoreName = "Undercutters"
            });
            mock.Add(new Models.PostOrder
            {
                AccountName = "mock",
                CardNumber = "1234567890",
                ProductId = 1,
                Quantity = 1,
                StoreName = "DodgyDealers"
            });
            mock.Add(new Models.PostOrder
            {
                AccountName = "mock",
                CardNumber = "1234567890",
                ProductId = 1,
                Quantity = 1,
                StoreName = "KhansKwikiMart"
            });
            return mock.AsEnumerable();
        }
        public JArray ExpectedOrderMock()
        {
            var responses = new List<Models.Order>();
            responses.Add(new Models.Order
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
                StoreName = "BazzasBazaar"
            });
            responses.Add(new Models.Order
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
                StoreName = "Undercutters"
            });
            responses.Add(new Models.Order
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
                StoreName = "DodgyDealers"
            });
            responses.Add(new Models.Order
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

            var jStr = JsonConvert.SerializeObject(responses);
            var jArr = JArray.Parse(jStr);
            return jArr;
        }
        public Models.JReturnModel UnsuccessfulOrderMock()
        {
            var model = new Models.JReturnModel()
            {
                Success = false,
                ErrorMessage = "Orders successfully cancelled"
            };
            return model;
        }
        public JObject UnsuccessfulOrderResponseMock()
        {
            var model = JObject.FromObject(new Models.OrderError()
            {
                ErrorMessage = "Orders successfully cancelled"
            });
            return model;
        }
        public Models.JReturnModel UnsuccessfulOrderCancelMock()
        {
            var responses = new List<Models.Order>();
            responses.Add(new Models.Order
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
                StoreName = "DodgyDealers"
            });
            responses.Add(new Models.Order
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

            var errorModel = new Models.JReturnModel()
            {
                Success = false,
                ErrorMessage = "Orders unsuccessfully cancelled",
                Json = JArray.FromObject(responses)
            };
            return errorModel;
        }
        public JObject UnsuccessfulOrderCancelResponseMock()
        {
            var responses = new List<Models.Order>();
            responses.Add(new Models.Order
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
                StoreName = "DodgyDealers"
            });
            responses.Add(new Models.Order
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

            var errorModel = JObject.FromObject(new Models.OrderError()
            {
                ErrorMessage = "Orders unsuccessfully cancelled",
                UnsuccessfulOrders = responses
            });
            return errorModel;
        }
    }
}
