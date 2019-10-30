using System;
using System.Collections.Generic;
using System.Text;

namespace QuestHelper.Model.Messages
{
    public class SyncProgressImageLoadingMessage
    {
        public string RouteId;
        public double ProgressValue = 0;
    }
}
