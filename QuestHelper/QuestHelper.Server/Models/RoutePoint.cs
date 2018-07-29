using System;
using System.ComponentModel.DataAnnotations;

namespace QuestHelper.Server.Models
{
    public class RoutePoint
    {
        public RoutePoint()
        {
        }
        [Key]
        public string RoutePointId { get; set; }
        public string RouteId { get; set; }
        public string Name { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UpdatedUserId { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}
