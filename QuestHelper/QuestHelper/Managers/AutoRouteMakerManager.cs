﻿using System;
using System.Linq;
using QuestHelper.LocalDB.Model;
using QuestHelper.Model;
using QuestHelper.Resources;

namespace QuestHelper.Managers
{
    public class AutoRouteMakerManager
    {
        private LocalFileCacheManager _cacheManager;
        private readonly IImageManager _imageManager;

        public AutoRouteMakerManager(IImageManager imageManager)
        {
            _cacheManager = new LocalFileCacheManager();
            _imageManager = imageManager;
        }

        public bool Make(AutoGeneratedRouted autoRoute, string creatorId)
        {
            if (autoRoute.Points.Count > 0)
            {
                var vroute = new ViewRoute(string.Empty);
                vroute.CreatorId = creatorId;
                vroute.CreateDate = DateTime.Now;
                vroute.Name = autoRoute.Name;
                if (vroute.Save())
                {
                    foreach (var autoPoint in autoRoute.Points.Where(p=>!p.IsDeleted))
                    {
                        var vroutePoint = new ViewRoutePoint(vroute.RouteId, string.Empty);
                        vroutePoint.Name = autoPoint.Name;
                        vroutePoint.Longitude = autoPoint.Longitude;
                        vroutePoint.Latitude = autoPoint.Latitude;
                        vroutePoint.Version++;
                        if (vroutePoint.Save())
                        {
                            foreach(var autoMedia in autoPoint.Images.Where(i=>!i.IsDeleted))
                            {
                                string localImageId = autoMedia.Id;
                                /*if (mediaObjectIsExists(mediaId))
                                {
                                    //в случае если мы используем одно и то же фото в нескольких альбомах, придется его дублировать
                                    //связано с тем, что id media соответствует имени файла и если меняем свойства объекта в каком-то маршруте, меняется для всех
                                    //media object в этом случае один, и это проблема
                                    mediaId = makeNewMediaObject(mediaId);
                                }*/
                                string mediaId = makeNewMediaObject(localImageId);
                                vroutePoint.AddMediaItem(mediaId, MediaObjectTypeEnum.Image);
                            }
                            vroutePoint.Save();
                        }
                    }
                    return true;
                }
            }


            return false;
        }

        private string makeNewMediaObject(string mediaId)
        {
            string pathImage = _cacheManager.GetFullPathAndFilename(mediaId);
            string newId = Guid.NewGuid().ToString();
            _imageManager.SetPreviewImageQuality(ImageQualityType.MiddleSizeHiQuality);
            var currentMetadata = _imageManager.GetPhoto(newId, pathImage);
            if (currentMetadata.getMetadataPhotoResult)
            {
                _imageManager.SetPreviewImageQuality(ImageQualityType.OriginalSizeLowQuality);
                var makeOriginalResult = _imageManager.GetPhoto(newId, pathImage, false);

            }
            return currentMetadata.getMetadataPhotoResult ? newId : string.Empty;
        }

        private bool mediaObjectIsExists(string mediaId)
        {
            RoutePointMediaObjectManager routePointMediaObjectManager = new RoutePointMediaObjectManager();
            return routePointMediaObjectManager.MediaIsExists(mediaId);
        }
    }
}
