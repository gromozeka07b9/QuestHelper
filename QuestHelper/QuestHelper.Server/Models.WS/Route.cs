using System;

namespace QuestHelper.Server.Models.WS
{
    public class Route : ModelBase
    {
        public Route()
        {
        }
        public string Name { get; set; }
        public string CreatorId { get; set; }
        public bool IsShared { get; set; }
        public bool IsPublished { get; set; }
        public DateTimeOffset UpdateDate { get; set; }
    }
}
