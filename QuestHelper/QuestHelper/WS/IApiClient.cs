using QuestHelper.Model.DB;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuestHelper.WS
{
    public interface IApiClient
    {
        Task<List<Route>> GetRoutes();
    }
}