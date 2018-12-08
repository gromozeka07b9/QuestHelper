using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QuestHelper.Server.Models;

namespace QuestHelper.Server.Controllers.Medias
{
    [Route("api/routepointmediaobjects/[controller]")]
    public class SyncController : Controller
    {
        private DbContextOptions<ServerDbContext> _dbOptions = ServerDbContext.GetOptionsContextDbServer();

        [HttpPost]
        public IActionResult Post([FromBody]SyncObjectStatus syncObject)
        {
            //string userId = "0";
            SyncObjectStatus report = new SyncObjectStatus();
            if (syncObject.Statuses != null)
            {
                using (var db = new ServerDbContext(_dbOptions))
                {
                    var syncIds = syncObject.Statuses.Select(t => t.ObjectId);
                    var dbObjects = db.RoutePointMediaObject.Where(r => syncIds.Contains(r.RoutePointMediaObjectId));
                    foreach (var clientVersion in syncObject.Statuses)
                    {
                        var dbObject = dbObjects.SingleOrDefault(r => r.RoutePointMediaObjectId == clientVersion.ObjectId);
                        if (dbObject != null)
                        {
                            if (dbObject.Version != clientVersion.Version)
                                report.Statuses.Add(new SyncObjectStatus.ObjectStatus { ObjectId = dbObject.RoutePointMediaObjectId, Version = dbObject.Version });
                        }
                        else
                        {
                            report.Statuses.Add(new SyncObjectStatus.ObjectStatus { ObjectId = clientVersion.ObjectId, Version = 0 });
                        }
                    }
                    var dbNewObjects = db.RoutePointMediaObject.Where(r => !syncIds.Contains(r.RoutePointMediaObjectId));
                    foreach (var newObject in dbNewObjects)
                    {
                        report.Statuses.Add(new SyncObjectStatus.ObjectStatus { ObjectId = newObject.RoutePointMediaObjectId, Version = newObject.Version });
                    }
                }
            }
            return new ObjectResult(report);
        }
    }
}
