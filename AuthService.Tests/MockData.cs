using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Tests
{
    class MockData
    {
        public JObject CreateUserMock()
        {
            string json = "{\"Email\":\"mock@mock.mock\", \"UserName\":\"mock\", \"Password\":\"MockP@ss\", \"ConfirmPassword\":\"MockP@ss\", \"FirstName\":\"Mock\", \"LastName\":\"Mock\"}";
            var jObj = JObject.Parse(json);
            return jObj;
        }
    }
}
