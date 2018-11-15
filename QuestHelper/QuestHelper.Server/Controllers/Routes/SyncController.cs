using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QuestHelper.Server.Models;

namespace QuestHelper.Server.Controllers.Routes
{
    [Route("api/routes/[controller]")]
    public class SyncController : Controller
    {
        [HttpPost]
        public IActionResult Post([FromBody]SyncObjectStatus syncObject)
        {
            string userId = "0";
            SyncObjectStatus report = new SyncObjectStatus();
            if (syncObject.Statuses != null)
            {
                using (var db = new ServerDbContext())
                {
                    var syncIds = syncObject.Statuses.Select(t => t.ObjectId);
                    var dbRoutes = db.Route.Where(r => syncIds.Contains(r.RouteId) && r.UserId == userId);
                    foreach (var routeVersion in syncObject.Statuses)
                    {
                        var dbRoute = dbRoutes.SingleOrDefault(r => r.RouteId == routeVersion.ObjectId && r.UserId == userId);
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
                    var dbNewRoutes = db.Route.Where(r => !syncIds.Contains(r.RouteId) && r.UserId == userId);
                    foreach (var newRoute in dbNewRoutes)
                    {
                        report.Statuses.Add(new SyncObjectStatus.ObjectStatus { ObjectId = newRoute.RouteId, Version = newRoute.Version });
                    }
                }
            }
            return new ObjectResult(report);
        }
    }
}
