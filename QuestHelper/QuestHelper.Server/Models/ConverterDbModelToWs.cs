using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuestHelper.Server.Models
{
    public class ConverterDbModelToWs
    {
        public static SharedModelsWS.Route RouteConvert(Models.Route dbRoute)
        {
            SharedModelsWS.Route route = new SharedModelsWS.Route();
            if (dbRoute != null)
            {
                route.Points = new List<SharedModelsWS.RoutePoint>();
                route.Name = dbRoute.Name;
                route.CreatorId = dbRoute.CreatorId;
                route.IsShared = dbRoute.IsShared;
                route.IsPublished = dbRoute.IsPublished;
                route.IsDeleted = dbRoute.IsDeleted;
                route.Id = dbRoute.RouteId;
                route.CreateDate = dbRoute.CreateDate;
                route.Version = dbRoute.Version;
                route.ImgFilename = dbRoute.ImgFilename;
                route.Description = dbRoute.Description;
            }

            return route;
        }
        public static SharedModelsWS.RoutePoint RoutePointConvert(Models.RoutePoint dbPoint)
        {
            SharedModelsWS.RoutePoint point = new SharedModelsWS.RoutePoint();
            if (dbPoint != null)
            {
                point.MediaObjects = new List<SharedModelsWS.RoutePointMediaObject>();
                point.Name = dbPoint.Name;
                point.RouteId = dbPoint.RouteId;
                point.UpdateDate = dbPoint.UpdateDate;
                point.UpdatedUserId = dbPoint.UpdatedUserId;
                point.Latitude = dbPoint.Latitude;
                point.Longitude = dbPoint.Longitude;
                point.Address = dbPoint.Address;
                point.Description = dbPoint.Description;
                point.Id = dbPoint.RoutePointId;
                point.CreateDate = dbPoint.CreateDate;
                point.IsDeleted = dbPoint.IsDeleted;
                point.Version = dbPoint.Version;
            }

            return point;
        }

        public static SharedModelsWS.RoutePointMediaObject RoutePointMediaObjectConvert(Models.RoutePointMediaObject dbMediaObject)
        {
            SharedModelsWS.RoutePointMediaObject mediaObject = new SharedModelsWS.RoutePointMediaObject();
            if (dbMediaObject != null)
            {
                mediaObject.RoutePointId = dbMediaObject.RoutePointId;
                mediaObject.ImageLoadedToServer = dbMediaObject.ImageLoadedToServer;
                mediaObject.ImagePreviewLoadedToServer = dbMediaObject.ImagePreviewLoadedToServer;
                mediaObject.Id = dbMediaObject.RoutePointMediaObjectId;
                mediaObject.IsDeleted = dbMediaObject.IsDeleted;
                mediaObject.Version = dbMediaObject.Version;
                mediaObject.MediaType = (int)dbMediaObject.MediaType;
            }

            return mediaObject;
        }

    }
}
