using System;
using System.ComponentModel.DataAnnotations;

namespace QuestHelper.Server.Models
{
    public class Route
    {
        public Route()
        {
        }
        [Key]
        public string RouteId { get; set; }
        public string Name { get; set; }
        public DateTime CreateDate { get; set; }
        public string UserId { get; set; }
        /// <summary>
        /// Версия записи
        /// </summary>
        public int Version { get; set; }
    }
}
