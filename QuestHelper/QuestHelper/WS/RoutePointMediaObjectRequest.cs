﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using QuestHelper.LocalDB.Model;
using Newtonsoft.Json.Linq;
using QuestHelper.Managers;
using QuestHelper.Model;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;

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
            throw new Exception("Не используется!");
            /*List<RoutePointMediaObject> deserializedValue = new List<RoutePointMediaObject>();
            try
            {
                ApiRequest api = new ApiRequest();
                var response = await api.HttpRequestGET($"{this._hostUrl}/api/routepointmediaobjects/{routePointId}");
                deserializedValue = JsonConvert.DeserializeObject<List<RoutePointMediaObject>>(response);
            }
            catch (Exception)
            {

            }
            return deserializedValue;*/
        }
        public async Task<ViewRoutePointMediaObject> GetRoutePointMediaObject(string routePointMediaObjectId)
        {
            ViewRoutePointMediaObject deserializedValue = new ViewRoutePointMediaObject();
            try
            {
                ApiRequest api = new ApiRequest();
                var response = await api.HttpRequestGET($"{this._hostUrl}/routepointmediaobjects/{routePointMediaObjectId}");
                deserializedValue = JsonConvert.DeserializeObject<ViewRoutePointMediaObject>(response);
            }
            catch (Exception e)
            {
                HandleError.Process("RoutePointMediaObjectApiRequest", "GetRoutePointMediaObject", e, false);
            }
            return deserializedValue;
        }
        public async Task<bool> GetImage(string routePointId, string routePointMediaObjectId, string filename)
        {
            bool result = false;
            try
            {
                ApiRequest api = new ApiRequest();
                string pathToMediaFile = ImagePathManager.GetImagePath(routePointMediaObjectId);
                result = await api.HttpRequestGetFile($"{this._hostUrl}/routepointmediaobjects/{routePointId}/{routePointMediaObjectId}/{filename}", pathToMediaFile);
            }
            catch (Exception e)
            {
                HandleError.Process("RoutePointMediaObjectApiRequest", "GetFile", e, false);
            }
            return result;
        }
        public async Task<bool> SendImage(string routePointId, string routePointMediaObjectId, bool isPreview = false)
        {
            bool result = false;
            string nameMediafile = ImagePathManager.GetImageFilename(routePointMediaObjectId, isPreview);
            string pathToMediaFile = ImagePathManager.GetImagePath(routePointMediaObjectId, isPreview);
            using (Stream image = File.Open(pathToMediaFile, FileMode.Open))
            {
                using (HttpContent content = new StreamContent(image))
                {
                    content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data") { Name = "file", FileName = nameMediafile };
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    using (var client = new HttpClient())
                    {
                        using (var formData = new MultipartFormDataContent())
                        {
                            formData.Add(content);
                            var response = await client.PostAsync($"{ this._hostUrl }/routepointmediaobjects/{ routePointId }/{ routePointMediaObjectId }/uploadfile", formData);
                            result = response.IsSuccessStatusCode;
                        }
                    }
                }
            }
            return result;
        }

        public async Task<bool> AddRoutePointMediaObject(RoutePointMediaObject routePointMediaObject)
        {
            bool addResult = false;            
            try
            {
                JObject jsonObject = JObject.FromObject(new
                {
                    RoutePointMediaObjectId = routePointMediaObject.RoutePointMediaObjectId,
                    RoutePointId = routePointMediaObject.RoutePointId,
                    Version = routePointMediaObject.Version
                });
                ApiRequest api = new ApiRequest();
                await api.HttpRequestPOST($"{_hostUrl}/routepointmediaobjects", jsonObject.ToString());
                addResult = true;
            }
            catch (Exception e)
            {
                HandleError.Process("RoutePointMediaObjectApiRequest", "AddRoutePointMediaObject", e, false);
            }
            return addResult;
        }

        public async Task<SyncObjectStatus> GetSyncStatus(IEnumerable<Tuple<string, int>> medias)
        {
            SyncObjectStatus requestValue = new SyncObjectStatus();
            foreach (var item in medias)
            {
                requestValue.Statuses.Add(new SyncObjectStatus.ObjectStatus()
                {
                    ObjectId = item.Item1,
                    Version = item.Item2
                });
            }
            JObject jsonRequestObject = JObject.FromObject(requestValue);

            SyncObjectStatus deserializedValue = new SyncObjectStatus();
            try
            {
                ApiRequest api = new ApiRequest();
                var response = await api.HttpRequestPOST($"{_hostUrl}/routepointmediaobjects/sync", jsonRequestObject.ToString());
                deserializedValue = JsonConvert.DeserializeObject<SyncObjectStatus>(response);
            }
            catch (Exception e)
            {
                HandleError.Process("RoutePointMediaObjectApiRequest", "GetSyncStatus", e, false);
            }

            return deserializedValue;
        }
    }
}
