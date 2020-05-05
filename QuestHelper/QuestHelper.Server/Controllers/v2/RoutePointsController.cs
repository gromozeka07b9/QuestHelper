using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestHelper.Server.Managers;
using QuestHelper.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuestHelper.Server.Controllers.v2
{
    /// <summary>
    /// CRUD for route points
    /// </summary>
    [Authorize]
    [ServiceFilter(typeof(RequestFilter))]
    [Route("api/v2/[controller]")]
    public class RoutePointsController : Controller
    {
        private DbContextOptions<ServerDbContext> _dbOptions = ServerDbContext.GetOptionsContextDbServer();

        /// <summary>
        /// List all available route points for user
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get([FromQuery] PagingParameters pagingParameters)
        {
            FilterParameters filters = new FilterParameters(pagingParameters.Filter);
            int pageNumber = pagingParameters.IndexesRangeToPageNumber(pagingParameters.Range, pagingParameters.PageSize);
            int totalCountRows = 0;
            List<RoutePoint> items = new List<RoutePoint>();
            if (!string.IsNullOrEmpty(pagingParameters.Range))
            {
                string userId = IdentityManager.GetUserId(HttpContext);
                using (var db = new ServerDbContext(_dbOptions))
                {
                    var publishRoutes = db.Route.Where(r => r.IsPublished && r.IsDeleted == false).Select(r => r.RouteId).ToList();
                    var routeaccess = db.RouteAccess.Where(u => u.UserId == userId).Select(u => u.RouteId).ToList();
                    var withoutFilter = db.RoutePoint.Where(x=>routeaccess.Contains(x.RouteId) || (publishRoutes.Contains(x.RouteId)));

                    withoutFilter = filters.isFilterPresent("isDeleted") ? withoutFilter.Where(r => r.IsDeleted == filters.GetBooleanByName("isDeleted")) : withoutFilter;
                    withoutFilter = filters.isFilterPresent("routeId") ? withoutFilter.Where(r => r.RouteId.Equals(filters.GetStringByName("routeId"))) : withoutFilter;
                    withoutFilter = filters.isFilterPresent("address") ? withoutFilter.Where(r => r.Name.Contains(filters.GetStringByName("address"))) : withoutFilter;
                    withoutFilter = filters.isFilterPresent("name") ? withoutFilter.Where(r => r.Name.Contains(filters.GetStringByName("name"))) : withoutFilter;
                    withoutFilter = filters.isFilterPresent("description") ? withoutFilter.Where(r => r.Description.Contains(filters.GetStringByName("description"))) : withoutFilter;
                    if (filters.isFilterPresent("createDate"))
                    {
                        var cd = filters.GetDateTimeByName("createDate");
                        withoutFilter = withoutFilter.Where(r => r.CreateDate.Year.Equals(cd.Year) && r.CreateDate.Month.Equals(cd.Month) && r.CreateDate.Day.Equals(cd.Day));
                    }

                    totalCountRows = withoutFilter.Count();
                    items = withoutFilter.OrderBy(r => r.CreateDate).Skip((pageNumber - 1) * pagingParameters.PageSize).Take(pagingParameters.PageSize).ToList();
                }
            }
            HttpContext.Response.Headers.Add("x-total-count", totalCountRows.ToString());
            HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "x-total-count");
            return new ObjectResult(items);
        }

        [HttpGet("{RoutePointId}")]
        public IActionResult Get(string RoutePointId)
        {
            string userId = IdentityManager.GetUserId(HttpContext);
            RoutePoint point = new RoutePoint();
            using (var db = new ServerDbContext(_dbOptions))
            {
                var publishRoutes = db.Route.Where(r => r.IsPublished && r.IsDeleted == false).Select(r => r.RouteId).ToList();
                var routeaccess = db.RouteAccess.Where(u => u.UserId == userId).Select(u => u.RouteId).ToList();
                point = db.RoutePoint.SingleOrDefault(x => x.RoutePointId == RoutePointId && (routeaccess.Contains(x.RouteId) || (publishRoutes.Contains(x.RouteId))));
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
                        Response.StatusCode = 405;
                    }
                    /*else
                    {
                        db.Entry(entity).CurrentValues.SetValues(routePointObject);
                    }*/
                }
                else
                {
                    Response.StatusCode = 400;
                }
            }
        }

        [HttpPut("{routePointId}")]
        public void Put(string routePointId, [FromBody]RoutePoint routePointObject)
        {
            string userId = IdentityManager.GetUserId(HttpContext);
            using (var db = new ServerDbContext(_dbOptions))
            {
                bool accessGranted = db.RouteAccess.Where(u => u.UserId == userId && u.RouteId == routePointObject.RouteId).Any();
                if (accessGranted)
                {
                    var entity = db.RoutePoint.Find(routePointObject.RoutePointId);
                    if (entity != null)
                    {
                        db.Entry(entity).CurrentValues.SetValues(routePointObject);
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
                        Response.StatusCode = 405;
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
