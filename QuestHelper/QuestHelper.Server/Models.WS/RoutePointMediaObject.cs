using System;

namespace QuestHelper.Server.Models.WS
{
    public class RoutePointMediaObject : ModelBase
    {
        public RoutePointMediaObject(Models.RoutePointMediaObject dbMediaObject)
        {
            if (dbMediaObject != null)
            {
                RoutePointId = dbMediaObject.RoutePointId;
                ImageLoadedToServer = dbMediaObject.ImageLoadedToServer;
                ImagePreviewLoadedToServer = dbMediaObject.ImagePreviewLoadedToServer;
                Id = dbMediaObject.RoutePointMediaObjectId;
                IsDeleted = dbMediaObject.IsDeleted;
                Version = dbMediaObject.Version;
            }
        }
        public string RoutePointId { get; set; }
        public bool ImageLoadedToServer { get; set; }
        public bool ImagePreviewLoadedToServer { get; set; }
    }
}
