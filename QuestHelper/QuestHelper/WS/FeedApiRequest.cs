using Acr.UserDialogs;
using Newtonsoft.Json;
using QuestHelper.Managers;
using QuestHelper.SharedModelsWS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Autofac;
using Xamarin.Forms;

namespace QuestHelper.WS
{
    public class FeedApiRequest : IHTTPStatusCode
    {
        private readonly IServerRequest _serverRequest = App.Container.Resolve<IServerRequest>();

        private readonly string _authToken;

        public HttpStatusCode LastHttpStatusCode;


        public FeedApiRequest(string authToken)
        {
            _authToken = authToken;
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
                var response = await _serverRequest.HttpRequestGet("/api/feed", _authToken);
                LastHttpStatusCode = _serverRequest.GetLastStatusCode();
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
                        imgUrl = imgUrl.Replace("http:", "https:");
                        result = await _serverRequest.HttpRequestGetFile(imgUrl, pathToMediaFile, _authToken);
                        LastHttpStatusCode = _serverRequest.GetLastStatusCode();
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
