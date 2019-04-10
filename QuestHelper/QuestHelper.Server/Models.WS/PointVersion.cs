using System;
using System.Collections.Generic;

namespace QuestHelper.Server.Models.WS
{
    public class PointVersion : ModelVersionBase
    {
        public ICollection<MediaVersion> Media { get; set; }
        public PointVersion()
        {
            Media = new List<MediaVersion>();
        }
    }
}
