﻿using Realms;
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
        public string CreatorId { get; set; }
        public bool ServerSynced { get; set; }//Признак того, что маршрут уже на сервере
        public DateTimeOffset ServerSyncedDate { get; set; }//Дата синхронизации
        /// <summary>
        /// Версия записи
        /// </summary>
        public int Version { get; set; }
        public string ObjVerHash { get; set; }
        public bool IsShared { get; set; }
        public bool IsPublished { get; set; }
        public string DefaultImage { get; set; }
        public string LengthMetres { get; set; }
        public string LengthSteps { get; set; }
        public bool IsDeleted { get; set; }
        public string ImgFilename { get; set; }
        public string Description { get; set; }
    }
}
