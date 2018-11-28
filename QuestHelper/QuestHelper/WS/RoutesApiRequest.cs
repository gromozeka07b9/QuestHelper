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
    public class RoutesApiRequest : IRoutesApiRequest, IDownloadable<ViewRoute>, IUploadable<ViewRoute>
    {
        private string _hostUrl = string.Empty;
        public RoutesApiRequest(string hostUrl) => _hostUrl = hostUrl;

        public async Task<List<Route>> GetRoutes()
        {
            List<Route> deserializedValue = new List<Route>();
            try
            {
                ApiRequest api = new ApiRequest();
                var response = await api.HttpRequestGET($"{this._hostUrl}/routes");
                deserializedValue = JsonConvert.DeserializeObject<List<Route>>(response);
            }
            catch (Exception e)
            {
                HandleError.Process("RoutesApiRequest", "GetRoutes", e, false);
            }
            return deserializedValue;
        }


        public async Task<SyncObjectStatus> GetSyncStatus(IEnumerable<Tuple<string, int>> routes)
        {
            SyncObjectStatus requestValue = new SyncObjectStatus();
            foreach (var route in routes)
            {
                requestValue.Statuses.Add(new SyncObjectStatus.ObjectStatus()
                {
                    ObjectId = route.Item1,
                    Version = route.Item2
                });
            }
            JObject jsonRequestObject = JObject.FromObject(requestValue);

            SyncObjectStatus deserializedValue = new SyncObjectStatus();
            try
            {
                ApiRequest api = new ApiRequest();
                var response = await api.HttpRequestPOST($"{_hostUrl}/routes/sync", jsonRequestObject.ToString());
                deserializedValue = JsonConvert.DeserializeObject<SyncObjectStatus>(response);
            }
            catch (Exception e)
            {
                HandleError.Process("RoutesApiRequest", "GetSyncStatus", e, false);
            }

            return deserializedValue;
        }
        public async Task<bool> DeleteRoute(Route routeObject)
        {
            bool deleteResult = false;
            try
            {
                ApiRequest api = new ApiRequest();
                deleteResult = await api.HttpRequestDELETE($"{_hostUrl}/routes/{routeObject.RouteId}");
            }
            catch (Exception e)
            {
                HandleError.Process("RoutesApiRequest", "DeleteRoute", e, false);
            }
            return deleteResult;
        }

        public async Task<ISaveable> DownloadFromServerAsync(string id)
        {
            ViewRoute deserializedValue = new ViewRoute();
            try
            {
                ApiRequest api = new ApiRequest();
                var response = await api.HttpRequestGET($"{this._hostUrl}/routes/{id}");
                deserializedValue = JsonConvert.DeserializeObject<ViewRoute>(response);
            }
            catch (Exception e)
            {
                HandleError.Process("RoutesApiRequest", "GetRoute", e, false);
            }
            return deserializedValue;
        }

        public async Task<bool> UploadToServerAsync(string jsonStructure)
        {
            bool addResult = false;
            try
            {
                ApiRequest api = new ApiRequest();
                await api.HttpRequestPOST($"{_hostUrl}/routes", jsonStructure);
                addResult = true;
            }
            catch (Exception e)
            {
                HandleError.Process("RoutesApiRequest", "AddRoute", e, false);
            }
            return addResult;
        }
    }
}
