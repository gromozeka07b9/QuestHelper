using Realms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamarin.Forms;

namespace QuestHelper.LocalDB.Model
{
    public class RoutePoint : RealmObject
    {
        [PrimaryKey, Required]
        public string RoutePointId { get; set; } = Guid.NewGuid().ToString();
        public string RouteId { get; set; }
        public string Name { get; set; }
        public DateTimeOffset CreateDate { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset UpdateDate { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        [Obsolete("Не нужен", false)]
        public bool IsNew { get; set; } = true;
        public Route MainRoute { get; set; }
        public bool ServerSynced { get; set; }//Признак того, что точка уже на сервере
        public DateTimeOffset ServerSyncedDate { get; set; }//Дата синхронизации
        public IList<RoutePointMediaObject> MediaObjects { get; }
        /// <summary>
        /// Версия записи
        /// </summary>
        public int Version { get; set; }
        public bool IsDeleted { get; set; }
    }
}
