using System;

namespace QuestHelper.Server.Models.WS
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
