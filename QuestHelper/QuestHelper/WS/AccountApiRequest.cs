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
            public string Username = string.Empty;
            public string Email = string.Empty;
            public string Password = string.Empty;
        }

        public class TokenRequestByOAuth
        {
            public string Username = string.Empty;
            public string Email = string.Empty;
            public string Locale = string.Empty;
            public string ImgUrl = string.Empty;
            public string AuthenticatorUserId = string.Empty;
        }

        public class TokenResponse
        {
            public string Access_Token = string.Empty;
            public string Username = string.Empty;
            public string Email = string.Empty;
            public string UserId = string.Empty;
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
                HandleError.Process("AccountApiRequest", "RegisterNewUser", e, false);
            }

            return authData;
        }

        public async System.Threading.Tasks.Task<TokenResponse> LoginByOAuthAsync(string username, string email, string locale, string imgUrl, string authenticatorUserId )
        {
            TokenResponse authData = new TokenResponse();

            JObject jsonRequestObject = JObject.FromObject(new TokenRequestByOAuth() { Username = username, Email = email, Locale = locale, ImgUrl = imgUrl, AuthenticatorUserId = authenticatorUserId });

            try
            {
                ApiRequest api = new ApiRequest();
                var response = await api.HttpRequestPOST($"{_hostUrl}/account/google", jsonRequestObject.ToString(), string.Empty);
                authData = JsonConvert.DeserializeObject<TokenResponse>(response);
            }
            catch (Exception e)
            {
                HandleError.Process("AccountApiRequest", "LoginByOAuth", e, false);
            }

            return authData;
        }
    }
}
