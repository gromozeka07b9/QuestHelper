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
    public class RoutePointsApiRequest : IRoutePointsApiRequest
    {
        private string _hostUrl = string.Empty;
        public RoutePointsApiRequest(string hostUrl)
        {
            _hostUrl = hostUrl;
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
                var response = await api.HttpRequestPOST($"{_hostUrl}/routepoints/sync", jsonRequestObject.ToString());
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
                var response = await api.HttpRequestGET($"{this._hostUrl}/routepoints/{routeId}");
                deserializedValue = JsonConvert.DeserializeObject<List<RoutePoint>>(response);
            }
            catch (Exception e)
            {
                HandleError.Process("RoutePointsApiRequest", "GetRoutePoints", e, false);
            }
            return deserializedValue;
        }
        public async Task<ViewRoutePoint> GetRoutePoint(string routePointId)
        {
            ViewRoutePoint deserializedValue = new ViewRoutePoint();
            try
            {
                ApiRequest api = new ApiRequest();
                var response = await api.HttpRequestGET($"{this._hostUrl}/routepoints/{routePointId}");
                deserializedValue = JsonConvert.DeserializeObject<ViewRoutePoint>(response);
            }
            catch (Exception e)
            {
                HandleError.Process("RoutePointsApiRequest", "GetRoutePoint", e, false);
            }
            return deserializedValue;
        }
        public async Task<bool> AddRoutePoint(RoutePoint routePointObject)
        {
            bool addResult = false;
            JObject jsonObject = JObject.FromObject(new {
                RoutePointId = routePointObject.RoutePointId,
                RouteId = routePointObject.MainRoute.RouteId,
                Name = routePointObject.Name,
                CreateDate = routePointObject.CreateDate.DateTime,
                UpdateDate = routePointObject.CreateDate.DateTime,
                UpdatedUserId = "",
                Latitude = routePointObject.Latitude,
                Longitude = routePointObject.Longitude,
                Address = routePointObject.Address,
                Description = routePointObject.Description,
                Version = routePointObject.Version
            });
            
            try
            {
                ApiRequest api = new ApiRequest();
                await api.HttpRequestPOST($"{_hostUrl}/routepoints", jsonObject.ToString());
                addResult = true;
            }
            catch (Exception e)
            {
                HandleError.Process("RoutePointsApiRequest", "AddRoutePoint", e, false);
            }
            return addResult;
        }
    }
}
