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
        //public string RouteHash { get; set; }//ToDo: Возможно и не нужен, надо подумать
        public FeedItemType ItemType { get; set; }
    }
}
