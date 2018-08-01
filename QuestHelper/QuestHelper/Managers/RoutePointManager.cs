using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuestHelper.Model.DB;
using Realms;

namespace QuestHelper.Managers
{
    public class RoutePointManager
    {
        Realm _realmInstance;
        public RoutePointManager()
        {
            _realmInstance = RealmAppInstance.GetAppInstance();
        }
        internal static Realm GetRealmInstance()
        {
            return RealmAppInstance.GetAppInstance();
        }
        internal IEnumerable<RoutePoint> GetPointsByRoute(Route routeItem)
        {
            var collection = _realmInstance.All<Model.DB.RoutePoint>().Where(point => point.MainRoute == routeItem).OrderByDescending(point => point.CreateDate);
            return collection;
        }
        internal RoutePoint GetPointByCoordinates(double latitude, double longitude)
        {
            var collection = _realmInstance.All<Model.DB.RoutePoint>().Where(point => point.Latitude == latitude && point.Longitude == longitude);
            return collection.FirstOrDefault();
        }

        internal bool Add(RoutePoint point, Route route)
        {
            bool result = false;
            try
            {
                _realmInstance.Write(() =>
                {
                    _realmInstance.Add(route, true);
                    point.IsNew = false;
                    //point.CreateDate = point.CreateDate;
                    //point.UpdateDate = point.UpdateDate;
                    //var routeObject = _realmInstance.All<Model.DB.RoutePoint>().Where(x => x.RouteId == route.RouteId).SingleOrDefault();
                    point.MainRoute = route;
                    _realmInstance.Add(point, true);
                }
                );
                result = true;
            }
            catch (Exception e)
            {
                //пишем лог
            }
            return result;
        }

        internal void UpdateLocalData(Route route, List<RoutePoint> points)
        {
            foreach (var point in points)
            {
                Add(point, route);
            }
        }
    }
}
