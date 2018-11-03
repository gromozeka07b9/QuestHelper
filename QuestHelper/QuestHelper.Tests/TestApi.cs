using QuestHelper.LocalDB.Model;
using QuestHelper.Managers;
using QuestHelper.Model;
using QuestHelper.WS;
using System;
using System.Threading.Tasks;
using Xunit;

namespace QuestHelper.Tests
{
    public class TestApi
    {
        private string apiUrl = "http://questhelperserver.azurewebsites.net";

        [Fact]
        public async Task TestMust_GetRoutesAsync()
        {
            RoutesApiRequest _routesApi = new RoutesApiRequest(apiUrl);
            var result = await _routesApi.GetRoutes();
            Assert.True(result.Count > 0);
        }

        [Fact]
        public async Task TestMust_AddAndDeleteRouteAsync()
        {
            bool result = false;
            Route route = new Route();
            string routeName = "XunitTests";
            route.Name = routeName;
            RoutesApiRequest _routesApi = new RoutesApiRequest(apiUrl);
            var added = await _routesApi.AddRoute(route);
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
        }
        [Fact]
        public async Task TestMust_GetSyncStatusAsync()
        {
            bool result = false;
            Route route = new Route();
            string routeName = "XunitTests";
            route.Name = routeName;
            RoutesApiRequest _routesApi = new RoutesApiRequest(apiUrl);
            //var added = await _routesApi.(route);

            Assert.True(result);
        }
    }
}
