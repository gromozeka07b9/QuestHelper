using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Net.Http;
using QuestHelper.LocalDB.Model;
using Newtonsoft.Json.Linq;

namespace QuestHelper.WS
{
    public class ApiRequest
    {
        public ApiRequest()
        {
        }

        public async Task<string> HttpRequestGET(string url)
        {
            string result = string.Empty;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
            request.Method = "GET";

            try
            {
                using (WebResponse response = await request.GetResponseAsync())
                {
                    var webresponse = (HttpWebResponse)response;
                    using (System.IO.Stream stream = response.GetResponseStream())
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            result = reader.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                HandleError.Process("ApiRequest", "HttpRequestGET", e, false);
            }
            return result;
        }

        public async Task<bool> HttpRequestGetFile(string url, string fullNameFile)
        {
            bool result = false;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
            request.Method = "GET";

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
                                result = true;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                HandleError.Process("ApiRequest", "HttpRequestGET", e, false);
            }
            return result;
        }

        public async Task<string> HttpRequestPOST(string url, string parameters)
        {
            string result = string.Empty;
            var client = new HttpClient();
            try
            {
                var requestResult = await client.PostAsync(url, new StringContent(parameters, System.Text.Encoding.UTF8, "application/json"));
                if (requestResult.IsSuccessStatusCode)
                {
                    result = await requestResult.Content.ReadAsStringAsync();
                }
            }
            catch (Exception e)
            {
                HandleError.Process("ApiRequest", "HttpRequestPOST", e, false);
            }
            return result;
        }
        public async Task<bool> HttpRequestDELETE(string url)
        {
            bool result = false;
            var client = new HttpClient();
            try
            {
                var requestResult = await client.DeleteAsync(url);
                result = requestResult.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                HandleError.Process("ApiRequest", "HttpRequestPOST", e, false);
            }
            return result;
        }
    }
}
