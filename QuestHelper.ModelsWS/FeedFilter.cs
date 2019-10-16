using System;

namespace QuestHelper.SharedModelsWS
{
    public class FeedFilter
    {
        public string Description;
        public string Address;
        public string CreatorId;
        public DateTimeOffset DateFrom;
        public DateTimeOffset DateTo;
        public double Latitude;
        public double Longitude;
        public int MaxItems;
    }
}