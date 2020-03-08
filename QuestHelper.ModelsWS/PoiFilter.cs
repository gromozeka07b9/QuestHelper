using System;

namespace QuestHelper.SharedModelsWS
{
    public class PoiFilter
    {
        public string SearchString;
        public string CreatorId;
        public bool IsPrivate;
        public DateTimeOffset DateFrom;
        public DateTimeOffset DateTo;
        public double LatitudeFrom;
        public double LongitudeFrom;
        public double LatitudeTo;
        public double LongitudeTo;
        public int MaxItems;
    }
}