using QuestHelper.LocalDB.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuestHelper.WS
{
    public interface IRoutePointMediaObjectRequest
    {
        Task<List<RoutePointMediaObject>> GetRoutePointMediaObjects(string routePointId);
        Task<bool> AddRoutePointMediaObject(RoutePointMediaObject routePointMediaObject);
    }
}