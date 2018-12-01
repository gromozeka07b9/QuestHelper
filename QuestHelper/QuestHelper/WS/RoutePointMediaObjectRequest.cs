using System;
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
    public class RoutePointMediaObjectRequest : IRoutePointMediaObjectRequest, IDownloadable<ViewRoute>, IUploadable<ViewRoute>
    {
        private string _hostUrl = string.Empty;
        public RoutePointMediaObjectRequest(string hostUrl)
        {
            _hostUrl = hostUrl;
        }
        /*public async Task<List<RoutePointMediaObject>> GetRoutePointMediaObjects(string routePointId)
        {
            throw new Exception("Не используется!");
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
        }*/
        public async Task<bool> GetImage(string routePointId, string routePointMediaObjectId, string pathToPictures, string filename)
        {
            bool result = false;
            try
            {
                ApiRequest api = new ApiRequest();
                string pathToMediaFile = Path.Combine(pathToPictures, filename);
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

        public async Task<bool> UploadToServerAsync(string jsonStructure)
        {
            bool addResult = false;
            try
            {
                ApiRequest api = new ApiRequest();
                await api.HttpRequestPOST($"{_hostUrl}/routepointmediaobjects", jsonStructure);
                addResult = true;
            }
            catch (Exception e)
            {
                HandleError.Process("RoutePointMediaObjectApiRequest", "AddRoutePointMediaObject", e, false);
            }
            return addResult;
        }

        public async Task<ISaveable> DownloadFromServerAsync(string routePointMediaObjectId)
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
    }
}
