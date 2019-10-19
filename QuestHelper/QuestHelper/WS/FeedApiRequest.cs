using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
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
    public class FeedApiRequest : IHTTPStatusCode
    {
        private const string _apiUrl = "http://igosh.pro/api";
        private const string _feedCacheId = "FeedApiCache";
        private IMemoryCache _memoryCache;
        private string _authToken = string.Empty;

        public HttpStatusCode LastHttpStatusCode;


        public FeedApiRequest(string authToken)
        {
            _authToken = authToken;
            _memoryCache = App.Container.Resolve<IMemoryCache>();
        }

        public async Task<List<FeedItem>> GetFeed()
        {
            List<FeedItem> feed = new List<FeedItem>();
            if (!_memoryCache.TryGetValue(_feedCacheId, out feed))
            {
                try
                {
                    feed = await tryToRequestFeedAsync();
                    if (LastHttpStatusCode == HttpStatusCode.OK)
                    {
                        _memoryCache.Set(_feedCacheId, feed, new MemoryCacheEntryOptions()
                        {
#if DEBUG
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10)
#else
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(120)
#endif
                        });
                    }
                }
                catch (Exception e)
                {
                    HandleError.Process("FeedApiRequest", "GetFeed", e, false);
                }
            }
            else
            {
                //если вытащили из кэша то нужен корректный http статус
                //ToDo: похоже на косяк, судя по всему часть БЛ переползла сюда, а не надо
                LastHttpStatusCode = HttpStatusCode.OK;
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
                    }
                    catch (Exception e)
                    {
                        HandleError.Process("FeedApiRequest", "GetCoverImage", e, false);
                    }
                }
            }
            return result;
        }

        public HttpStatusCode GetLastHttpStatusCode()
        {
            return LastHttpStatusCode;
        }
    }
}
