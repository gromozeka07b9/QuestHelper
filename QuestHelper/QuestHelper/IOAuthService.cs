using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuestHelper
{
    public interface IOAuthService
    {
        bool Login();
        void Logout();

    }
}
