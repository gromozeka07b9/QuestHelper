using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuestHelper.LocalDB.Model;
using QuestHelper.Model;
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
            var points = _realmInstance.All<Route>();
            return points;
        }

        /*internal bool Add(Route route)
        {
            bool result = false;
            try
            {
                _realmInstance.Write(() =>
                {
                    //_realmInstance.Add(route, true);
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
        }*/

        internal void UpdateLocalData(List<Route> routes)
        {
            /*foreach(var route in routes)
            {
                Add(route);
            }*/
        }

        internal IEnumerable<Route> GetNotSynced()
        {
            return _realmInstance.All<Route>().Where(item => !item.ServerSynced);
        }
        public bool SetSyncStatus(string Id, bool Status)
        {
            bool result = false;
            try
            {
                _realmInstance.Write(() =>
                {
                    var route = _realmInstance.Find<Route>(Id);
                    route.ServerSynced = Status;
                    route.ServerSyncedDate = DateTime.Now;
                }
                );
                result = true;
            }
            catch (Exception e)
            {
                HandleError.Process("RouteManager", "SetSyncStatus", e, false);
            }
            return result;
        }

        internal bool Save(ViewRoute viewRoute)
        {
            bool result = false;
            RouteManager routeManager = new RouteManager();
            try
            {
                _realmInstance.Write(() =>
                {
                    Route route;
                    if (string.IsNullOrEmpty(viewRoute.Id))
                    {
                        route = new Route();
                        _realmInstance.Add(route);
                    }
                    else
                    {
                        route = _realmInstance.Find<Route>(viewRoute.Id);
                    }
                    route.Name = viewRoute.Name;
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
    }
}
