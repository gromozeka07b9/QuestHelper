using System;
using System.Collections.Generic;
using System.Text;

namespace QuestHelper
{
    public interface IApplicationInstalledService
    {
        bool AppInstalled(string appName);
    }
}
