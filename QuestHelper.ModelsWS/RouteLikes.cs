using System;
using System.Collections.Generic;

namespace QuestHelper.SharedModelsWS
{
    public class RouteLikes
    {
        public RouteLikes()
        {
        }
        public string RouteId { get; set; }
        public string UserId { get; set; }
        public int IsLike { get; set; }
        public DateTime SetDate { get; set; }

    }
}
