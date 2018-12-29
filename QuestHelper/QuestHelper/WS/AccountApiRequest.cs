using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace QuestHelper.WS
{
    public class AccountApiRequest : IAccountApiRequest
    {
        private string _hostUrl = string.Empty;

        public AccountApiRequest(string hostUrl) => _hostUrl = hostUrl;
        public class TokenRequest
        {
            public string Username;
            public string Password;
        }
        public class TokenResponse
        {
            public string Access_Token;
            public string Username;
        }
        public async System.Threading.Tasks.Task<string> GetTokenAsync(string login, string password)
        {
            string token = string.Empty;

            JObject jsonRequestObject = JObject.FromObject(new TokenRequest(){Username = login, Password = password});

            try
            {
                ApiRequest api = new ApiRequest();
                var response = await api.HttpRequestPOST($"{_hostUrl}/account", jsonRequestObject.ToString(), string.Empty);
                var result = JsonConvert.DeserializeObject<TokenResponse>(response);
                if (!string.IsNullOrEmpty(result.Access_Token))
                {
                    token = result.Access_Token;
                }
            }
            catch (Exception e)
            {
                HandleError.Process("AccountApiRequest", "GetToken", e, false);
            }

            return token;
        }
    }
}
