using System;
using System.Collections.Generic;

namespace QuestHelper.SharedModelsWS
{
    public class FeedItem : ModelBase
    {
        public FeedItem()
        {
        }
        public string Name { get; set; }
        public string CreatorId { get; set; }
        public string CreatorName { get; set; }
        public string ImgUrl { get; set; }
        public string Description { get; set; }
        public int LikeCount { get; set; }
        public int DislikeCount { get; set; }
        public int ViewCount { get; set; }
        public int IsUserLiked { get; set; }
        public int IsUserViewed { get; set; }
        public FeedItemType ItemType { get; set; }
    }
}
