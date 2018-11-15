using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QuestHelper.Server.Models;

namespace QuestHelper.Server.Controllers.Points
{
    [Route("api/points/[controller]")]
    public class SyncController : Controller
    {
        [HttpPost]
        public IActionResult Post([FromBody]SyncObjectStatus syncObject)
        {
            //string userId = "0";
            SyncObjectStatus report = new SyncObjectStatus();
            if (syncObject.Statuses != null)
            {
                using (var db = new ServerDbContext())
                {
                    var syncIds = syncObject.Statuses.Select(t => t.ObjectId);
                    var dbObjects = db.RoutePoint.Where(r => syncIds.Contains(r.RoutePointId));
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
                    var dbNewObjects = db.RoutePoint.Where(r => !syncIds.Contains(r.RoutePointId));
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
