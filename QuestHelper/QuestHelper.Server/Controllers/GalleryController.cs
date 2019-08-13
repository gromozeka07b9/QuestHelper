using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using QuestHelper.Server.Managers;
using QuestHelper.Server.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace QuestHelper.Server.Controllers
{
    public class GalleryController : Controller
    {
        private DbContextOptions<ServerDbContext> _dbOptions = ServerDbContext.GetOptionsContextDbServer();

        // GET: /<controller>/
        public IActionResult Gallery(string sharedRouteId)
        {
            Route resultRoute = new Route();
            string routeId = sharedRouteId;//будет другой объект

            List<GalleryItemModel> galleryItems = new List<GalleryItemModel>();
            using (var db = new ServerDbContext(_dbOptions))
            {
                RouteManager routeManager = new RouteManager(db);
                resultRoute = routeManager.Get("ed3d1c79-3d21-4f08-8680-6815586003bd", routeId);
                var query = from media in db.RoutePointMediaObject
                    join point in db.RoutePoint on media.RoutePointId equals point.RoutePointId
                    where point.RouteId == resultRoute.RouteId
                    orderby point.CreateDate
                    select new GalleryItemModel() { PointName = point.Name, PointDescription = point.Description, ImgId = media.RoutePointMediaObjectId };
                
                galleryItems = query.ToList();
            }
            ViewData["RouteName"] = resultRoute.Name;

            MediaManager mediaManager = new MediaManager();
            foreach (var mediaItem in galleryItems)
            {
                string imgFileName = $"img_{mediaItem.ImgId.ToLowerInvariant()}.jpg";
                string imgPreviewFileName = $"img_{mediaItem.ImgId.ToLowerInvariant()}_preview.jpg";
                if (!mediaManager.SharedMediaFileExist(imgFileName))
                {
                    mediaManager.CopyMediaFileToSharedCatalog(imgFileName);
                }
                if (!mediaManager.SharedMediaFileExist(imgPreviewFileName))
                {
                    mediaManager.CopyMediaFileToSharedCatalog(imgPreviewFileName);
                }
            }
            return View(galleryItems);
        }
    }

    public class GalleryItemModel
    {
        public string PointName = string.Empty;
        public string PointDescription = string.Empty;
        public string ImgId = string.Empty;
    }

}
