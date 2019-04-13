using QuestHelper.LocalDB.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuestHelper.WS
{
    public interface IRoutesApiRequest
    {
        Task<List<Route>> GetRoutes();
        Task<List<Route>> GetRoutesVersions();
    }
}