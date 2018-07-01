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
            //_realmInstance = Realm.GetInstance();
            _realmInstance = RealmAppInstance.GetAppInstance();
        }
        internal IEnumerable<RoutePoint> GetPointsByRoute(Route routeItem)
        {
            var points = _realmInstance.All<Model.DB.RoutePoint>().Where(point=>point.MainRoute==routeItem);
            /*foreach (var item in points)
            {
                _pointsOfNewRoute.Add($"name:{item.Name} latitude:{item.Latitude} longitude: {item.Longitude}");
            }*/
            return points;
        }

        internal bool Save(RoutePoint point, Route route)
        {
            bool result = false;
            try
            {
                _realmInstance.Write(() =>
                {
                    _realmInstance.Add(route);
                    _realmInstance.Add(point);
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
    }
}
