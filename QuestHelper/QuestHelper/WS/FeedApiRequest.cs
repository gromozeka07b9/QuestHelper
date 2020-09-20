using Acr.UserDialogs;
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
    public class FeedApiRequest : IHTTPStatusCode
    {
        private const string _apiUrl = "http://igosh.pro/api";
        private const string _feedCacheId = "FeedApiCache";
        //private IMemoryCache _memoryCache;
        private string _authToken = string.Empty;

        public HttpStatusCode LastHttpStatusCode;


        public FeedApiRequest(string authToken)
        {
            _authToken = authToken;
            //_memoryCache = App.Container.Resolve<IMemoryCache>();
        }

        public async Task<List<FeedItem>> GetFeed()
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
        }

        private async Task<List<FeedItem>> tryToRequestFeedAsync()
        {
            try
            {
                ApiRequest api = new ApiRequest();
                var response = await api.HttpRequestGET($"{_apiUrl}/feed", _authToken);
                LastHttpStatusCode = api.LastHttpStatusCode;
                return JsonConvert.DeserializeObject<List<FeedItem>>(response);
            }
            catch (Exception e)
            {
                HandleError.Process("FeedApiRequest", "tryToRequestFeedAsync", e, false);
            }

            return new List<FeedItem>();
        }

        public async Task<bool> GetCoverImage(string imgUrl)
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
                            UserDialogs.Instance.Toast("Error downloading cover image");
                            HandleError.Process("FeedApiRequest", "GetCoverImage", new Exception(LastHttpStatusCode.ToString()), false, imgUrl);
                            deleteFile(pathToMediaFile);
                        }
                    }
                    catch (Exception e)
                    {
                        HandleError.Process("FeedApiRequest", "GetCoverImage", e, showWarning: false);
                        deleteFile(pathToMediaFile);
                    }
                }
            }
            return result;
        }

        private void deleteFile(string pathToFile)
        {
            try
            {

                File.Delete(pathToFile);

            }catch(Exception e)
            {

                HandleError.Process("FeedApiRequest", "DeleteCorruptedFile", e, showWarning: false);

            }
        }

        public HttpStatusCode GetLastHttpStatusCode()
        {
            return LastHttpStatusCode;
        }
    }
}
