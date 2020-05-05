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
        public IActionResult GetPois([FromQuery] PagingParameters pagingParameters)
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

        [HttpGet("{poiId}")]
        public IActionResult GetPoiById(string poiId)
        {
            DateTime startDate = DateTime.Now;
            SharedModelsWS.Poi poi = new SharedModelsWS.Poi();
            string userId = IdentityManager.GetUserId(HttpContext);

            using (var db = new ServerDbContext(_dbOptions))
            {
                poi = db.Poi.Where(p => p.PoiId.Equals(poiId)).Select(getWsModelPoi(db)).SingleOrDefault();
            }

            TimeSpan delay = DateTime.Now - startDate;
            Console.WriteLine($"v2 GetPoiById: status 200, {userId}, delay:{delay.TotalMilliseconds}");

            return new ObjectResult(poi);
        }

        [HttpPut("{PoiId}")]
        public void PutPoi(string PoiId, [FromBody]SharedModelsWS.Poi poi)
        {
            DateTime startDate = DateTime.Now;
            string userId = IdentityManager.GetUserId(HttpContext);

            using (var db = new ServerDbContext(_dbOptions))
            {
                var poiObject = db.Poi.Where(p => p.PoiId.Equals(poi.Id)).FirstOrDefault();
                if (!string.IsNullOrEmpty(poiObject?.PoiId))
                {
                    if (poiObject.CreatorId.Equals(userId))
                    {
                        if (!string.IsNullOrEmpty(poi.ImgBase64))
                        {
                            Base64Manager.SaveBase64ToFile(poi.ImgBase64, Path.Combine(_pathToMediaCatalog, poi.ImgFilename));
                        }

                        var convertedPoiDb = ConverterWsToDbModel.PoiConvert(poi);
                        convertedPoiDb.CreatorId = userId;
                        convertedPoiDb.Version = ++poiObject.Version;

                        var poiDb = db.Poi.Find(convertedPoiDb.PoiId);
                        db.Entry(poiDb).CurrentValues.SetValues(convertedPoiDb);
                        db.SaveChanges();
                    }
                    else
                    {
                        Response.StatusCode = 403;
                    }
                }
            }

            TimeSpan delay = DateTime.Now - startDate;
            Console.WriteLine($"v2 PutPoi: status 200, {userId}, delay:{delay.TotalMilliseconds}");
        }

        [HttpPost("{PoiId}")]
        public void PostPoi(string PoiId, [FromBody]SharedModelsWS.Poi poi)
        {
            DateTime startDate = DateTime.Now;
            string userId = IdentityManager.GetUserId(HttpContext);

            using (var db = new ServerDbContext(_dbOptions))
            {
                var poiObject = db.Poi.Where(p => p.PoiId.Equals(poi.Id)).FirstOrDefault();
                if (string.IsNullOrEmpty(poiObject?.PoiId))
                {
                    if (!string.IsNullOrEmpty(poi.ImgBase64))
                    {
                        Base64Manager.SaveBase64ToFile(poi.ImgBase64, Path.Combine(_pathToMediaCatalog, poi.ImgFilename));
                    }
                    var poiDb = ConverterWsToDbModel.PoiConvert(poi);
                    db.Poi.Add(poiDb);
                    db.SaveChanges();
                }
                else
                {
                    Response.StatusCode = 405;
                }
            }

            TimeSpan delay = DateTime.Now - startDate;
            Console.WriteLine($"v2 PostPoi: status 200, {userId}, delay:{delay.TotalMilliseconds}");
        }

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
