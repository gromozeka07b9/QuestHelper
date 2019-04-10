using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuestHelper.Server.Models.WS
{
    public class RouteRoot
    {
        public Route Route;
        public ICollection<RoutePoint> Points { get; set; }

        public RouteRoot()
        {
            Points = new List<RoutePoint>();
        }

    }
}
