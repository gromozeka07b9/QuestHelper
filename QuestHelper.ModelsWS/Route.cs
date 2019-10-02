using System;
using System.Collections.Generic;

namespace QuestHelper.SharedModelsWS
{
    public class Route : ModelBase
    {
        public ICollection<RoutePoint> Points { get; set; }
        public Route()
        {
            Points = new List<RoutePoint>();
        }
        public string Name { get; set; }
        public string CreatorId { get; set; }
        public bool IsShared { get; set; }
        public bool IsPublished { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        public int Version { get; set; }
        public bool IsDeleted { get; set; }
        public string ImgFilename { get; set; }
        public string Description { get; set; }
    }
}
