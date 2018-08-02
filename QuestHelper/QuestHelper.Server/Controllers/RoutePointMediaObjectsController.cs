using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QuestHelper.Server.Models;

namespace QuestHelper.Server.Controllers
{
    [Route("api/[controller]")]
    public class RoutePointMediaObjectsController : Controller
    {
        [HttpGet("{routePointId}")]
        public IActionResult Get(string routePointId)
        {
            List<RoutePointMediaObject> items = new List<RoutePointMediaObject>();
            using (var db = new ServerDbContext())
            {
                items = db.RoutePointMediaObject.Where(x=>x.RoutePointId == routePointId).ToList();
            }
            return new ObjectResult(items);
        }

        [HttpPost]
        public void Post([FromBody]RoutePointMediaObject routeMediaObject)
        {
            using (var db = new ServerDbContext())
            {
                var entity = db.RoutePointMediaObject.Find(routeMediaObject.RoutePointMediaObjectId);
                if (entity == null)
                {
                    db.RoutePointMediaObject.Add(routeMediaObject);
                }
                else
                {
                    db.Entry(entity).CurrentValues.SetValues(routeMediaObject);
                }
                db.SaveChanges();
            }
        }
    }
}
