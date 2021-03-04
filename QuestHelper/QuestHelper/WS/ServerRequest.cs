using System;
using System.IO;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using Autofac;

namespace QuestHelper.WS
{
    public class ServerRequest : IServerRequest
    {
        static readonly HttpClient _httpClient = new HttpClient();
        static readonly ITextfileLogger _logger = App.Container.Resolve<ITextfileLogger>();
        readonly Uri _apiBaseUri = null;

        HttpStatusCode _lastHttpStatusCode = 0;
        public ServerRequest(string apiUrlBase)
        {
            _apiBaseUri = new Uri(apiUrlBase);
        }


        public HttpStatusCode GetLastStatusCode()
        {
            return _lastHttpStatusCode;
        }

        public async Task<string> HttpRequestGet(string relativeUrl, string bodyText, string authToken)
        {
            string result = string.Empty;
            string url = new Uri(_apiBaseUri, relativeUrl).AbsoluteUri;
            _logger.AddStringEvent($"GET start:{url}");
            fillHeaders(authToken);
            DateTime dStart = DateTime.Now;
            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url),
                //Content = new StringContent(bodyText)
                Content = new StringContent(bodyText, Encoding.UTF8, "application/json")
            };
            try
            {
                HttpResponseMessage response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsStringAsync();
                }
                _lastHttpStatusCode = response.StatusCode;
            }
            catch (Exception e)
            {
                _logger.AddStringEvent($"GET error:[{url}], delay [{(dStart - DateTime.Now).Milliseconds}], error [{e.Message}]");
                HandleError.Process("ServerRequest", "HttpRequestGETwithBody", e, false);
            }
            _logger.AddStringEvent($"GET end:[{url}], delay [{(dStart - DateTime.Now).Milliseconds}]");
            return result;
        }
        public async Task<string> HttpRequestGet(string relativeUrl, string authToken)
        {
            string result = string.Empty;
            string url = new Uri(_apiBaseUri, relativeUrl).AbsoluteUri;
            _logger.AddStringEvent($"GET start:{url}");
            DateTime dStart = DateTime.Now;
            fillHeaders(authToken);
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsStringAsync();
                }
                _lastHttpStatusCode = response.StatusCode;
            }
            catch (Exception e)
            {
                _logger.AddStringEvent($"GET error:[{url}], delay [{(dStart - DateTime.Now).Milliseconds}], error [{e.Message}]");
                HandleError.Process("ServerRequest", "HttpRequestGET", e, false);
            }
            _logger.AddStringEvent($"GET end:[{url}], delay [{(dStart - DateTime.Now).Milliseconds}]");
            return result;
        }

        public async Task<string> HttpRequestPost(string relativeUrl, string authToken, string parameters)
        {
            string result = string.Empty;
            string url = new Uri(_apiBaseUri, relativeUrl).AbsoluteUri;
            _logger.AddStringEvent($"POST start:{url}");
            DateTime dStart = DateTime.Now;
            fillHeaders(authToken);
            try
            {
                HttpResponseMessage response = await _httpClient.PostAsync(url, new StringContent(parameters, System.Text.Encoding.UTF8, "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsStringAsync();
                }
                _lastHttpStatusCode = response.StatusCode;
            }
            catch (Exception e)
            {
                _logger.AddStringEvent($"POST error:[{url}], delay [{(dStart - DateTime.Now).Milliseconds}], error [{e.Message}]");
                HandleError.Process("ServerRequest", "HttpRequestPost", e, false);
            }
            _logger.AddStringEvent($"POST end:[{url}], delay [{(dStart - DateTime.Now).Milliseconds}]");
            return result;
            
        }
        public async Task<bool> HttpRequestGetFile(string fileUrl, string fullNameFile, string authToken, bool urlRelative = true)
        {
            _lastHttpStatusCode = 0;
            bool result = false;
            string url = urlRelative ? new Uri(_apiBaseUri, fileUrl).AbsoluteUri : new Uri(fileUrl).AbsoluteUri;
            _logger.AddStringEvent($"GET file start:{url}");
            DateTime dStart = DateTime.Now;
            fillHeaders(authToken);
            
            using (HttpResponseMessage response =
                await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
            {
                if (response.IsSuccessStatusCode)
                {
                    using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync())
                    {
                        using (Stream streamToWriteTo = File.Open(fullNameFile, FileMode.Create))
                        {
                            try
                            {
                                await streamToReadFrom.CopyToAsync(streamToWriteTo);
                                result = true;
                            }
                            catch (Exception e)
                            {
                                _logger.AddStringEvent($"GET error:[{url}], delay [{(dStart - DateTime.Now).Milliseconds}], error [{e.Message}]");
                                HandleError.Process("ServerRequest", "HttpRequestGetFile", e, false, $"Fullnamefile:{fullNameFile}");
                            }
                        }
                    }
                }
                else
                {
                    HandleError.Process("ServerRequest", "HttpRequestGetFile", new Exception($"Http error [{response.StatusCode}]"), false, $"Fullnamefile:{fullNameFile}");
                }
                _lastHttpStatusCode = response.StatusCode;
            }
            _logger.AddStringEvent($"GET file end:[{url}], delay [{(dStart - DateTime.Now).Milliseconds}]");
            return result;
        }
        
        private void fillHeaders(string authToken)
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + authToken);
        }
    }
}
