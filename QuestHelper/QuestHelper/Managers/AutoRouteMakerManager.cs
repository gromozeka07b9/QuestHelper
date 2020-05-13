using QuestHelper.LocalDB.Model;
using QuestHelper.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuestHelper.Managers
{
    public class AutoRouteMakerManager
    {
        internal bool Make(int depthInDays, string creatorId)
        {
            ImagesDataStoreManager imagesGalleryManager = new ImagesDataStoreManager(100, false, depthInDays);
            imagesGalleryManager.LoadListImages();
            if (imagesGalleryManager.Count() > 0)
            {
                var vroute = new ViewRoute(string.Empty);
                vroute.CreatorId = creatorId;
                vroute.CreateDate = DateTime.Now;
                vroute.Description = "Автоматически созданный маршрут";
                vroute.Name = "Новый маршрут";
                if (vroute.Save())
                {
                    ImageManager imageManager = new ImageManager();
                    foreach (var image in imagesGalleryManager.GetItems(0))
                    {
                        var resultImage = imageManager.GetPhoto(image.ImagePath);
                        if(resultImage.getMetadataPhotoResult)
                        {
                            var vroutePoint = new ViewRoutePoint(vroute.RouteId, string.Empty);
                            vroutePoint.Name = image.CreateDate.ToString();
                            vroutePoint.Longitude = resultImage.imageGpsCoordinates.Longitude;
                            vroutePoint.Latitude = resultImage.imageGpsCoordinates.Latitude;
                            vroutePoint.Version++;
                            if (vroutePoint.Save())
                            {
                                vroutePoint.AddMediaItem(resultImage.newMediaId, MediaObjectTypeEnum.Image);
                                vroutePoint.Save();
                            }
                        }
                    }
                    return true;
                }
                else return false;
            }
            return false;
        }
    }
}
