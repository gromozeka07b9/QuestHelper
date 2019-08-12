using Microsoft.EntityFrameworkCore;
using QuestHelper.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuestHelper.Server.Managers
{
    public class RouteManager
    {
        ServerDbContext _db;
        public RouteManager(ServerDbContext db)
        {
            _db = db;
        }

        public Route Get(string userId, string RouteId)
        {
            Route resultRoute = new Route();
            bool accessGranted = _db.RouteAccess.Where(u => u.UserId == userId && u.RouteId == RouteId).Any();
            Route route = _db.Route.Find(RouteId);
            if (route!=null && ((accessGranted) || (route.IsPublished) || (route.IsDeleted)))
            {
                resultRoute = route;
            }
            return resultRoute;
        }
    }
}
