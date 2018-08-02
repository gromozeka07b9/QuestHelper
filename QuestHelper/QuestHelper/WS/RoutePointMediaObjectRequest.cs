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
    public class RoutePointMediaObjectRequest : IRoutePointMediaObjectRequest
    {
        private string _hostUrl = string.Empty;
        public RoutePointMediaObjectRequest(string hostUrl)
        {
            _hostUrl = hostUrl;
        }
        public async Task<List<RoutePointMediaObject>> GetRoutePointMediaObjects(string routePointId)
        {
            List<RoutePointMediaObject> deserializedValue = new List<RoutePointMediaObject>();
            try
            {
                ApiRequest api = new ApiRequest();
                var response = await api.HttpRequestGET($"{this._hostUrl}/api/routepointmediaobjects/{routePointId}");
                deserializedValue = JsonConvert.DeserializeObject<List<RoutePointMediaObject>>(response);
            }
            catch (Exception)
            {

            }
            return deserializedValue;
        }
        public async Task<bool> AddRoutePointMediaObject(RoutePointMediaObject routePointMediaObject)
        {
            bool addResult = false;
            JObject jsonObject = JObject.FromObject(new {
                RoutePointMediaObjectId = routePointMediaObject.RoutePointMediaObjectId,
                RoutePointId = routePointMediaObject.Point.RoutePointId,
                FileName = routePointMediaObject.FileName,
                PreviewImage = routePointMediaObject.PreviewImage
            });
            
            try
            {
                ApiRequest api = new ApiRequest();
                await api.HttpRequestPOST($"{_hostUrl}/api/routepointmediaobjects", jsonObject.ToString());
                addResult = true;
            }
            catch (Exception)
            {
            }
            return addResult;
        }
    }
}
