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
            _realmInstance = RealmAppInstance.GetAppInstance();
        }
        internal IEnumerable<Route> GetRoutes()
        {
            var points = _realmInstance.All<Model.DB.Route>();
            return points;
        }

        internal bool Add(Route route)
        {
            bool result = false;
            try
            {
                _realmInstance.Write(() =>
                {
                    _realmInstance.Add(route);
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

        internal void UpdateLocalData(List<Route> routes)
        {
            foreach(var route in routes)
            {
                Add(route);
            }
        }
    }
}
