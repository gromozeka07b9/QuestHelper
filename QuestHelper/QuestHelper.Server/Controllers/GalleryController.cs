using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
        
        [HttpGet("gallery/{SharedRouteRef}")]
        public IActionResult Gallery(string SharedRouteRef)
        {
            Route resultRoute = new Route();
            User routeUser = new User();
            string routeId = string.Empty;//будет другой объект

            List<GalleryItemModel> galleryItems = new List<GalleryItemModel>();

            using (var db = new ServerDbContext(_dbOptions))
            {
                var routeIds = from share in db.RouteShare
                          where share.ReferenceHash == SharedRouteRef
                          select share.RouteId;
                if(routeIds.Any())
                {
                    routeId = routeIds.Single();
                    var resultRoutes = from route in db.Route
                                       where route.RouteId == routeId && !route.IsDeleted
                                       select route;
                    if(resultRoutes.Any())
                    {
                        resultRoute = resultRoutes.Single();
                        routeUser = (from user in db.User where user.UserId == resultRoute.CreatorId select user).Single();
                    }
                }
                if(!string.IsNullOrEmpty(resultRoute.RouteId))
                {
                    var query = from media in db.RoutePointMediaObject
                                join point in db.RoutePoint on media.RoutePointId equals point.RoutePointId
                                where point.RouteId == resultRoute.RouteId && !point.IsDeleted && !media.IsDeleted
                                orderby point.CreateDate
                                select new GalleryItemModel() { PointName = point.Name, PointDescription = point.Description, ImgId = media.RoutePointMediaObjectId };

                    galleryItems = query.ToList();
                }
            }
            ViewData["RouteName"] = resultRoute.Name;
            ViewData["RouteCreatorName"] = routeUser.Name;
            ViewData["RouteDefaultImgUrl"] = galleryItems.Count > 0 ? $"../shared/img_{galleryItems[0].ImgId}_preview.jpg" : "http://igosh.pro/images/icon.png";

            MediaManager mediaManager = new MediaManager();
            foreach (var mediaItem in galleryItems)
            {
                string imgFileName = $"img_{mediaItem.ImgId.ToLowerInvariant()}.jpg";
                string imgPreviewFileName = $"img_{mediaItem.ImgId.ToLowerInvariant()}_preview.jpg";
                if (!mediaManager.SharedMediaFileExist(imgFileName))
                {
                    bool copied = mediaManager.CopyMediaFileToSharedCatalog(imgFileName);
                    /*if (!copied)
                    {
                        imgFileName = imgPreviewFileName;
                    }*/
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
