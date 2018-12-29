﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QuestHelper.Server.Managers;
using QuestHelper.Server.Models;

namespace QuestHelper.Server.Controllers.Points
{
    [Authorize]
    [ServiceFilter(typeof(RequestFilter))]
    [Route("api/routepoints/[controller]")]
    public class SyncController : Controller
    {
        private DbContextOptions<ServerDbContext> _dbOptions = ServerDbContext.GetOptionsContextDbServer();

        [HttpPost]
        public IActionResult Post([FromBody]SyncObjectStatus syncObject)
        {
            string userId = IdentityManager.GetUserId(HttpContext);
            SyncObjectStatus report = new SyncObjectStatus();
            if (syncObject.Statuses != null)
            {
                using (var db = new ServerDbContext(_dbOptions))
                {
                    var routeaccess = db.RouteAccess.Where(u => u.UserId == userId).Select(u => u.RouteId).ToList();
                    var syncIds = syncObject.Statuses.Select(t => t.ObjectId);
                    var dbObjects = db.RoutePoint.Where(r => syncIds.Contains(r.RoutePointId) && routeaccess.Contains(r.RouteId));
                    foreach (var clientVersion in syncObject.Statuses)
                    {
                        var dbObject = dbObjects.SingleOrDefault(r => r.RoutePointId == clientVersion.ObjectId);
                        if (dbObject != null)
                        {
                            if (dbObject.Version != clientVersion.Version)
                                report.Statuses.Add(new SyncObjectStatus.ObjectStatus { ObjectId = dbObject.RoutePointId, Version = dbObject.Version });
                        }
                        else
                        {
                            report.Statuses.Add(new SyncObjectStatus.ObjectStatus { ObjectId = clientVersion.ObjectId, Version = 0 });
                        }
                    }
                    var dbNewObjects = db.RoutePoint.Where(r => !syncIds.Contains(r.RoutePointId) && routeaccess.Contains(r.RouteId));
                    foreach (var newObject in dbNewObjects)
                    {
                        report.Statuses.Add(new SyncObjectStatus.ObjectStatus { ObjectId = newObject.RoutePointId, Version = newObject.Version });
                    }
                }
            }
            return new ObjectResult(report);
        }
    }
}
