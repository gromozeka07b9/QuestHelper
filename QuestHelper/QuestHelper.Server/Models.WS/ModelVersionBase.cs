using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuestHelper.Server.Models.WS
{
    public abstract class ModelVersionBase
    {
        public string Id { get; set; }
        public int Version { get; set; }
    }
}
