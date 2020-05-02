using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestHelper.Server.Managers;
using QuestHelper.Server.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace QuestHelper.Server.Controllers.Routes
{
    /// <summary>
    /// Obsolete!
    /// </summary>
    [Authorize]
    [ServiceFilter(typeof(RequestFilter))]
    [Route("api/[controller]")]
    [Obsolete]
    public class RoutesController : Controller
    {
        private DbContextOptions<ServerDbContext> _dbOptions = ServerDbContext.GetOptionsContextDbServer();
        private MediaManager _mediaManager;
        private PoiManager _poiManager;
        private string _pathToMediaCatalog = string.Empty;

        public class ShareRequest
        {
            public string RouteIdForShare;
            public string[] UserId;
            public bool CanChangeRoute;
        }

        public class PreviewsArray
        {
            //public string Filename;
            //public string ImgBase64;
            public Dictionary<string, string> ImgDictionary;
        }
        public RoutesController()
        {
            _mediaManager = new MediaManager();
            _poiManager = new PoiManager(_dbOptions);
            _pathToMediaCatalog = _mediaManager.PathToMediaCatalog;
        }
        /// <summary>
        /// Returns list of available routes
        /// </summary>
        /// <returns>List routes</returns>
        [HttpGet]
        public IActionResult Get()
        {
            string userId = IdentityManager.GetUserId(HttpContext);
            List<Route> items = new List<Route>();
            using (var db = new ServerDbContext(_dbOptions))
            {
                var routeaccess = db.RouteAccess.Where(u => u.UserId == userId).Select(u => u.RouteId).ToList();
                items = db.Route.Where(r=>routeaccess.Contains(r.RouteId)).ToList();
            }
            return new ObjectResult(items);
        }

        [HttpGet("{RouteId}")]
        public IActionResult Get(string RouteId)
        {
            Route resultRoute = new Route();
            string userId = IdentityManager.GetUserId(HttpContext);
            using (var db = new ServerDbContext(_dbOptions))
            {
                RouteManager routeManager = new RouteManager(_dbOptions);
                resultRoute = routeManager.Get(userId, RouteId);
            }
            return new ObjectResult(resultRoute);
        }

        [HttpPost]
        public void Post([FromBody]Route routeObject)
        {
            string userId = IdentityManager.GetUserId(HttpContext);
            using (var db = new ServerDbContext(_dbOptions))
            {
                var entity = db.Route.Find(routeObject.RouteId);
                if (entity == null)
                {
                    routeObject.CreatorId = userId;
                    routeObject.VersionsHash = string.Empty;
                    routeObject.VersionsList = string.Empty;
                    db.Route.Add(routeObject);
                    RouteAccess accessObject = new RouteAccess();
                    accessObject.UserId = userId;
                    accessObject.RouteId = routeObject.RouteId;
                    accessObject.RouteAccessId = Guid.NewGuid().ToString();
                    accessObject.CreateDate = DateTime.Now;
                    db.RouteAccess.Add(accessObject);
                }
                else
                {
                    if (!string.IsNullOrEmpty(entity.CreatorId))
                    {
                        if (!entity.CreatorId.Equals(routeObject.CreatorId))
                        {
                            //throw new Exception($"Gosh server: Client try to change CreatorId! was {entity.CreatorId} try {routeObject.CreatorId}");
                            //Непонятно, почему меняется CreatorId - он устанавливается только при создании маршрута
                            routeObject.CreatorId = entity.CreatorId;
                        }
                    }
                    routeObject.VersionsHash = string.Empty;
                    routeObject.VersionsList = string.Empty;
                    db.Entry(entity).CurrentValues.SetValues(routeObject);
                    if (!string.IsNullOrEmpty(routeObject.CoverImgBase64))
                    {
                        Base64Manager.SaveBase64ToFile(routeObject.CoverImgBase64, Path.Combine(_pathToMediaCatalog, routeObject.ImgFilename));
                    }
                }
                db.SaveChanges();
            }
        }

        [HttpDelete("{RouteId}")]
        public void Delete(string RouteId)
        {
            string userId = IdentityManager.GetUserId(HttpContext);
            using (var db = new ServerDbContext(_dbOptions))
            {
                bool userIsCreatorRoute = db.Route.Where(u => u.RouteId == RouteId && u.CreatorId == userId).Any();
                if (userIsCreatorRoute)
                {
                    bool accessGranted = db.RouteAccess.Where(u => u.UserId == userId).Select(u => u.RouteId).Any();
                    if (accessGranted)
                    {
                        var entity = db.Route.Find(RouteId);
                        if (entity != null)
                        {
                            entity.IsDeleted = true;
                            entity.VersionsHash = string.Empty;
                            entity.VersionsList = string.Empty;
                            db.Entry(entity).CurrentValues.SetValues(entity);
                            //db.Remove(entity);
                        }
                        db.SaveChanges();
                    }
                    else
                    {
                        Response.StatusCode = 400;
                    }
                }
                else
                {
                    Response.StatusCode = 400;
                }
            }
        }

        [Route("share")]
        [HttpPost]
        public void Share([FromBody]ShareRequest request)
        {
            string userId = IdentityManager.GetUserId(HttpContext);
            using (var db = new ServerDbContext(_dbOptions))
            {
                bool userIsCreatorRoute = db.Route.Where(u => u.RouteId == request.RouteIdForShare && u.CreatorId == userId).Any();
                if (userIsCreatorRoute)
                {
                    bool accessGranted = db.RouteAccess.Where(u => u.UserId == userId).Select(u => u.RouteId).Any();
                    if (accessGranted)
                    {
                        foreach (string shareUserId in request.UserId)
                        {
                            bool userForShareExist = db.User.Where(u => u.UserId == shareUserId).Any();
                            if (userForShareExist)
                            {
                                bool userAlreadyHaveAccess = db.RouteAccess.Where(u => u.UserId == shareUserId && u.RouteId == request.RouteIdForShare).Any();
                                if (!userAlreadyHaveAccess)
                                {
                                    RouteAccess routeAccess = new RouteAccess();
                                    routeAccess.UserId = shareUserId;
                                    routeAccess.CreateDate = DateTime.Now;
                                    routeAccess.RouteId = request.RouteIdForShare;
                                    routeAccess.RouteAccessId = Guid.NewGuid().ToString();
                                    routeAccess.CanChange = request.CanChangeRoute;
                                    db.RouteAccess.Add(routeAccess);
                                    Route routeEntity = db.Route.Find(request.RouteIdForShare);
                                    if (!routeEntity.IsShared)
                                    {
                                        routeEntity.IsShared = true;
                                        routeEntity.VersionsHash = string.Empty;
                                        routeEntity.VersionsList = string.Empty;
                                        routeEntity.Version++;
                                        db.Entry(routeEntity).CurrentValues.SetValues(routeEntity);
                                    }
                                }
                            }
                            else
                            {
                                Response.StatusCode = 404;
                                return;
                            }
                        }
                        db.SaveChanges();
                    }
                    else
                    {
                        Response.StatusCode = 400;
                    }
                }
                else
                {
                    Response.StatusCode = 400;
                }
            }
        }

        [HttpGet("{RouteId}/linkhash")]
        public IActionResult GetSharedRouteLinkHash(string RouteId)
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

        /// <summary>
        /// Create short id for route (only if short id is absent yet)
        /// </summary>
        /// <param name="RouteId">Full route Id </param>
        /// <response code="200">Returns short id</response>
        /// <response code="401">Returns if user has no access to route</response>
        [HttpPost("{RouteId}/createshortlink")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public void MakeRouteShared(string RouteId)
        {
            string userId = IdentityManager.GetUserId(HttpContext);
            using (var db = new ServerDbContext(_dbOptions))
            {
                if (db.Route.Where(r => r.CreatorId == userId && r.RouteId == RouteId).Any())
                {
                    if (!db.RouteShare.Where(s => s.RouteId == RouteId && s.UserId == userId).Any())
                    {
                        string shareShortId = IdGenerator.Generate(7);
                        if (!string.IsNullOrEmpty(shareShortId))
                        {
                            if (!db.RouteShare.Where(rs => rs.ReferenceHash.Equals(shareShortId)).Any())
                            {
                                RouteShare shareObject = new RouteShare();
                                shareObject.RouteShareId = Guid.NewGuid().ToString();
                                shareObject.UserId = userId;
                                shareObject.CreateDate = DateTime.Now;
                                shareObject.RouteId = RouteId;
                                shareObject.ReferenceHash = shareShortId;
                                db.RouteShare.Add(shareObject);
                                db.SaveChanges();
                            }
                            else Response.StatusCode = 500;
                        } else Response.StatusCode = 500;
                    }
                }
                else
                {
                    Response.StatusCode = 401;
                }
            }
            Response.StatusCode = 200;
        }

        [HttpGet("preview/{routeId}")]
        public IActionResult GetPreviews(string routeId)
        {
            DateTime startDate = DateTime.Now;

            string userId = IdentityManager.GetUserId(HttpContext);
            //MemoryStream memStream = new MemoryStream();
            PreviewsArray previews = new PreviewsArray() { ImgDictionary = new Dictionary<string, string>() };
            using (var db = new ServerDbContext(_dbOptions))
            {
                var publishRoutes = db.Route.Where(r => r.IsPublished && r.IsDeleted == false && r.RouteId.Equals(routeId)).Select(r => r.RouteId).ToList();
                var routeAccess = db.RouteAccess.Where(u => u.UserId.Equals(userId) && u.RouteId.Equals(routeId)).Select(u => u.RouteId).ToList();

                var points = db.RoutePoint.Where(p => (routeAccess.Contains(p.RouteId) || publishRoutes.Contains(p.RouteId)) && p.RouteId.Equals(routeId)).Select(p=>p.RoutePointId).ToList();
                if (points.Count() > 0)
                {
                    var medias = db.RoutePointMediaObject.Where(m => (points.Contains(m.RoutePointId)));
                    //int index = 0;
                    foreach (var media in medias)
                    {
                        if (media.ImagePreviewLoadedToServer)
                        {
                            string mediaFileName = $"img_{media.RoutePointMediaObjectId}_preview.jpg";
                            //mediaFileName = "img_890022d6-62a8-48e9-b6ef-c9e9b4a7e6c0_preview.jpg";//отладка
                            string imgPath = Path.Combine(_pathToMediaCatalog, mediaFileName);
                            if (System.IO.File.Exists(imgPath))
                            {
                                try
                                {
                                    byte[] imgBytes = System.IO.File.ReadAllBytes(imgPath);
                                    string imgBase64String = Convert.ToBase64String(imgBytes);
                                    previews.ImgDictionary.Add(mediaFileName, imgBase64String);
                                    //previews.ImgDictionary.Add(index.ToString() + mediaFileName, imgBase64String);
                                    //index++;
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine($"Route Get previews: image file read error {e.InnerException}");
                                }
                            }
                            else
                            {
                                Console.WriteLine($"Route Get previews: image file does not exist {imgPath}");
                            }
                        }
                    }
                }
                else
                {
                    Response.StatusCode = 204;
                }
            }

            TimeSpan delay = DateTime.Now - startDate;
            Console.WriteLine($"Route Get previews: status {Response.StatusCode}, {userId}, delay:{delay.TotalMilliseconds}");

            return new ObjectResult(previews);
        }

    }
}
