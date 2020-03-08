using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestHelper.Server.Managers;
using QuestHelper.Server.Models;
using QuestHelper.SharedModelsWS;

namespace QuestHelper.Server.Controllers
{
    /// <summary>
    /// Getting poi
    /// </summary>
    [Authorize]
    [ServiceFilter(typeof(RequestFilter))]
    [Route("api/[controller]")]
    public class PoiController : Controller
    {
        private DbContextOptions<ServerDbContext> _dbOptions = ServerDbContext.GetOptionsContextDbServer();
        private MediaManager _mediaManager;
        private string _pathToMediaCatalog;

        public PoiController()
        {
            _mediaManager = new MediaManager();
            _pathToMediaCatalog = _mediaManager.PathToMediaCatalog;
        }
        /// <summary>
        /// Get all available POIs for current user - published other people, private
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetAllPoi()
        {
            DateTime startDate = DateTime.Now;
            string userId = IdentityManager.GetUserId(HttpContext);
            var filter = new PoiFilter();
            filter.IsPrivate = false;
            filter.CreatorId = string.Empty;
            List<SharedModelsWS.Poi> pois = selectPois(filter);

            TimeSpan delay = DateTime.Now - startDate;
            Console.WriteLine($"GetAllPoi: status 200, {userId}, delay:{delay.TotalMilliseconds}");

            return new ObjectResult(pois);
        }
        /// <summary>
        /// Get only private POI for current user
        /// </summary>
        /// <returns>List of POI</returns>
        [HttpGet("private")]
        public IActionResult GetPrivatePoi()
        {
            DateTime startDate = DateTime.Now;
            string userId = IdentityManager.GetUserId(HttpContext);

            var filter = new PoiFilter();
            filter.IsPrivate = true;
            filter.CreatorId = userId;
            List<SharedModelsWS.Poi> pois = selectPois(filter);

            TimeSpan delay = DateTime.Now - startDate;
            Console.WriteLine($"GetPrivatePoi: status 200, {userId}, delay:{delay.TotalMilliseconds}");

            return new ObjectResult(pois);
        }

        [HttpDelete]
        public void DeletePoi([FromBody] string poiId)
        {
            DateTime startDate = DateTime.Now;
            string userId = IdentityManager.GetUserId(HttpContext);

            using (var db = new ServerDbContext(_dbOptions))
            {
                var poiObject = db.Poi.Where(p => p.PoiId.Equals(poiId) && p.CreatorId.Equals(userId)).FirstOrDefault();
                if (!string.IsNullOrEmpty(poiObject?.PoiId))
                {
                    if (!poiObject.IsDeleted)
                    {
                        var poiDb = db.Poi.Find(poiId);
                        poiDb.IsDeleted = true;
                        db.Entry(poiDb).CurrentValues.SetValues(poiDb);
                        db.SaveChanges();
                    }
                }
                else
                {
                    Response.StatusCode = 404;
                }
            }

            TimeSpan delay = DateTime.Now - startDate;
            Console.WriteLine($"DeletePoi: status 200, {userId}, delay:{delay.TotalMilliseconds}");
        }

        [HttpPost]
        public void UpdatePoi([FromBody]SharedModelsWS.Poi poi)
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
                else
                {
                    var poiDb = ConverterWsToDbModel.PoiConvert(poi);
                    db.Poi.Add(poiDb);
                    db.SaveChanges();
                }
            }

            TimeSpan delay = DateTime.Now - startDate;
            Console.WriteLine($"UpdatePoi: status 200, {userId}, delay:{delay.TotalMilliseconds}");
        }

        /// <summary>
        /// Получение самого маленького превью, для отображения на карте
        /// </summary>
        /// <returns></returns>
        [HttpGet("{poiId}/image/{filename}")]
        public IActionResult GetPoiImage(string poiId, string filename)
        {
            DateTime startDate = DateTime.Now;
            string userId = IdentityManager.GetUserId(HttpContext);

            using (var db = new ServerDbContext(_dbOptions))
            {
                var poi = db.Poi.Find(poiId);
                if(!string.IsNullOrEmpty(poi?.ImgFilename) && poi.ImgFilename.Equals(filename))
                {
                    FileStream fileStream = System.IO.File.OpenRead(Path.Combine(_pathToMediaCatalog, filename));
                    TimeSpan delay = DateTime.Now - startDate;
                    Console.WriteLine($"GetPoiImage: status 200, {userId}, delay:{delay.TotalMilliseconds}");
                    return new FileStreamResult(fileStream, "image/jpeg");
                }
                else
                {
                    Response.StatusCode = 404;
                }
            }
            return new ObjectResult(null);
        }

        private List<SharedModelsWS.Poi> selectPois(PoiFilter filter)
        {
            List<SharedModelsWS.Poi> pois = new List<SharedModelsWS.Poi>();
            
            using (var db = new ServerDbContext(_dbOptions))
            {
                pois = db.Poi
                    .Where(p=>
                    (filter.IsPrivate && p.CreatorId.Equals(filter.CreatorId)) || (!filter.IsPrivate)
                    && !p.IsDeleted
                    )
                    .Select(p => new SharedModelsWS.Poi()
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
                    Latitude = p.Latitude,
                    Longitude = p.Longitude,
                    ImgFilename = p.ImgFilename,
                    IsPublished = p.IsPublished,
                    Version = p.Version
                }).ToList();
            }

            return pois;
        }
    }
}
