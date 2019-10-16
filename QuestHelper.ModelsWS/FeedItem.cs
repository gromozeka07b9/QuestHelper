using System;
using System.Collections.Generic;

namespace QuestHelper.SharedModelsWS
{
    public class FeedItem : ModelBase
    {
        public FeedItem()
        {
            //Points = new List<RoutePoint>();
        }
        public string Name { get; set; }
        public string CreatorId { get; set; }
        public string CreatorName { get; set; }
        public string ImgUrl { get; set; }
        public string Description { get; set; }
        public string RouteHash { get; set; }
        public FeedItemType ItemType { get; set; }
    }
}
