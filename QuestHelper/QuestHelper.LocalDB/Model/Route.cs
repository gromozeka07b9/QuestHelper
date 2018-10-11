using Realms;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuestHelper.LocalDB.Model
{
    public class Route : RealmObject
    {
        [PrimaryKey]
        public string RouteId { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public DateTimeOffset CreateDate { get; set; } = DateTime.Now;
        public IList<RoutePoint> Points { get; }
    }
}
