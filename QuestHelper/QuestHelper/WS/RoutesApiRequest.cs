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
using QuestHelper.Model;
using System.Threading;
using QuestHelper.SharedModelsWS;
using QuestHelper.Managers;

namespace QuestHelper.WS
{
    //public class RoutesApiRequest : IRoutesApiRequest, IDownloadable<ViewRoute>, IUploadable<ViewRoute>
    public class RoutesApiRequest : IHTTPStatusCode, IUploadable<ViewRoute>
    {
        private string _hostUrl = string.Empty;
        private string _authToken = string.Empty;
        public HttpStatusCode LastHttpStatusCode;

        public RoutesApiRequest(string hostUrl, string authToken)
        {
            _hostUrl = hostUrl;
            _authToken = authToken;
        }

        public async Task<List<RouteVersion>> GetRoutesVersions()
        {
            List<RouteVersion> deserializedValue = new List<RouteVersion>();
            try
            {
                ApiRequest api = new ApiRequest();
                var response = await api.HttpRequestGET($"{this._hostUrl}/route/version/get", _authToken);
                LastHttpStatusCode = api.LastHttpStatusCode;
                deserializedValue = JsonConvert.DeserializeObject<List<RouteVersion>>(response);
            }
            catch (Exception e)
            {
                HandleError.Process("RoutesApiRequest", "GetRoutesVersions", e, false);
            }
            return deserializedValue;
        }

        public async Task<RouteRoot> GetRouteRoot(string routeId)
        {
            RouteRoot deserializedValue = new RouteRoot();
            try
            {
                ApiRequest api = new ApiRequest();
                var response = await api.HttpRequestGET($"{this._hostUrl}/route/{routeId}", _authToken);
                LastHttpStatusCode = api.LastHttpStatusCode;
                deserializedValue = JsonConvert.DeserializeObject<RouteRoot>(response);
            }
            catch (Exception e)
            {
                HandleError.Process("RoutesApiRequest", "GetRouteRoot", e, false);
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

            bool requestResult = false;
            int triesCount = 0;
            while (!requestResult)
            {
                deserializedValue = await TryToRequestAsync(jsonRequestObject.ToString(), _authToken);
                requestResult = LastHttpStatusCode == HttpStatusCode.OK;
                if ((!requestResult) && (triesCount < 5) && (LastHttpStatusCode==HttpStatusCode.InternalServerError))//пока не пойму почему на сервере 500 при валидации пользователя
                {
                    triesCount++;
                    Thread.Sleep(30);//ToDo: останавливает основной поток, UI будет тупить, надо на таймер переделать
                }
                else
                {
                    break;
                }
            }

            return deserializedValue;
        }

        private async Task<SyncObjectStatus> TryToRequestAsync(string jsonRequestString, string authToken)
        {
            try
            {
                ApiRequest api = new ApiRequest();
                var response = await api.HttpRequestPOST($"{_hostUrl}/routes/sync", jsonRequestString, authToken);
                LastHttpStatusCode = api.LastHttpStatusCode;
                return JsonConvert.DeserializeObject<SyncObjectStatus>(response);
            }
            catch (Exception e)
            {
                HandleError.Process("RoutesApiRequest", "GetSyncStatus", e, false);
            }

            return new SyncObjectStatus();
        }

        public async Task<bool> DeleteRoute(QuestHelper.LocalDB.Model.Route routeObject)
        {
            bool deleteResult = false;
            try
            {
                ApiRequest api = new ApiRequest();
                deleteResult = await api.HttpRequestDELETE($"{_hostUrl}/routes/{routeObject.RouteId}", _authToken);
                LastHttpStatusCode = api.LastHttpStatusCode;
            }
            catch (Exception e)
            {
                HandleError.Process("RoutesApiRequest", "DeleteRoute", e, false);
            }
            return deleteResult;
        }

        internal async Task<bool> ShareRouteAsync(string jsonStructure)
        {
            bool shareResult = false;
            try
            {
                ApiRequest api = new ApiRequest();
                await api.HttpRequestPOST($"{_hostUrl}/routes/share", jsonStructure, _authToken);
                LastHttpStatusCode = api.LastHttpStatusCode;
                shareResult = LastHttpStatusCode == HttpStatusCode.OK;
            }
            catch (Exception e)
            {
                HandleError.Process("RoutesApiRequest", "Share", e, false);
            }
            return shareResult;
        }

        public async Task<bool> UploadToServerAsync(string jsonStructure)
        {
            bool addResult = false;
            try
            {
                ApiRequest api = new ApiRequest();
                await api.HttpRequestPOST($"{_hostUrl}/routes", jsonStructure, _authToken);
                LastHttpStatusCode = api.LastHttpStatusCode;
                addResult = LastHttpStatusCode == HttpStatusCode.OK;
            }
            catch (Exception e)
            {
                HandleError.Process("RoutesApiRequest", "AddRoute", e, false);
            }
            return addResult;
        }

        public async Task<string> GetShortLinkId(string routeId)
        {
            string routeShortId = string.Empty;

            try
            {
                ApiRequest api = new ApiRequest();
                var resultRequest = await api.HttpRequestGET($"{_hostUrl}/routes/{routeId}/linkhash", _authToken);
                LastHttpStatusCode = api.LastHttpStatusCode;
                if (LastHttpStatusCode == HttpStatusCode.OK)
                {
                    routeShortId = resultRequest;
                }
            }
            catch (Exception e)
            {
                HandleError.Process("RoutesApiRequest", "GetShortLinkId", e, false);
            }

            return routeShortId;
        }

        public async Task<bool> CreateShortLinkIdAsync(string routeId)
        {
            bool createResult = false;
            try
            {
                ApiRequest api = new ApiRequest();
                await api.HttpRequestPOST($"{_hostUrl}/routes/{routeId}/createshortlink", string.Empty, _authToken);
                LastHttpStatusCode = api.LastHttpStatusCode;
                createResult = LastHttpStatusCode == HttpStatusCode.OK;
            }
            catch (Exception e)
            {
                HandleError.Process("RoutesApiRequest", "CreateShortLinkIdAsync", e, false);
            }
            return createResult;
        }

        public async Task<bool> DownloadCoverImage(string routeId, string imgFilename)
        {
            bool result = false;
            try
            {
                ApiRequest api = new ApiRequest();
                string pathToMediaFile = Path.Combine(ImagePathManager.GetPicturesDirectory(), imgFilename);
                result = await api.HttpRequestGetFile($"{this._hostUrl}/route/{routeId}/cover", pathToMediaFile, _authToken);
                LastHttpStatusCode = api.LastHttpStatusCode;
            }
            catch (Exception e)
            {
                HandleError.Process("RoutesApiRequest", "DownloadCoverImage", e, false);
            }
            return result;
        }

        public HttpStatusCode GetLastHttpStatusCode()
        {
            return LastHttpStatusCode;
        }
    }
}
