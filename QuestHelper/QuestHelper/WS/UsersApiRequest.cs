using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using QuestHelper.LocalDB.Model;
using QuestHelper.Model;

namespace QuestHelper.WS
{
    public class UsersApiRequest
    {
        private string _hostUrl = string.Empty;
        private string _authToken = string.Empty;
        public HttpStatusCode LastHttpStatusCode;

        public UsersApiRequest(string hostUrl, string authToken)
        {
            _hostUrl = hostUrl;
            _authToken = authToken;
        }

        public async Task<List<ViewUserInfo>> SearchUsers(string textForSearch)
        {
            List<ViewUserInfo> deserializedValue = new List<ViewUserInfo>();
            try
            {
                ApiRequest api = new ApiRequest();
                var response = await api.HttpRequestGET($"{this._hostUrl}/user/search/{textForSearch}", _authToken);
                LastHttpStatusCode = api.LastHttpStatusCode;
                deserializedValue = JsonConvert.DeserializeObject<List<ViewUserInfo>>(response);
            }
            catch (Exception e)
            {
                HandleError.Process("UsersApiRequest", "SearchUsers", e, false);
            }
            return deserializedValue;
        }

        public async Task<ViewUserInfo> GetUserAsync(string userId)
        {
            try
            {
                ApiRequest api = new ApiRequest();
                var response = await api.HttpRequestGET($"{_hostUrl}/account/{userId}", _authToken);
                LastHttpStatusCode = api.LastHttpStatusCode;
                return JsonConvert.DeserializeObject<ViewUserInfo>(response);
            }
            catch (Exception e)
            {
                HandleError.Process("UsersApiRequest", "GetUserAsync", e, false);
            }

            return new ViewUserInfo();
        }

    }
}
