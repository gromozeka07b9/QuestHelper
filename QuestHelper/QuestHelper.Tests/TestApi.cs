using QuestHelper.LocalDB.Model;
using QuestHelper.WS;
using System;
using System.Collections.Generic;
using System.Linq;
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
        }
        [Fact]
        public async Task TestMust_GetSyncStatusRoute_NoChanges_Async()
        {
            //Arrange
            Route route = new Route();
            string routeName = "XunitTests";
            route.Name = routeName;
            route.Version = 123;
            bool added = await new RoutesApiRequest(apiUrl).UpdateRoute(route);

            if (!added)
            {
                throw new Exception("route doesn't created");
            }

            //Act
            IEnumerable<Route> routes = new List<Route>() { route };
            var syncStatus = new RoutesApiRequest(apiUrl).GetSyncStatus(routes);
            var deleteResult = await new RoutesApiRequest(apiUrl).DeleteRoute(route);
            if (!deleteResult) throw new Exception("Error while deleting route");

            //Assert
            //если версия маршрута на сервере и клиенте одна и та же, он не возвращается с сервера                       
            var routePresent = syncStatus.Result.Statuses.Any(r => r.ObjectId == route.RouteId);
            Assert.True(!routePresent);
        }

        [Fact]
        public async Task TestMust_GetSyncStatusRoute_Changed_Async()
        {
            //Arrange
            Route route = new Route();
            string routeName = "XunitTests";
            route.Name = routeName;
            route.Version = 123;
            bool added = await new RoutesApiRequest(apiUrl).UpdateRoute(route);

            if (!added)
            {
                throw new Exception("route doesn't created");
            }

            //Act
            //На клиенте версия другая
            route.Version++;
            IEnumerable<Route> routes = new List<Route>() { route };
            var syncStatus = new RoutesApiRequest(apiUrl).GetSyncStatus(routes);
            var deleteResult = await new RoutesApiRequest(apiUrl).DeleteRoute(route);
            if (!deleteResult) throw new Exception("Error while deleting route");

            //Assert
            //если версия маршрута на сервере и клиенте одна и та же, он не возвращается с сервера                       
            var routePresent = syncStatus.Result.Statuses.Any(r => r.ObjectId == route.RouteId);
            Assert.True(routePresent);
        }

        [Fact]
        public async Task TestMust_GetSyncStatusRoute_ServerHasNewRoute_Async()
        {
            //Arrange
            Route route = new Route();
            string routeName = "XunitTests";
            route.Name = routeName;
            route.Version = 123;
            bool added = await new RoutesApiRequest(apiUrl).UpdateRoute(route);

            if (!added)
            {
                throw new Exception("route doesn't created");
            }

            //Act
            //На клиенте еще нет этого маршрута, сервер должен его вернуть
            IEnumerable<Route> routes = new List<Route>() {  };
            var syncStatus = new RoutesApiRequest(apiUrl).GetSyncStatus(routes);
            var deleteResult = await new RoutesApiRequest(apiUrl).DeleteRoute(route);
            if (!deleteResult) throw new Exception("Error while deleting route");

            //Assert
            //если версия маршрута на сервере и клиенте одна и та же, он не возвращается с сервера                       
            var routePresent = syncStatus.Result.Statuses.Any(r => r.ObjectId == route.RouteId);
            Assert.True(routePresent);
        }
    }
}
