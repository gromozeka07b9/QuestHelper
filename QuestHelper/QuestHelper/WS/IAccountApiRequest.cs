using QuestHelper.LocalDB.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static QuestHelper.WS.AccountApiRequest;

namespace QuestHelper.WS
{
    public interface IAccountApiRequest
    {
        Task<TokenResponse> GetTokenAsync(string login, string password, bool demomode);
    }
}