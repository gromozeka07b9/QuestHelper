using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuestHelper
{
    public interface IAuthService
    {
        Task<string> GetAuthTokenAsync();
        Task SetAuthTokenAsync(string authToken);
    }
}
