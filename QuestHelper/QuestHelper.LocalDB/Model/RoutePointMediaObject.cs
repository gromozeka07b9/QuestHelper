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
        public string FileName { get; set; }
        public string FileNamePreview { get; set; }
        public bool ServerSynced { get; set; }//Признак того, что файл уже на сервере - и оригинал, и превью
        public DateTimeOffset ServerSyncedDate { get; set; }//Дата синхронизации
        public string RoutePointId { get; set; }
        public RoutePoint Point { get; set; }

        /// <summary>
        /// Версия записи
        /// </summary>
        public int Version { get; set; }
    }
}
