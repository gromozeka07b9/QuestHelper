using Realms;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuestHelper.Model.DB
{
    public class RoutePoint : RealmObject
    {
        [PrimaryKey]
        public string RoutePointId { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        public DateTimeOffset UpdateDate { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Description { get; set; }
        public bool IsNew { get; set; } = true;
        public Route MainRoute { get; set; }
        public IList<RoutePointMediaObject> MediaObjects { get; }

        public string CoordinatesByText
        {
            get
            {
                string coordsText = "Not defined";
                if ((Latitude > 0) && (Longitude > 0))
                {
                    coordsText = $"{Latitude},{Longitude}";
                }
                return coordsText;
            }
            }
    }
}
