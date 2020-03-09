using Newtonsoft.Json;
using QuestHelper.Managers;
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

        /*public async Task<List<FeedItem>> GetFeed()
        {
            List<FeedItem> feed = new List<FeedItem>();
            try
            {
                feed = await tryToRequestFeedAsync();
            }
            catch (Exception e)
            {
                HandleError.Process("FeedApiRequest", "GetFeed", e, false);
            }
            return feed;
        }*/

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

        /*public async Task<bool> GetCoverImage(string imgUrl)
        {
            bool result = false;
            string fileName = System.IO.Path.GetFileName(imgUrl);
            if (!string.IsNullOrEmpty(fileName))
            {
                string pathToMediaFile = Path.Combine(ImagePathManager.GetPicturesDirectory(), fileName);
                if (!File.Exists(pathToMediaFile))
                {
                    try
                    {
                        ApiRequest api = new ApiRequest();
                        result = await api.HttpRequestGetFile(imgUrl, pathToMediaFile, _authToken);
                        LastHttpStatusCode = api.LastHttpStatusCode;
                        if (LastHttpStatusCode != HttpStatusCode.OK)
                        {
                            HandleError.Process("FeedApiRequest", "GetCoverImage", new Exception(LastHttpStatusCode.ToString()), false, imgUrl);
                        }
                    }
                    catch (Exception e)
                    {
                        HandleError.Process("FeedApiRequest", "GetCoverImage", e, false);
                    }
                }
            }
            return result;
        }*/

        public HttpStatusCode GetLastHttpStatusCode()
        {
            return LastHttpStatusCode;
        }
    }
}
