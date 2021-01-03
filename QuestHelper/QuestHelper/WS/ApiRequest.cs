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
using QuestHelper.LocalDB.Model;
using Newtonsoft.Json.Linq;

namespace QuestHelper.WS
{
    public class ApiRequest
    {
        //static readonly HttpClient httpClient = new HttpClient(); ToDo: Переделать на GetAsync!

        private HttpStatusCode _lastHttpStatusCode = 0;
        public ApiRequest()
        {
        }

        public HttpStatusCode LastHttpStatusCode => _lastHttpStatusCode;

        public async Task<string> HttpRequestGET(string url, string authToken)
        {
            _lastHttpStatusCode = 0;
            string result = string.Empty;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
            request.Method = "GET";
            request.Headers.Add("Authorization", "Bearer " + authToken);
            request.PreAuthenticate = !string.IsNullOrEmpty(authToken);
            try
            {
                using (WebResponse response = request.GetResponseAsync().GetAwaiter().GetResult())
                {
                    var webresponse = (HttpWebResponse) response;
                    using (System.IO.Stream stream = response.GetResponseStream())
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            result = reader.ReadToEnd();
                            _lastHttpStatusCode = webresponse.StatusCode;
                        }
                    }
                }
            }
            catch (WebException webException)
            {
                _lastHttpStatusCode = ((HttpWebResponse) webException.Response).StatusCode;
                HandleError.Process("ApiRequest", "HttpRequestGET", webException, false);
            }
            catch (Exception e)
            {
                HandleError.Process("ApiRequest", "HttpRequestGET", e, false);
            }
            return result;
        }

        //ToDo: Переделать на httpClient! Сейчас запросы GET СИНХРОННЫЕ!!!
        /*public async Task<string> HttpRequestGET(string url, string authToken)
        {
            string result = string.Empty;
            try
            {
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + authToken);
                HttpResponseMessage response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsStringAsync();
                }
                _lastHttpStatusCode = response.StatusCode;
            }
            catch (Exception e)
            {
                HandleError.Process("ApiRequest", "HttpRequestGET", e, false);
            }
            return result;
        }*/

        public async Task<bool> HttpRequestGetFile(string url, string fullNameFile, string authToken)
        {
            _lastHttpStatusCode = 0;
            bool result = false;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
            request.Method = "GET";
            request.Headers.Add("Authorization", "Bearer " + authToken);
            request.PreAuthenticate = !string.IsNullOrEmpty(authToken);

            try
            {
                using (WebResponse response = await request.GetResponseAsync())
                {
                    var webresponse = (HttpWebResponse)response;
                    
                    using (System.IO.Stream stream = response.GetResponseStream())
                    {
                        if (stream != null)
                        {
                            using (FileStream outputfile = File.Create(fullNameFile))
                            {
                                stream.CopyTo(outputfile);
                                _lastHttpStatusCode = webresponse.StatusCode;
                                result = true;
                            }
                        }
                    }
                }
            }
            catch (WebException webException)
            {
                _lastHttpStatusCode = ((HttpWebResponse)webException.Response).StatusCode;
                HandleError.Process("ApiRequest", "HttpRequestGETFile", webException, false, $"Fullnamefile:{fullNameFile}");
            }
            catch (Exception e)
            {
                HandleError.Process("ApiRequest", "HttpRequestGETFile", e, false, $"Fullnamefile:{fullNameFile}");
            }
            return result;
        }

        public async Task<string> HttpRequestPOST(string url, string parameters, string authToken)
        {
            _lastHttpStatusCode = 0;
            string result = string.Empty;
            var client = new HttpClient();
            if (!string.IsNullOrEmpty(authToken))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
            }
            try
            {
                var requestResult = await client.PostAsync(url, new StringContent(parameters, System.Text.Encoding.UTF8, "application/json"));
                _lastHttpStatusCode = requestResult.StatusCode;
                if (requestResult.IsSuccessStatusCode)
                {
                    result = await requestResult.Content.ReadAsStringAsync();
                }
            }
            catch (WebException webException)
            {
                _lastHttpStatusCode = ((HttpWebResponse)webException.Response).StatusCode;
                HandleError.Process("ApiRequest", "HttpRequestPOST", webException, false);
            }
            catch (Exception e)
            {
                HandleError.Process("ApiRequest", "HttpRequestPOST", e, false);
            }
            return result;
        }
        public async Task<bool> HttpRequestDELETE(string url, string authToken)
        {
            _lastHttpStatusCode = 0;
            bool result = false;
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
            try
            {
                var requestResult = await client.DeleteAsync(url);
                _lastHttpStatusCode = requestResult.StatusCode;
                result = requestResult.IsSuccessStatusCode;
            }
            catch (WebException webException)
            {
                _lastHttpStatusCode = ((HttpWebResponse)webException.Response).StatusCode;
                HandleError.Process("ApiRequest", "HttpRequestDELETE", webException, false);
            }
            catch (Exception e)
            {
                HandleError.Process("ApiRequest", "HttpRequestDELETE", e, false);
            }
            return result;
        }
    }
}
