using System;

namespace QuestHelper.SharedModelsWS
{
    public class RoutePointMediaObject : ModelBase
    {
        public RoutePointMediaObject()
        {
        }
        public string RoutePointId { get; set; }
        public bool ImageLoadedToServer { get; set; }
        public bool ImagePreviewLoadedToServer { get; set; }
    }
}
