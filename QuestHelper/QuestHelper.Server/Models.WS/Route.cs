using System;
using System.Collections.Generic;

namespace QuestHelper.Server.Models.WS
{
    public class Route : ModelBase
    {
        public ICollection<RoutePoint> Points { get; set; }
        public Route(Models.Route dbRoute)
        {
            if (dbRoute != null)
            {
                Points = new List<RoutePoint>();
                Name = dbRoute.Name;
                CreatorId = dbRoute.CreatorId;
                IsShared = dbRoute.IsShared;
                IsPublished = dbRoute.IsPublished;
                IsDeleted = dbRoute.IsDeleted;
                Id = dbRoute.RouteId;
                CreateDate = dbRoute.CreateDate;
                Version = dbRoute.Version;
            }
        }
        public string Name { get; set; }
        public string CreatorId { get; set; }
        public bool IsShared { get; set; }
        public bool IsPublished { get; set; }
    }
}
