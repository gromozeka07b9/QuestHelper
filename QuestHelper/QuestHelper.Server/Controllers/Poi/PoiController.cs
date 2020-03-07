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
        /// Get POI with filter
        /// </summary>
        /// <returns>List of POI</returns>
        [HttpGet]
        public IActionResult Get(PoiFilter filter)
        {
            DateTime startDate = DateTime.Now;
            string userId = IdentityManager.GetUserId(HttpContext);

            List<Poi> pois = new List<Poi>();
            using (var db = new ServerDbContext(_dbOptions))
            {
                pois = db.Poi.Select(p=>new Poi() 
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
                    Version = p.Version
                }).ToList();
            }

            TimeSpan delay = DateTime.Now - startDate;
            Console.WriteLine($"Get POI: status 200, {userId}, delay:{delay.TotalMilliseconds}");

            return new ObjectResult(pois);
        }





    }
}
