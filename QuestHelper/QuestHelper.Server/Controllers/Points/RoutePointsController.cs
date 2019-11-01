using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestHelper.Server.Managers;
using QuestHelper.Server.Models;

namespace QuestHelper.Server.Controllers.Points
{
    [Authorize]
    [ServiceFilter(typeof(RequestFilter))]
    [Route("api/[controller]")]
    public class RoutePointsController : Controller
    {
        private DbContextOptions<ServerDbContext> _dbOptions = ServerDbContext.GetOptionsContextDbServer();

        [HttpGet("{RoutePointId}")]
        public IActionResult Get(string RoutePointId)
        {
            string userId = IdentityManager.GetUserId(HttpContext);
            RoutePoint point = new RoutePoint();
            using (var db = new ServerDbContext(_dbOptions))
            {
                var publishRoutes = db.Route.Where(r => r.IsPublished && r.IsDeleted == false).Select(r => r.RouteId).ToList();
                var routeaccess = db.RouteAccess.Where(u => u.UserId == userId).Select(u => u.RouteId).ToList();
                point = db.RoutePoint.SingleOrDefault(x => x.RoutePointId == RoutePointId && (routeaccess.Contains(x.RouteId)||(publishRoutes.Contains(x.RouteId))));
            }
            return new ObjectResult(point);
        }

        [HttpPost]
        public void Post([FromBody]RoutePoint routePointObject)
        {
            string userId = IdentityManager.GetUserId(HttpContext);
            using (var db = new ServerDbContext(_dbOptions))
            {
                bool accessGranted = db.RouteAccess.Where(u => u.UserId == userId && u.RouteId == routePointObject.RouteId).Any();
                if (accessGranted)
                {
                    var entity = db.RoutePoint.Find(routePointObject.RoutePointId);
                    if (entity == null)
                    {
                        db.RoutePoint.Add(routePointObject);
                    }
                    else
                    {
                        db.Entry(entity).CurrentValues.SetValues(routePointObject);
                    }
                    var entityRoute = db.Route.Find(routePointObject.RouteId);
                    if (entityRoute != null)
                    {
                        entityRoute.VersionsHash = string.Empty;
                        entityRoute.VersionsList = string.Empty;
                        db.Entry(entityRoute).CurrentValues.SetValues(entityRoute);
                    }
                    db.SaveChanges();
                }
                else
                {
                    Response.StatusCode = 400;
                }
            }
        }
    }
}
