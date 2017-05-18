using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AyycornMVCUsersFacade.Tests
{
    class MockData
    {
        public JObject TokenMock()
        {
            var tokenStr = "{ \"access_token\":\"eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJuYW1laWQiOiJiNTQyYmNhYi1mZDBiLTQzZGYtODc0MC1lODJkYzA1MjZkNTYiLCJ1bmlxdWVfbmFtZSI6IlN1cGVyVXNlciIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vYWNjZXNzY29udHJvbHNlcnZpY2UvMjAxMC8wNy9jbGFpbXMvaWRlbnRpdHlwcm92aWRlciI6IkFTUC5ORVQgSWRlbnRpdHkiLCJBc3BOZXQuSWRlbnRpdHkuU2VjdXJpdHlTdGFtcCI6ImRjYzQ0NDgxLWQ0NjMtNDA5Ny04YTBlLWU0NGFmOGVmOWQ0ZSIsInJvbGUiOlsiU3VwZXJBZG1pbiIsIkFkbWluIl0sImlzcyI6Imh0dHA6Ly9sb2NhbGhvc3Q6NDAyMCIsImF1ZCI6IjQxNGUxOTI3YTM4ODRmNjhhYmM3OWY3MjgzODM3ZmQxIiwiZXhwIjoxNDgzNzE4MDA3LCJuYmYiOjE0ODM2MzE2MDd9.wRYDQsycgYKQhdCBMAbW1a5bmUPq3cUse0ths8b7lco\",\"token_type\":\"bearer\",\"expires_in\":86399}";
            var token = JObject.Parse(tokenStr);
            return token;
        }
        public JObject InvalidCredentialsMock()
        {
            var jStr = "{ \"error\":\"invalid_grant\",\"error_description\":\"The user name or password is incorrect.\"}";
            var jObj = JObject.Parse(jStr);
            return jObj;
        }
        public JObject UserInfoMock()
        {
            var jStr = "{\"url\": \"http://mocktest.com\",\"id\": \"mockId\",\"userName\": \"mock\",\"fullName\": \"Mockety Mock\",\"email\": \"mock@mock.com\",\"emailConfirmed\": true, \"level\": 1,\"joinDate\": \"2016-12-30T16:02:22.86\",\"roles\": [\"SuperAdmin\",\"Admin\"],\"claims\": []}";
            var jObj = JObject.Parse(jStr);
            return jObj;
        }
        public JObject AuthDenied()
        {
            var jStr = "{\"message\": \"Authorization has been denied for this request.\"}";
            var jObj = JObject.Parse(jStr);
            return jObj;
    }
    }
}
