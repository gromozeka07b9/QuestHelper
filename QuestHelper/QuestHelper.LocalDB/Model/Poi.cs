using Realms;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuestHelper.LocalDB.Model
{
    public class Poi : RealmObject
    {
        [PrimaryKey]
        public string PoiId { get; set; } = Guid.NewGuid().ToString();
        //public string RouteId { get; set; }//НЕ Используется!
        public string CreatorId { get; set; }
        public string Name { get; set; }
        public DateTimeOffset CreateDate { get; set; } = DateTime.Now;
        public DateTimeOffset UpdateDate { get; set; } = DateTime.Now;
        public string Address { get; set; }
        public string ByRoutePointId { get; set; }
        public int PoiType { get; set; }
        public bool IsPublished { get; set; }
        public bool IsDeleted { get; set; }
        public string ImgFilename { get; set; }
        public string Description { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int LikesCount { get; set; }
        public int ViewsCount { get; set; }
        public IList<RoutePointMediaObject> MediaObjects { get; }
    }
}
