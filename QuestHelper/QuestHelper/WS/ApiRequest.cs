using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Net.Http;
using QuestHelper.Model.DB;
using Newtonsoft.Json.Linq;

namespace QuestHelper.WS
{
    public class ApiRequest : IApiClient
    {
        private string _hostUrl = string.Empty;
        public ApiRequest(string hostUrl)
        {
            _hostUrl = hostUrl;
        }
        /*public async Task<List<int>> GetChangedIDsFromServer(string tableName, DateTime localMaxTimeStamp)
        {
            List<int> deserializedValue = new List<int>();
            try
            {
                var response = await HttpRequestGET($"{this._hostUrl}/idiom/changes?clientMaxDate={localMaxTimeStamp.ToString("yyyy-MM-ddTHH:mm:ss")}");
                deserializedValue = JsonConvert.DeserializeObject<List<int>>(response);
            }
            catch (Exception)
            {

            }

            return deserializedValue;
        }*/

        /*public async Task<List<Idiom>> GetIdiomsFromServer(List<int> iDs)
        {
            string jsonIds = JsonConvert.SerializeObject(iDs);
            List<Idiom> deserializedValue = new List<Idiom>();
            try
            {
                //var response = await HttpRequest($"{this._hostUrl}/idiom?{idFilter.ToString()}");
                var response = await HttpRequestPOST($"{this._hostUrl}/idiom/array", jsonIds);
                deserializedValue = JsonConvert.DeserializeObject<List<Idiom>>(response);
            }
            catch (Exception E)
            {

            }
            return deserializedValue;
        }*/

        public async Task<List<Route>> GetRoutes()
        {
            List<Route> deserializedValue = new List<Route>();
            try
            {
                var response = await HttpRequestGET($"{this._hostUrl}/api/routes");
                deserializedValue = JsonConvert.DeserializeObject<List<Route>>(response);
            }
            catch (Exception)
            {

            }
            return deserializedValue;
        }
        public async Task<bool> AddRoute(Route routeObject)
        {
            bool addResult = false;
            JObject jsonObject = JObject.FromObject(new {
                RouteId = routeObject.RouteId,
                Name = routeObject.Name,
                CreateDate = routeObject.CreateDate.DateTime,
                UserId = 0
            });
            
            try
            {
                await HttpRequestPOST($"{this._hostUrl}/api/routes", jsonObject.ToString());
                addResult = true;
            }
            catch (Exception)
            {
            }
            return addResult;
        }

        private async Task<string> HttpRequestGET(string url)
        {
            string result = string.Empty;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
            request.Method = "GET";

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
            return result;
        }
        private async Task<string> HttpRequestPOST(string url, string parameters)
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

            }
            return result;
        }
    }
}
