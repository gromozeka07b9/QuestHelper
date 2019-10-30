using System;
using System.Collections.Generic;
using System.Text;

namespace QuestHelper.Model.Messages
{
    public class SyncProgressRouteLoadingMessage
    {
        public string RouteId;
        public bool SyncInProgress = false;
        public double ProgressValue = 0;
    }
}
