using Newtonsoft.Json;
using QuestHelper.Managers;
using QuestHelper.Model;
using QuestHelper.SharedModelsWS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace QuestHelper.WS
{
    public class PoiApiRequest : IHTTPStatusCode
    {
        private const string _apiUrl = "http://igosh.pro/api";
        //private const string _cacheId = "PoiApiCache";
        private string _authToken = string.Empty;

        public HttpStatusCode LastHttpStatusCode;


        public PoiApiRequest(string authToken)
        {
            _authToken = authToken;
        }

        public async Task<List<Poi>> GetMyPoisAsync()
        {
            try
            {
                ApiRequest api = new ApiRequest();
                var response = await api.HttpRequestGET($"{_apiUrl}/poi", _authToken);
                LastHttpStatusCode = api.LastHttpStatusCode;
                return JsonConvert.DeserializeObject<List<Poi>>(response);
            }
            catch (Exception e)
            {
                HandleError.Process("PoiApiRequest", "GetMyPoisAsync", e, false);
            }

            return new List<Poi>();
        }

        public async Task<Poi> GetPoiByRoutePointIdAsync(string routePointId)
        {
            try
            {
                ApiRequest api = new ApiRequest();
                var response = await api.HttpRequestGET($"{_apiUrl}/poi/byRoutePointId/{routePointId}", _authToken);
                LastHttpStatusCode = api.LastHttpStatusCode;
                return JsonConvert.DeserializeObject<Poi>(response);
            }
            catch (Exception e)
            {
                HandleError.Process("PoiApiRequest", "GetPoiByRoutePointIdAsync", e, false);
            }

            return new Poi();
        }

        public async Task<bool> UploadPoiAsync(string jsonStructure)
        {
            bool addResult = false;

            try
            {
                ApiRequest api = new ApiRequest();
                await api.HttpRequestPOST($"{_apiUrl}/poi", jsonStructure, _authToken);
                LastHttpStatusCode = api.LastHttpStatusCode;
                addResult = LastHttpStatusCode == HttpStatusCode.OK;
            }
            catch (Exception e)
            {
                HandleError.Process("PoiApiRequest", "UploadPoiAsync", e, false);
            }

            return addResult;
        }
        public async Task<bool> DeleteAsync(string poiId)
        {
            bool result = false;

            try
            {
                ApiRequest api = new ApiRequest();
                await api.HttpRequestDELETE($"{_apiUrl}/poi/{poiId}", _authToken);
                LastHttpStatusCode = api.LastHttpStatusCode;
                result = LastHttpStatusCode == HttpStatusCode.OK;
            }
            catch (Exception e)
            {
                HandleError.Process("PoiApiRequest", "DeleteAsync", e, false);
            }

            return result;
        }

        public async Task<bool> DownloadImg(string poiId, string pathToMediaFile)
        {
            bool result = false;
            try
            {
                ApiRequest api = new ApiRequest();
                string fileName = System.IO.Path.GetFileName(pathToMediaFile);
                result = await api.HttpRequestGetFile($"{_apiUrl}/poi/{poiId}/image/{fileName}", pathToMediaFile, _authToken);
                LastHttpStatusCode = api.LastHttpStatusCode;
            }
            catch (Exception e)
            {
                HandleError.Process("PoiApiRequest", "DownloadImg", e, false);
            }
            return result;
        }

        public HttpStatusCode GetLastHttpStatusCode()
        {
            return LastHttpStatusCode;
        }
    }
}
