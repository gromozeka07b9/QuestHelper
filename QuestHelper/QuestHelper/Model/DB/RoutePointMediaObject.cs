using Realms;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuestHelper.Model.DB
{
    public class RoutePointMediaObject : RealmObject
    {
        [PrimaryKey]
        public string RoutePointMediaObjectId { get; set; } = Guid.NewGuid().ToString();
        public string FileName { get; set; }
        public byte[] PreviewImage { get; set; } //Не может быть больше 16мб, ограничение Realm
        public string RoutePointId { get; set; }
        public RoutePoint Point { get; set; }
    }
}
