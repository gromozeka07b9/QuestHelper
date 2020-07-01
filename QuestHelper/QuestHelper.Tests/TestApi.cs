using QuestHelper.LocalDB.Model;
using QuestHelper.WS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QuestHelper.Model;
using Xunit;

namespace QuestHelper.Tests
{
    public class TestApi
    {
        private string apiUrl = "http://igosh.pro/api";

        /*[Theory]
        [InlineData("d892d420-f5e7-4263-b0bc-d9b433b9a4ef")]
        public async Task TestMust_MakeRequestDowload_Async(string mediaObjectId)
        {
            bool result = false;
            string routePointId = "not used";
            string fileId = $"img_{mediaObjectId}.jpg";
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync($"{apiUrl}/RoutePointMediaObjects/{routePointId}/{mediaObjectId}/{fileId}");
                result = response.IsSuccessStatusCode;
            }

            Assert.True(result);
        }*/

    }
}
