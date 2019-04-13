using System;
using System.Collections.Generic;

namespace QuestHelper.Server.Models.WS
{
    public class RoutePoint : ModelBase
    {
        public ICollection<RoutePointMediaObject> MediaObjects { get; set; }
        public RoutePoint(Models.RoutePoint dbPoint)
        {
            if (dbPoint != null)
            {
                MediaObjects = new List<RoutePointMediaObject>();
                Name = dbPoint.Name;
                RouteId = dbPoint.RouteId;
                UpdateDate = dbPoint.UpdateDate;
                UpdatedUserId = dbPoint.UpdatedUserId;
                Latitude = dbPoint.Latitude;
                Longitude = dbPoint.Longitude;
                Address = dbPoint.Address;
                Description = dbPoint.Description;
                Id = dbPoint.RoutePointId;
                IsDeleted = dbPoint.IsDeleted;
                Version = dbPoint.Version;
            }
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
