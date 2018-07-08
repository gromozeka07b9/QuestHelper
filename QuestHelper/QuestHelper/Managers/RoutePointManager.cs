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
            return _realmInstance.All<Model.DB.RoutePoint>().Where(point => point.MainRoute == routeItem).OrderBy(point=>point.UpdateDate).ThenByDescending(point=>point.UpdateDate);
        }
        
        internal bool Add(RoutePoint point, Route route)
        {
            bool result = false;
            try
            {
                _realmInstance.Write(() =>
                {
                    _realmInstance.Add(route);
                    point.IsNew = false;
                    point.CreateDate = DateTime.Now;
                    point.UpdateDate = DateTime.Now;
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
