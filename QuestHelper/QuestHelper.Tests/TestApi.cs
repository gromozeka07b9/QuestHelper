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
        //private string apiUrl = "http://localhost:31193";
        private string authToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiU2VyZ2V5IER5YWNoZW5rbyIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6ImFkbWluIiwibmJmIjoxNTQ1MTY1ODQwLCJleHAiOjE1NDYwMjk4NDAsImlzcyI6ImlHb3NoLnBybyIsImF1ZCI6Imh0dHA6Ly9pZ29zaC5wcm8iLCJVc2VyS2V5IjoiMCJ9.tEq3tKJVcJJr87lbhOE4RbU8js4p7NFUUnmxaQj-EkU";

        [Fact]
        public async Task TestMust_GetRoutesAsync()
        {
            RoutesApiRequest _routesApi = new RoutesApiRequest(apiUrl, authToken);
            var result = await _routesApi.GetRoutes();
            Assert.True(result.Count > 0);
        }

        /*[Fact]
        public async Task TestMust_AddAndDeleteRouteAsync()
        {
            bool result = false;
            Route route = new Route();
            string routeName = "XunitTests";
            route.Name = routeName;
            RoutesApiRequest _routesApi = new RoutesApiRequest(apiUrl);
            var added = await _routesApi.UpdateRoute(route);
            if(added)
            {
                var all = await _routesApi.GetRoutes();
                var found = all.FindAll(t => t.Name == routeName);
                if(found.Count > 0)
                {
                    foreach(var item in found)
                    {
                        result = await _routesApi.DeleteRoute(item);
                        if (!result) throw new Exception("Error while deleting route");
                    }
                }
            }
            Assert.True(result);
        }*/
        /*[Fact]
        public async Task TestMust_GetSyncStatusRoute_NoChanges_Async()
        {
            //Arrange
            Route route = new Route();
            string routeName = "XunitTests";
            route.Name = routeName;
            route.Version = 123;

            //Act
            IEnumerable<Route> routes = new List<Route>() { route };
            var syncStatus = new RoutesApiRequest(apiUrl).GetSyncStatus(routes.Select(x => new Tuple<string, int>(x.RouteId, x.Version)));
            if (syncStatus.IsCompleted)
            {
                var deleteResult = await new RoutesApiRequest(apiUrl).DeleteRoute(route);
                if (!deleteResult) throw new Exception("Error while deleting route");
            }

            //Assert
            //если версия маршрута на сервере и клиенте одна и та же, он не возвращается с сервера                       
            var routePresent = syncStatus.Result.Statuses.Any(r => r.ObjectId == route.RouteId);
            Assert.True(!routePresent);
        }*/

        [Fact]
        public async Task TestMust_GetSyncStatusRoute_Changed_Async()
        {
            //Arrange
            Route route = new Route();
            string routeName = "XunitTests";
            route.Name = routeName;
            route.Version = 123;

            //Act
            //На клиенте версия другая
            route.Version++;
            IEnumerable<Route> routes = new List<Route>() { route };
            var syncStatus = new RoutesApiRequest(apiUrl, authToken).GetSyncStatus(routes.Select(x => new Tuple<string, int>(x.RouteId, x.Version)));
            var deleteResult = await new RoutesApiRequest(apiUrl, authToken).DeleteRoute(route);
            if (!deleteResult) throw new Exception("Error while deleting route");

            //Assert
            //если версия маршрута на сервере и клиенте одна и та же, он не возвращается с сервера                       
            var routePresent = syncStatus.Result.Statuses.Any(r => r.ObjectId == route.RouteId);
            Assert.True(routePresent);
        }

        /*[Fact]
        public async Task TestMust_GetSyncStatusRoute_ServerHasNewRoute_Async()
        {
            //Arrange
            Route route = new Route();
            string routeName = "XunitTests";
            route.Name = routeName;
            route.Version = 123;

            //Act
            //На клиенте еще нет этого маршрута, сервер должен его вернуть
            IEnumerable<Route> routes = new List<Route>() {  };
            var syncStatus = new RoutesApiRequest(apiUrl).GetSyncStatus(routes.Select(x => new Tuple<string, int>(x.RouteId, x.Version)));
            var deleteResult = await new RoutesApiRequest(apiUrl).DeleteRoute(route);
            if (!deleteResult) throw new Exception("Error while deleting route");

            //Assert
            //если версия маршрута на сервере и клиенте одна и та же, он не возвращается с сервера                       
            var routePresent = syncStatus.Result.Statuses.Any(r => r.ObjectId == route.RouteId);
            Assert.True(routePresent);
        }*/
        [Theory]
        [InlineData("2ba9c945-3c41-4fef-9d07-aaebe15b11cd")]
        public async Task TestMust_MakeRequestUpload_Async(string mediaObjectId)
        {
            bool result = false;
            string testBlobFilename = "1testblob.jpg";
            string routePointId = "not used";
            string fileId = $"img_{mediaObjectId}.jpg";
            Stream image = File.Open(@".\testblob\" + testBlobFilename, FileMode.Open);
            HttpContent content = new StreamContent(image);
            content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data") { Name = "file", FileName = fileId };
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            using (var client = new HttpClient())
            {
                using (var formData = new MultipartFormDataContent())
                {
                    formData.Add(content);
                    var response = await client.PostAsync($"{ apiUrl }/RoutePointMediaObjects/1/{ mediaObjectId }/uploadfile", formData);
                    result = response.IsSuccessStatusCode;
                }
            }

            Assert.True(result);
        }

        [Theory]
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
        }

        /*[Fact]
        public async Task TestMust_GetSyncStatusRoutePoint_Async()
        {
            //Arrange

            //Act
            var syncStatus = await new RoutePointsApiRequest(apiUrl).GetSyncStatus(new List<Tuple<string, int>>(){});

            //Assert
            Assert.True(syncStatus.Statuses.Count > 0);
            Assert.DoesNotContain(syncStatus.Statuses, x => x.ObjectId == string.Empty);
            Assert.DoesNotContain(syncStatus.Statuses, x => x.Version == 0);
        }*/


    }
}
