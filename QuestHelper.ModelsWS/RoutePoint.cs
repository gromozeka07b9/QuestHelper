using System;
using System.Collections.Generic;

namespace QuestHelper.SharedModelsWS
{
    public class RoutePoint : ModelBase
    {
        public ICollection<RoutePointMediaObject> MediaObjects { get; set; }
        public RoutePoint()
        {
            MediaObjects = new List<RoutePointMediaObject>();
        }
        public string RouteId { get; set; }
        public string Name { get; set; }
        public DateTimeOffset UpdateDate { get; set; }
        public string UpdatedUserId { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
    }
}
