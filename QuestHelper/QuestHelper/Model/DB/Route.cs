using Realms;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuestHelper.Model.DB
{
    public class Route : RealmObject
    {
        public string Name { get; set; }
        public IList<RoutePoint> Points { get; }
    }
}
