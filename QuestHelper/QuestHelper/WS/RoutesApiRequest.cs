using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Net.Http;
using QuestHelper.Model.DB;
using Newtonsoft.Json.Linq;

namespace QuestHelper.WS
{
    public class RoutesApiRequest : IRoutesApiRequest
    {
        private string _hostUrl = string.Empty;
        public RoutesApiRequest(string hostUrl)
        {
            _hostUrl = hostUrl;
        }
        public async Task<List<Route>> GetRoutes()
        {
            List<Route> deserializedValue = new List<Route>();
            try
            {
                ApiRequest api = new ApiRequest();
                var response = await api.HttpRequestGET($"{this._hostUrl}/api/routes");
                deserializedValue = JsonConvert.DeserializeObject<List<Route>>(response);
            }
            catch (Exception)
            {

            }
            return deserializedValue;
        }
        public async Task<bool> AddRoute(Route routeObject)
        {
            bool addResult = false;
            JObject jsonObject = JObject.FromObject(new {
                RouteId = routeObject.RouteId,
                Name = routeObject.Name,
                CreateDate = routeObject.CreateDate.DateTime,
                UserId = 0
            });
            
            try
            {
                ApiRequest api = new ApiRequest();
                await api.HttpRequestPOST($"{_hostUrl}/api/routes", jsonObject.ToString());
                addResult = true;
            }
            catch (Exception)
            {
            }
            return addResult;
        }
    }
}
