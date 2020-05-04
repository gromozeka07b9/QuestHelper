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
                    totalCountRows = withoutFilter.Count();
                    items = withoutFilter.Skip((pageNumber - 1) * pagingParameters.PageSize).Take(pagingParameters.PageSize).ToList();
                    /*var routeaccess = db.RouteAccess.Where(u => u.UserId == userId).Select(u => u.RouteId).ToList();
                    var withoutFilter = db.Route.Where(r => routeaccess.Contains(r.RouteId)).OrderBy(r => r.CreateDate);
                    totalCountRows = withoutFilter.Count();
                    items = withoutFilter.Skip((pageNumber - 1) * pagingParameters.PageSize).Take(pagingParameters.PageSize).ToList();*/
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

    }
}
