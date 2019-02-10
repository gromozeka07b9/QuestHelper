using System;
using System.ComponentModel.DataAnnotations;

namespace QuestHelper.Server.Models
{
    public class RoutePoint : IVersionedObject
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
        public string Address { get; set; }
        public string Description { get; set; }
        /// <summary>
        /// Версия записи
        /// </summary>
        public int Version { get; set; }
        public bool IsDeleted { get; set; }

        public string GetObjectId()
        {
            return RoutePointId;
        }

        public int GetVersion()
        {
            return Version;
        }
    }
}
