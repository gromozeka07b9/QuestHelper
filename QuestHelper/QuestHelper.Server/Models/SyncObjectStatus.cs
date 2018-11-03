using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QuestHelper.Server.Models
{
    public class SyncObjectVersions
    {
        public SyncObjectVersions()
        {
            Statuses = new List<ObjectStatus>();
        }
        public ICollection<ObjectStatus> Statuses { get; set; }

        public class ObjectStatus
        {
            public string ObjectId;
            public int Version;
        }
    }
}
