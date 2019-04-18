using System;
using System.Collections.Generic;

namespace QuestHelper.SharedModelsWS
{
    public class RouteVersion : ModelVersionBase
    {
        public ICollection<PointVersion> Points { get; set; }
        public string ObjVerHash = string.Empty;
        public string ObjVer = string.Empty;
        public RouteVersion()
        {
            Points = new List<PointVersion>();
        }
    }
}
