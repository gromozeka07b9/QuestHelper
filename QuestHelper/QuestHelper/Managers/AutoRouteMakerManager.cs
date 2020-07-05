﻿using QuestHelper.LocalDB.Model;
using QuestHelper.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuestHelper.Managers
{
    public class AutoRouteMakerManager
    {
        private LocalFileCacheManager _cacheManager;

        public AutoRouteMakerManager(LocalFileCacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        internal AutoGeneratedRoute Make(DateTimeOffset periodStart, DateTimeOffset periodEnd)
        {
            AutoGeneratedRoute route = new AutoGeneratedRoute(new ImageManager());

            var localCacheFiles =  _cacheManager.GetImagesInfo(periodStart, periodEnd);
            if (localCacheFiles.Count > 0)
            {
                //route.SourceGalleryImages = imagesGalleryManager.GetItems(0);
                route.Name = "Мой маршрут за 7 дней";
                route.BuildOld(localCacheFiles);
            }
            return route;
        }
    }

}
