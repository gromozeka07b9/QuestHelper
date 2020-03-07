using System;
using System.Collections.Generic;
using System.Text;

namespace QuestHelper.SharedModelsWS
{
    public class Poi : ModelBase
    {
        public string Name { get; set; }
        public DateTime UpdateDate { get; set; }
        public string CreatorId { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string ByRouteId { get; set; }
    }
}
