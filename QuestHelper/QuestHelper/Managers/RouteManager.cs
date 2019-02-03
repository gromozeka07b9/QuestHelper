using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuestHelper.LocalDB.Model;
using QuestHelper.Model;
using Realms;
using Xamarin.Essentials;

namespace QuestHelper.Managers
{
    public class RouteManager
    {
        readonly Realm _realmInstance;
        public RouteManager()
        {
            _realmInstance = RealmAppInstance.GetAppInstance();
        }
        internal IEnumerable<Route> GetRoutes()
        {
            return _realmInstance.All<Route>().OrderByDescending(r => r.CreateDate);
        }

        public IEnumerable<Route> GetNotSynced()
        {
            return _realmInstance.All<Route>().Where(item => !item.ServerSynced);
        }

        public bool Save(ViewRoute viewRoute)
        {
            bool result = false;
            RouteManager routeManager = new RouteManager();
            try
            {
                _realmInstance.Write(() =>
                {
                    var route = !string.IsNullOrEmpty(viewRoute.Id) ? _realmInstance.Find<Route>(viewRoute.Id) : null;
                    if (null == route)
                    {
                        route = string.IsNullOrEmpty(viewRoute.Id) ? new Route() : new Route() { RouteId = viewRoute.Id };
                        _realmInstance.Add(route);
                    }
                    route.Name = viewRoute.Name;
                    route.Version = viewRoute.Version;
                    route.CreateDate = viewRoute.CreateDate;
                    route.IsShared = viewRoute.IsShared;
                    viewRoute.Refresh(route.RouteId);
                });
                result = true;
            }
            catch (Exception e)
            {
                HandleError.Process("RouteManager", "AddRoute", e, false);
            }
            return result;
        }

        internal Route GetRouteById(string routeId)
        {
            return _realmInstance.Find<Route>(routeId);
        }

        internal double GetLength(string routeId)
        {
            double length = 0;
            if (!string.IsNullOrEmpty(routeId))
            {
                var route = _realmInstance.Find<Route>(routeId);
                if (route != null)
                {
                    int countPoints = route.Points.Count;
                    for (int index = 0; index < countPoints; index++)
                    {
                        if (index + 1 < countPoints)
                        {
                            var firstPoint = route.Points[index];
                            var secondPoint = route.Points[index + 1];
                            if((firstPoint.Latitude != 0)&&(firstPoint.Longitude!=0))
                            {
                                if ((secondPoint.Latitude != 0) && (secondPoint.Longitude != 0))
                                {
                                    Location firstPointLocation = new Location(firstPoint.Latitude, firstPoint.Longitude);
                                    Location secondPointLocation = new Location(secondPoint.Latitude, secondPoint.Longitude);
                                    length += Location.CalculateDistance(firstPointLocation, secondPointLocation, DistanceUnits.Kilometers);
                                }

                            }
                        }
                    }
                }
            }

            return length;
        }
    }
}
