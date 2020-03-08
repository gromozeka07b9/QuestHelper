using System;
using System.ComponentModel.DataAnnotations;

namespace QuestHelper.Server.Models
{
    public class Poi : IVersionedObject
    {
        public Poi()
        {
        }
        [Key]
        public string PoiId { get; set; }
        public string Name { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string CreatorId { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        /// <summary>
        /// Создана на основе точки маршрута
        /// </summary>
        public string ByRoutePointId { get; set; }
        /// <summary>
        /// доступна всем
        /// </summary>
        public bool IsPublished { get; set; }
        /// <summary>
        /// имя файла картинки обложки
        /// </summary>
        public string ImgFilename { get; set; }

        /// <summary>
        /// Версия записи
        /// </summary>
        public int Version { get; set; }
        public bool IsDeleted { get; set; }

        public string GetObjectId()
        {
            return PoiId;
        }

        public int GetVersion()
        {
            return Version;
        }
    }
}
