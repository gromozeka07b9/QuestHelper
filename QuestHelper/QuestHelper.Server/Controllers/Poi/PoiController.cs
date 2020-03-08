using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestHelper.Server.Managers;
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
            List<Poi> pois = selectPois(filter);

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
            List<Poi> pois = selectPois(filter);

            TimeSpan delay = DateTime.Now - startDate;
            Console.WriteLine($"GetPrivatePoi: status 200, {userId}, delay:{delay.TotalMilliseconds}");

            return new ObjectResult(pois);
        }

        private List<Poi> selectPois(PoiFilter filter)
        {
            List<Poi> pois = new List<Poi>();
            
            using (var db = new ServerDbContext(_dbOptions))
            {
                pois = db.Poi
                    .Where(p=>
                    (filter.IsPrivate && p.CreatorId.Equals(filter.CreatorId)) || (!filter.IsPrivate)
                    && !p.IsDeleted
                    )
                    .Select(p => new Poi()
                {
                    Id = p.PoiId,
                    Name = p.Name,
                    Description = p.Description,
                    CreatorId = p.CreatorId,
                    Address = p.Address,
                    CreateDate = p.CreateDate,
                    UpdateDate = p.UpdateDate,
                    IsDeleted = p.IsDeleted,
                    ByRouteId = p.ByRouteId,
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
