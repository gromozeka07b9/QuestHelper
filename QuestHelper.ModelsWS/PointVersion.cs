using System;
using System.Collections.Generic;

namespace QuestHelper.SharedModelsWS
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
