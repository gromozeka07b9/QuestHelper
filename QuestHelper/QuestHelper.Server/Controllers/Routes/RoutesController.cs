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
        public class ShareRequest
        {
            public string RouteIdForShare;
            public string[] UserId;
            public bool CanChangeRoute;
        }

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
            Route resultRoute = new Route();
            string userId = IdentityManager.GetUserId(HttpContext);
            using (var db = new ServerDbContext(_dbOptions))
            {
                RouteManager routeManager = new RouteManager(db);
                resultRoute = routeManager.Get(userId, RouteId);
            }
            /*Route resultRoute = new Route();
                using (var db = new ServerDbContext(_dbOptions))
                {
                    bool accessGranted = db.RouteAccess.Where(u => u.UserId == userId && u.RouteId == RouteId).Any();
                    Route route = db.Route.Find(RouteId);
                    if ((accessGranted)||(route.IsPublished)||(route.IsDeleted))
                    {
                        resultRoute = route;
                    }
                    else
                    {
                        return new ObjectResult(null);
                    }
                }*/
            return new ObjectResult(resultRoute);
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
                    routeObject.CreatorId = userId;
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
                    if (!string.IsNullOrEmpty(entity.CreatorId))
                    {
                        if (!entity.CreatorId.Equals(routeObject.CreatorId))
                        {
                            //throw new Exception($"Gosh server: Client try to change CreatorId! was {entity.CreatorId} try {routeObject.CreatorId}");
                            //Непонятно, почему меняется CreatorId - он устанавливается только при создании маршрута
                            routeObject.CreatorId = entity.CreatorId;
                        }
                    }
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
                bool userIsCreatorRoute = db.Route.Where(u => u.RouteId == RouteId && u.CreatorId == userId).Any();
                if (userIsCreatorRoute)
                {
                    bool accessGranted = db.RouteAccess.Where(u => u.UserId == userId).Select(u => u.RouteId).Any();
                    if (accessGranted)
                    {
                        var entity = db.Route.Find(RouteId);
                        if (entity != null)
                        {
                            entity.IsDeleted = true;
                            db.Entry(entity).CurrentValues.SetValues(entity);
                            //db.Remove(entity);
                        }
                        db.SaveChanges();
                    }
                    else
                    {
                        Response.StatusCode = 400;
                    }
                }
                else
                {
                    Response.StatusCode = 400;
                }
            }
        }

        [Route("share")]
        [HttpPost]
        public void Share([FromBody]ShareRequest request)
        {
            string userId = IdentityManager.GetUserId(HttpContext);
            using (var db = new ServerDbContext(_dbOptions))
            {
                bool userIsCreatorRoute = db.Route.Where(u => u.RouteId == request.RouteIdForShare && u.CreatorId == userId).Any();
                if (userIsCreatorRoute)
                {
                    bool accessGranted = db.RouteAccess.Where(u => u.UserId == userId).Select(u => u.RouteId).Any();
                    if (accessGranted)
                    {
                        foreach (string shareUserId in request.UserId)
                        {
                            bool userForShareExist = db.User.Where(u => u.UserId == shareUserId).Any();
                            if (userForShareExist)
                            {
                                bool userAlreadyHaveAccess = db.RouteAccess.Where(u => u.UserId == shareUserId && u.RouteId == request.RouteIdForShare).Any();
                                if (!userAlreadyHaveAccess)
                                {
                                    RouteAccess routeAccess = new RouteAccess();
                                    routeAccess.UserId = shareUserId;
                                    routeAccess.CreateDate = DateTime.Now;
                                    routeAccess.RouteId = request.RouteIdForShare;
                                    routeAccess.RouteAccessId = Guid.NewGuid().ToString();
                                    routeAccess.CanChange = request.CanChangeRoute;
                                    db.RouteAccess.Add(routeAccess);
                                    Route routeEntity = db.Route.Find(request.RouteIdForShare);
                                    if (!routeEntity.IsShared)
                                    {
                                        routeEntity.IsShared = true;
                                        routeEntity.Version++;
                                        db.Entry(routeEntity).CurrentValues.SetValues(routeEntity);
                                    }
                                }
                            }
                            else
                            {
                                Response.StatusCode = 404;
                                return;
                            }
                        }
                        db.SaveChanges();
                    }
                    else
                    {
                        Response.StatusCode = 400;
                    }
                }
                else
                {
                    Response.StatusCode = 400;
                }
            }
        }
    }
}
