using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QuestHelper.Server.Auth;
using QuestHelper.Server.Managers;
using QuestHelper.Server.Models;

namespace QuestHelper.Server.Controllers.Routes
{
    [Authorize]
    [ServiceFilter(typeof(RequestFilter))]
    [Route("api/routes/[controller]")]
    public class SyncController : Controller
    {
        private DbContextOptions<ServerDbContext> _dbOptions = ServerDbContext.GetOptionsContextDbServer();

        [HttpPost]
        public IActionResult Post([FromBody]SyncObjectStatus syncObject)
        {
            DateTime startDate = DateTime.Now;

            string userId = IdentityManager.GetUserId(HttpContext);
            SyncObjectStatus report = new SyncObjectStatus();
            if (syncObject.Statuses != null)
            {
                using (var db = new ServerDbContext(_dbOptions))
                {
                    var routeaccess = db.RouteAccess.Where(u => u.UserId == userId).Select(u=>u.RouteId).ToList();
                    var syncIds = syncObject.Statuses.Select(t => t.ObjectId);
                    //var dbRoutes = db.Route.Where(r => (syncIds.Contains(r.RouteId) && routeaccess.Contains(r.RouteId)) || r.IsPublished);
                    var dbRoutes = db.Route.Where(r => (syncIds.Contains(r.RouteId) || routeaccess.Contains(r.RouteId) || r.IsPublished));
                    foreach (var routeVersion in syncObject.Statuses)
                    {
                        var dbRoute = dbRoutes.SingleOrDefault(r => r.RouteId == routeVersion.ObjectId);
                        if (dbRoute != null)
                        {
                            //если маршрут на сервере есть, но версии разные, вернем его
                            if (dbRoute.Version != routeVersion.Version)
                                report.Statuses.Add(new SyncObjectStatus.ObjectStatus { ObjectId = dbRoute.RouteId, Version = dbRoute.Version });
                            //если версия одна, не возвращаем
                        }
                        else
                        {
                            //если маршрута нет, вернем инфу о том, что надо его запушить на сервер
                            report.Statuses.Add(new SyncObjectStatus.ObjectStatus { ObjectId = routeVersion.ObjectId, Version = 0 });
                        }
                    }
                    //И надо найти те маршруты, которых еще может не быть на клиенте, и ему отправить чтобы забрал
                    var dbNewRoutes = db.Route.Where(r => !syncIds.Contains(r.RouteId) && (routeaccess.Contains(r.RouteId) || (r.IsPublished)));
                    foreach (var newRoute in dbNewRoutes)
                    {
                        report.Statuses.Add(new SyncObjectStatus.ObjectStatus { ObjectId = newRoute.RouteId, Version = newRoute.Version });
                    }
                }
            }

            TimeSpan delay = DateTime.Now - startDate;
            Console.WriteLine($"Route Sync (old): status 200, {userId}, delay:{delay.TotalMilliseconds}");

            return new ObjectResult(report);
        }
    }
}
