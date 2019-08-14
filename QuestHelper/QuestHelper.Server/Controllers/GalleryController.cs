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
                    }
                }
                if(!string.IsNullOrEmpty(resultRoute.RouteId))
                {
                    var query = from media in db.RoutePointMediaObject
                                join point in db.RoutePoint on media.RoutePointId equals point.RoutePointId
                                where point.RouteId == resultRoute.RouteId
                                orderby point.CreateDate
                                select new GalleryItemModel() { PointName = point.Name, PointDescription = point.Description, ImgId = media.RoutePointMediaObjectId };

                    galleryItems = query.ToList();
                }
            }
            ViewData["RouteName"] = resultRoute.Name;
            ViewData["RouteDefaultImgUrl"] = galleryItems.Count > 0 ? $"../shared/img_{galleryItems[0].ImgId}_preview.jpg" : "http://igosh.pro/images/icon.png";

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

        [Authorize]
        [ServiceFilter(typeof(RequestFilter))]
        [HttpPost("gallery/route/{RouteId}/share")]
        public void MakeRouteShared(string RouteId)
        {
            string userId = IdentityManager.GetUserId(HttpContext);
            using (var db = new ServerDbContext(_dbOptions))
            {
                if(db.Route.Where(r=>r.CreatorId == userId && r.RouteId == RouteId).Any())
                {
                    if(!db.RouteShare.Where(s=>s.RouteId == RouteId && s.UserId == userId).Any())
                    {
                        RouteShare shareObject = new RouteShare();
                        shareObject.RouteShareId = Guid.NewGuid().ToString();
                        shareObject.UserId = userId;
                        shareObject.CreateDate = DateTime.Now;
                        shareObject.RouteId = RouteId;
                        shareObject.ReferenceHash = shareObject.RouteShareId.TrimStart().Substring(0, 8);
                        db.RouteShare.Add(shareObject);
                        db.SaveChanges();
                    }
                }
                else
                {
                    Response.StatusCode = 401;
                }
            }
            Response.StatusCode = 200;
        }

        [Authorize]
        [ServiceFilter(typeof(RequestFilter))]
        [HttpGet("gallery/route/{RouteId}")]
        public IActionResult GetSharedRouteReferenceHash(string RouteId)
        {
            string sharedRouteReferenceHash = string.Empty;
            string userId = IdentityManager.GetUserId(HttpContext);
            using (var db = new ServerDbContext(_dbOptions))
            {
                if (db.Route.Where(r => r.CreatorId == userId && r.RouteId == RouteId).Any())
                {
                    sharedRouteReferenceHash = db.RouteShare.Where(s => s.RouteId == RouteId && s.UserId == userId).Select(s => s.ReferenceHash).SingleOrDefault();
                }
                else
                {
                    Response.StatusCode = 401;
                }
            }
            return new ObjectResult(sharedRouteReferenceHash);
        }
    }

    public class GalleryItemModel
    {
        public string PointName = string.Empty;
        public string PointDescription = string.Empty;
        public string ImgId = string.Empty;
    }

}
