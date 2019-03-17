using System;
using System.Collections.Generic;
using System.Text;

namespace QuestHelper.Model.WS
{
    public class ShareRequest
    {
        public string RouteIdForShare;
        public string[] UserId;
        public bool CanChangeRoute;
    }
}
