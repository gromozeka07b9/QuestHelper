using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;

namespace QuestHelper.WS
{
    public class AccountApiRequest : IAccountApiRequest
    {
        private string _hostUrl = string.Empty;

        public AccountApiRequest(string hostUrl) => _hostUrl = hostUrl;
        public class TokenRequest
        {
            public string Username;
            public string Email;
            public string Password;
        }
        public class TokenResponse
        {
            public string Access_Token;
            public string Username;
            public string Email;
            public string UserId;
        }
        public async System.Threading.Tasks.Task<TokenResponse> GetTokenAsync(string login, string password, bool demomode = false)
        {
            TokenResponse authData = new TokenResponse();
            JObject jsonRequestObject = JObject.FromObject(new TokenRequest(){Username = login, Password = password});

            try
            {
                ApiRequest api = new ApiRequest();
                string demoroute = demomode ? "demotoken" : "";
                var response = await api.HttpRequestPOST($"{_hostUrl}/account/{demoroute}", jsonRequestObject.ToString(), string.Empty);
                authData = JsonConvert.DeserializeObject<TokenResponse>(response);
            }
            catch (Exception e)
            {
                HandleError.Process("AccountApiRequest", "GetToken", e, false);
            }

            return authData;
        }

        public async System.Threading.Tasks.Task<TokenResponse> RegisterNewUserAsync(string username, string password, string email)
        {
            TokenResponse authData = new TokenResponse();

            JObject jsonRequestObject = JObject.FromObject(new TokenRequest() { Username = username, Password = password, Email = email});

            try
            {
                ApiRequest api = new ApiRequest();
                var response = await api.HttpRequestPOST($"{_hostUrl}/account/new", jsonRequestObject.ToString(), string.Empty);
                authData = JsonConvert.DeserializeObject<TokenResponse>(response);
            }
            catch (Exception e)
            {
                HandleError.Process("AccountApiRequest", "GetToken", e, false);
            }

            return authData;
        }
    }
}
