using Realms;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuestHelper.LocalDB.Model
{
    public class RouteTrackPlace : RealmObject
    {
        [PrimaryKey]
        public string Id { get; set; }
        public string RouteTrackId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string Category { get; set; }
        public string Distance { get; set; }
        public DateTimeOffset DateTimeBegin { get; set; }        
        public DateTimeOffset DateTimeEnd { get; set; }        
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? Elevation { get; set; }
    }
}
