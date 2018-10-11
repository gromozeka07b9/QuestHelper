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

namespace QuestHelper.WS
{
    public class RoutePointsApiRequest : IRoutePointsApiRequest
    {
        private string _hostUrl = string.Empty;
        public RoutePointsApiRequest(string hostUrl)
        {
            _hostUrl = hostUrl;
        }
        public async Task<List<RoutePoint>> GetRoutePoints(string routeId)
        {
            List<RoutePoint> deserializedValue = new List<RoutePoint>();
            try
            {
                ApiRequest api = new ApiRequest();
                var response = await api.HttpRequestGET($"{this._hostUrl}/api/routepoints/{routeId}");
                deserializedValue = JsonConvert.DeserializeObject<List<RoutePoint>>(response);
            }
            catch (Exception)
            {

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
                Description = routePointObject.Description
            });
            
            try
            {
                ApiRequest api = new ApiRequest();
                await api.HttpRequestPOST($"{_hostUrl}/api/routepoints", jsonObject.ToString());
                addResult = true;
            }
            catch (Exception)
            {
            }
            return addResult;
        }
    }
}
