﻿using System;
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
using Autofac;
using QuestHelper.SharedModelsWS;
using QuestHelper.Managers;
using Microsoft.Extensions.Caching.Memory;

namespace QuestHelper.WS
{
    public class RoutesApiRequest : IHTTPStatusCode, IUploadable<ViewRoute>
    {
        private IMemoryCache _memoryCache;
        private string _hostUrl = string.Empty;
        private string _authToken = string.Empty;
        private readonly IServerRequest _serverRequest = App.Container.Resolve<IServerRequest>();

        private const string routesVersionsCacheId = "routesVersions";
        public HttpStatusCode LastHttpStatusCode;


        public RoutesApiRequest(string hostUrl, string authToken)
        {
            _hostUrl = hostUrl;
            _authToken = authToken;
            _memoryCache = App.Container.Resolve<IMemoryCache>();
        }

        public async Task<List<RouteVersion>> GetRoutesVersions(bool onlyPersonal)
        {
            List<RouteVersion> deserializedValue = new List<RouteVersion>();
            string cacheKey = $"{routesVersionsCacheId}-{onlyPersonal}";
            if (!_memoryCache.TryGetValue(cacheKey, out deserializedValue))
            {
                try
                {
                    ApiRequest api = new ApiRequest();
                    var response = await api.HttpRequestGET($"{this._hostUrl}/route/version/get?onlyPersonal={onlyPersonal}", _authToken);
                    LastHttpStatusCode = api.LastHttpStatusCode;
                    deserializedValue = JsonConvert.DeserializeObject<List<RouteVersion>>(response);
                    _memoryCache.Set(cacheKey, deserializedValue, new MemoryCacheEntryOptions()
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(20)
                    });
                }
                catch (Exception e)
                {
                    HandleError.Process("RoutesApiRequest", "GetRoutesVersions", e, false);
                }
            }
            return deserializedValue;
        }

        public async Task<RouteRoot> GetRouteRoot(string routeId)
        {
            RouteRoot deserializedValue = new RouteRoot();
            try
            {
                var response = await _serverRequest.HttpRequestGet($"/api/route/{routeId}", _authToken);
                LastHttpStatusCode = _serverRequest.GetLastStatusCode();

                deserializedValue = JsonConvert.DeserializeObject<RouteRoot>(response);
            }
            catch (Exception e)
            {
                HandleError.Process("RoutesApiRequest", "GetRouteRoot", e, false);
            }
            return deserializedValue;
        }
        public async Task<RouteVersion> GetRouteVersion(string routeId)
        {
            RouteVersion routeVersion = new RouteVersion();
            try
            {
                var response = await _serverRequest.HttpRequestGet($"/api/route/{routeId}/version", _authToken);
                LastHttpStatusCode = _serverRequest.GetLastStatusCode();

                var deserializedValue = JsonConvert.DeserializeObject<List<RouteVersion>>(response);
                if (deserializedValue.Any())
                {
                    routeVersion = deserializedValue.FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                HandleError.Process("RoutesApiRequest", "GetRouteVersion", e, false);
            }
            return routeVersion;
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
                var resultRequest = await _serverRequest.HttpRequestGet($"/api/routes/{routeId}/linkhash", _authToken);
                LastHttpStatusCode = _serverRequest.GetLastStatusCode();

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

        public async Task<bool> AddUserViewAsync(string routeId)
        {
            bool result = false;
            try
            {
                ApiRequest api = new ApiRequest();
                await api.HttpRequestPOST($"{_hostUrl}/route/{routeId}/addviewed", string.Empty, _authToken);
                LastHttpStatusCode = api.LastHttpStatusCode;
                result = LastHttpStatusCode == HttpStatusCode.OK;
            }
            catch (Exception e)
            {
                HandleError.Process("RoutesApiRequest", "AddUserViewAsync", e, false);
            }
            return result;
        }
        public async Task<bool> SetEmotionAsync(string RouteId, bool Emotion)
        {
            bool result = false;
            try
            {
                var emotionStructure = new
                {
                    EmotionNum = Emotion ? 1 : 0
                };
                string body = JsonConvert.SerializeObject(emotionStructure);
                ApiRequest api = new ApiRequest();
                await api.HttpRequestPOST($"{this._hostUrl}/likes/{RouteId}/addemotion", body, _authToken);
                LastHttpStatusCode = api.LastHttpStatusCode;
                result = LastHttpStatusCode == HttpStatusCode.OK;
            }
            catch (Exception e)
            {
                HandleError.Process("RoutesApiRequest", "SetEmotionAsync", e, false);
            }
            return result;
        }

        public async Task<bool> DownloadCoverImage(string routeId, string imgFilename)
        {
            bool result = false;
            try
            {
                string pathToMediaFile = Path.Combine(ImagePathManager.GetPicturesDirectory(), imgFilename);
                result = await _serverRequest.HttpRequestGetFile($"/api/route/{routeId}/cover", pathToMediaFile, _authToken);
                LastHttpStatusCode = _serverRequest.GetLastStatusCode();
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
