using QuestHelper.LocalDB.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuestHelper.WS
{
    public interface IAccountApiRequest
    {
        Task<string> GetTokenAsync(string login, string password, bool demomode);
    }
}