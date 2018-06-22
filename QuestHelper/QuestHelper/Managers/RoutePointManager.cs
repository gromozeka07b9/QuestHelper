using System;
using System.Collections.Generic;
using System.Text;
using QuestHelper.Model.DB;
using Realms;

namespace QuestHelper.Managers
{
    public class RoutePointManager
    {
        Realm realmInstance;
        public RoutePointManager()
        {
            realmInstance = Realm.GetInstance();
        }
        internal IEnumerable<RoutePoint> GetPointsByRoute()
        {
            var points = realmInstance.All<Model.DB.RoutePoint>();
            /*foreach (var item in points)
            {
                _pointsOfNewRoute.Add($"name:{item.Name} latitude:{item.Latitude} longitude: {item.Longitude}");
            }*/
            return points;
        }
    }
}
