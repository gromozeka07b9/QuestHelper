using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QuestHelper.Server.Managers;
using QuestHelper.Server.Models;

namespace QuestHelper.Server.Controllers.Routes
{
    [Authorize]
    [ServiceFilter(typeof(RequestFilter))]
    [Route("api/[controller]")]
    public class RoutesController : Controller
    {
        private DbContextOptions<ServerDbContext> _dbOptions = ServerDbContext.GetOptionsContextDbServer();

        [HttpGet]
        public IActionResult Get()
        {
            string userId = IdentityManager.GetUserId(HttpContext);
            List<Route> items = new List<Route>();
            using (var db = new ServerDbContext(_dbOptions))
            {
                var routeaccess = db.RouteAccess.Where(u => u.UserId == userId).Select(u => u.RouteId).ToList();
                items = db.Route.Where(r=>routeaccess.Contains(r.RouteId)).ToList();
            }
            return new ObjectResult(items);
        }

        [HttpGet("{RouteId}")]
        public IActionResult Get(string RouteId)
        {
            string userId = IdentityManager.GetUserId(HttpContext);
            Route item = new Route();
            using (var db = new ServerDbContext(_dbOptions))
            {
                bool accessGranted = db.RouteAccess.Where(u => u.UserId == userId && u.RouteId == RouteId).Any();
                if (accessGranted)
                {
                    item = db.Route.Find(RouteId);
                }
                else
                {
                    return new ObjectResult(null);
                }
            }
            return new ObjectResult(item);
        }

        [HttpPost]
        public void Post([FromBody]Route routeObject)
        {
            string userId = IdentityManager.GetUserId(HttpContext);
            using (var db = new ServerDbContext(_dbOptions))
            {
                var entity = db.Route.Find(routeObject.RouteId);
                if (entity == null)
                {
                    db.Route.Add(routeObject);
                    RouteAccess accessObject = new RouteAccess();
                    accessObject.UserId = userId;
                    accessObject.RouteId = routeObject.RouteId;
                    accessObject.RouteAccessId = Guid.NewGuid().ToString();
                    accessObject.CreateDate = DateTime.Now;
                    db.RouteAccess.Add(accessObject);
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
            string userId = IdentityManager.GetUserId(HttpContext);
            using (var db = new ServerDbContext(_dbOptions))
            {
                bool accessGranted = db.RouteAccess.Where(u => u.UserId == userId).Select(u => u.RouteId).Any();
                if (accessGranted)
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
}
