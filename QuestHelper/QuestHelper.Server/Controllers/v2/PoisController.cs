using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestHelper.Server.Managers;
using QuestHelper.Server.Models;
using QuestHelper.SharedModelsWS;
using Poi = QuestHelper.SharedModelsWS.Poi;

namespace QuestHelper.Server.Controllers.v2
{
    /// <summary>
    /// Getting poi
    /// </summary>
    [Authorize]
    [ServiceFilter(typeof(RequestFilter))]
    [Route("api/v2/[controller]")]
    public class PoisController : Controller
    {
        private DbContextOptions<ServerDbContext> _dbOptions = ServerDbContext.GetOptionsContextDbServer();
        private MediaManager _mediaManager;
        private string _pathToMediaCatalog;
        public PoisController()
        {
            _mediaManager = new MediaManager();
            _pathToMediaCatalog = _mediaManager.PathToMediaCatalog;
        }

        /// <summary>
        /// Get all available POIs for current user - published other people, private
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetPoi([FromQuery] PagingParameters pagingParameters)
        {
            DateTime startDate = DateTime.Now;
            FilterParameters filters = new FilterParameters(pagingParameters.Filter);
            int pageNumber = pagingParameters.IndexesRangeToPageNumber(pagingParameters.Range, pagingParameters.PageSize);
            int totalCountRows = 0;
            string userId = IdentityManager.GetUserId(HttpContext);
            List<Poi> items = new List<Poi>();
            using (var db = new ServerDbContext(_dbOptions))
            {
                var withoutFilter = db.Poi.Where(p =>(p.IsPublished && p.CreatorId.Equals(userId)) || !p.IsPublished);
                withoutFilter = filters.isFilterPresent("createDate") ? withoutFilter.Where(r => r.CreateDate.Equals(filters.GetDateTimeByName("createDate"))) : withoutFilter;
                withoutFilter = filters.isFilterPresent("isPublished") ? withoutFilter.Where(r => r.IsPublished == filters.GetBooleanByName("isPublished")) : withoutFilter;
                withoutFilter = filters.isFilterPresent("isDeleted") ? withoutFilter.Where(r => r.IsDeleted == filters.GetBooleanByName("isDeleted")) : withoutFilter;
                withoutFilter = filters.isFilterPresent("creatorId") ? withoutFilter.Where(r => r.CreatorId.Contains(filters.GetStringByName("creatorId"))) : withoutFilter;
                withoutFilter = filters.isFilterPresent("name") ? withoutFilter.Where(r => r.Name.Contains(filters.GetStringByName("name"))) : withoutFilter;
                withoutFilter = filters.isFilterPresent("description") ? withoutFilter.Where(r => r.Description.Contains(filters.GetStringByName("description"))) : withoutFilter;
                //withoutFilter = filters.isFilterPresent("byRouteId") ? withoutFilter.Where(r => r.Name.Contains(filters.GetStringByName("byRouteId"))) : withoutFilter;
                //withoutFilter = filters.isFilterPresent("byRoutePointId") ? withoutFilter.Where(r => r.Name.Contains(filters.GetStringByName("byRoutePointId"))) : withoutFilter;
                totalCountRows = withoutFilter.Count();
                items = withoutFilter.OrderBy(r => r.CreateDate).Skip((pageNumber - 1) * pagingParameters.PageSize).Take(pagingParameters.PageSize).Select(getWsModelPoi(db)).ToList();
            }
            HttpContext.Response.Headers.Add("x-total-count", totalCountRows.ToString());
            HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "x-total-count");
            TimeSpan delay = DateTime.Now - startDate;
            Console.WriteLine($"V2 GetAllPoi: status 200, {userId}, delay:{delay.TotalMilliseconds}");

            return new ObjectResult(items);
        }

        /*private List<SharedModelsWS.Poi> selectPois(PoiFilter filter)
        {
            List<SharedModelsWS.Poi> pois = new List<SharedModelsWS.Poi>();

            using (var db = new ServerDbContext(_dbOptions))
            {
                pois = db.Poi
                    .Where(p =>
                    (filter.IsPrivate && p.CreatorId.Equals(filter.CreatorId)) || (!filter.IsPrivate)
                    && !p.IsDeleted
                    )
                    .Select(getWsModelPoi(db)).ToList();
            }

            return pois;
        }*/
        private static Expression<Func<Models.Poi, SharedModelsWS.Poi>> getWsModelPoi(ServerDbContext db)
        {
            return p => new SharedModelsWS.Poi()
            {
                Id = p.PoiId,
                Name = p.Name,
                Description = p.Description,
                CreatorId = p.CreatorId,
                Address = p.Address,
                CreateDate = p.CreateDate,
                UpdateDate = p.UpdateDate,
                IsDeleted = p.IsDeleted,
                ByRoutePointId = p.ByRoutePointId,
                ByRouteId = db.RoutePoint.Where(point => point.RoutePointId.Equals(p.ByRoutePointId)).FirstOrDefault().RouteId,
                Latitude = p.Latitude,
                Longitude = p.Longitude,
                ImgFilename = p.ImgFilename,
                IsPublished = p.IsPublished,
                Version = p.Version
            };
        }

    }
}
