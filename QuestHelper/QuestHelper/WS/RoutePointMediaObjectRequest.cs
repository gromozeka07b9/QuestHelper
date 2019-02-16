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
using System.Threading;
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
        private string _authToken = string.Empty;
        public HttpStatusCode LastHttpStatusCode;
        public RoutePointMediaObjectRequest(string hostUrl, string authToken)
        {
            _hostUrl = hostUrl;
            _authToken = authToken;
        }
        public async Task<bool> GetImage(string routePointId, string routePointMediaObjectId, string pathToPictures, string filename)
        {
            bool result = false;
            try
            {
                ApiRequest api = new ApiRequest();
                string pathToMediaFile = Path.Combine(pathToPictures, filename);
                result = await api.HttpRequestGetFile($"{this._hostUrl}/routepointmediaobjects/{routePointId}/{routePointMediaObjectId}/{filename}", pathToMediaFile, _authToken);
                LastHttpStatusCode = api.LastHttpStatusCode;
            }
            catch (Exception e)
            {
                HandleError.Process("RoutePointMediaObjectApiRequest", "GetFile", e, false);
            }
            return result;
        }
        public async Task<bool> SendImage(string routePointId, string routePointMediaObjectId, bool isPreview = false)
        {
            bool sendResult = false;
            string nameMediafile = ImagePathManager.GetImageFilename(routePointMediaObjectId, isPreview);
            string pathToMediaFile = ImagePathManager.GetImagePath(routePointMediaObjectId, isPreview);
            if (File.Exists(pathToMediaFile))
            {
                int triesCount = 0;
                while (!sendResult)
                {
                    sendResult = await TryToSendFileAsync(pathToMediaFile, nameMediafile, routePointId, routePointMediaObjectId);
                    if ((!sendResult)&&(triesCount < 10))
                    {
                        triesCount++;
                        Thread.Sleep(30);//ToDo: останавливает основной поток, UI будет тупить, надо на таймер переделать
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                HandleError.Process("RoutePointMediaObjectApiRequest", "SendImage", new Exception($"File {pathToMediaFile} not found"), false);
            }
            return sendResult;
        }

        private async Task<bool> TryToSendFileAsync(string pathToMediaFile, string nameMediafile, string routePointId, string routePointMediaObjectId)
        {
            bool result = false;
            try
            {
                using (Stream image = File.Open(pathToMediaFile, FileMode.Open))
                {
                    using (HttpContent content = new StreamContent(image))
                    {
                        content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data") { Name = "file", FileName = nameMediafile };
                        content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                        using (var client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                            using (var formData = new MultipartFormDataContent())
                            {
                                formData.Add(content);
                                HttpResponseMessage response = new HttpResponseMessage();
                                response = await client.PostAsync($"{ this._hostUrl }/routepointmediaobjects/{ routePointId }/{ routePointMediaObjectId }/uploadfile", formData);
                                LastHttpStatusCode = response.StatusCode;
                                result = response.IsSuccessStatusCode;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                HandleError.Process("RoutePointMediaObjectApiRequest", "TryToSendFileAsync", e, false);
                result = false;
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
                var response = await api.HttpRequestPOST($"{_hostUrl}/routepointmediaobjects/sync", jsonRequestObject.ToString(), _authToken);
                LastHttpStatusCode = api.LastHttpStatusCode;
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
                await api.HttpRequestPOST($"{_hostUrl}/routepointmediaobjects", jsonStructure, _authToken);
                LastHttpStatusCode = api.LastHttpStatusCode;
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
                var response = await api.HttpRequestGET($"{this._hostUrl}/routepointmediaobjects/{routePointMediaObjectId}", _authToken);
                LastHttpStatusCode = api.LastHttpStatusCode;
                deserializedValue = JsonConvert.DeserializeObject<ViewRoutePointMediaObject>(response);
                deserializedValue.ServerSynced = true;
            }
            catch (Exception e)
            {
                HandleError.Process("RoutePointMediaObjectApiRequest", "GetRoutePointMediaObject", e, false);
            }
            return deserializedValue;
        }
        public HttpStatusCode GetLastHttpStatusCode()
        {
            return LastHttpStatusCode;
        }
    }
}
