using QuestHelper.LocalDB.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuestHelper.WS
{
    public interface IRoutePointsApiRequest
    {
        Task<List<RoutePoint>> GetRoutePoints(string routeId);
        Task<bool> AddRoutePoint(RoutePoint routePointObject);
    }
}