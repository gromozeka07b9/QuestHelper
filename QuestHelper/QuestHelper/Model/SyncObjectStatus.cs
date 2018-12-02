using System.Collections.Generic;

namespace QuestHelper.Model
{
    public class SyncObjectStatus
    {
        public SyncObjectStatus()
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
