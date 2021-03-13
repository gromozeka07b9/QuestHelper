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
using Autofac;
using QuestHelper.LocalDB.Model;
using Newtonsoft.Json.Linq;
using QuestHelper.Managers;
using QuestHelper.Model;
using QuestHelper.Model.WS;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;

namespace QuestHelper.WS
{
    public class RoutePointMediaObjectRequest : IRoutePointMediaObjectRequest, IHTTPStatusCode, IUploadable<ViewRoute>
    {
        private string _hostUrl = string.Empty;
        private string _authToken = string.Empty;
        public HttpStatusCode LastHttpStatusCode;
        private readonly IServerRequest _serverRequest = App.Container.Resolve<IServerRequest>();

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
                string pathToMediaFile = Path.Combine(pathToPictures, filename);
                result = await _serverRequest.HttpRequestGetFile($"/api/routepointmediaobjects/{routePointId}/{routePointMediaObjectId}/{filename}", pathToMediaFile, _authToken);
                LastHttpStatusCode = _serverRequest.GetLastStatusCode();
                
            }
            catch (Exception e)
            {
                HandleError.Process("RoutePointMediaObjectApiRequest", "GetFile", e, false);
            }
            return result;
        }
        public async Task<bool> SendImage(string routePointId, string routePointMediaObjectId, MediaObjectTypeEnum mediaType, bool isPreview = false)
        {
            bool sendResult = false;
            string nameMediafile = ImagePathManager.GetMediaFilename(routePointMediaObjectId, mediaType, isPreview);
            string pathToMediaFile = ImagePathManager.GetImagePath(routePointMediaObjectId, mediaType, isPreview);
            if (File.Exists(pathToMediaFile))
            {
                int maxRetryCount = 3;
                for(int triesCount = 0; triesCount < maxRetryCount; triesCount++)
                {
                    sendResult = await TryToSendFileAsync(pathToMediaFile, nameMediafile, routePointId, routePointMediaObjectId);
                    if (sendResult)
                    {
                        return sendResult;
                    }
                    else
                    {
                        Thread.Sleep(30); //ToDo: останавливает основной поток, UI будет тупить, надо на таймер переделать
                        HandleError.Process("RoutePointMediaObjectApiRequest", "SendImage",new Exception($"Retry send {nameMediafile}"), false);
                        if (triesCount >= maxRetryCount - 1)
                        {
                            HandleError.Process("RoutePointMediaObjectApiRequest", "SendImage", new Exception($"Error send {nameMediafile}"), false);
                        }
                    }
                }
            }
            else
            {
                HandleError.Process("RoutePointMediaObjectApiRequest", "SendImage", new Exception($"File {pathToMediaFile} not found"), false);
            }
            return sendResult;
        }

        public async Task<HttpStatusCode> ImageExist(string nameMediafile)
        {
            ApiRequest api = new ApiRequest();
            try
            {
                var result = await api.HttpRequestGET($"{this._hostUrl}/routepointmediaobjects/{nameMediafile}/imageexist", _authToken);
                LastHttpStatusCode = api.LastHttpStatusCode;
            }
            catch (Exception e)
            {
                HandleError.Process("RoutePointMediaObjectApiRequest", "ImageExist", e, false);
            }
            return LastHttpStatusCode;
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
                                HttpResponseMessage response = await client.PostAsync($"{ this._hostUrl }/routepointmediaobjects/{ routePointId }/{ routePointMediaObjectId }/uploadfile", formData);
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

        public async Task<ImagesServerStatus> GetImagesStatus(ImagesServerStatus imagesServerStatus)
        {
            JObject jsonRequestObject = JObject.FromObject(imagesServerStatus);

            ImagesServerStatus deserializedValue = new ImagesServerStatus();
            try
            {
                ApiRequest api = new ApiRequest();
                var response = await api.HttpRequestPOST($"{_hostUrl}/routepointmediaobjects/sync/imagestatus", jsonRequestObject.ToString(), _authToken);
                LastHttpStatusCode = api.LastHttpStatusCode;
                deserializedValue = JsonConvert.DeserializeObject<ImagesServerStatus>(response);
            }
            catch (Exception e)
            {
                HandleError.Process("RoutePointMediaObjectApiRequest", "GetImagesStatus", e, false);
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
                addResult = LastHttpStatusCode==HttpStatusCode.OK;
            }
            catch (Exception e)
            {
                HandleError.Process("RoutePointMediaObjectApiRequest", "AddRoutePointMediaObject", e, false);
            }
            return addResult;
        }

        public HttpStatusCode GetLastHttpStatusCode()
        {
            return LastHttpStatusCode;
        }
    }
}
