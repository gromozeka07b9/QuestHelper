using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestHelper.Server.Managers;
using QuestHelper.Server.Models;
using QuestHelper.SharedModelsWS;
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

        [HttpGet]
        public ContentResult Get()
        {
            string version = typeof(Startup).Assembly.GetName().Version.ToString();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Gosh! Api version " + version);
            sb.AppendLine("Получение данных о маршруте через методы:");
            sb.AppendLine("GET api/route/version/get - получение версий всех, доступных пользователю, маршрутов");
            sb.AppendLine("GET api/route/{routeid} - получение данных всех точек и медиаобъектов, входящих в конкретный маршрут");
            /*sb.AppendLine("POST api/route/{routeid} - добавление/обновление данных по конкретному маршруту");
            sb.AppendLine("POST api/route/{routeid}/adduser - добавление прав доступа к маршруту конкретному пользователю");
            sb.AppendLine("POST api/route/{routeid}/share - поделиться маршрутов всем пользователям");
            sb.AppendLine("DELETE api/route/{routeid} - удаление маршрута");*/
            return Content(sb.ToString());
        }

        [HttpGet("version/get")]
        public IActionResult GetRouteData()
        {
            string userId = IdentityManager.GetUserId(HttpContext);

            DateTime startDate = DateTime.Now;
            List<RouteVersion> routeVersions = new List<RouteVersion>();
            AvailableRoutes availRoutes = new AvailableRoutes(_dbOptions);
            var routes = availRoutes.Get(userId);
            using (var db = new ServerDbContext(_dbOptions))
            {
                routeVersions = routes.Select(r => new RouteVersion() {Id = r.RouteId, Version = r.Version}).ToList();
                var pointIds = db.RoutePoint.Where(p => routeVersions.Where(r => r.Id == p.RouteId).Any()).Select(p => new {p.RouteId, p.RoutePointId, p.Version}).ToList();
                var mediaIds = db.RoutePointMediaObject
                    .Where(m => pointIds.Where(p => p.RoutePointId == m.RoutePointId).Any()).Select(m => new {m.RoutePointMediaObjectId, m.Version, m.RoutePointId})
                    .ToList();
                foreach (var route in routeVersions)
                {
                    StringBuilder versions = new StringBuilder();
                    versions.Append(route.Version.ToString());
                    var routePoints = pointIds.Where(p => p.RouteId == route.Id).Select(p=>new {p.Version, p.RoutePointId}).OrderBy(p=>p.RoutePointId);
                    foreach (var item in routePoints)
                    {
                        versions.Append(item.Version.ToString());
                    }
                    foreach (var point in routePoints)
                    {
                        var mediaVersions = mediaIds.Where(m => m.RoutePointId == point.RoutePointId).Select(m => m.Version).OrderBy(m=>m);
                        foreach (int version in mediaVersions)
                        {
                            versions.Append(version.ToString());
                        }
                    }
                    //FFDC094EA6D4AF89DFC4FC5882631B9005FCD682D89249DF8018782ECC3AC338
                    route.ObjVer = versions.ToString();
                    route.ObjVerHash = HashGenerator.Generate(route.ObjVer);
                }
            }

            TimeSpan delay = DateTime.Now - startDate;
            return new ObjectResult(routeVersions);
        }


        [HttpGet("{routeid}")]
        public IActionResult GetRouteData(string routeId)
        {
            string userId = IdentityManager.GetUserId(HttpContext);
            RouteRoot routeRoot = new RouteRoot();

            AvailableRoutes availRoutes = new AvailableRoutes(_dbOptions);
            var dbRoute = availRoutes.Get(userId).FirstOrDefault(r=>r.RouteId == routeId);
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
            return new ObjectResult(routeRoot);
        }
    }
}