using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QuestHelper.Server.Models;

namespace QuestHelper.Server.Controllers
{
    [Route("api/[controller]")]
    public class RoutePointsController : Controller
    {
        [HttpGet("{RouteId}")]
        public IActionResult Get(string RouteId)
        {
            List<RoutePoint> items = new List<RoutePoint>();
            using (var db = new ServerDbContext())
            {
                items = db.RoutePoint.Where(x=>x.RouteId == RouteId).ToList();
            }
            return new ObjectResult(items);
        }

        [HttpPost]
        public void Post([FromBody]RoutePoint routePointObject)
        {
            using (var db = new ServerDbContext())
            {
                var entity = db.RoutePoint.Find(routePointObject.RoutePointId);
                if (entity == null)
                {
                    db.RoutePoint.Add(routePointObject);
                } else
                {
                    db.Entry(entity).CurrentValues.SetValues(routePointObject);
                }
                db.SaveChanges();
            }
        }
    }
}
