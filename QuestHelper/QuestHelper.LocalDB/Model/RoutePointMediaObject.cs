using Realms;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuestHelper.LocalDB.Model
{
    public class RoutePointMediaObject : RealmObject
    {
        [PrimaryKey]
        public string RoutePointMediaObjectId { get; set; } = Guid.NewGuid().ToString();
        public bool PreviewServerSynced { get; set; }//Признак того, что файл уже на сервере - превью
        public bool OriginalServerSynced { get; set; }//Признак того, что файл уже на сервере - оригинал
        public DateTimeOffset ServerSyncedDate { get; set; }//Дата синхронизации оригинала
        public string RoutePointId { get; set; }
        public RoutePoint Point { get; set; }

        /// <summary>
        /// Версия записи
        /// </summary>
        public int Version { get; set; }
        public bool IsDeleted { get; set; }
    }
}
