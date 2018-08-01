using QuestHelper.Model.DB;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuestHelper.WS
{
    public interface IRoutesApiRequest
    {
        Task<List<Route>> GetRoutes();
        Task<bool> AddRoute(Route routeObject);
    }
}