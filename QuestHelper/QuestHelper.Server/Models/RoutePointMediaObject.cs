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
        public bool NeedProcess { get; set; }
        public bool Processed { get; set; }
        public string ProcessResultText { get; set; }
        public MediaObjectTypeEnum MediaType { get; set; }
        /// <summary>
        /// Версия записи
        /// </summary>
        public int Version { get; set; }
        public bool IsDeleted { get; set; }
    }
}
