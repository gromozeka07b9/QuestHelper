using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Net.Http;
using QuestHelper.LocalDB.Model;
using Newtonsoft.Json.Linq;
using QuestHelper.Model;

namespace QuestHelper.WS
{
    public class RoutePointsApiRequest : IRoutePointsApiRequest, IDownloadable<ViewRoute>, IUploadable<ViewRoute>
    {
        private string _hostUrl = string.Empty;
        private string _authToken = string.Empty;
        public HttpStatusCode LastHttpStatusCode;
        public RoutePointsApiRequest(string hostUrl, string authToken)
        {
            _hostUrl = hostUrl;
            _authToken = authToken;
        }
        public async Task<SyncObjectStatus> GetSyncStatus(IEnumerable<Tuple<string, int>> points)
        {
            SyncObjectStatus requestValue = new SyncObjectStatus();
            foreach (var point in points)
            {
                requestValue.Statuses.Add(new SyncObjectStatus.ObjectStatus()
                {
                    ObjectId = point.Item1,
                    Version = point.Item2
                });
            }
            JObject jsonRequestObject = JObject.FromObject(requestValue);

            SyncObjectStatus deserializedValue = new SyncObjectStatus();
            try
            {
                ApiRequest api = new ApiRequest();
                var response = await api.HttpRequestPOST($"{_hostUrl}/routepoints/sync", jsonRequestObject.ToString(), _authToken);
                LastHttpStatusCode = api.LastHttpStatusCode;
                deserializedValue = JsonConvert.DeserializeObject<SyncObjectStatus>(response);
            }
            catch (Exception e)
            {
                HandleError.Process("PointsApiRequest", "GetSyncStatus", e, false);
            }

            return deserializedValue;
        }

        public async Task<List<RoutePoint>> GetRoutePoints(string routeId)
        {
            List<RoutePoint> deserializedValue = new List<RoutePoint>();
            try
            {
                ApiRequest api = new ApiRequest();
                var response = await api.HttpRequestGET($"{this._hostUrl}/routepoints/{routeId}", _authToken);
                LastHttpStatusCode = api.LastHttpStatusCode;
                deserializedValue = JsonConvert.DeserializeObject<List<RoutePoint>>(response);
            }
            catch (Exception e)
            {
                HandleError.Process("RoutePointsApiRequest", "GetRoutePoints", e, false);
            }
            return deserializedValue;
        }

        public async Task<ISaveable> DownloadFromServerAsync(string routePointId)
        {
            ViewRoutePoint deserializedValue = new ViewRoutePoint();
            try
            {
                ApiRequest api = new ApiRequest();
                var response = await api.HttpRequestGET($"{this._hostUrl}/routepoints/{routePointId}", _authToken);
                LastHttpStatusCode = api.LastHttpStatusCode;
                deserializedValue = JsonConvert.DeserializeObject<ViewRoutePoint>(response);
            }
            catch (Exception e)
            {
                HandleError.Process("RoutePointsApiRequest", "GetRoutePoint", e, false);
            }
            return deserializedValue;
        }

        public async Task<bool> UploadToServerAsync(string jsonStructure)
        {
            bool addResult = false;

            try
            {
                ApiRequest api = new ApiRequest();
                await api.HttpRequestPOST($"{_hostUrl}/routepoints", jsonStructure, _authToken);
                LastHttpStatusCode = api.LastHttpStatusCode;
                addResult = true;
            }
            catch (Exception e)
            {
                HandleError.Process("RoutePointsApiRequest", "AddRoutePoint", e, false);
            }

            return addResult;
        }
        public HttpStatusCode GetLastHttpStatusCode()
        {
            return LastHttpStatusCode;
        }

    }
}
