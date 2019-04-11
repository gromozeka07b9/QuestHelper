using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestHelper.Server.Managers;
using QuestHelper.Server.Models.WS;

namespace QuestHelper.Server.Controllers.RouteSync
{
    [Authorize]
    [ServiceFilter(typeof(RequestFilter))]
    [Produces("application/json")]
    [Route("api/route")]//Внимательно, маршрут отличается от контроллера!
    public class RouteSyncController : Controller
    {
        private DbContextOptions<ServerDbContext> _dbOptions = ServerDbContext.GetOptionsContextDbServer();

        [HttpGet]
        public ContentResult Get()
        {
            string version = typeof(Startup).Assembly.GetName().Version.ToString();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Gosh! Api version " + version);
            sb.AppendLine("Получение данных о маршруте через методы:");
            sb.AppendLine("GET api/route/version/get - получение версий всех, доступных пользователю, маршрутов");
            sb.AppendLine("GET api/route/{routeid}/version/get - получение версии всех точек и фото, входящих в конкретный маршрут");
            sb.AppendLine("GET api/route/{routeid} - получение данных всех точек и фото, входящих в конкретный маршрут");
            sb.AppendLine("POST api/route/{routeid} - добавление/обновление данных по конкретному маршруту");
            sb.AppendLine("POST api/route/{routeid}/adduser - добавление прав доступа к маршруту конкретному пользователю");
            sb.AppendLine("POST api/route/{routeid}/share - поделиться маршрутов всем пользователям");
            sb.AppendLine("DELETE api/route/{routeid} - удаление маршрута");
            return Content(sb.ToString());
        }

        [HttpGet("{routeid}/version/get")]
        public IActionResult GetRouteVersion(string routeId)
        {
            string testGuid = "34843751-0cfc-4eeb-8f9c-a26a3452cc9a";
            StringBuilder versions = new StringBuilder();
            SyncVersionRoot route = new SyncVersionRoot();
            route.Route = new RouteVersion();
            route.Route.Id = testGuid;
            route.Route.Version = 45;
            versions.Append(route.Route.Id);
            versions.Append(route.Route.Version);

            for (int i = 0; i < 24; i++)
            {
                PointVersion point = new PointVersion();
                point.Id = testGuid;
                point.Version = i;
                versions.Append(point.Id);
                versions.Append(point.Version);
                for (int j = 0; j < 2; j++)
                {
                    MediaVersion media = new MediaVersion();
                    media.Id = testGuid;
                    media.Version = j+1;
                    versions.Append(media.Id);
                    versions.Append(media.Version);
                    point.Media.Add(media);
                }
                route.Route.Points.Add(point);
            }

            /*using (SHA256 hash = SHA256.Create())
            {
                byte[] versionAsBytes = Encoding.ASCII.GetBytes(versions.ToString());
                byte[] versionHash = hash.ComputeHash(versionAsBytes);

                route.VersionHash = BitConverter.ToString(versionHash).Replace("-",string.Empty);
            }*/
            return new ObjectResult(route);
        }

        [HttpGet("version/get")]
        public IActionResult GetRouteData()
        {
            string userId = IdentityManager.GetUserId(HttpContext);
            //string userId = "bf7d6d63-6b18-4e54-a2d9-703a60ca73f6";

            DateTime startDate = DateTime.Now;
            List<RouteVersion> routeVersions = new List<RouteVersion>();
            using (var db = new ServerDbContext(_dbOptions))
            {
                var routeaccess = db.RouteAccess.Where(u => u.UserId == userId).Select(u => u.RouteId).ToList();
                routeVersions = db.Route.Where(r => routeaccess.Contains(r.RouteId)).Select(r=>new RouteVersion(){Id = r.RouteId, Version = r.Version}).ToList();
                var pointIds = db.RoutePoint.Where(p => routeVersions.Where(r => r.Id == p.RouteId).Any()).Select(p => new {p.RouteId, p.RoutePointId, p.Version}).ToList();
                var mediaIds = db.RoutePointMediaObject
                    .Where(m => pointIds.Where(p => p.RoutePointId == m.RoutePointId).Any()).Select(m => new {m.RoutePointMediaObjectId, m.Version, m.RoutePointId})
                    .ToList();
                foreach (var route in routeVersions)
                {
                    int pointsVersionSum = 0;
                    int mediasVersionSum = 0;
                    var routePoints = pointIds.Where(p => p.RouteId == route.Id);
                    pointsVersionSum = routePoints.Select(p=>p.Version).Sum();
                    foreach (var point in routePoints)
                    {
                        mediasVersionSum += mediaIds.Where(m => m.RoutePointId == point.RoutePointId).Select(m => m.Version).Sum();
                    }

                    route.ObjVerHash = HashGenerator.Generate((route.Version + pointsVersionSum + mediasVersionSum).ToString());
                }
            }

            TimeSpan delay = DateTime.Now - startDate;
            return new ObjectResult(routeVersions);
        }


        [HttpGet("{routeid}")]
        public IActionResult GetRouteData(string routeId)
        {
            RouteRoot route = new RouteRoot();
            route.Route = new Route();
            route.Route.Id = "34843751-0cfc-4eeb-8f9c-a26a3452cc9a";
            route.Route.Name = "test " + routeId;
            route.Route.CreatorId = "34843751-0cfc-4eeb-8f9c-a26a3452cc9a";
            route.Route.IsPublished = true;
            route.Route.IsShared = true;
            route.Route.CreateDate = DateTimeOffset.Now;
            route.Route.UpdateDate = DateTimeOffset.Now;
            route.Route.IsDeleted = false;
            route.Route.Version = 45;

            for (int i = 0; i < 24; i++) 
            {
                RoutePoint point = new RoutePoint();
                point.Id = Guid.NewGuid().ToString();
                point.CreateDate = DateTimeOffset.Now;
                point.IsDeleted = false;
                point.Name = "point_" + i.ToString();
                point.Address = "Австралия, Барьерный риф, рядом с акулой номер двести сорок девять";
                point.RouteId = route.Route.Id;
                point.UpdatedUserId = Guid.NewGuid().ToString();
                point.UpdateDate = DateTimeOffset.Now;
                point.Description = "What is a GUID? GUID(or UUID) is an acronym for 'Globally Unique Identifier'(or 'Universally Unique Identifier').It is a 128 - bit integer number used to identify resources.The term GUID is generally used by developers working with Microsoft technologies, while UUID is used everywhere else. How unique is a GUID ? 128 - bits is big enough and the generation algorithm is unique enough that if 1,000,000,000 GUIDs per second were generated for 1 year the probability of a duplicate would be only 50 %.Or if every human on Earth generated 600,000,000 GUIDs there would only be a 50 % probability of a duplicate. How are GUIDs used? GUIDs are used in enterprise software development in C#, Java, and C++ as database keys, component identifiers, or just about anywhere else a truly unique identifier is required. GUIDs are also used to identify all interfaces and objects in COM programming.";
                point.Latitude = 12.6545;
                point.Longitude = 44.223545433;
                point.Version = i;
                for (int j = 0; j < 2; j++)
                {
                    RoutePointMediaObject media = new RoutePointMediaObject();
                    media.RoutePointId = point.Id;
                    media.ImageLoadedToServer = false;
                    media.ImagePreviewLoadedToServer = true;
                    media.CreateDate =  DateTimeOffset.Now;
                    media.Id = "media_" + j.ToString();
                    media.IsDeleted = false;
                    media.Version = j;
                    point.MediaObjects.Add(media);
                }
                route.Points.Add(point);
            }
            return new ObjectResult(route);
        }
    }
}