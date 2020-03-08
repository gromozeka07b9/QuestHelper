using Microsoft.EntityFrameworkCore;
using QuestHelper.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuestHelper.Server.Managers
{
    public class PoiManager
    {
        DbContextOptions<ServerDbContext> _db;

        public PoiManager(DbContextOptions<ServerDbContext> db)
        {
            _db = db;
        }

        public bool UpdatePoiByRoute(Route route)
        {
            using (var db = new ServerDbContext(_db))
            {
                var poi = db.Poi.Where(p => p.ByRouteId.Equals(route.RouteId)).FirstOrDefault();
                if(!string.IsNullOrEmpty(poi.PoiId))
                {
                    poi.Description = route.Description;
                    poi.ImgFilename = route.ImgFilename;
                    poi.IsPublished = route.IsPublished;
                    poi.Name = route.Name;
                    poi.UpdateDate = DateTime.Now;
                    poi.IsDeleted = route.IsDeleted;
                    poi.Longitude = 0;
                    poi.Latitude = 0;
                    poi.Version++;
                }
                else
                {

                }
            }
            return true;
        }
        /*public Route Get(string userId, string RouteId)
        {
            Route resultRoute = new Route();
            using (var db = new ServerDbContext(_db))
            {
                bool accessGranted = db.RouteAccess.Where(u => u.UserId == userId && u.RouteId == RouteId).Any();
                Route route = db.Route.Find(RouteId);
                if (route != null && ((accessGranted) || (route.IsPublished) || (route.IsDeleted)))
                {
                    resultRoute = route;
                }
            }
            return resultRoute;
        }

        public void SetHash(string RouteId, string Hash, string Versions)
        {

            using (var db = new ServerDbContext(_db))
            {
                var entity = db.Route.Find(RouteId);
                if (entity != null)
                {
                    entity.VersionsHash = Hash;
                    entity.VersionsList = Versions;
                    db.Entry(entity).CurrentValues.SetValues(entity);
                    db.SaveChanges();
                }
            }

        }*/
    }
}
