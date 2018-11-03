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
    public class RoutesController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            List<Route> items = new List<Route>();
            using (var db = new ServerDbContext())
            {
                items = db.Route.ToList();
            }
            return new ObjectResult(items);
        }

        [HttpGet("{RouteId}")]
        public IActionResult Get(string RouteId)
        {
            Route item = new Route();
            using (var db = new ServerDbContext())
            {
                item = db.Route.Find(RouteId);
            }
            return new ObjectResult(item);
        }

        [HttpPost]
        public void Post([FromBody]Route routeObject)
        {
            using (var db = new ServerDbContext())
            {
                var entity = db.RoutePoint.Find(routeObject.RouteId);
                if (entity == null)
                {
                    db.Route.Add(routeObject);
                }
                else
                {
                    db.Entry(entity).CurrentValues.SetValues(routeObject);
                }
                db.SaveChanges();
            }
        }

        [HttpDelete("{RouteId}")]
        public void Delete(string RouteId)
        {
            using (var db = new ServerDbContext())
            {
                var entity = db.Route.Find(RouteId);
                if (entity != null)
                {
                    db.Remove(entity);
                }
                db.SaveChanges();
            }
        }
    }
}
