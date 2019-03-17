using System;
using System.ComponentModel.DataAnnotations;
using MySql.Data.MySqlClient;

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
        public bool ImageLoadedToServer { get; set; }
        public bool ImagePreviewLoadedToServer { get; set; }
        /// <summary>
        /// Версия записи
        /// </summary>
        public int Version { get; set; }
        public bool IsDeleted { get; set; }
    }
}
