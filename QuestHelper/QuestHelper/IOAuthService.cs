using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using QuestHelper.Model;

namespace QuestHelper
{
    public interface IOAuthService
    {
        Task<OAuthUser> LoginAsync();
        void Logout();

    }
}
