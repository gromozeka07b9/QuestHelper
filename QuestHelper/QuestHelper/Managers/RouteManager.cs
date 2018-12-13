﻿using System;
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
        readonly Realm _realmInstance;
        public RouteManager()
        {
            _realmInstance = RealmAppInstance.GetAppInstance();
        }
        internal IEnumerable<Route> GetRoutes()
        {
            var points = _realmInstance.All<Route>();
            return points;
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