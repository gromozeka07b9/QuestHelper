using System;
using System.ComponentModel.DataAnnotations;

namespace QuestHelper.Server.Models
{
    public class FeedItem
    {
        public FeedItem()
        {
        }

        [Key]
        public string RouteId { get; set; }
        public string Name { get; set; }
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// Версия записи
        /// </summary>
        public int Version { get; set; }
        public string ImgFilename { get; set; }
        public string CreatorId { get; set; }
        public string CreatorName { get; set; }
        public string Description { get; set; }
        public int ViewCount { get; set; }
        public int LikeCount { get; set; }
        public int IsUserLikes { get; set; }
        public int IsUserViewed { get; set; }
    }
}
