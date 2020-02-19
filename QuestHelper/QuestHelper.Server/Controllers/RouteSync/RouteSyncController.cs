using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestHelper.Server.Managers;
using QuestHelper.Server.Models;
using QuestHelper.SharedModelsWS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Route = QuestHelper.SharedModelsWS.Route;
using RoutePoint = QuestHelper.SharedModelsWS.RoutePoint;

namespace QuestHelper.Server.Controllers.RouteSync
{
    [Authorize]
    [ServiceFilter(typeof(RequestFilter))]
    [Produces("application/json")]
    [Route("api/route")]//Внимательно, маршрут отличается от контроллера!
    public class RouteSyncController : Controller
    {
        private DbContextOptions<ServerDbContext> _dbOptions = ServerDbContext.GetOptionsContextDbServer();
        private RouteManager _routeManager;

        public RouteSyncController()
        {
            _routeManager = new RouteManager(_dbOptions);
        }

        [HttpGet]
        public ContentResult Get()
        {
            string version = typeof(Startup).Assembly.GetName().Version.ToString();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Gosh! Api version " + version);
            sb.AppendLine("Получение данных о маршруте через методы:");
            sb.AppendLine("GET api/route/version/get - получение версий всех, доступных пользователю, маршрутов");
            sb.AppendLine("GET api/route/{routeid} - получение данных всех точек и медиаобъектов, входящих в конкретный маршрут");
            return Content(sb.ToString());
        }

        [HttpGet("version/get")]
        public IActionResult GetRouteData([FromQuery]bool onlyPersonal)
        {
            DateTime startDate = DateTime.Now;

            string userId = IdentityManager.GetUserId(HttpContext);

            //List<RouteVersion> routeVersions = new List<RouteVersion>();
            AvailableRoutes availRoutes = new AvailableRoutes(_dbOptions);

            List<Models.Route> routes = new List<Models.Route>();
            if (onlyPersonal)
            {
                routes = availRoutes.GetByUserIdOnlyPersonal(userId);
            }
            else
            {
                routes = availRoutes.GetByUserIdWithPublished(userId);
            }

            List<RouteVersion> routeVersions = makeRoutesVersion(routes);

            TimeSpan delay = DateTime.Now - startDate;
            Console.WriteLine($"GetRouteData full: status 200, {userId}, delay:{delay.TotalMilliseconds}");

            return new ObjectResult(routeVersions);
        }

        /// <summary>
        /// Get route hash
        /// </summary>
        /// <param name="routeId"></param>
        /// <returns>Route hash structure</returns>
        [HttpGet("{routeId}/version")]
        public IActionResult GetRouteVersion(string routeId)
        {
            DateTime startDate = DateTime.Now;

            string userId = IdentityManager.GetUserId(HttpContext);

            List<RouteVersion> routeVersions = new List<RouteVersion>();
            AvailableRoutes availRoutes = new AvailableRoutes(_dbOptions);
            var route = availRoutes.GetByUserIdAndRouteId(userId, routeId);
            List<Models.Route> routes = new List<Models.Route>();
            routes.Add(route);

            if (string.IsNullOrEmpty(route?.VersionsHash))
            {
                routeVersions = makeRoutesVersion(routes);
            }
            else
            {
                routeVersions = getRoutesVersion(routes);
            }

            TimeSpan delay = DateTime.Now - startDate;
            Console.WriteLine($"GetRouteVersion: status 200, {userId}, delay:{delay.TotalMilliseconds}");

            return new ObjectResult(routeVersions);
        }

        private List<RouteVersion> getRoutesVersion(List<Models.Route> routes)
        {
            List<RouteVersion> routeVersions = new List<RouteVersion>();

            foreach (var route in routes)
            {
                routeVersions.Add(new RouteVersion()
                {
                    Id = route.RouteId, ObjVerHash = route.VersionsHash, ObjVer = route.VersionsList, Version = route.Version
                });
            }

            return routeVersions;
        }

        private List<RouteVersion> makeRoutesVersion(List<Models.Route> routes)
        {
            List<RouteVersion> routeVersions;

            using (var db = new ServerDbContext(_dbOptions))
            {
                routeVersions = routes.Select(r => new RouteVersion() {Id = r.RouteId, Version = r.Version, ObjVerHash = r.VersionsHash, ObjVer = r.VersionsList}).ToList();
                var pointIds = db.RoutePoint.Where(p => routeVersions.Where(r => r.Id == p.RouteId).Any())
                    .Select(p => new {p.RouteId, p.RoutePointId, p.Version}).ToList();
                var mediaIds = db.RoutePointMediaObject
                    .Where(m => pointIds.Where(p => p.RoutePointId == m.RoutePointId).Any())
                    .Select(m => new {m.RoutePointMediaObjectId, m.Version, m.RoutePointId})
                    .ToList();
                foreach (var route in routeVersions)
                {
                    if (string.IsNullOrEmpty(route.ObjVerHash))
                    {
                        StringBuilder versions = new StringBuilder();
                        versions.Append(route.Version.ToString());
                        var routePoints = pointIds.Where(p => p.RouteId == route.Id).Select(p => new { p.Version, p.RoutePointId })
                            .OrderBy(p => p.RoutePointId);
                        foreach (var item in routePoints)
                        {
                            versions.Append(item.Version.ToString());
                        }

                        var mediaVersions = mediaIds.Where(m => routePoints.Any(p => p.RoutePointId == m.RoutePointId))
                            .OrderBy(m => m.RoutePointMediaObjectId).Select(m => m.Version);
                        foreach (int version in mediaVersions)
                        {
                            versions.Append(version.ToString());
                        }

                        route.ObjVer = versions.ToString();
                        route.ObjVerHash = HashGenerator.Generate(route.ObjVer);

                        _routeManager.SetHash(route.Id, route.ObjVerHash, route.ObjVer);
                    }
                }
            }

            return routeVersions != null ? routeVersions : new List<RouteVersion>();
        }


        [HttpGet("{routeid}")]
        public IActionResult GetRouteData(string routeId)
        {
            DateTime startDate = DateTime.Now;
            string userId = IdentityManager.GetUserId(HttpContext);
            RouteRoot routeRoot = new RouteRoot();

            AvailableRoutes availRoutes = new AvailableRoutes(_dbOptions);
            var dbRoute = availRoutes.GetByUserIdWithPublished(userId).FirstOrDefault(r=>r.RouteId == routeId);
            if (dbRoute != null)
            {                
                routeRoot.Route = ConverterDbModelToWs.RouteConvert(dbRoute);

                using (var db = new ServerDbContext(_dbOptions))
                {
                    var dbPoints = db.RoutePoint.Where(p => p.RouteId == dbRoute.RouteId).ToList();
                    foreach (var dbPoint in dbPoints)
                    {
                        RoutePoint routePoint = ConverterDbModelToWs.RoutePointConvert(dbPoint);
                        var dbMedias = db.RoutePointMediaObject.Where(m => m.RoutePointId == dbPoint.RoutePointId);
                        foreach (var dbMedia in dbMedias)
                        {
                            routePoint.MediaObjects.Add(ConverterDbModelToWs.RoutePointMediaObjectConvert(dbMedia));
                        }
                        routeRoot.Route.Points.Add(routePoint);
                    }
                }
            }

            TimeSpan delay = DateTime.Now - startDate;
            Console.WriteLine($"GetRouteData by Id: status 200, {userId}, {routeId}, delay:{delay.TotalMilliseconds}");
            return new ObjectResult(routeRoot);
        }

        /// <summary>
        /// Get image cover for selected route
        /// </summary>
        /// <param name="routeId">route Id</param>
        /// <returns></returns>
        [HttpGet("{routeid}/cover")]
        public IActionResult GetCoverImage(string routeId)
        {
            DateTime startDate = DateTime.Now;
            string userId = IdentityManager.GetUserId(HttpContext);
            AvailableRoutes availRoutes = new AvailableRoutes(_dbOptions);
            var dbRoute = availRoutes.GetByUserIdWithPublished(userId).FirstOrDefault(r => r.RouteId == routeId);
            if (dbRoute != null)
            {
                if (!string.IsNullOrEmpty(dbRoute.ImgFilename))
                {
                    MediaManager _mediaManager = new MediaManager();
                    MemoryStream memStream = new MemoryStream();
                    _mediaManager.DownloadToStream(memStream, dbRoute.ImgFilename);
                    memStream.Position = 0;
                    return File(memStream, "image/jpeg", dbRoute.ImgFilename);
                }
                else Response.StatusCode = 404;

            }
            else Response.StatusCode = 204;
            TimeSpan delay = DateTime.Now - startDate;
            Console.WriteLine($"GetCoverImage by Id: status 200, {userId}, {routeId}, delay:{delay.TotalMilliseconds}");
            return new ObjectResult("");
        }

        /// <summary>
        /// Route is viewed
        /// </summary>
        /// <param name="RouteId">Id</param>
        [HttpPost("{RouteId}/addviewed")]
        [ProducesResponseType(200)]
        public void AddViewed(string RouteId)
        {
            DateTime startDate = DateTime.Now;
            string userId = IdentityManager.GetUserId(HttpContext);

            using (var db = new ServerDbContext(_dbOptions))
            {
                if (!db.RouteView.Any(v => v.RouteId.Equals(RouteId) && v.UserId.Equals(userId)))
                {
                    db.RouteView.Add(new Models.RouteView() { RouteId = RouteId, UserId = userId, ViewDate = DateTime.Now });
                    db.SaveChanges();
                }
            }

            TimeSpan delay = DateTime.Now - startDate;
            Console.WriteLine($"Add route view: status 200, {userId}, delay:{delay.TotalMilliseconds}");
        }

    }
}