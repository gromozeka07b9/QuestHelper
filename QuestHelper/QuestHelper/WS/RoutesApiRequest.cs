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
            catch (Exception e)
            {
                HandleError.Process("RoutesApiRequest", "GetRoutes", e, false);
            }
            return deserializedValue;
        }
        public async Task<SyncObjectStatus> GetSyncStatus()
        {
            SyncObjectStatus deserializedValue = new SyncObjectStatus();
            JObject jsonObject = JObject.FromObject(new
            {
                /*RouteId = routeObject.RouteId,
                Name = routeObject.Name,
                CreateDate = routeObject.CreateDate.DateTime,
                UserId = 0*/
            });

            try
            {
                ApiRequest api = new ApiRequest();
                var response = await api.HttpRequestPOST($"{_hostUrl}/api/routessync", jsonObject.ToString());
                deserializedValue = JsonConvert.DeserializeObject<SyncObjectStatus>(response);
            }
            catch (Exception e)
            {
                HandleError.Process("RoutesApiRequest", "AddRoute", e, false);
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
            catch (Exception e)
            {
                HandleError.Process("RoutesApiRequest", "AddRoute", e, false);
            }
            return addResult;
        }
        public async Task<bool> DeleteRoute(Route routeObject)
        {
            bool deleteResult = false;
            try
            {
                ApiRequest api = new ApiRequest();
                deleteResult = await api.HttpRequestDELETE($"{_hostUrl}/api/routes/{routeObject.RouteId}");
            }
            catch (Exception e)
            {
                HandleError.Process("RoutesApiRequest", "DeleteRoute", e, false);
            }
            return deleteResult;
        }
    }
}
