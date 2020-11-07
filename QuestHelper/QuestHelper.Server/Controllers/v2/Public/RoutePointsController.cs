using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestHelper.Server.Managers;
using QuestHelper.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuestHelper.Server.Controllers.v2.Public
{
    /// <summary>
    /// Доступ к точкам маршрутов, доступных без авторизации
    /// </summary>
    [Route("api/v2/public/[controller]")]
    public class RoutePointsController : Controller
    {
        private DbContextOptions<ServerDbContext> _dbOptions = ServerDbContext.GetOptionsContextDbServer();

        /// <summary>
        /// List all public route points for route
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get([FromQuery] PagingParameters pagingParameters)
        {
            FilterParameters filters = new FilterParameters(pagingParameters.Filter);
            int pageNumber = pagingParameters.IndexesRangeToPageNumber(pagingParameters.Range, pagingParameters.PageSize);
            int totalCountRows = 0;
            List<RoutePoint> items = new List<RoutePoint>();
            if (!string.IsNullOrEmpty(pagingParameters.Range) && filters.isFilterPresent("routeId"))
            {
                using (var db = new ServerDbContext(_dbOptions))
                {
                    var publishRoutes = db.Route.Where(r => r.IsPublished && r.IsDeleted == false).Select(r => r.RouteId).ToList();
                    var sharedRoutes = db.RouteShare.Select(s => s.RouteId).ToList();
                    var withoutFilter = db.RoutePoint.Where(x=> x.RouteId.Equals(filters.GetStringByName("routeId")) && sharedRoutes.Contains(x.RouteId) && (publishRoutes.Contains(x.RouteId)) && !x.IsDeleted);
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

        /// <summary>
        /// List url public images by route point
        /// </summary>
        /// <returns></returns>
        [HttpGet("{RoutePointId}/medias/")]
        public IActionResult Get(string RoutePointId)
        {
            List<Uri> urisImages = new List<Uri>();
            int totalCountRows = 0;
            using (var db = new ServerDbContext(_dbOptions))
            {
                var publishRoutes = db.Route.Where(r => r.IsPublished && r.IsDeleted == false).Select(r => r.RouteId).ToList();
                var sharedRoutes = db.RouteShare.Select(s => s.RouteId).ToList();
                var routePoints = db.RoutePoint.Where(x => x.RoutePointId.Equals(RoutePointId) && sharedRoutes.Contains(x.RouteId) && (publishRoutes.Contains(x.RouteId)) && !x.IsDeleted).Select(x=>x.RoutePointId).ToList();
                var medias = db.RoutePointMediaObject.Where(m => routePoints.Contains(m.RoutePointId) && m.MediaType == MediaObjectTypeEnum.Image && !m.IsDeleted);
                totalCountRows = medias.Count();
                urisImages = medias.Select(m => new Uri($"http://igosh.pro/shared/img_{m.RoutePointMediaObjectId}.jpg")).ToList();
            }
            HttpContext.Response.Headers.Add("x-total-count", totalCountRows.ToString());
            HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "x-total-count");
            return new ObjectResult(urisImages);
        }
    }
}
