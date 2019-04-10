using System;
using System.Collections.Generic;

namespace QuestHelper.Server.Models.WS
{
    public class RouteVersion : ModelVersionBase
    {
        public ICollection<PointVersion> Points { get; set; }
        public RouteVersion()
        {
            Points = new List<PointVersion>();
        }
    }
}
