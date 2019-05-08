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
        public List<Route> Get(string userId)
        {
            using (var db = new ServerDbContext(_dbOptions))
            {
                var routeaccess = db.RouteAccess.Where(u => u.UserId == userId).Select(u => u.RouteId).ToList();
                return db.Route.Where(r => routeaccess.Contains(r.RouteId) || r.IsPublished).ToList();
            }
        }
    }
}
