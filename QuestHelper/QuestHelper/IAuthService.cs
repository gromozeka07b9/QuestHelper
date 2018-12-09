using System;
using System.Collections.Generic;
using System.Text;

namespace QuestHelper
{
    public interface IAuthService
    {
        string GetAuthToken();
        void SetAuthToken(string authToken);
    }
}
