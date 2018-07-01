using System;
using System.Collections.Generic;
using System.Text;
using QuestHelper.Model.DB;
using Realms;

namespace QuestHelper.Managers
{
    public class RouteManager
    {
        Realm _realmInstance;
        public RouteManager()
        {
            //_realmInstance = Realm.GetInstance();
            _realmInstance = RealmAppInstance.GetAppInstance();
        }
        internal IEnumerable<Route> GetRoutes()
        {
            var points = _realmInstance.All<Model.DB.Route>();
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
            catch (Exception)
            {
                //пишем лог
            }
            return result;
        }
    }
}
