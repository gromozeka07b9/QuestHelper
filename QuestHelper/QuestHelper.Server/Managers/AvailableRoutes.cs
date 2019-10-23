using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuestHelper.Server.Models;

namespace QuestHelper.Server.Managers
{
    public class AvailableRoutes
    {
        private DbContextOptions<ServerDbContext> _dbOptions = ServerDbContext.GetOptionsContextDbServer();

        public AvailableRoutes(DbContextOptions<ServerDbContext>  dbOptions)
        {
            _dbOptions = dbOptions;
        }
        public List<Route> GetByUserIdWithPublished(string userId)
        {
            using (var db = new ServerDbContext(_dbOptions))
            {
                var routeaccess = db.RouteAccess.Where(u => u.UserId == userId).Select(u => u.RouteId).ToList();
                return db.Route.Where(r => routeaccess.Contains(r.RouteId) || r.IsPublished).ToList();
            }
        }
        public List<Route> GetByUserIdOnlyPersonal(string userId)
        {
            using (var db = new ServerDbContext(_dbOptions))
            {
                var routeaccess = db.RouteAccess.Where(u => u.UserId == userId).Select(u => u.RouteId).ToList();
                return db.Route.Where(r => routeaccess.Contains(r.RouteId)).ToList();
            }
        }

        /// <summary>
        /// Get route by user id and route id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="routeId"></param>
        /// <returns></returns>
        public Route GetByUserIdAndRouteId(string userId, string routeId)
        {
            Route route = new Route();
            using (var db = new ServerDbContext(_dbOptions))
            {
                var routeAccess = db.RouteAccess.Where(u => u.UserId.Equals(userId)&&u.RouteId.Equals(routeId)).Select(u => u.RouteId).ToList();
                route = db.Route.Where(r => routeAccess.Contains(r.RouteId) || (r.IsPublished && r.RouteId.Equals(routeId))).FirstOrDefault();
            }

            return route;
        }
    }
}
