using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuestHelper.SharedModelsWS
{
    public abstract class ModelBase
    {
        public string Id { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        public int Version { get; set; }
        public bool IsDeleted { get; set; }
    }
}
