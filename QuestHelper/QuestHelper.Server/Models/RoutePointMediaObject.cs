using System;
using System.ComponentModel.DataAnnotations;

namespace QuestHelper.Server.Models
{
    public class RoutePointMediaObject
    {
        public RoutePointMediaObject()
        {
        }
        [Key]
        public string RoutePointMediaObjectId { get; set; }
        public string RoutePointId { get; set; }
        public string FileName { get; set; }
        public string FileNamePreview { get; set; }
        /// <summary>
        /// Версия записи
        /// </summary>
        public int Version { get; set; }
    }
}
