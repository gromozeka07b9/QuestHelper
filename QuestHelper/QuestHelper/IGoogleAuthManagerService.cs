using QuestHelper.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuestHelper
{
    public interface IGoogleAuthManagerService
    {
        void Login(Action<GoogleUser, string> OnLoginComplete);
        void Logout();
    }
}
