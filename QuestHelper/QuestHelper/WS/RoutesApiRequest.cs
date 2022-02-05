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
using Autofac;
using Microsoft.AppCenter.Analytics;
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
        private IMediaFileManager _mediaFileManager = App.Container.Resolve<IMediaFileManager>();


        private const string routesVersionsCacheId = "routesVersions";
        public HttpStatusCode LastHttpStatusCode;


        public RoutesApiRequest(string hostUrl, string authToken)
        {
            _hostUrl = hostUrl;
            _authToken = authToken;
            _memoryCache = App.Container.Resolve<IMemoryCache>();
        }

        //для перехода на IServerRequest
        public RoutesApiRequest(string authToken)
        {
            _authToken = authToken;
            _memoryCache = App.Container.Resolve<IMemoryCache>();
        }
        public async Task<List<Route>> GetPrivateRoutes(int pageSize, int indexStart, int count)
        {
            List<Route> deserializedValue = new List<Route>();
            try
            {
                string filter = @"&filter={""isDeleted"":""False""}";
                var response = await _serverRequest.HttpRequestGet($"/api/v2/routes?pageSize={pageSize}&range=%5B{indexStart}%2C{indexStart + count}%5D{filter}", _authToken);
                LastHttpStatusCode = _serverRequest.GetLastStatusCode();
                if (LastHttpStatusCode == HttpStatusCode.OK)
                {
                    deserializedValue = JsonConvert.DeserializeObject<List<Route>>(response);
                }
                else
                {
                    HandleError.Process("RoutesApiRequest", "GetPrivateRoutes", new HttpRequestException(LastHttpStatusCode.ToString()), false);
                }
            }
            catch (Exception e)
            {
                HandleError.Process("RoutesApiRequest", "GetPrivateRoutes", e, false);
            }
            return deserializedValue;
        }

        public async Task<string> UpdateHash(string routeId)
        {
            string serverHash = String.Empty;
            try
            {
                var response = await _serverRequest.HttpRequestPost($"/api/v2/routes/{routeId}/updatehash", _authToken, String.Empty);
                LastHttpStatusCode = _serverRequest.GetLastStatusCode();
                if (LastHttpStatusCode == HttpStatusCode.OK)
                {
                    serverHash = response;
                }
                else
                {
                    throw new HttpRequestException(LastHttpStatusCode.ToString());
                }
            }
            catch (Exception e)
            {
                HandleError.Process("RoutesApiRequest", "UpdateHash", e, false);
            }
            return serverHash;
        }

        public async Task<List<RouteVersion>> GetRoutesVersions(bool onlyPersonal)
        {
            List<RouteVersion> deserializedValue = new List<RouteVersion>();
            string cacheKey = $"{routesVersionsCacheId}-{onlyPersonal}";
            if (!_memoryCache.TryGetValue(cacheKey, out deserializedValue))
            {
                try
                {
                    var response = await _serverRequest.HttpRequestGet("/api/route/version/get?onlyPersonal={onlyPersonal}", _authToken);
                    //ApiRequest api = new ApiRequest();
                    //var response = await api.HttpRequestGET($"{this._hostUrl}/route/version/get?onlyPersonal={onlyPersonal}", _authToken);
                    LastHttpStatusCode = _serverRequest.GetLastStatusCode();
                    deserializedValue = JsonConvert.DeserializeObject<List<RouteVersion>>(response);
                    _memoryCache.Set(cacheKey, deserializedValue, new MemoryCacheEntryOptions()
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
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

        public async Task<string> DownloadRoutesCovers(IEnumerable<string> routesWithoutImg)
        {
            string result = string.Empty;
            DateTime dStart = DateTime.Now;
            try
            {
                StringBuilder sb = new StringBuilder();
                StringWriter sw = new StringWriter(sb);

                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    writer.Formatting = Formatting.Indented;
                    writer.WriteStartObject();
                    writer.WritePropertyName("IdArray");
                    writer.WriteStartArray();

                    foreach (var id in routesWithoutImg)
                    {
                        writer.WriteValue(id);
                    }
                    
                    writer.WriteEnd();
                    writer.WriteEndObject();
                } 
                result = await _serverRequest.HttpRequestGet("/api/v2/routes/covers/list", sb.ToString(), _authToken);
                LastHttpStatusCode = _serverRequest.GetLastStatusCode();
                if ((LastHttpStatusCode == HttpStatusCode.OK) && !string.IsNullOrEmpty(result))
                {
                    Analytics.TrackEvent("Route covers: network", new Dictionary<string, string> { { "Count", routesWithoutImg.Count().ToString() }, { "Delay", (dStart - DateTime.Now).Milliseconds.ToString() } });

                    JsonTextReader reader = new JsonTextReader(new StringReader(result));
                    string routeId = string.Empty;
                    string imgCoverFilename = string.Empty;
                    string imgCover = string.Empty;
                    bool routeIdIsGot = false, imgCoverFilenameIsGot = false, imgCoverIsGot = false;
                    while (reader.Read())
                    {
                        if ((reader.Value != null) && (reader.TokenType == JsonToken.PropertyName))
                        {
                            Console.WriteLine(reader.TokenType);
                            Console.WriteLine(reader.Value);
                            switch (reader.Value)
                            {
                                case "routeId":
                                {
                                    routeIdIsGot = true;
                                };break;
                                case "imgCoverFilename":
                                {
                                    imgCoverFilenameIsGot = true;
                                };break;
                                case "imgCover":
                                {
                                    imgCoverIsGot = true;
                                };break;
                       
                            }
                        }

                        if ((reader.Value != null) && (reader.TokenType == JsonToken.String))
                        {
                            if (routeIdIsGot)
                            {
                                routeId = reader.Value.ToString();
                                routeIdIsGot = false;
                            } else if (imgCoverFilenameIsGot)
                            {
                                imgCoverFilename = reader.Value.ToString();
                                imgCoverFilenameIsGot = false;
                            } else if (imgCoverIsGot)
                            {
                                imgCover = reader.Value.ToString();
                                imgCoverIsGot = false;
                            }
                        }

                        if (!string.IsNullOrEmpty(routeId) && !string.IsNullOrEmpty(imgCoverFilename) && !string.IsNullOrEmpty(imgCover))
                        {
                            _mediaFileManager.SaveMediaFileFromBase64(imgCoverFilename, imgCover);
                            routeId = string.Empty;
                            imgCoverFilename = String.Empty;
                            imgCover = String.Empty;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                HandleError.Process("RoutesApiRequest", "DownloadRoutesCovers", e, false);
            }
            
            Analytics.TrackEvent("Route covers: full", new Dictionary<string, string> { { "Count", routesWithoutImg.Count().ToString() }, { "Delay", (dStart - DateTime.Now).Milliseconds.ToString() } });
            
            return result;
        }

        public HttpStatusCode GetLastHttpStatusCode()
        {
            return LastHttpStatusCode;
        }
    }
}
